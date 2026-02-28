using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;

public class BasicCamera : MonoBehaviour
{
    public Camera targetCamera;

    public int resW = 1920;
    public int resH = 1080;

    public void TakePicture()
    {
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
    }
}
