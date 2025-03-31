using DefineEnum.NonePlayerDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalk : BaseNpcStatAction
{
    protected override NonePlayerAction NpcAction => NonePlayerAction.Talk;
    public override NPCAttachData npcData => null;
    public override bool IsTalkWithPlayer => false;
    public override float MoveSpeed => 3f;
    public override Transform SpotPos => null;

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
