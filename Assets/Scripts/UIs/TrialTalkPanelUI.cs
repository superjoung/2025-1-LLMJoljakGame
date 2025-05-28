using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrialTalkPanelUI : NPCTalkPanelUI
{
    private enum TrialTexts
    {
        NPCNameText1,
        NPCNameText2,
        NPCNameText3,
        NPCNameText4,
        NPCNameText5,
        LastStatementText,
        LastCheckText,
        ResultText
    }
    private enum TrialImages
    {
        NPCImage1,
        NPCImage2,
        NPCImage3,
        NPCImage4,
        NPCImage5,
        Result,
        Loading,
    }
    private enum Buttons
    {
        CheckButton1,
        CheckButton2,
        CheckButton3,
        CheckButton4,
        CheckButton5,
        LastCheckButton,
    }

    private List<string> _lastStatements = new();
    private List<bool> _isClicked = new();
	private int _isClickedCount = 0;
    private string _lastCheckNpcName;
    
    public override void Init()
    {
        Bind<TMP_Text>(typeof(TrialTexts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(TrialImages));
        StartCoroutine(LLMConnectManager.Instance.GetFinalStatements(SetFinalStatements));
        for (int i = 0; i < 5; i++)
        {
            int idx = i;
            _isClicked.Add(false);
            GetImage((int)TrialImages.NPCImage1 + i).sprite = LLMConnectManager.Instance.GetNpcPortraitToID(i);
            GetButton((int)Buttons.CheckButton1 + i).onClick.AddListener(() =>
                {
                    if (_isClicked[idx])
                    {
                        return;
                    }

                    _isClicked[idx] = true;
                    GetText((int)TrialTexts.LastStatementText).text = _lastStatements[idx];
                    GetImage((int)TrialImages.NPCImage1 + idx).color = new Color(0.5f, 0.5f, 0.5f);
                    GetButton((int)Buttons.CheckButton1 + idx).interactable = false;
                    GetButton((int)Buttons.CheckButton1 + idx).onClick.RemoveAllListeners();
                    _isClickedCount++;
                    if (_isClickedCount < 5) { return; }
                    
                    GetButton((int)Buttons.LastCheckButton).gameObject.SetActive(true);
                    GetButton((int)Buttons.LastCheckButton).onClick.AddListener(() =>
                    {
                        GetText((int)TrialTexts.LastStatementText).text = "최종 선택을 하십시오.";
                        for (int j = 0; j < 5; j++)
                        {
                            int lastCheck = j;
                            GetImage((int)TrialImages.NPCImage1 + j).color = Color.white;
                            GetButton((int)Buttons.CheckButton1 + j).onClick.AddListener(() => { SetFinalSuspect(lastCheck); });
                            GetButton((int)Buttons.CheckButton1 + j).interactable = true;
                        }
                        GetButton((int)Buttons.LastCheckButton).onClick.RemoveAllListeners();
                        GetButton((int)Buttons.LastCheckButton).onClick.AddListener(PrintResult);
                    });
                }
            );
        }
        
        GetButton((int)Buttons.LastCheckButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CheckButton1).transform.parent.parent.gameObject.SetActive(false);
        GetText((int)TrialTexts.LastStatementText).transform.parent.gameObject.SetActive(false);
        GetImage((int)TrialImages.Result).gameObject.SetActive(false);
    }

    private void SetFinalStatements(Dictionary<string, string> finalStatements)
    {
        GetImage((int)TrialImages.Loading).gameObject.SetActive(false);
        GetText((int)TrialTexts.LastStatementText).transform.parent.gameObject.SetActive(true);
        GetButton((int)Buttons.CheckButton1).transform.parent.parent.gameObject.SetActive(true);
        int count = 0;
        foreach (var item in finalStatements)
        {
            GetText((int)TrialTexts.NPCNameText1 + count).text = item.Key;
            _lastStatements.Add(item.Value);
            count++;
        }
    }

    private void SetFinalSuspect(int idx)
    {
        GetText((int)TrialTexts.LastCheckText).text = "최종 선택 완료";
        string npcName = GetText((int)TrialTexts.NPCNameText1 + idx).text;
        _lastCheckNpcName = npcName;
        GetText((int)TrialTexts.LastStatementText).text = "최종 선택을 하십시오.\n선택한 용의자: " + npcName;  
    }

    private void PrintResult()
    {
        print(LLMConnectManager.Instance.GetWitchSuspect().name);
        GetButton((int)Buttons.LastCheckButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CheckButton1).transform.parent.parent.gameObject.SetActive(false);
        GetText((int)TrialTexts.LastStatementText).transform.parent.gameObject.SetActive(false);
        GetImage((int)TrialImages.Result).gameObject.SetActive(true);
        bool isWitch = LLMConnectManager.Instance.GetWitchSuspect().name == _lastCheckNpcName;
        string text = "플레이어가 선택한 " + _lastCheckNpcName + "은(는)...\n";
        GetText((int)TrialTexts.ResultText).text = text + (isWitch ? "마녀가 맞았습니다." : "마녀가 아니었습니다.");
    }
}
