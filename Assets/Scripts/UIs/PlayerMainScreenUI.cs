using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerMainScreenUI : BaseUI
{
    enum Texts
    {
        DayText // 시간 or 일차를 보여주는 Text
    }
    enum Sliders
    {
        DayProgressBar // 아침 or 밤이 얼마나 남았는지 보여주는 진행도 바
    }
    enum Buttons
    {
        OptionButton, // 마우스 설정 및 기타 편의 사항 설정 버튼
        PlayerChatStartButton,   // NPC 대화 시작 버튼
        PlayerChatEndButton     // NPC 대화 종료 버튼
    }
    enum InputFields
    {
        PlayerInput
    }
    enum GameObjects
    {
        DayBackColor, // 시간 진행도 뒷배경 오브젝트
        DayFillColor, // 시간 채워지는 색 오브젝트
        PlayerChatPopUpUI   // Player 채팅창 On/Off 용
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.PlayerMainScreenUI;
    private PlayerMove _playerMove;

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

        GetObject((int)GameObjects.PlayerChatPopUpUI).SetActive(false);

        // 이벤트 연결
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);
        GetButton((int)Buttons.PlayerChatStartButton).gameObject.BindEvent(OnClickTalkStartButton);
        GetButton((int)Buttons.PlayerChatEndButton).gameObject.BindEvent(OnClickTalkEndButton);
        GetSlider((int)Sliders.DayProgressBar).onValueChanged.AddListener(OnChangeDayProgressBar);

        GetText((int)Texts.DayText).text = GameManager.Instance.Days + "일차 " + (GameManager.Instance.IsMorning ? "아침" : "밤");
        
        _playerMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
    }

    public void ShowChatUI()
    {
        GetObject((int)GameObjects.PlayerChatPopUpUI).gameObject.SetActive(true);
    }

    public void HideChatUI()
    {
        GetObject((int)GameObjects.PlayerChatPopUpUI).gameObject.SetActive(false);
    }

    private void OnClickOptionButton(PointerEventData data)
    {
        // 옵션 판넬 생성
        UIManager.Instance.ShowPopupUI<OptionPopUpUI>();
        _playerMove.CanPlayerAction = false;
    }

    // 대화 시작 함수
    private void OnClickTalkStartButton(PointerEventData data)
    {

    }
    // 대화 종료 함수
    private void OnClickTalkEndButton(PointerEventData data)
    {
        // Player Chat 끄기
        GetObject((int)GameObjects.PlayerChatPopUpUI).SetActive(false);
    }

    private void OnChangeDayProgressBar(float changeValue)
    {

    }
}
