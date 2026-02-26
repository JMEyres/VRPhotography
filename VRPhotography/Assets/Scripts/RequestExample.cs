using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RequestExample : MonoBehaviour
{
[System.Serializable]
    public class ServerResponse
    {
        public string reply;
        public int status_code;
    }

    void Start()
    {
        StartCoroutine(TestConnection());
    }

    IEnumerator TestConnection()
    {
        WWWForm form = new WWWForm();
        form.AddField("message", "Hello Flask, are you there?");

        using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:5000/ping", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Connection failed: {www.error}");
            }
            else
            {
                // 2. Grab the raw JSON string sent by Flask
                string rawJson = www.downloadHandler.text;
                
                // 3. Convert that string into our ServerResponse C# object
                ServerResponse responseData = JsonUtility.FromJson<ServerResponse>(rawJson);

                // 4. Now you can use the data like any normal C# variable!
                Debug.Log($"The server replied with: {responseData.reply}");
                Debug.Log($"The status code is: {responseData.status_code}");
                
                // Example of actually using it:
                // if (responseData.status_code == 200) { myTextUI.text = responseData.reply; }
            }
        }
    }
}
