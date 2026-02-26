using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageUploaderForm : MonoBehaviour
{
    void Start()
    {
        string myImagePath = @"C:\Users\joshu\Pictures\Castle.png"; 
        string serverUrl = "http://127.0.0.1:5000/assess";
        
        StartCoroutine(SendImageAsForm(myImagePath, serverUrl));
    }

    IEnumerator SendImageAsForm(string imagePath, string url)
    {
        if (!File.Exists(imagePath))
        {
            Debug.LogError($"Could not find image at: {imagePath}");
            yield break;
        }

        // 1. Read the raw image bytes (No Base64 conversion needed!)
        byte[] imageBytes = File.ReadAllBytes(imagePath);

        // 2. Create the form
        WWWForm form = new WWWForm();
        
        // 3. Add the raw binary data to the form
        // Parameters: (Field Name for Flask, The Bytes, File Name, MIME type)
        form.AddBinaryData("image_file", imageBytes, "upload.png", "image/png");

        // 4. Send the POST request
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            Debug.Log("Uploading image...");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                Debug.Log($"Success! Server replied: {www.downloadHandler.text}");
            }
        }
    }
}