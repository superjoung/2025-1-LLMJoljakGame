using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixNpcSelectButtonFrame : BaseUI
{
    enum Texts
    {
        NpcName
    }
    enum Buttons
    {
        SelectButton
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.FixNPCSelectButtonUI;

    public int Id = -1;

    private Button _selectButton;

    public void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        //GetText((int)Texts.NpcName).text = NoneCharacterManager.Instance.GetNpcNameToID(Id);
        GetText((int)Texts.NpcName).text = NoneCharacterManager.Instance.TempLLMNpcNames[Id];

        _selectButton = GetButton((int)Buttons.SelectButton);
        Debug.Log(_selectButton);
        _selectButton.gameObject.BindEvent(OnClickNpcNameButton);
    }

    public void OnClickNpcNameButton(PointerEventData data)
    {
        if (NoneCharacterManager.Instance.CanPlayerEnterText) // NPC 대화 출력이 끝났을 때 
        {
            // 버튼 클릭시 대화 시작
            NoneCharacterManager.Instance.GetFixTalkString(NoneCharacterManager.Instance.GetTalkAnswerText(Id));
        }
    }
}
