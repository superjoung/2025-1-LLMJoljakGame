using System.Collections;
using System.Collections.Generic;
using LLM;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
        CheckButton,
        StartButton,
        ExitButton,
    }
    private enum ChiefImages
    {
        Loading,
        NPCImage1,
        NPCImage2,
        NPCImage3,
        NPCImage4,
        NPCImage5,
        Logo,
    }
    private bool _isVillageTextOn = true;
    
    public override void Init()
    {
        Bind<TMP_Text>(typeof(ChiefTexts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(ChiefImages));
        
        GetButton((int)Buttons.CheckButton).gameObject.BindEvent(OnClickCheckButton);
        GetButton((int)Buttons.StartButton).onClick.AddListener(() =>
        {
            StartCoroutine(LLMConnectManager.Instance.GetGameSetup(LoadChiefStatement));
            GetImage((int)ChiefImages.Logo).gameObject.SetActive(false);
            GetImage((int)ChiefImages.Loading).gameObject.SetActive(true);
        });
        GetButton((int)Buttons.ExitButton).onClick.AddListener(() =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
        GetText((int)ChiefTexts.ChiefName).text = "촌장";
        _targetText = GetText((int)ChiefTexts.ChiefText);

        for (int i = 0; i < 5; i++)
        {
            GetText((int)ChiefTexts.NPCNameText1 + i).text = "<UNK>";
            GetText((int)ChiefTexts.NPCTalkText1 + i).text = "<UNK>";
        }
        GetText((int)ChiefTexts.NPCNameText1).transform.parent.parent.gameObject.SetActive(false);
        GetImage((int)ChiefImages.Loading).gameObject.SetActive(false);
        GetButton((int)Buttons.CheckButton).gameObject.SetActive(false);
        GetText((int)ChiefTexts.ChiefName).transform.parent.gameObject.SetActive(false);
    }

    private void LoadChiefStatement()
    {
        StartCoroutine(LLMConnectManager.Instance.GetChiefStatement(SetChiefDialogue));
    }

    private void SetChiefDialogue(string statement)
    {
        FinishLoading();
        ShowText(statement);
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
                GetImage((int)ChiefImages.NPCImage1 + i).sprite = LLMConnectManager.Instance.GetNpcPortraitToID(i);
                GetText((int)ChiefTexts.NPCNameText1 + i).text = suspect.name;
                string talkText = "직업: " + suspect.occupation + "\n" + '"' + suspect.statement + '"';
                GetText((int)ChiefTexts.NPCTalkText1 + i).text = talkText;
            }
            GetText((int)ChiefTexts.NPCNameText1).transform.parent.parent.gameObject.SetActive(true);
        }
        else
        {
            // 씬 넘어가기
            SceneManager.LoadScene("MainScene");
        }
    }

    public void FinishLoading()
    {
        GetImage((int)ChiefImages.Loading).gameObject.SetActive(false);
        GetButton((int)Buttons.CheckButton).gameObject.SetActive(true);
        GetText((int)ChiefTexts.ChiefName).transform.parent.gameObject.SetActive(true);   
    }
}
