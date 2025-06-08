using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DefineEnum.GameModeDefine;
using DefineEnum.SpotNameDefine;
using UnityEngine.UIElements;

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
            if(GameManager.Instance._playerMainScreenUI == null)
            {
                Debug.Log("[WARN] NPCAttachData - TalkText 프로퍼티에 문제가 있습니다.");
                return;
            }
            GameManager.Instance._playerMainScreenUI.ShowText(value);
            _animator.SetTrigger("Talk");
            _talkText = value;
        }
    }

    private string _talkText = "";
    private NPCInteractionPopUpUI _popUpUI;
    //public NPCTalkPanelUI PopUpTalkUI
    //{
    //    get
    //    {
    //        NPCTalkPanelUI npcTalkUI = null;
    //        foreach(Transform child in UIPos)
    //        {
    //            if(child.name == "NPCTalkPanelUI") npcTalkUI = child.GetComponent<NPCTalkPanelUI>();
    //        }
    //        return npcTalkUI;
    //    }
    //}
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
                if(_moveCount < 3)
                {
                    SpotName currentName = NoneCharacterManager.Instance.LLMNPCMoveSpots[ID][_moveCount];
                    TargetSpot = NoneCharacterManager.Instance.GetMoveSpotPos(currentName, ID);
                    _timer = 0;
                    _moveCount += 1;
                }
                else if(_moveCount == 3)
                {
                    SpotName currentName = SpotName.House;
                    TargetSpot = NoneCharacterManager.Instance.GetMoveSpotPos(currentName, ID);
                    _timer = 0;
                    _moveCount += 1;
                }
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
            _currentDestination = value;
            Agent.SetDestination(value.position);
        }
    }
    public NavMeshAgent Agent;
    private float _timer = 0f;
    // 해당 시간에 도달하면 다시 다른 곳으로 이동할 수 있도록 제작
    private float _targetTime = 5f;
    private bool _canMove = false;
    private bool _isGoal = false;
    public int _moveCount = 0;
    private Transform _currentDestination = null;
    #endregion

    private Animator _animator;
    
    private void Start()
    {
        ID = int.Parse(gameObject.name.Split("_")[1]);
        _popUpUI = UIManager.Instance.ShowNPCUI<NPCInteractionPopUpUI>(UIPos);
        _popUpUI.NpcID = ID;
        UINeck.gameObject.SetActive(false);
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (_currentDestination)
        {
            Agent.SetDestination(_currentDestination.position);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameMode == GameFlowMode.TalkMode || GameManager.Instance.CurrentGameMode == GameFlowMode.HearingMode)
        {
            _popUpUI.gameObject.SetActive(false);
        }
        else
        {
            _popUpUI.gameObject.SetActive(true);
        }
        // 도착지점에 도달했을 때
        if (Agent.velocity.sqrMagnitude >= 0.2f * 0.2f && Agent.remainingDistance <= 0.5f)
        {
            _isGoal = true;
        }

        if (_isGoal)
        {
            // 도착지점에 도달했을 때 타이머 시작
            _timer += Time.deltaTime;
            if (_timer >= _targetTime)
            {
                _isGoal = false;
                CanMove = true;
            }
        }
        _animator.SetBool("IsWalking", (Agent.hasPath && !Agent.isStopped));
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
