using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.GameModeDefine;

public class NPCAttachData : MonoBehaviour
{
    public int ID;
    public Transform UIPos;
    public Transform UINeck;
    public Transform SeePoint;

    public bool IsTalkStart = false;  // 대화 시작
    public bool CanTalkStart = false; // 대화 시작 가능 (범위 안에 들어왔을 때)
    public string TalkText
    {
        get
        {
            return _talkText;
        }
        set // TalkText 입력시 자동으로 텍스트 화면에 출력
        {
            if(_popUpTalkUI == null)
            {
                Debug.Log("[WARN] NPCAttachData - TalkText 프로퍼티에 문제가 있습니다.");
                return;
            }

            _popUpTalkUI.ShowText(value);
            _talkText = value;
        }
    }

    private string _talkText = "";
    private NPCInteractionPopUpUI _popUpUI;
    private NPCTalkPanelUI _popUpTalkUI
    {
        get
        {
            NPCTalkPanelUI npcTalkUI = null;
            foreach(Transform child in UIPos)
            {
                if(child.name == "NPCTalkPanelUI") npcTalkUI = child.GetComponent<NPCTalkPanelUI>();
            }
            return npcTalkUI;
        }
    }

    private void Start()
    {
        ID = int.Parse(gameObject.name.Split("_")[1]);
        _popUpUI =  UIManager.Instance.ShowNPCUI<NPCInteractionPopUpUI>(UIPos);
        _popUpUI.NpcID = ID;
        UINeck.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameMode == GameFlowMode.TalkMode)
        {
            _popUpUI.gameObject.SetActive(false);
        }
        else
        {
            _popUpUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CanTalkStart = true;
            Debug.Log($"{ID} NCP에게 {other.gameObject.name} In.");
            UINeck.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            UINeck.LookAt(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CanTalkStart = false;
            Debug.Log($"{ID} NCP에게 {other.gameObject.name} Out.");
            UINeck.gameObject.SetActive(false);
        }
    }
}
