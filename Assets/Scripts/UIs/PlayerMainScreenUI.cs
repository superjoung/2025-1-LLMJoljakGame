using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMainScreenUI : BaseUI
{
    enum Texts
    {
        DayText
    }
    enum Buttons
    {

    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.PlayerMainScreenUI;

    public void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetText((int)Texts.DayText).text = GameManager.Instance.Days + "일차 " + (GameManager.Instance.IsMorning ? "아침" : "밤");
    }
}
