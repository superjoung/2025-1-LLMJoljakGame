using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class NPCTalkPanelUI : BaseUI
{
    enum Texts
    {
        NPCNameText, // NPC 이름
        NPCTalkText  // NPC 대화창
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.NPCTalkPanelUI;

    private float _textDelay = 0.12f; // 텍스트가 나오는 속도
    private bool _printEnd = false;
    private TMP_Text _targetText;

    public void Awake()
    {
        Init();
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
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
    }
}
