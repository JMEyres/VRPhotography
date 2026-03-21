using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BasicCamera : MonoBehaviour
{
    [Header("Camera variables")]
    // Maybe change this to be command pattern
    public Camera targetCamera;
    public Volume globalVolume;
    public LensBase lens;
    public int resW = 1920;
    public int resH = 1080;
    private float ev, iso, ss = 0;
   
    [Header("XR Controller button")]
    public InputActionReference rightJoystick;

    Vector2 test;

    public void TakePicture()
    {
        // Mathf.Log(result,base)
        // Math for getting iso to global post exposure would be Mathf.Log(ISO/100, 2), basically how many times do we need to double to get to the iso value
        // Math for getting shutter speed (SS) to global post exposure would be Mathf.Log(SS*100, 2) then just add to iso result
        // Depth of field is a separate thing

        // temporary rendertexture
        RenderTexture rt = new RenderTexture(resW, resH, 24);
        targetCamera.targetTexture = rt;

        // empty texture to hold pixel data
        Texture2D photo = new Texture2D(resW, resH, TextureFormat.RGB24, false);

        // force camera to render 1 frame
        targetCamera.Render();
        
        // save frame to photo object
        RenderTexture.active = rt;
        photo.ReadPixels(new Rect(0,0,resW,resH),0,0);
        photo.Apply();

        // cleanup
        targetCamera.targetTexture = null;
        RenderTexture.active = null; 
        Destroy(rt);

        byte[] bytes = photo.EncodeToPNG();
        
        // This saves the image in the project's Assets folder
        string filename = Application.persistentDataPath + "/Photos/CameraOutput_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        File.WriteAllBytes(filename, bytes);
        
        Destroy(photo);

        Debug.Log("Picture taken and saved to: " + filename);
    }

    void Update()
    {
        if(Keyboard.current.pKey.wasPressedThisFrame) TakePicture();
        iso = targetCamera.iso;
        ss = targetCamera.shutterSpeed;

        ev = Mathf.Log(iso/100, 2) + Mathf.Log(ss*100, 2);
        globalVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments);
        colorAdjustments.postExposure.value = ev;

        globalVolume.profile.TryGet<DepthOfField>(out var depthOfField);
        depthOfField.focalLength.value = targetCamera.focalLength;
        depthOfField.focusDistance.value = targetCamera.focusDistance;
        depthOfField.aperture.value = targetCamera.aperture;

        //Debug.Log(test);
        // 1. Apply the zoom movement first
        targetCamera.focalLength += test.y;

        // 2. "Clamp" the value so it can never be smaller than Min or larger than Max
        targetCamera.focalLength = Mathf.Clamp(
            targetCamera.focalLength, 
            lens.focalLength, 
            lens.focalLengthMax
        );
        if (targetCamera.focusDistance + test.x *0.01f >= 0) targetCamera.focusDistance += test.x *0.01f;
    }

    public void TestOnActivated() // Use this with onactivated on the grab interactable to trigger taking a photo
    {
        Debug.Log("BUTTON activated");
    }

    public void StartHolding()
    {
        rightJoystick.action.performed += SaveFloat;
        rightJoystick.action.canceled += SaveFloat;
    }

    public void StopHolding()
    {
        rightJoystick.action.performed -= SaveFloat;
        rightJoystick.action.canceled -= SaveFloat;
    }
    
    void SaveFloat(InputAction.CallbackContext context)
    {
        test = rightJoystick.action.ReadValue<Vector2>();
    }
}
