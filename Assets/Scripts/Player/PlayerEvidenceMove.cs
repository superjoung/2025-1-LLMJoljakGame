using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Util;

public class PlayerEvidenceMove : MonoBehaviour
{
    public Transform ClickPos;
    public Camera UseCamera;

    private NavMeshAgent _agent;
    private TouchUtils.TouchState _touchState = TouchUtils.TouchState.None;
    private Vector2 _touchPos;

    private void Awake()
    {
        _agent = transform.GetComponent<NavMeshAgent>();
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
    }
}
