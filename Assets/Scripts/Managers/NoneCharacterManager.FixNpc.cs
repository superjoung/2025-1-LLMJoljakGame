using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NoneCharacterManager
{
    // 고정 NPC 저장
    public List<GameObject> FixNpcs = new List<GameObject>();
    
    public void FixNPCSpawn()
    {
        int count = 0;

        foreach (Transform child in GameManager.Instance.ParentPrefabs.FixNpcMovePoint.transform)
        {
            // movePoint 처음 포인트 위치 할당
            Transform spawnPos = child.GetChild(0);
            GameObject npc = ResourceManager.Instance.Instantiate("NPC/FixNPC", spawnPos.position, GameManager.Instance.ParentPrefabs.FixNpcBox.transform);
            npc.transform.rotation = spawnPos.rotation;
            npc.GetComponent<NPCFixAttachData>().id = count;
            count++;
        }
    }
}
