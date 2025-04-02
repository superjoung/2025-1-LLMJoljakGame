using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCTalkPanelUI : BaseUI
{
    enum Texts
    {
        NPCNameText, // NPC 이름
        NPCTalkText  // NPC 대화창
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.NPCTalkPanelUI;

    public int NpcId = -1; // ID를 입력받아야함

    public void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
    }
}
