using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class NPCTalkPanelUI : BaseUI
{
    private enum Texts
    {
        NPCNameText, // NPC 이름
        NPCTalkText  // NPC 대화창
    }

    private enum Images
    {
        NPCImage,
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.NPCTalkPanelUI;

    private float _textDelay = 0.12f; // 텍스트가 나오는 속도
    private bool _printEnd = false;
    protected TMP_Text _targetText;
    private List<GameObject> _hideList = new List<GameObject>();

    public void Awake()
    {
        Init();
    }

    public void OnDisable()
    {
        foreach (GameObject child in _hideList)
        {
            child.gameObject.SetActive(true);
        }
        _hideList.Clear();
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        // 무조건 LLM NPC먼저 동작하도록 설정
        if(NoneCharacterManager.Instance.CanTalkNpcCount > 0)
        {
            GetText((int)Texts.NPCNameText).text = NoneCharacterManager.Instance.GetNpcNameToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
            GetImage((int)Images.NPCImage).sprite = LLMConnectManager.Instance.GetNpcPortraitToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
        }
        // 고정 NPC에서 스크립트 실행 시 else 실행
        else
        {
            GetText((int)Texts.NPCNameText).text = NoneCharacterManager.Instance.GetFixNpcNameToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
            GetImage((int)Images.NPCImage).sprite = NoneCharacterManager.Instance.GetFixNpcPortraitToID(NoneCharacterManager.Instance.CurrentTalkNpcID);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "TalkRange" && other.tag != "LLMNpc" && other.tag != "FixNpc")
        {
            _hideList.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
