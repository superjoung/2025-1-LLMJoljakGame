using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DefineEnum.SpotNameDefine;

public partial class NoneCharacterManager
{
    // 고정 NPC 저장
    public List<GameObject> FixNpcs = new List<GameObject>();
    // Key - 고정 NPC ID / Value - 관측 완료된 LLM NPC 리스트
    public Dictionary<int, List<int>> CompleteWatchingLLMNpc = new Dictionary<int, List<int>>();
    public List<string> FixNpcNames = new List<string>() {"제임스", "메리", "프리아", "카이", "제이스", "크리세", "크림슨"};
    // 고정 NPC 대화 스트링 전부 저장
    public List<Dictionary<string, object>> FixNpcTalkData = new List<Dictionary<string, object>>();
    // 질문 1회 끝났을 때
    public bool IsEndSelect = false;
    // 임시 LLM 이름들 나중에 삭제 예정
    public List<string> TempLLMNpcNames = new List<string>() { "캐리미", "썸밋", "데즐러", "갓데드", "텍스처" };

    // 고정 NPC와 플레이어가 대화할 때 어떤 NPC와 대화할 수 있는지 ID값으로 넘겨주기
    public int CanTalkStartFixNPC
    {
        get
        {
            int npcID = -1;
            foreach(GameObject child in FixNpcs)
            {
                NPCFixAttachData data = child.GetComponent<NPCFixAttachData>();
                if (data.CanTalkStart)
                {
                    // 다중의 NPC가 겹칠 경우 예외처리
                    npcID = data.Id;
                    Debug.Log($"[INFO]NoneCharacterManager.FixNpc - CanTalkStartFixNPC 프로퍼티 동작 {npcID}가 선택되었습니다.");
                    break;
                }
            }
            return npcID;
        }
    }

    // 고정 NPC 스크립스 시작부
    private void FixNpcInit()
    {   
        FixNpcTalkData.Clear();
        FixNpcTalkData = CSVReader.Read("CSV/FixNpcTalkDatas");
    }
    
    public void FixNPCSpawn()
    {
        int count = 0;

        foreach (Transform child in GameManager.Instance.ParentPrefabs.FixNpcMovePoint.transform)
        {
            // movePoint 처음 포인트 위치 할당
            Transform spawnPos = child.GetChild(0);
            GameObject npc = ResourceManager.Instance.Instantiate("NPC/FixNPC", spawnPos.position, GameManager.Instance.ParentPrefabs.FixNpcBox.transform);
            // 움직이지 않는 고정 NPC 목 돌려주기
            npc.transform.rotation = spawnPos.rotation;
            // NPC 아이디 넣어주기
            npc.GetComponent<NPCFixAttachData>().Id = count;
            // 현재 고정 NPC가 어디에 위치해있는지 확인 코드
            npc.GetComponent<NPCFixAttachData>().StandingSpotName = spawnPos.GetComponent<SpotAttachData>().SpotName;
            // 고정 NPC에 맞는 Key 생성 후 리스트 선언
            CompleteWatchingLLMNpc.Add(count, new List<int>());
            count++;
            // 모델 할당
            ResourceManager.Instance.Instantiate("NPC/Normal_NPC/Normal_NPC_" + count, npc.transform.position + Vector3.down, npc.transform);

            FixNpcs.Add(npc);
        }
    }

    public bool ObservationCompleted(int FixId, int LLMId)
    {
        // 해당 고정 NPC가 관측 완료한 LLMNpc일 경우
        if (CompleteWatchingLLMNpc[FixId].Contains(LLMId))
        {
            return true;
        }

        // 아닐 경우
        return false;
    }

    // 안에 들어가는 여러명일 경우 수정이 필요
    public void FixNPCTalkStartWithPlayer(int NpcId)
    {
        // 플레이어랑 대화중에는 이동 중이던 행동 멈추기
        // NonePlayersAction[NpcId].Peek().IsTalkWithPlayer = true;
        GameObject npc = GetFixNpcToID(NpcId);

        CurrentTalkNpcID = NpcId;

        npc.GetComponent<NPCFixAttachData>().Agent.isStopped = true;

        GameObject player = GameObject.FindWithTag("Player");

        // 파괴해야하는 오브젝트에 추가
        UIManager.Instance.ShowNPCUI<NPCTalkPanelUI>(npc.GetComponent<NPCFixAttachData>().UIPos);

        CanPlayerEnterText = false;
        // TEMP : test입니당
        GetFixTalkString(GetTalkStartText(npc.GetComponent<NPCFixAttachData>().StandingSpotName));

        npc.transform.LookAt(player.transform.position);

        // 플레이어가 바라보는 각도 조절
        PlayerLookAtToNpc(npc);
    }

