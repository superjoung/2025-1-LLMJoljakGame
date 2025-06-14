using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using DefineEnum.GameModeDefine;

public class PlayerMainScreenUI : BaseUI
{
    enum Texts
    {
        DayText,        // 시간 or 일차를 보여주는 Text
        NPCTalkText,
        NPCNameText
    }
    enum Sliders
    {
        DayProgressBar, // 아침 or 밤이 얼마나 남았는지 보여주는 진행도 바
        EvidecneProgressBar // 증거 수집 시간 타이머
    }
    enum Images
    {
        NPCCharImage    // 캐릭터 초상화
    }
    enum Buttons
    {
        OptionButton, // 마우스 설정 및 기타 편의 사항 설정 버튼
        PlayerChatStartButton,   // NPC 대화 시작 버튼
        PlayerChatEndButton,     // NPC 대화 종료 버튼
        SelectStopButton         // 고정 NPC 선택 종료 버튼
    }
    enum GameObjects
    {
        DayBackColor, // 시간 진행도 뒷배경 오브젝트
        DayFillColor, // 시간 채워지는 색 오브젝트
        PlayerChatPopUpUI,  // Player 채팅창 On/Off 용
        DayPanelUI,         // 시간창
        OptionPanelUI,      // 옵션창
        LLMNPCShowChatUI, // LLM 캐릭터 대화 시도 시 뜨는 레이어
        PlayerSelectChatPopUpUI, // FIX 캐릭터와 대화할 때 선택창
        PlayerChatBoundary,      // 고정 캐릭터와 대화 중인 상자 ON/OFF
        HearingEvidencePanelUI,  // 심문 증거 UI 팝업
        HearingContent,          // 심문 증거 넣어두는 부모 오브젝트
        EvidenceModePanelUI,     // 증거 탐색 시 위쪽 상단 증거 UI
        EvidenceContent,         // 증거 탐색 시 찾기 성공한 이미지 보여주기
        NPCTalkPanelUI,          // NPC 대화창 띄우기
        NPCLayer        // 대화 가능 NPC 띄어주기
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.PlayerMainScreenUI;

    private float _textDelay = 0.12f; // 텍스트가 나오는 속도
    private bool _printEnd = false;

    private PlayerMove _playerMove;

    public float Timer
    {
        set
        {
            if (GameManager.Instance.CurrentGameMode == GameFlowMode.FreeMoveMode)
            {
                GetSlider((int)Sliders.DayProgressBar).value = value / 3;
            }
            else if(GameManager.Instance.CurrentGameMode == GameFlowMode.EvidenceMode)
            {
                GetSlider((int)Sliders.EvidecneProgressBar).value = value;
            }
        }
    }

    public void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        // UI 숨기기
        GetObject((int)GameObjects.PlayerChatPopUpUI).SetActive(false);
        GetObject((int)GameObjects.LLMNPCShowChatUI).SetActive(false);
        GetObject((int)GameObjects.PlayerChatBoundary).SetActive(false);
        GetObject((int)GameObjects.HearingEvidencePanelUI).SetActive(false);
        GetObject((int)GameObjects.EvidenceModePanelUI).SetActive(false);
        GetObject((int)GameObjects.NPCTalkPanelUI).SetActive(false);

        // 이벤트 연결
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);
        GetButton((int)Buttons.PlayerChatStartButton).gameObject.BindEvent(OnClickTalkStartButton);
        GetButton((int)Buttons.PlayerChatEndButton).gameObject.BindEvent(OnClickTalkEndButton);
        GetButton((int)Buttons.SelectStopButton).gameObject.BindEvent(OnClickTalkEndButton);
        //GetSlider((int)Sliders.DayProgressBar).onValueChanged.AddListener(OnChangeDayProgressBar);

        GetText((int)Texts.DayText).text = GameManager.Instance.Days + "일차 " + "아침";
        
