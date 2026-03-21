using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
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
