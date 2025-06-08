using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class NoneCharacterManager
{
    public List<GameObject> TalkList = new List<GameObject>();
    public int CurrentTalkNpcID = -1;
    public string PlayerText
    {
        set
        {
            if (CanPlayerEnterText)
            {
                CanPlayerEnterText = false;
                ResponseNpcText(value);
            }
        }
    }

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

    public bool CanPlayerEnterText = true;

    private List<int> _canStartTalkNpcs;

    // 안에 들어가는 여러명일 경우 수정이 필요
    public void TalkStartWithPlayer(int NpcId)
    {
        // 플레이어랑 대화중에는 이동 중이던 행동 멈추기
        // NonePlayersAction[NpcId].Peek().IsTalkWithPlayer = true;
        GameObject npc = GetNpcToID(NpcId);
        TalkList.Add(npc); // 여러명과 대화할 때를 염두해 수정
        CurrentTalkNpcID = TalkList[0].GetComponent<NPCAttachData>().ID;

        npc.GetComponent<NPCAttachData>().Agent.isStopped = true;

        GameObject player = GameObject.FindWithTag("Player");

        // 파괴해야하는 오브젝트에 추가
        //UIManager.Instance.ShowNPCUI<NPCTalkPanelUI>(npc.GetComponent<NPCAttachData>().UIPos);

        // TEMP : test입니당
        CanPlayerEnterText = false;
        GetTalkString("안녕하세요! 심판관님!");

        npc.transform.LookAt(player.transform.position);
        
        npc.GetComponent<NPCAttachData>().UINeck.localEulerAngles = new Vector3(0, 30f, 0);

        // 플레이어가 바라보는 각도 조절
        PlayerLookAtToNpc(npc);
    }

    public void PlayerLookAtToNpc(GameObject npc)
    {
        GameObject player = GameObject.FindWithTag("Player");

        Transform seePoint = null;
        // 플레이어가 바라보는 각도 조절
        if (npc.tag == "FixNpc")
        {
            seePoint = npc.GetComponent<NPCFixAttachData>().SeePoint;
        }
        else if(npc.tag == "LLMNpc")
        {
            seePoint = npc.GetComponent<NPCAttachData>().SeePoint;
        }

        //player.transform.LookAt(seePoint.position - (new Vector3(0, npc.transform.position.y, 0) - new Vector3(0, player.transform.position.y, 0)));
        player.transform.LookAt(seePoint.position);

        player.GetComponent<PlayerMove>().PlayerHead.transform.LookAt(seePoint.position);
    }

    // ID에 일치하는 NPC에게 대화 문장 넘겨주기
    public void GetTalkString(string Sentence)
    {
        GameObject npc = GetNpcToID(CurrentTalkNpcID);
        npc.GetComponent<NPCAttachData>().TalkText = Sentence;
    }

    // NPC에게 문자열을 전달해 어떤 대답을 하게 시킬건지 계산하는 함수
    private void ResponseNpcText(string inputText)
    {
        if (inputText.Count() > 0)
        {
            // 증거 제출 시
            if (SaveEvidenceData != string.Empty && GameManager.Instance.CurrentGameMode == DefineEnum.GameModeDefine.GameFlowMode.HearingMode)
            {
                StartCoroutine(LLMConnectManager.Instance.SubmitEvidence(GetNpcNameToID(CurrentTalkNpcID),
                    SaveEvidenceData, response =>
                    {
                        if (response != null)
                        {
                            GetTalkString(response);
                            SaveEvidenceData = string.Empty;
                        }
                        else
                        {
                            Debug.Log("응답이 null입니다.");
                        }
                    })
                );
            }
            else
            {
                // 일반 대화 시도
                StartCoroutine(LLMConnectManager.Instance.AskLLM(inputText, GetNpcNameToID(CurrentTalkNpcID), response => {
                    if (response != null)
                    {
                        GetTalkString(response);
                    }
                    else
                    {
                        Debug.Log("응답이 null입니다.");
                    }
                }));
            }
        }
    }
}
