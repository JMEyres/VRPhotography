using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public string currentScene = "Menu";

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    [ContextMenu ("LoadScene")]
    public void LoadScene(string scene)
    {
        StartCoroutine(LoadAsync(scene));
    }
    [ContextMenu ("UnLoadScene")]
    public void UnLoadScene(string scene)
    {
        StartCoroutine(UnLoadAsync(scene));
    }

  
    IEnumerator LoadAsync(string scene)
    {
        if (!string.IsNullOrEmpty(currentScene))
        {
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        while (!asyncLoad.isDone) // this is for a loading bar or something like that
        {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(scene);
    
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
            currentScene = scene;
        }
    }

    IEnumerator UnLoadAsync(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(scene);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
