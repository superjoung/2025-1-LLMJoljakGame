﻿using System.Collections;
using System.Collections.Generic;
using LLM;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChiefTalkPanelUI : NPCTalkPanelUI
{
    private enum ChiefTexts
    {
        ChiefName, // NPC 이름
        ChiefText,  // NPC 대화창
        NPCNameText1,
        NPCNameText2,
        NPCNameText3,
        NPCNameText4,
        NPCNameText5,
        NPCTalkText1,
        NPCTalkText2,
        NPCTalkText3,
        NPCTalkText4,
        NPCTalkText5,
    }
    private enum Buttons
    {
        CheckButton // 확인 버튼
    }
    private bool _isVillageTextOn = true;
    
    public override void Init()
    {
        Bind<TMP_Text>(typeof(ChiefTexts));
        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnClickCheckButton);
        GetText((int)ChiefTexts.ChiefName).text = "촌장";
        _targetText = GetText((int)ChiefTexts.ChiefText);

        for (int i = 0; i < 5; i++)
        {
            GetText((int)ChiefTexts.NPCNameText1 + i).text = "<UNK>";
            GetText((int)ChiefTexts.NPCTalkText1 + i).text = "<UNK>";
        }
        GetText((int)ChiefTexts.NPCNameText1).transform.parent.parent.gameObject.SetActive(false);
    }

    private void OnClickCheckButton(UnityEngine.EventSystems.PointerEventData data)
    {
        if (_isVillageTextOn)
        {
            _isVillageTextOn = false;
            GetText((int)ChiefTexts.ChiefText).transform.parent.gameObject.SetActive(false);
            GetText((int)ChiefTexts.NPCNameText1).transform.parent.parent.gameObject.SetActive(true);

            for (int i = 0; i < 5; i++)
            {
                Suspect suspect = LLMConnectManager.Instance.GetAllSuspects()[i];
                GetText((int)ChiefTexts.NPCNameText1 + i).text = suspect.name;
                string talkText = "직업: " + suspect.occupation + "\n" + '"' + suspect.statement + '"';
                GetText((int)ChiefTexts.NPCTalkText1 + i).text = talkText;
            }
            GetText((int)ChiefTexts.NPCNameText1).transform.parent.parent.gameObject.SetActive(true);
        }
        else
        {
            // 씬 넘어가기
        }
    }
}
