using System.Collections.Generic;
using LLM;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    }
    private enum TrialImages
    {
        NPCImage1,
        NPCImage2,
        NPCImage3,
        NPCImage4,
        NPCImage5,
    }
    private enum Buttons
    {
        CheckButton1,
        CheckButton2,
        CheckButton3,
        CheckButton4,
        CheckButton5,
    }

    private List<string> _lastStatements = new();
    private List<bool> _isClicked = new();
    
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
            GetButton((int)Buttons.CheckButton1 + i).gameObject.BindEvent((PointerEventData data) => {
                print(idx);
                if (_isClicked[idx])
                {
                    return;
                }
                _isClicked[idx] = true;
                GetText((int)TrialTexts.LastStatementText).text = _lastStatements[idx];
                GetImage((int)TrialImages.NPCImage1 + idx).color = new Color(0.5f, 0.5f, 0.5f);
            });
        }
    }

    private void SetFinalStatements(Dictionary<string, string> finalStatements)
    {
        int count = 0;
        foreach (var item in finalStatements)
        {
            GetText((int)TrialTexts.NPCNameText1 + count).text = item.Key;
            _lastStatements.Add(item.Value);
            count++;
        }
    }
}
