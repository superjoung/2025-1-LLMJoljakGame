using DefineEnum.GameModeDefine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using DefineEnum.SpotNameDefine;

public class NPCFixAttachData : MonoBehaviour
{
    public float ViewRadius;
    [Range(0, 360)]
    public float ViewAngle;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;
    // 있어야함, 왜냐면 타겟 사이에 다른 오브젝트가 있는데 그 오브젝트를 투과해서 뒤의 타겟오브젝트를 볼 수 있음 
    public List<Transform> VisibleTargets = new List<Transform>();
    // 현재 관측하고 있는 NPC ID
    public List<int> SeeNpcIDs = new List<int>();

    public int Id;
    public SpotName StandingSpotName;
    public List<Transform> MovePoints = new List<Transform>();

    #region Talking
    public Transform UIPos;
    public Transform UINeck;
    public Transform SeePoint;

    public bool IsTalkStart = false;  // 대화 시작
    public bool CanTalkStart = false; // 대화 시작 가능 (범위 안에 들어왔을 때)

    public NPCTalkPanelUI PopUpTalkUI
    {
        get
        {
            NPCTalkPanelUI npcTalkUI = null;
            foreach (Transform child in UIPos)
            {
                if (child.name == "NPCTalkPanelUI") npcTalkUI = child.GetComponent<NPCTalkPanelUI>();
            }
            return npcTalkUI;
        }
    }

    public string TalkText
    {
        get
        {
            return _talkText;
        }
        set // TalkText 입력시 자동으로 텍스트 화면에 출력
        {
            if (PopUpTalkUI == null)
            {
                Debug.Log("[WARN] NPCAttachData - TalkText 프로퍼티에 문제가 있습니다.");
                return;
            }
            PopUpTalkUI.ShowText(value);
            _talkText = value;
        }
    }

    private string _talkText = "";
    private FixNPCInteractionPopUpUI _popUpUI;
    #endregion

    public NavMeshAgent Agent;
    private int _moveCount = 1; // 리스트 인덱스
    private bool _moveFlag = true; // 어느 방향으로 이동할지 결정
    private Transform _currentDestination = null;

    private void Start()
    {
        // 에이전트 등록
        Agent = GetComponent<NavMeshAgent>();

        // 해당 id를 가진 NPC가 어떤 경로로 이동해야하는지 리스트에 추가
        foreach (Transform child in GameManager.Instance.ParentPrefabs.FixNpcMovePoint.transform.GetChild(Id))
        {
            MovePoints.Add(child);
        }

        // 목적지로 출발
        if (MovePoints.Count > 1)
        {
            _currentDestination = MovePoints[_moveCount];
            Agent.SetDestination(MovePoints[_moveCount].position);
        }
        else Agent.SetDestination(MovePoints[0].position);

        // 상호작용 UI 설정
        _popUpUI = UIManager.Instance.ShowNPCUI<FixNPCInteractionPopUpUI>(UIPos);
        _popUpUI.NpcID = Id;
        UINeck.gameObject.SetActive(false);

        StartCoroutine("FindTargetsDelay", .5f);
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
        // 이동해야하는 곳이 존재하면
        if(MovePoints.Count > 1)
        {
            AroundMove();
        }

        // 고정 NPC 대화 모드로 이동했을 때 상호작용 PopUp 비활성화
        if (GameManager.Instance.CurrentGameMode == GameFlowMode.FixTalkMode)
        {
            _popUpUI.gameObject.SetActive(false);
        }
        else
        {
            _popUpUI.gameObject.SetActive(true);
        }
    }

    private void AroundMove()
    {
        // Point 지점에 도달 했을 경우
        if (Agent.velocity.sqrMagnitude >= 0.2f * 0.2f && Agent.remainingDistance <= 0.5f)
        {
            // 현 좌표가 리스트의 끝 또는 처음 지점에 도달했는지 확인
            if (_moveCount + 1 == MovePoints.Count || _moveCount == 0)
            {
                _moveFlag = !_moveFlag;
            }
            // 리스트의 끝 지점에 이동하고 있는 경우
            if (_moveFlag)
            {
                _moveCount++;
            }
            else
            {
                _moveCount--;
            }
            _currentDestination = MovePoints[_moveCount];
            Agent.SetDestination(MovePoints[_moveCount].position);
        }
    }

    IEnumerator FindTargetsDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindTargets();
        }
    }

    void FindTargets()
    {
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, ViewRadius, TargetMask);
        List<int> tempIDs = new List<int>();
        for (int i = 0; i < targetInViewRadius.Length; i++)
        //ViewRadius 안에 있는 타겟의 개수 = 배열의 개수보다 i가 작을 때 for 실행 
        {
            Transform target = targetInViewRadius[i].transform; //타겟[i]의 위치 
            Vector3 dirToTarget = (target.position - transform.position).normalized;
             //vector3타입의 타겟의 방향 변수 선언 = 타겟의 방향벡터, 타겟의 position - 이 게임오브젝트의 position) normalized = 벡터 크기 정규화 = 단위벡터화
            if (Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
            // 전방 벡터와 타겟방향벡터의 크기가 시야각의 1/2이면 = 시야각 안에 타겟 존재 
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position); //타겟과의 거리를 계산 
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleMask))
                //레이캐스트를 쐈는데 ObstacleMask가 아닐 때 참이고 아래를 실행함 
                {
                    // 위치 정보 저장
                    if (!VisibleTargets.Contains(target))
                    {
                        VisibleTargets.Add(target);
                    }
                    // 관측한 LLM NPC 아이디 추출 후 이미 관측한 NPC인지 확인
                    Debug.Log(target.name);
                    int targetID = int.Parse(target.name.Split('_')[1]);
                    if(!NoneCharacterManager.Instance.ObservationCompleted(Id, targetID))
                    {
                        // 만약 관측하지 않았다면 targetID에 저장
                        NoneCharacterManager.Instance.CompleteWatchingLLMNpc[Id].Add(targetID);
                        Debug.Log($"[INFO]NPCFixAttachData(FindTargets) : 아이디 - {Id} 고정 NPC가 아이디 - {targetID}를 관측 기록했습니다.");
                    }
                    // 임시 관측된 ID 저장
                    tempIDs.Add(targetID);
                    if (!SeeNpcIDs.Contains(targetID))
                    {
                        SeeNpcIDs.Add(targetID);
                    }
                    print("raycast hit!");
                    Debug.DrawRay(transform.position, dirToTarget * 10f, Color.red, 5f);
                }
            }
        }
        // 리스트 정리
        List<int> delIDs = new List<int>();
        foreach(int npcId in SeeNpcIDs)
        {
            if (!tempIDs.Contains(npcId))
            {
                delIDs.Add(npcId);
            }
        }
        // 리스트안 식별 끝난 리스트 삭제
        foreach(int npcId in delIDs)
        {
            SeeNpcIDs.Remove(npcId);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CanTalkStart = true;
            Debug.Log($"{Id} NCP에게 {other.gameObject.name} In.");
            UINeck.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            UINeck.LookAt(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CanTalkStart = false;
            Debug.Log($"{Id} NCP에게 {other.gameObject.name} Out.");
            UINeck.gameObject.SetActive(false);
        }
    }
}