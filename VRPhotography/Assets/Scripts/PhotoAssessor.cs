using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoAssessor : MonoBehaviour
{
    public RawImage photoDisplay;
    private string[] photos;
    private int currentPhotoIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPhotos();
    }

    public void LoadPhotos()
    {
        // match path from camera
        string folderPath = Application.persistentDataPath + "/Photos";
        Directory.CreateDirectory(folderPath);


        // get files from folder
        photos = Directory.GetFiles(folderPath, "*.png");

        if (photos.Length > 0)
        {
            DisplayPhoto(0);
        }
        else
        {
            Debug.Log("No photos found in the folder.");
        }
    }

    public void DisplayPhoto(int index)
    {
        if (photos.Length == 0 || index < 0 || index >= photos.Length) return;

        // Read data from the file
        byte[] fileData = File.ReadAllBytes(photos[index]);

        // create empty texture
        Texture2D texture = new Texture2D(1920, 1080);

        // load data into the texture
        texture.LoadImage(fileData); 

        photoDisplay.texture = texture;
    }

    public void NextPhoto()
    {
        currentPhotoIndex++;
        if (currentPhotoIndex >= photos.Length) currentPhotoIndex = 0;
        DisplayPhoto(currentPhotoIndex);
    }

    public void PreviousPhoto()
    {
        currentPhotoIndex--;
        if (currentPhotoIndex < 0) currentPhotoIndex = photos.Length - 1;
        DisplayPhoto(currentPhotoIndex);
    }
}
