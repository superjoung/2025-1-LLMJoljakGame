using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NoneCharacterManager
{
    // 현재 대화중인 NPC ID 추후 여러명과 대화할 수 있으면 리스트 형식으로 수정할 것
    private int _currentTalkNpcID = -1;

    public List<int> CanStartTalkNpcs // 대화할 수 있는 NPC 선정 리스트로 전달
    {
        get
        {
            _canStartTalkNpcs = new List<int>();
            foreach (GameObject child in NpcList)
            {
                NPCAttachData data = child.GetComponent<NPCAttachData>();
                if (data.CanTalkStart)
                {
                    _canStartTalkNpcs.Add(data.ID);
                }
            }
            return _canStartTalkNpcs;
        }
    }

    public int CanTalkNpcCount // 대화 가능 NPC 변수로 설정
    {
        get
        {
            int count = 0;
            foreach (GameObject child in NpcList)
            {
                NPCAttachData data = child.GetComponent<NPCAttachData>();
                if (data.CanTalkStart)
                {
                    count++;
                }
            }
            return count;
        }
    }

    private List<int> _canStartTalkNpcs;

    public void TalkStartWithPlayer(int NpcId)
    {
        // 플레이어랑 대화중에는 이동 중이던 행동 멈추기
        // NonePlayersAction[NpcId].Peek().IsTalkWithPlayer = true;
        _currentTalkNpcID = NpcId;
        GameObject npc = GetNpcToID(NpcId);
        GameObject player = GameObject.FindWithTag("Player");
        npc.transform.LookAt(player.transform.position);
        // 플레이어가 바라보는 각도 조절
        player.transform.LookAt(npc.GetComponent<NPCAttachData>().SeePoint.position - (new Vector3(0, npc.transform.position.y, 0) - new Vector3(0, player.transform.position.y, 0)));

        UIManager.Instance.ShowNPCUI<NPCTalkPanelUI>(npc.GetComponent<NPCAttachData>().UIPos);

        // TEMP : test입니당
        GetTalkString(NpcId, "안녕하세요. 저는 테스트 문자열입니다.");
    }

    // ID에 일치하는 NPC에게 대화 문장 넘겨주기
    public void GetTalkString(int ID, string Sentence)
    {
        GameObject npc = GetNpcToID(ID);
        npc.GetComponent<NPCAttachData>().TalkText = Sentence;
    }
}