    // 고정 NPC에게 대화 문장 넘겨주기 이거 기존에 없던거라 코드 효율 개박음 양해좀 ;
    public void GetFixTalkString(string Sentence)
    {
        GameObject npc = GetFixNpcToID(CurrentTalkNpcID);
        npc.GetComponent<NPCFixAttachData>().TalkText = Sentence;
    }

    public SpotName ConvertIdToSpotName(int NpcId)
    {
        SpotName spotName = SpotName.None;


        return spotName;
    }

    // 관측기록 초기화 함수
    public void ObservationReset(int ID = -1)
    {
        // 아이디 직접 입력시 해당 고정 NPC만 관측 결과 초기화
        if (ID >= 0)
        {
            CompleteWatchingLLMNpc[ID].Clear();
            Debug.Log($"[INFO]NoneCharacterManager.FixNpc(ObservationReset) : 아이디 - {ID} 고정 NPC의 관측 기록을 제거했습니다.");
        }
        // 인수 입력 안할 시 모든 관측 결과 초기화
        else
        {
            for (int i = 0; i < CompleteWatchingLLMNpc.Count; i++)
            {
                // 고정 NPC 관측결과 초기화
                int key = CompleteWatchingLLMNpc.Keys.ToList()[i];
                CompleteWatchingLLMNpc[key].Clear();
            }
            Debug.Log("[INFO]NoneCharacterManager.FixNpc(ObservationReset) : 모든 고정 NPC의 관측 기록을 제거했습니다.");
        }
    }

    // 초기 대화 시작
    public string GetTalkStartText(SpotName Spot)
    {
        return FixNpcTalkData[Random.Range(0, FixNpcTalkData.Count)][GetSpotName(Spot)].ToString();
    }

    // 대화 끝나고 내용 전달
    public string GetTalkAnswerText(int WantNpcID)
    {
        // 고정 NPC가 관측에 성공했는지 안했는지
        bool isSee = false;
        string textFrame = "";
        // 관측에 성공했는지 반복문 실시
        if (CompleteWatchingLLMNpc[CurrentTalkNpcID].Contains(WantNpcID))
        {
            isSee = true;
        }
        if (isSee)
        {
            // 때마침 옆에 있을 때
            if (FixNpcs[CurrentTalkNpcID].GetComponent<NPCFixAttachData>().SeeNpcIDs.Contains(WantNpcID))
            {
                textFrame = FixNpcTalkData[Random.Range(0, FixNpcTalkData.Count - 1 )]["NearDetectionSuccess"].ToString();
            }
            // 본적이 있을 때
            else
            {
                textFrame = FixNpcTalkData[Random.Range(0, FixNpcTalkData.Count)]["DetectionSuccess"].ToString();
            }
        }
        // 관측 실패
        else
        {
            // 한번도 본적이 없을 때
            textFrame = FixNpcTalkData[Random.Range(0, FixNpcTalkData.Count)]["DetectionFail"].ToString();
        }

        // Frame 내용 변환
        if (textFrame.Contains("NpcName"))
        {
            // 해당 부분 LLM 연결 후 아랫줄 주석 풀고 사용
            // textFrame = textFrame.Replace("NpcName", GetNpcNameToID(WantNpcID));
            textFrame = textFrame.Replace("NpcName", GetNpcNameToID(WantNpcID));
        }
        if (textFrame.Contains("SpotName"))
        {
            textFrame =  textFrame.Replace("SpotName", GetSpotName(FixNpcs[CurrentTalkNpcID].GetComponent<NPCFixAttachData>().StandingSpotName, true));
        }
        IsEndSelect = true;
        return textFrame;
    }

    private string GetSpotName(SpotName Spot, bool IsKorea = false)
    {
        string spotName = "";
        switch (Spot)
        {
            case SpotName.House:
                if (IsKorea) spotName = "집";
                else spotName = "House";
                break;
            case SpotName.Church:
                if (IsKorea) spotName = "성당";
                else spotName = "Church";
                break;
            case SpotName.Brook:
                if (IsKorea) spotName = "개울";
                else spotName = "Brook";
                break;
            case SpotName.Square:
                if (IsKorea) spotName = "광장";
                else spotName = "Square";
                break;
            case SpotName.Forest:
                if (IsKorea) spotName = "숲";
                else spotName = "Forest";
                break;
            default:
                Debug.Log("[WANR]NoneCharacterManager.FixNpc(GetTalkStartText) - 올바르지 않은 SpotName입니다.");
                break;
        }
        return spotName;
    }
}