        _playerMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
    }

    public void ShowChatUI()
    {
        GetObject((int)GameObjects.PlayerChatPopUpUI).gameObject.SetActive(true);
        GetObject((int)GameObjects.LLMNPCShowChatUI).gameObject.SetActive(true);
        GetObject((int)GameObjects.NPCTalkPanelUI).gameObject.SetActive(true);

        GetObject((int)GameObjects.DayPanelUI).gameObject.SetActive(false);
        GetObject((int)GameObjects.OptionPanelUI).gameObject.SetActive(false);

        GetText((int)Texts.NPCNameText).text = NoneCharacterManager.Instance.GetNpcNameToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
        GetImage((int)Images.NPCCharImage).sprite = LLMConnectManager.Instance.GetNpcPortraitToID(NoneCharacterManager.Instance.CurrentTalkNpcID);

        // 선택 가능 보여주기
        foreach (GameObject child in NoneCharacterManager.Instance.TalkList)
        {
            NPCInfoFrame npcInfoFrame = UIManager.Instance.MakeSubItem<NPCInfoFrame>(GetObject((int)GameObjects.NPCLayer).transform);
            // 파괴 오브젝트 추가
            GameManager.Instance.DestoryGameobjects.Add(npcInfoFrame.gameObject);
            npcInfoFrame.id = child.GetComponent<NPCAttachData>().ID;
        }
    }

    public void ShowHearingEvidence()
    {
        GetObject((int)GameObjects.HearingEvidencePanelUI).gameObject.SetActive(true);
        GetObject((int)GameObjects.NPCTalkPanelUI).gameObject.SetActive(true);
        Transform parent = GetObject((int)GameObjects.HearingContent).transform;

        // 게임 매니저에 있는 증거 증거창에 띄어주기
        foreach(string child in GameManager.Instance.EvidenceInventory)
        {
            HREvidenceFrame data = UIManager.Instance.MakeSubItem<HREvidenceFrame>(parent);
            data.EvidenceID = child;
            GameManager.Instance.DestoryGameobjects.Add(data.gameObject);
        }
    }

    public void InputEvidenceData(string ID)
    {
        EVEvidenceFrame evidenceFrame = UIManager.Instance.MakeSubItem<EVEvidenceFrame>(GetObject((int)GameObjects.EvidenceContent).transform);
        // 파괴 오브젝트 추가
        evidenceFrame.EvidenceID = ID;
        GameManager.Instance.DestoryGameobjects.Add(evidenceFrame.gameObject);
    }

    public void ShowFixChatUI()
    {
        GetObject((int)GameObjects.PlayerChatBoundary).SetActive(true);
        GetObject((int)GameObjects.NPCTalkPanelUI).gameObject.SetActive(true);

        GetObject((int)GameObjects.DayPanelUI).gameObject.SetActive(false);
        GetObject((int)GameObjects.OptionPanelUI).gameObject.SetActive(false);

        GetText((int)Texts.NPCNameText).text = NoneCharacterManager.Instance.GetFixNpcNameToID(NoneCharacterManager.Instance.CurrentTalkNpcID);
        GetImage((int)Images.NPCCharImage).sprite = NoneCharacterManager.Instance.GetFixNpcPortraitToID(NoneCharacterManager.Instance.CurrentTalkNpcID);

        // 자식에 아무것도 없을 때 LLM 용의자 생성
        if (GetObject((int)GameObjects.PlayerSelectChatPopUpUI).transform.childCount == 0)
        {
            foreach (GameObject child in NoneCharacterManager.Instance.NpcList)
            {
                // 해당 칸에 프레임 소환 후 아이디 넘겨주기
                UIManager.Instance.MakeSubItem<FixNpcSelectButtonFrame>(GetObject((int)GameObjects.PlayerSelectChatPopUpUI).transform).Id = child.GetComponent<NPCAttachData>().ID;
            }
        }
    }

    public void ShowEvidenceModeUI()
    {
        GetObject((int)GameObjects.DayPanelUI).gameObject.SetActive(false);
        GetObject((int)GameObjects.OptionPanelUI).gameObject.SetActive(false);

        GetObject((int)GameObjects.EvidenceModePanelUI).SetActive(true);
    }

    public void HideChatUI()
    {
        GetObject((int)GameObjects.PlayerChatPopUpUI).gameObject.SetActive(false);
        GetObject((int)GameObjects.LLMNPCShowChatUI).gameObject.SetActive(false);
        GetObject((int)GameObjects.HearingEvidencePanelUI).gameObject.SetActive(false);
        GetObject((int)GameObjects.NPCTalkPanelUI).gameObject.SetActive(false);

        GetObject((int)GameObjects.DayPanelUI).gameObject.SetActive(true);
        GetObject((int)GameObjects.OptionPanelUI).gameObject.SetActive(true);
        NoneCharacterManager.Instance.GetNpcToID(NoneCharacterManager.Instance.CurrentTalkNpcID).GetComponent<NPCAttachData>().Agent.isStopped = false;
    }

    public void HideFixChatUI()
    {
        GetObject((int)GameObjects.PlayerChatBoundary).gameObject.SetActive(false);
        GetObject((int)GameObjects.NPCTalkPanelUI).gameObject.SetActive(false);

        GetObject((int)GameObjects.DayPanelUI).gameObject.SetActive(true);
        GetObject((int)GameObjects.OptionPanelUI).gameObject.SetActive(true);
        NoneCharacterManager.Instance.GetFixNpcToID(NoneCharacterManager.Instance.CurrentTalkNpcID).GetComponent<NPCFixAttachData>().Agent.isStopped = false;
    }

    public void HideEvidenceModeUI()
    {
        GetObject((int)GameObjects.DayPanelUI).gameObject.SetActive(true);
        GetObject((int)GameObjects.OptionPanelUI).gameObject.SetActive(true);

        GetObject((int)GameObjects.EvidenceModePanelUI).SetActive(false);
    }

    public void ChangeDays()
    {
        GetText((int)Texts.DayText).text = GameManager.Instance.Days + "일차 " + "아침";
    }

    private void OnClickOptionButton(PointerEventData data)
    {
        // 자유시점 모드일 때만 감도조절할 수 있도록 설정
        if (GameManager.Instance.CurrentGameMode == GameFlowMode.FreeMoveMode)
        {
            // 옵션 판넬 생성
            UIManager.Instance.ShowPopupUI<OptionPopUpUI>();
            _playerMove.CanPlayerAction = false;
        }
    }

    // 대화 시작 함수
    private void OnClickTalkStartButton(PointerEventData data)
    {
        Debug.Log(GetObject((int)GameObjects.PlayerChatPopUpUI).GetComponent<TMP_InputField>().text);
        if (NoneCharacterManager.Instance.CanPlayerEnterText) // NPC 대화 출력이 끝났을 때 
        {
            NoneCharacterManager.Instance.PlayerText = GetObject((int)GameObjects.PlayerChatPopUpUI).GetComponent<TMP_InputField>().text;
            GetObject((int)GameObjects.PlayerChatPopUpUI).GetComponent<TMP_InputField>().text = "";
        }
    }
    // 대화 종료 함수
    private void OnClickTalkEndButton(PointerEventData data)
    {
        // 자유 시점 모드로 이동
        GameManager.Instance.CurrentGameMode = GameFlowMode.FreeMoveMode;
    }

    public void ShowText(string talkText)
    {
        _printEnd = false;
        StartCoroutine(textPrint(talkText));
    }

    IEnumerator textPrint(string text)
    {
        int count = 0;
        GetText((int)Texts.NPCTalkText).text = " ";

        while (count != text.Length)
        {
            if (count < text.Length)
            {
                GetText((int)Texts.NPCTalkText).text += text[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(_textDelay);
        }
        _printEnd = true;
        NoneCharacterManager.Instance.CanPlayerEnterText = true;
    }
}
