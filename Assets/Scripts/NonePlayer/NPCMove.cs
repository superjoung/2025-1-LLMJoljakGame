using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DefineEnum.NonePlayerDefine;

public class NPCMove : BaseNpcStatAction
{
    public override NonePlayerAction NpcAction => NonePlayerAction.Move;
    public override int ID => -1;
    public override bool IsTalkWithPlayer => false;
    public override float MoveSpeed => 5;

    // NPC NavMeshAgen »Æ¿Œ
    private NavMeshAgent _agent;
    private GameObject _targetNpc;
    public SphereCollider TalkRange;
    public Transform MoveSpot = null;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        _agent = GetComponent<NavMeshAgent>();
        _targetNpc = NoneCharacterManager.Instance.GetNpcToID(ID);
    }

    public override void ActionEnd()
    {
        base.ActionEnd();
    }

    public override void ActionStart()
    {
        base.ActionStart();
        _agent.SetDestination(MoveSpot.position);
    }

    public override void ActionUpdate()
    {
        base.ActionUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
