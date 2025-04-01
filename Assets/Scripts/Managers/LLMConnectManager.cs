using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LLMConnectManager : Singleton<LLMConnectManager>
{
    private string url = "http://127.0.0.1:8000/ask";

    [System.Serializable]
    public class UserInput
    {
        public string input;
        public string npc;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string result;
    }

    public IEnumerator AskLLM(string input, string npc)
    {
        UserInput userInput = new UserInput { input = input, npc = npc };
        string jsonData = JsonUtility.ToJson(userInput);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }

    private void Start()
    {
        // For Test
        LLMConnectManager.Instance.StartCoroutine(AskLLM("너 지금 뭐하고 있었어?", "플레이어"));
    }
}