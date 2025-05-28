using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GetNotEvidencePopUpUI : BaseUI
{
    enum Buttons
    {
        CheckButton
    }

    protected override bool IsSorting => true;
    public override UIName ID => UIName.EvidenceMiniMapPopUpUI;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnClickCheckButton);
    }

    public void OnClickCheckButton(PointerEventData data)
    {
        UIManager.Instance.CloseUI(this);
    }
}
