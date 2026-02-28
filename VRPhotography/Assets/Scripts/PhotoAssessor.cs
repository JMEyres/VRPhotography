using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PhotoAssessor : MonoBehaviour
{
    public RawImage photoDisplay;
    public ImageUploaderForm imageUploaderForm;
    public int indexToSend = 0;
    private string[] photos;
    private int currentPhotoIndex = 0;
    private string folderPath;
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPhotos();
    }
    [ContextMenu("Test Load Photos")]
    public void LoadPhotos()
    {
        // match path from camera
        folderPath = Application.persistentDataPath + "/Photos";
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
    
    [ContextMenu("Test Next Photo")]
    public void NextPhoto()
    {
        currentPhotoIndex++;
        if (currentPhotoIndex >= photos.Length) currentPhotoIndex = 0;
        DisplayPhoto(currentPhotoIndex);
    }
   
    [ContextMenu("Test Previous Photo")]
    public void PreviousPhoto()
    {
        currentPhotoIndex--;
        if (currentPhotoIndex < 0) currentPhotoIndex = photos.Length - 1;
        DisplayPhoto(currentPhotoIndex);
    }

    [ContextMenu("DeleteAllPhotos")]
    private void ClearAllPhotos()
    {
        foreach (string photoPath in photos)
        {
            File.Delete(photoPath);
        }
    }
    
    [ContextMenu("Assess Photo")]
    private void AssessPhoto()
    {
        StartCoroutine(imageUploaderForm.SendImageAsForm(photos[indexToSend]));
    }
}
