using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad = "";
    public string sceneToUnLoad = "";
    
    [ContextMenu ("LoadScene")]
    void LoadScene()
    {
        StartCoroutine(LoadAsync(sceneToLoad));
    }
    [ContextMenu ("UnLoadScene")]
    void UnLoadScene()
    {
        StartCoroutine(UnLoadAsync(sceneToUnLoad));
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
