using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util; 
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;

public class DepthSaveFeature : ScriptableRendererFeature
{
    [Header("Capture Settings")]
    public bool trigger = false;
    [Range(0.1f, 10f)] public float contrastPower = 1.0f;

    private int _burstCounter = 0; 
    private const int WARM_UP_FRAMES = 10;
    
    private DepthCopyPass _depthPass;
    private RTHandle _persistentDepthTexture;

    public override void Create()
    {
        _depthPass = new DepthCopyPass(RenderPassEvent.AfterRenderingTransparents);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;

        if (trigger) { trigger = false; _burstCounter = WARM_UP_FRAMES; }

        if (_burstCounter > 0)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.graphicsFormat = GraphicsFormat.R32_SFloat;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            desc.width = 1920;
            desc.height = 1080;

            RenderingUtils.ReAllocateHandleIfNeeded(ref _persistentDepthTexture, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_PersistentDepthTexture");

            // Grab camera planes dynamically so meters are accurate
            float n = renderingData.cameraData.camera.nearClipPlane;
            float f = renderingData.cameraData.camera.farClipPlane;

            _depthPass.Setup(_persistentDepthTexture, _burstCounter == 1, contrastPower, n, f); 
            renderer.EnqueuePass(_depthPass);
            _burstCounter--;
        }
    }

    protected override void Dispose(bool disposing) => _persistentDepthTexture?.Release();

    class DepthCopyPass : ScriptableRenderPass
    {
        private RTHandle _destHandle;
        private bool _shouldSave;
        private float _power;
        private float _nearClip, _farClip;

        public DepthCopyPass(RenderPassEvent evt) { this.renderPassEvent = evt; ConfigureInput(ScriptableRenderPassInput.Depth); }
        
        public void Setup(RTHandle dest, bool saveNow, float pow, float n, float f) { 
            _destHandle = dest; 
            _shouldSave = saveNow; 
            _power = pow;
            _nearClip = n;
            _farClip = f;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
        {
            UniversalResourceData frameData = frameContext.Get<UniversalResourceData>();
            TextureHandle cameraDepth = frameData.cameraDepthTexture.IsValid() ? frameData.cameraDepthTexture : frameData.activeDepthTexture;
            if (!cameraDepth.IsValid()) return;

            TextureHandle depthTarget = renderGraph.ImportTexture(_destHandle);
            renderGraph.AddBlitPass(cameraDepth, depthTarget, Vector2.one, Vector2.zero, passName: "DepthBurst");

            if (_shouldSave)
            {
                AsyncGPUReadback.Request(_destHandle, 0, (request) => {
                    if (request.hasError) return;
                    var data = request.GetData<float>().ToArray();
                    Task.Run(() => ProcessAndSave(data, _power, _nearClip, _farClip));
                });
            }
        }


    private void ProcessAndSave(float[] rawData, float power, float n, float f)
{
    float minRaw = 1f;
    float maxRaw = 0f;

    for (int i = 0; i < rawData.Length; i++) {
        float r = rawData[i];
        if (r > 0.0001f && r < 0.9999f) {
            if (r < minRaw) minRaw = r;
            if (r > maxRaw) maxRaw = r;
        }
    }

    // --- THE MATHEMATICAL TRUTH (REVISED) ---
    // On Reversed-Z (PC), raw depth is usually 1.0 (near) to 0.0 (far).
    // This function calculates meters based on the Camera's Clip Planes.
    float Linearize(float depth, float near, float far)
    {
        // This is the standard perspective linearization for Reversed-Z
        return near * far / (near + depth * (far - near));
    }

    // On Reversed-Z:
    // minRaw (furthest pixel) -> should be far meters
    // maxRaw (closest pixel) -> should be near meters
    float distA = Linearize(minRaw, n, f);
    float distB = Linearize(maxRaw, n, f);

    float nearMeters = Mathf.Min(distA, distB);
    float farMeters = Mathf.Max(distA, distB);

    Color[] pixels = new Color[rawData.Length];
    for (int i = 0; i < rawData.Length; i++)
    {
        float raw = rawData[i];
        if (raw <= 0.0001f || raw >= 0.9999f) {
            pixels[i] = Color.black;
        } else {
            // Your visual logic is perfect, don't change it
            float normalized = (raw - minRaw) / Mathf.Max(0.0001f, (maxRaw - minRaw));
            float visual = normalized; 
            visual = Mathf.Pow(Mathf.Clamp01(visual), power);
            pixels[i] = new Color(visual, visual, visual, 1.0f);
        }
    }
    UnityEditor.EditorApplication.delayCall += () => {
        Texture2D output = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        output.SetPixels(pixels);
        output.Apply();
        byte[] pngData = output.EncodeToPNG();
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.dataPath, $"Depth_Meters_{System.DateTime.Now:HH-mm-ss}.png"), pngData);
        
        Debug.Log($"<color=white><b>[CALIBRATION]</b></color> White={nearMeters:F2}m | Black={farMeters:F2}m | CamNear={n}m");

        
        Object.DestroyImmediate(output);
    };
}
    }
    public void RequestCapture() => trigger = true;
}