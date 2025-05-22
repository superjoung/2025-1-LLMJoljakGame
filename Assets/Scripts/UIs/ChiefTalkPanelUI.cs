using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChiefTalkPanelUI : NPCTalkPanelUI
{
    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        GetText((int)Texts.NPCNameText).text = "촌장";
        _targetText = GetText((int)Texts.NPCTalkText);
    }
}
