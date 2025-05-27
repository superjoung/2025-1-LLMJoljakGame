using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvidenceMiniMapPopUpUI : BaseUI
{

    protected override bool IsSorting => true;
    public override UIName ID => UIName.EvidenceMiniMapPopUpUI;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
    }
}
