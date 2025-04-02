using DefineEnum.NonePlayerDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalk : BaseNpcStatAction
{
    protected override NonePlayerAction NpcAction => NonePlayerAction.Talk;
    public override NPCAttachData NpcData { get { return _npcData; }  set { _npcData = value; } }
    public override bool IsTalkWithPlayer
    {
        get
        {
            return _isTalkWithPlayer;
        }
        set
        {
            _isTalkWithPlayer = value;
        }
    }
    public override float MoveSpeed => 3f;
    public override Transform SpotPos { get { return _spotPos; } set { _spotPos = value; } }

    private bool _isTalkWithPlayer = false;
    private NPCAttachData _npcData;
    private Transform _spotPos = null;

    public override void Init()
    {
        base.Init();
    }

    public override void SpotMove()
    {
        base.SpotMove();
    }

    public override void ActionStart()
    {
        base.ActionStart();
    }

    public override void ActionUpdate()
    {
        base.ActionUpdate();
    }

    public override void ActionEnd()
    {
        base.ActionEnd();
    }
}
