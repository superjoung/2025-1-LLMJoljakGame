using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DefineEnum.GameModeDefine;
using DefineEnum.SpotNameDefine;

public class NPCAttachData : MonoBehaviour
{
    public int ID;
    #region TalkingUIValues
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
            if(PopUpTalkUI == null)
            {
                Debug.Log("[WARN] NPCAttachData - TalkText 프로퍼티에 문제가 있습니다.");
                return;
            }
            PopUpTalkUI.ShowText(value);
            _talkText = value;
        }
    }

    private string _talkText = "";
    private NPCInteractionPopUpUI _popUpUI;
    public NPCTalkPanelUI PopUpTalkUI
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
    #endregion

    #region MovingValues
    // 조건에 달성해 움직일 수 있게 되었을때
    public bool CanMove
    {
        get
        {
            return _canMove;
        }
        set
        {
            // 이동가능 상태로 변경되었을때
            if (value)
            {
                // 현 NPC가 이동해야하는 장소로 자동 할당 추후 이동을 멈춰야하는 경우 수정 필요
                SpotName currentName = NoneCharacterManager.Instance.LLMNPCMoveSpots[ID][_moveCount];
                TargetSpot = NoneCharacterManager.Instance.GetMoveSpotPos(currentName, ID);
            }
            _canMove = value;
        }
    }

    public Transform TargetSpot
    {
        set
        {
            // TargetSpot이 지정되었을 때 해당 지점으로 이동
            CanMove = false;
            _agent.SetDestination(value.position);
        }
    }
    private NavMeshAgent _agent;
    private float _timer = 0f;
    // 해당 시간에 도달하면 다시 다른 곳으로 이동할 수 있도록 제작
    private float _targetTime = 30f;
    private bool _canMove = true;
    private int _moveCount = 0;
    #endregion

    private void Start()
    {
        ID = int.Parse(gameObject.name.Split("_")[1]);
        _popUpUI = UIManager.Instance.ShowNPCUI<NPCInteractionPopUpUI>(UIPos);
        _popUpUI.NpcID = ID;
        UINeck.gameObject.SetActive(false);
        // agent 등록
        _agent = GetComponent<NavMeshAgent>();
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
        // 도착지점에 도달했을 때
        if (_agent.velocity.sqrMagnitude >= 0.2f * 0.2f && _agent.remainingDistance <= 0.5f)
        {
            // 도착지점에 도달했을 때 타이머 시작
            _timer += Time.deltaTime;
            if (_timer >= _targetTime)
            {
                CanMove = true;
            }
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
