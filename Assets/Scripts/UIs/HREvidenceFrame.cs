using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HREvidenceFrame : BaseUI
{
    enum Images
    {
        SelectButton
    }
    enum Buttons
    {
        SelectButton
    }
    enum Texts
    {
        EvidenceName
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.HREvidenceFrame;
    public string EvidenceID = string.Empty;

    public void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        // 해당 이미지 연결
        if(EvidenceID != string.Empty)
        {
            Sprite sprite = Resources.Load<Sprite>("Images/UI/" + EvidenceID);
            Debug.Log(sprite);
            GetImage((int)Images.SelectButton).sprite = Resources.Load<Sprite>("Images/UI/" + EvidenceID);
        }
        else
        {
            Debug.Log("[WARN]HREvidenceFrame(Init) - 올바른 증거 ID 값이 아닙니다.");
        }

        // 이벤트 등록
        GetButton((int)Buttons.SelectButton).gameObject.BindEvent(OnClickEvidenceButton);
        GetText((int)Texts.EvidenceName).text = GameManager.Instance.GetEvidenceDatas(EvidenceID)[1];
    }

    public void OnClickEvidenceButton(PointerEventData data)
    {
        NoneCharacterManager.Instance.SaveEvidenceData = EvidenceID;
    }
}
