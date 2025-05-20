using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixNPCInteractionPopUpUI : BaseUI
{
    enum Texts
    {
        NpcNameText
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.FixNPCInteractionPopUpUI;

    public int NpcID = 0;

    private void Start()
    {
        Init();
    }

    //private void 

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));

        GetText((int)Texts.NpcNameText).text = NoneCharacterManager.Instance.FixNpcNames[NpcID];
    }
}
