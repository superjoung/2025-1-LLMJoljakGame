using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCFixAttachData : MonoBehaviour
{
    public int id;
    public List<Transform> MovePoints = new List<Transform>();

    private NavMeshAgent _agent;
    private int _moveCount = 1; // 리스트 인덱스
    private bool _moveFlag = true; // 어느 방향으로 이동할지 결정

    private void Start()
    {
        // 에이전트 등록
        _agent = GetComponent<NavMeshAgent>();

        // 해당 id를 가진 NPC가 어떤 경로로 이동해야하는지 리스트에 추가
        foreach (Transform child in GameManager.Instance.ParentPrefabs.FixNpcMovePoint.transform.GetChild(id))
        {
            MovePoints.Add(child);
        }

        // 목적지로 출발
        if(MovePoints.Count > 1) _agent.SetDestination(MovePoints[_moveCount].position);
        else _agent.SetDestination(MovePoints[0].position);
    }

    private void Update()
    {
        // 이동해야하는 곳이 존재하면
        if(MovePoints.Count > 1)
        {
            AroundMove();
        }
    }

    private void AroundMove()
    {
        // Point 지점에 도달 했을 경우
        if (_agent.velocity.sqrMagnitude >= 0.2f * 0.2f && _agent.remainingDistance <= 0.5f)
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

            _agent.SetDestination(MovePoints[_moveCount].position);
        }
    }
}