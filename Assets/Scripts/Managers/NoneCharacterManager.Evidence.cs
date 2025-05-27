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
                //data.Name = GetNpcNameToID(NpcList[idCount].GetComponent<NPCAttachData>().ID);

                // TEMP : 임시 이름으로 잘 작동하는지 확인
                data.Name = TempLLMNpcNames[idCount] + "_" + idCount.ToString();
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
        string convertName = Name == SpotName.House ? TempLLMNpcNames[LLMID] : GetSpotName(Name, true);

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

    public void LLMNPCMoveStart()
    {
        // 모든 LLM 캐릭터 움직임 시작
        foreach(GameObject child in NpcList)
        {
            child.GetComponent<NPCAttachData>().CanMove = true;
        }
    }

    // 초기 LLM NPC 움직이는 장소 넣어두기 추후 삭제될 함수
    private void MoveSpotSetting()
    {
        LLMNPCMoveSpots.Add(0, new List<SpotName>() { SpotName.Brook, SpotName.House, SpotName.Forest });
        LLMNPCMoveSpots.Add(1, new List<SpotName>() { SpotName.Square, SpotName.House, SpotName.Church });
        LLMNPCMoveSpots.Add(2, new List<SpotName>() { SpotName.Square, SpotName.House, SpotName.Brook });
        LLMNPCMoveSpots.Add(3, new List<SpotName>() { SpotName.Forest, SpotName.Brook, SpotName.House });
        LLMNPCMoveSpots.Add(4, new List<SpotName>() { SpotName.Brook, SpotName.Church, SpotName.Forest });
    }
}
