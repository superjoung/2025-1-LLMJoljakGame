using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LLM;
using MiniJSON;
using UnityEngine;
using UnityEngine.Networking;

public class LLMConnectManager : Singleton<LLMConnectManager>
{
    private const string SetupUrl = "http://127.0.0.1:8000/generate_setup";
    private const string AskUrl = "http://127.0.0.1:8000/ask";
    private const string TurnUrl = "http://127.0.0.1:8000/generate_turn_data";
    private const string FinalStatementUrl = "http://localhost:8000/final_statements";
    private const string ChiefStatementUrl = "http://127.0.0.1:8000/chief_statement";
    private const string SubmitEvidenceUrl = "http://127.0.0.1:8000/submit_evidence";

    private Setup _currentSetup;
    private List<Sprite> _npcPortraitList = new List<Sprite>();
    
    // --- [게임 설정 받아오기] ---
    public IEnumerator GetGameSetup(Action onFinish)
    {
        UnityWebRequest request = UnityWebRequest.Post(SetupUrl, "");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            SetupResponse setupResponse = JsonUtility.FromJson<SetupResponse>(json);

            _currentSetup = setupResponse.setup;

            Debug.Log("마을 이름: " + _currentSetup.village);
            Debug.Log("사건 내용: " + _currentSetup.@event);
            Debug.Log("용의자 수: " + _currentSetup.suspects.Length);

            foreach (var suspect in _currentSetup.suspects)
            {
                Debug.Log($"용의자: {suspect.name}, 성별: {suspect.gender}, 나이: {suspect.age_group} 성격: {suspect.personality.behavior}, 감정: {suspect.personality.emotion}");
            }
            
            const int npcCount = 5;
            Dictionary<string, int> meshNameCount = new();
            for (int i = 0; i < npcCount; i++)
            {
                string prefabName = GetAllSuspects()[i].gender + "_" +
                                    GetAllSuspects()[i].age_group + "_";
                meshNameCount.TryAdd(prefabName, 0);
                meshNameCount[prefabName] += 1;
                prefabName += meshNameCount[prefabName];
                _npcPortraitList.Add(ResourceManager.Instance.LoadSprite(prefabName + "_Image"));
            }
        }
        else
        {
            Debug.Log("설정 가져오기 실패: " + request.error);
            GetLocalGameSetup();
        }
        onFinish?.Invoke();
        request.Dispose();
    }

    private void GetLocalGameSetup()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "setup.json");

        if (File.Exists(filePath))
        {
            string fallbackJson = File.ReadAllText(filePath);
            _currentSetup = JsonUtility.FromJson<Setup>(fallbackJson);
            Debug.LogWarning("로컬 setup.json 사용: " + _currentSetup.village);
        }
        else
        {
            Debug.LogError("로컬 setup.json 파일도 존재하지 않습니다: " + filePath);
        }
    }
    
    // --- [촌장 발언 받아오기] ---
    public IEnumerator GetChiefStatement(Action<string> onFinish)
    {
        UnityWebRequest request = UnityWebRequest.Get(ChiefStatementUrl);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // { "statement": "..." } 형태 파싱
            ChiefStatementResponse response = JsonUtility.FromJson<ChiefStatementResponse>(json);
            Debug.Log("[촌장 발언] " + response.statement);

            onFinish?.Invoke(response.statement);
        }
        else
        {
            Debug.LogError("촌장 발언 가져오기 실패: " + request.error);
            onFinish?.Invoke("촌장의 발언을 불러오는 데 실패했습니다.");
        }

        request.Dispose();
    }
    
    // --- [LLM 질문 보내기] ---
    public IEnumerator AskLLM(string input, string npc, Action<string> onResponse)
    {
        Debug.Log($"{npc}, {input}");
        UserInput userInput = new UserInput { input = input, npc = npc };
        string jsonData = JsonUtility.ToJson(userInput);

        UnityWebRequest request = new UnityWebRequest(AskUrl, "POST");
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
            onResponse?.Invoke(responseData.response); // 결과 전달
        }
        else
        {
            Debug.Log("LLM 질문 전송 실패: " + request.error);
            onResponse?.Invoke(null);
        }

        request.Dispose();
    }
