using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class NoneCharacterManager
{
    // 고정 NPC 저장
    public List<GameObject> FixNpcs = new List<GameObject>();
    // Key - 고정 NPC ID / Value - 관측 완료된 LLM NPC 리스트
    public Dictionary<int, List<int>> CompleteWatchingLLMNpc = new Dictionary<int, List<int>>();
    
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
            npc.GetComponent<NPCFixAttachData>().id = count;
            // 고정 NPC에 맞는 Key 생성 후 리스트 선언
            CompleteWatchingLLMNpc.Add(count, new List<int>());
            count++;
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
}
