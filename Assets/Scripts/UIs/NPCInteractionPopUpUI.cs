using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCInteractionPopUpUI : BaseUI
{
    enum Texts
    {
        NpcNameText
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.NPCInteractionPopUpUI;

    public int NpcID = 0;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));

        GetText((int)Texts.NpcNameText).text = NoneCharacterManager.Instance.GetNpcNameToID(NpcID);
    }
}
