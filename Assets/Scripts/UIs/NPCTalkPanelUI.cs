using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCTalkPanelUI : BaseUI
{
    protected enum Texts
    {
        NPCNameText, // NPC 이름
        NPCTalkText  // NPC 대화창
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.NPCTalkPanelUI;

    private float _textDelay = 0.12f; // 텍스트가 나오는 속도
    private bool _printEnd = false;
    protected TMP_Text _targetText;

    public void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));

        // 무조건 LLM NPC먼저 동작하도록 설정
        if(NoneCharacterManager.Instance.CanTalkNpcCount > 0)
        {
            GetText((int)Texts.NPCNameText).text = NoneCharacterManager.Instance.GetNpcNameToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
        }
        // 고정 NPC에서 스크립트 실행 시 else 실행
        else
        {
            GetText((int)Texts.NPCNameText).text = NoneCharacterManager.Instance.GetFixNpcNameToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
        }
        _targetText = GetText((int)Texts.NPCTalkText);
    }

    public void ShowText(string talkText)
    {
        _printEnd = false;
        StartCoroutine(textPrint(talkText));
    }

    IEnumerator textPrint(string text)
    {
        int count = 0;
        _targetText.text = " ";

        while (count != text.Length)
        {
            if (count < text.Length)
            {
                _targetText.text += text[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(_textDelay);
        }
        _printEnd = true;
        NoneCharacterManager.Instance.CanPlayerEnterText = true;
    }
}
