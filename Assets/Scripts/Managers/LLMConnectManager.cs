using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LLMConnectManager : Singleton<LLMConnectManager>
{
    private string askUrl = "http://127.0.0.1:8000/ask";
    private string setupUrl = "http://127.0.0.1:8000/generate_setup";

    // --- [API 전송용 데이터 구조] ---
    [System.Serializable]
    public class UserInput
    {
        public string input;
        public string npc;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string npc;
        public string response;
    }

    [System.Serializable]
    public class Personality
    {
        public string behavior;
        public string emotion;
    }

    [System.Serializable]
    public class Suspect
    {
        public string name;
        public Personality personality;
        public string occupation;
        public string[] relationships;
        public string statement;
        public bool is_witch;
    }

    [System.Serializable]
    public class Setup
    {
        public string village;
        public string @event; // event는 예약어라 escape 처리
        public Suspect[] suspects;
    }

    [System.Serializable]
    public class SetupResponse
    {
        public string message;
        public Setup setup;
    }

    // --- [LLM 질문 보내기] ---
    public IEnumerator AskLLM(string input, string npc)
    {
        UserInput userInput = new UserInput { input = input, npc = npc };
        string jsonData = JsonUtility.ToJson(userInput);

        UnityWebRequest request = new UnityWebRequest(askUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResult);
            Debug.Log($"[{responseData.npc}]: {responseData.response}");
        }
        else
        {
            Debug.Log("LLM 질문 전송 실패: " + request.error);
        }
    }

    // --- [게임 설정 받아오기] ---
    public IEnumerator GetGameSetup()
    {
        UnityWebRequest request = UnityWebRequest.Post(setupUrl, "");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            SetupResponse setupResponse = JsonUtility.FromJson<SetupResponse>(json);

            Debug.Log("마을 이름: " + setupResponse.setup.village);
            Debug.Log("사건 내용: " + setupResponse.setup.@event);
            Debug.Log("용의자 수: " + setupResponse.setup.suspects.Length);

            foreach (var suspect in setupResponse.setup.suspects)
            {
                Debug.Log($"용의자: {suspect.name}, 성격: {suspect.personality.behavior}, 감정: {suspect.personality.emotion}");
            }

            // TODO: setup을 게임 데이터에 적용하기
            // Test
            StartCoroutine(AskLLM("안녕하세요", setupResponse.setup.suspects[0].name));
        }
        else
        {
            Debug.Log("설정 가져오기 실패: " + request.error);
        }
    }

    // --- [테스트용 자동 실행] ---
    private void Start()
    {
        StartCoroutine(GetGameSetup());
    }
}
