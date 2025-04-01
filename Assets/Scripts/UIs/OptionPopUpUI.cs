using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class OptionPopUpUI : BaseUI
{
    enum Buttons
    {
        ExitButton
    }

    enum Texts
    {
        MouseXSensitivityText,
        MouseYSensitivityText
    }

    enum Sliders
    {
        MouseXSlider,
        MouseYSlider
    }

    protected override bool IsSorting => true;
    public override UIName ID => UIName.OptionPopUpUI;

    private PlayerMove _playerMove;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));
        
        // 이벤트 연결
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        GetSlider((int)Sliders.MouseXSlider).onValueChanged.AddListener(OnChangeMouseXSensitive);
        GetSlider((int)Sliders.MouseYSlider).onValueChanged.AddListener(OnChangeMouseYSensitive);

        // 변수 연결
        _playerMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        GetSlider((int)Sliders.MouseXSlider).value = _playerMove.CameraXSpeed;
        GetText((int)Texts.MouseXSensitivityText).text = GetSlider((int)Sliders.MouseXSlider).value.ToString("F1");
        GetSlider((int)Sliders.MouseYSlider).value = _playerMove.CameraYSpeed;
        GetText((int)Texts.MouseYSensitivityText).text = GetSlider((int)Sliders.MouseYSlider).value.ToString("F1");
    }

    private void OnClickExitButton(PointerEventData data)
    {
        UIManager.Instance.CloseUI(this);
        _playerMove.CanPlayerAction = true;
    }

    private void OnChangeMouseXSensitive(float changeValue)
    {
        if(changeValue == 10f) GetText((int)Texts.MouseXSensitivityText).text = Mathf.Ceil(changeValue).ToString();
        else GetText((int)Texts.MouseXSensitivityText).text = changeValue.ToString("F1");
        _playerMove.CameraXSpeed = changeValue;
    }

    private void OnChangeMouseYSensitive(float changeValue)
    {
        if (changeValue == 10f) GetText((int)Texts.MouseYSensitivityText).text = Mathf.Ceil(changeValue).ToString();
        else GetText((int)Texts.MouseYSensitivityText).text = changeValue.ToString("F1");
        _playerMove.CameraYSpeed = changeValue;
    }
}
