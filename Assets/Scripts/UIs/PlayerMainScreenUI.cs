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
        OptionButton // 마우스 설정 및 기타 편의 사항 설정 버튼
    }
    enum GameObjects
    {
        DayBackColor, // 시간 진행도 뒷배경 오브젝트
        DayFillColor  // 시간 채워지는 색 오브젝트
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.PlayerMainScreenUI;

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

        // 이벤트 연결
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);
        GetSlider((int)Sliders.DayProgressBar).onValueChanged.AddListener(OnChangeDayProgressBar);

        GetText((int)Texts.DayText).text = GameManager.Instance.Days + "일차 " + (GameManager.Instance.IsMorning ? "아침" : "밤");
    }

    private void OnClickOptionButton(PointerEventData data)
    {
        // 옵션 판넬 생성
        UIManager.Instance.ShowPopupUI<OptionPopUpUI>();
    }

    private void OnChangeDayProgressBar(float changeValue)
    {

    }
}
