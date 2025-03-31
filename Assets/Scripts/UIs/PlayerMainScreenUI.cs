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
        DayText // �ð� or ������ �����ִ� Text
    }
    enum Sliders
    {
        DayProgressBar // ��ħ or ���� �󸶳� ���Ҵ��� �����ִ� ���൵ ��
    }
    enum Buttons
    {
        OptionButton // ���콺 ���� �� ��Ÿ ���� ���� ���� ��ư
    }
    enum GameObjects
    {
        DayBackColor, // �ð� ���൵ �޹�� ������Ʈ
        DayFillColor  // �ð� ä������ �� ������Ʈ
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

        // �̺�Ʈ ����
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);
        GetSlider((int)Sliders.DayProgressBar).onValueChanged.AddListener(OnChangeDayProgressBar);

        GetText((int)Texts.DayText).text = GameManager.Instance.Days + "���� " + (GameManager.Instance.IsMorning ? "��ħ" : "��");
    }

    private void OnClickOptionButton(PointerEventData data)
    {
        // �ɼ� �ǳ� ����
        UIManager.Instance.ShowPopupUI<OptionPopUpUI>();
    }

    private void OnChangeDayProgressBar(float changeValue)
    {

    }
}