// --- [루트 + 증거 설정] ---
    public IEnumerator SetNPCTurnData(System.Action<Dictionary<string, List<string>> /* npc_routes */, List<Clue> /* clues */> onResponse)
    {
        UnityWebRequest request = UnityWebRequest.Post(TurnUrl, "");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("서버 응답 원본: " + jsonResult);

            var rawDict = Json.Deserialize(jsonResult) as Dictionary<string, object>;

            var npcRoutes = new Dictionary<string, List<string>>();
            var npcRouteRaw = rawDict["npc_routes"] as Dictionary<string, object>;
            foreach (var kvp in npcRouteRaw)
            {
                List<object> routes = kvp.Value as List<object>;
                List<string> routeList = routes.ConvertAll(r => r.ToString());
                npcRoutes.Add(kvp.Key, routeList);
            }

            // clues 파싱
			var cluesRaw = rawDict["clues"] as List<object>;
			var clueList = new List<Clue>();

			foreach (var c in cluesRaw)
			{
   				var clueDict = c as Dictionary<string, object>;
    			Clue clue = new Clue
    			{
        			id = clueDict["id"].ToString(),
        			name = clueDict["name"].ToString(),
        			location = clueDict["location"].ToString(),
        			importance = int.Parse(clueDict["importance"].ToString())
    			};
    			clueList.Add(clue);
			}

            onResponse?.Invoke(npcRoutes, clueList);
        }
        else
        {
            Debug.LogError("게임 데이터 생성 실패: " + request.error);
            onResponse?.Invoke(null, null);
        }

        request.Dispose();
    }
    
// --- [최종 발언] ---
    public IEnumerator GetFinalStatements(Action<Dictionary<string, string>> onResponse)
    {
        UnityWebRequest request = new UnityWebRequest(FinalStatementUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(new byte[0]);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            var dict = Json.Deserialize(jsonResult) as Dictionary<string, object>;

            // {NPC 이름, 최종 발언}
            var result = new Dictionary<string, string>();
            foreach (var pair in dict)
            {
                result[pair.Key] = pair.Value.ToString();
            }
            onResponse?.Invoke(result);
        }
        else
        {
            Debug.LogError("최종 발언 요청 실패: " + request.error);
            onResponse?.Invoke(null);
        }

        request.Dispose();
    }
    
    // --- [증거 제출] ---
    public IEnumerator SubmitEvidence(string npcName, string evidenceId, Action<string> onFinish)
    {
        EvidenceInput input = new EvidenceInput
        {
            npc = npcName,
            evidence = evidenceId
        };

        string jsonData = JsonUtility.ToJson(input);

        UnityWebRequest request = new UnityWebRequest(SubmitEvidenceUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string resultJson = request.downloadHandler.text;
            EvidenceResponse result = JsonUtility.FromJson<EvidenceResponse>(resultJson);
            Debug.Log($"[{result.npc}] 반응: {result.response}");

            onFinish?.Invoke(result.response);
        }
        else
        {
            Debug.LogError("증거 제출 실패: " + request.error);
            onFinish?.Invoke("증거 제출 중 오류가 발생했습니다.");
        }

        request.Dispose();
    }
    
    // --- [Getter 함수들] ---
    public Setup GetCurrentSetup()
    {
        if (_currentSetup == null)
        {
            GetLocalGameSetup();
        }
        return _currentSetup;
    }

    public string GetVillageName()
    {
        return GetCurrentSetup().village;
    }

    public string GetEventDescription()
    {
        return GetCurrentSetup().@event;
    }

    public Suspect[] GetAllSuspects()
    {
        return GetCurrentSetup().suspects;
    }

    public Suspect GetSuspectByName(string suspectName)
    {
        if (GetCurrentSetup().suspects == null) return null;
        foreach (var suspect in GetCurrentSetup().suspects)
        {
            if (suspect.name == suspectName)
                return suspect;
        }
        return null;
    }

    public Suspect GetWitchSuspect()
    {
        foreach (var suspect in GetCurrentSetup().suspects)
        {
            if (suspect.is_witch)
                return suspect;
        }
        return null;
    }
    
    public Sprite GetNpcPortraitToID(int ID)
    {
        if (ID < 0 && _npcPortraitList.Count <= ID)
        {
            Debug.LogWarning("[Warning] LLMConnectManager - GetNpcPortraitToID 올바른 인수를 넘기지 않았습니다.");
            return null;
        }
        return _npcPortraitList[ID];
    }
}
