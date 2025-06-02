using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.SpotNameDefine;

public partial class NoneCharacterManager
{
    public Dictionary<int, List<SpotName>> LLMNPCMoveSpots = new Dictionary<int, List<SpotName>>();

    // 증거 제출 변수 저장
    public string SaveEvidenceData = string.Empty;

    public void LLMMoveSpotNameSetting()
    {
        int idCount = 0;
        // 모든 이동 관련 오브젝트 가져오기
        foreach(Transform child in GameManager.Instance.ParentPrefabs.NpcMoveBox.transform)
        {
            LLMSpotAttachData data = child.GetComponent<LLMSpotAttachData>();
            // NPC 집 위치일 경우
            if (data.Name.Contains("NPC"))
            {
                // 이름 가지고 와서 장소 이름으로 변경
                data.Name = GetNpcNameToID(NpcList[idCount].GetComponent<NPCAttachData>().ID);

                // TEMP : 임시 이름으로 잘 작동하는지 확인
                //data.Name = TempLLMNpcNames[idCount] + "_" + idCount.ToString();
                Debug.Log("[INFO]NoneCharacterManager.Evidence(LLMMoveSpotNameSetting) - 장소 이름 변경");
                idCount += 1;
            }
        }
    }

    // SpotName에 따라 이동해야하는 장소 전달 함수
    public Transform GetMoveSpotPos(SpotName Name, int LLMID)
    {
        if(Name == SpotName.None)
        {
            Debug.Log("[WARN]NoneCharacterManager.Evidence(GetMoveSpotPos) - 인수값이 잘못됐습니다.");
            return null;
        }

        Transform pos = null;

        // Enum 형식을 String형식으로 변경
        string convertName = Name == SpotName.House ? GetNpcNameToID(LLMID) : GetSpotName(Name, true);

        // converName과 맞는 장소 pos를 전달
        foreach(Transform child in GameManager.Instance.ParentPrefabs.NpcMoveBox.transform)
        {
            if (child.GetComponent<LLMSpotAttachData>().Name.Contains(convertName))
            {
                pos = child;
                Debug.Log($"[INFO]NoneCharacterManager.Evidence(GetMoveSpotPos) - {LLMID} 번호가 {pos} 위치를 할당 받았습니다.");
            }
        }
        return pos;
    }

    public void LLMNPCMoveReset()
    {
        // 모든 LLM 캐릭터 움직임 시작
        foreach(GameObject child in NpcList)
        {
            child.GetComponent<NPCAttachData>()._moveCount = 0;
        }
    }
    
    public void LLMNPCMoveStart()
    {
        // 모든 LLM 캐릭터 움직임 시작
        foreach(GameObject child in NpcList)
        {
            child.GetComponent<NPCAttachData>().CanMove = true;
        }
    }

    // 초기 LLM NPC 움직이는 장소 넣어두기 추후 삭제될 함수
    public void MoveSpotSetting(Dictionary<string, List<string>> routes)
    {
        LLMNPCMoveSpots.Clear();

        foreach (var route in routes)
        {
            int npcId = 0;
            for (int i = 0; i < 5; i++)
            {
                if (LLMConnectManager.Instance.GetAllSuspects()[i].name == route.Key)
                {
                    npcId = i;
                    break;
                }
            }
            
            LLMNPCMoveSpots.Add(npcId, new List<SpotName>());
            SpotName spotName = SpotName.None;
            foreach (var spot in route.Value)
            {
                if (spot == "집")
                {
                    spotName = SpotName.House;
                }
                else if (spot == "성당")
                {
                    spotName = SpotName.Church;
                }
                else if (spot == "숲")
                {
                    spotName = SpotName.Forest;
                }
                else if (spot == "광장")
                {
                    spotName = SpotName.Square;
                }
                else if (spot == "우물")
                {
                    spotName = SpotName.Brook;
                }
                LLMNPCMoveSpots[npcId].Add(spotName);
            }
        }
    }
}
