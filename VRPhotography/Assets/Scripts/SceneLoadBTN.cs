using UnityEngine;

public class SceneLoadBTN : MonoBehaviour
{
    public string sceneToLoad;

    public void TriggerLoad()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("SceneManager not found! Is the Base Scene loaded?");
        }
    }
}
