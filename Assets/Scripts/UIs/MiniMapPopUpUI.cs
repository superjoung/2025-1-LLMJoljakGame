using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapPopUpUI : BaseUI
{
    enum Buttons
    {
        ExitButton  // 나가기 버튼
    }

    protected override bool IsSorting => true;
    public override UIName ID => UIName.MiniMapPopUpUI;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
    }

    private void OnClickExitButton(PointerEventData data)
    {
        UIManager.Instance.CloseUI(this);
    }
}
