using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Util;

public class PlayerEvidenceMove : MonoBehaviour
{
    public Transform ClickPos;
    public Camera UseCamera;
    public Collider EvidecneCol;

    private NavMeshAgent _agent;
    private TouchUtils.TouchState _touchState = TouchUtils.TouchState.None;
    private Vector2 _touchPos;
    private List<GameObject> _getEvidences = new List<GameObject>();
    private Animator _animator;
    private bool _canFind = false;

    private void Awake()
    {
        _agent = transform.GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 마우스 클릭 위치에 맞기 움직임
        TouchUtils.TouchSetUp(ref _touchState, ref _touchPos);
        if(_touchState == TouchUtils.TouchState.Began)
        {
            Ray ray = UseCamera.ScreenPointToRay(_touchPos);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                _agent.SetDestination(hit.point);
                ClickPos.position = hit.point;
            }
        }

        // 탐색 시작 키를 눌렀을 때
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(_getEvidences.Count > 0)
            {
                EvidenceObjectAttachData data = _getEvidences[0].GetComponent<EvidenceObjectAttachData>();
                UIManager.Instance.ShowPopupUI<GetEvidencePopUpUI>().EvidenceID = data.EvidenceID;
                Destroy(data);
                GameManager.Instance.EvidenceInventory.Add(data.EvidenceID);
                _getEvidences[0].tag = "TempEvidence";
                _getEvidences.RemoveAt(0);
            }
            else if(_canFind)
            {
                UIManager.Instance.ShowPopupUI<GetNotEvidencePopUpUI>();
            }
        }        
        
        _animator.SetBool("IsWalking", (_agent.hasPath && !_agent.isStopped));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Evidence")
        {
            _getEvidences.Add(other.gameObject);
        }

        if(other.tag == "TempEvidence")
        {
            _canFind = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Evidence")
        {
            if (_getEvidences.Contains(other.gameObject))
            {
                _getEvidences.Remove(other.gameObject);
            }
        }

        if (other.tag == "TempEvidence")
        {
            _canFind = false;
        }
    }
}
