using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TESTMOVE : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField]
    Transform _targetTransform;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _agent.SetDestination(_targetTransform.position);
        }
    }
}
