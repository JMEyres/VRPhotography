using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;


public class DepthImageTrigger : MonoBehaviour
{
    public UniversalRendererData rendererData; // Drag your Renderer Asset here

    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Capture();
        }
    }

    public void Capture()
    {
        if (rendererData == null) return;

        // Find our feature in the renderer list
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature is DepthSaveFeature depthFeature)
            {
                depthFeature.RequestCapture();
                break;
            }
        }
    }
}