using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GetEvidencePopUpUI : BaseUI
{
    enum Texts
    {
        DescriptionText,
        EvidenceNameText
    }

    enum Images
    {
        EvidenceImage
    }

    enum Buttons
    {
        CheckButton
    }

    protected override bool IsSorting => true;
    public override UIName ID => UIName.EvidenceMiniMapPopUpUI;

    public string EvidenceID;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnClickCheckButton);

        List<string> datas = GameManager.Instance.GetEvidenceDatas(EvidenceID);

        GetText((int)Texts.EvidenceNameText).text = datas[1];
        GetText((int)Texts.DescriptionText).text = datas[2];

        GetImage((int)Images.EvidenceImage).sprite = Resources.Load<Sprite>("Images/UI/" + EvidenceID);
    }

    public void OnClickCheckButton(PointerEventData data)
    {
        UIManager.Instance.CloseUI(this);
    }
}
