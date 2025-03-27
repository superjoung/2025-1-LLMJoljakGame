using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;

public class NPCMove : CharacterAction
{
    public override NonePlayerAction NpcAction => NonePlayerAction.Move;
    public override int ID => -1;
    public override bool IsTalkWithPlayer => false;

    public override void Init()
    {
        base.Init();
    }

    public override void ActionEnd()
    {
        base.ActionEnd();
    }

    public override void ActionStart()
    {
        base.ActionStart();
    }

    public override void ActionUpdate()
    {
        base.ActionUpdate();
    }
}
