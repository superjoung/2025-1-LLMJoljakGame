using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LLMTest : MonoBehaviour
{
    // 요청에 사용할 JSON 구조 정의
    [System.Serializable]
    public class LLMRequest
    {
        public string input;
        public string npc;
    }

    // 유니티 시작 시 호출됨
    void Start()
    {
        StartCoroutine(SendToServer());
    }

    // FastAPI 서버로 POST 요청 보내기
    IEnumerator SendToServer()
    {
        string url = "http://127.0.0.1:8000/ask"; // FastAPI의 /ask 경로

        // 요청 데이터 구성
        LLMRequest requestData = new LLMRequest
        {
            input = "지훈아, 어제 지현이랑 어디 있었어?",
            npc = "지훈은 지현을 감싸고 거짓말할 가능성이 있다."
        };

        // JSON으로 변환
        string json = JsonUtility.ToJson(requestData);

        // HTTP 요청 생성
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내고 대기
        yield return request.SendWebRequest();

        // 응답 확인
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ GPT 응답: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ 요청 실패: " + request.error);
        }
    }
}
