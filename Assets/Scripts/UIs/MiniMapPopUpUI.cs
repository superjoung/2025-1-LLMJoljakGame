using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapPopUpUI : BaseUI
{
    enum Buttons
    {
        ExitButton  // 나가기 버튼
    }
    enum GameObjects
    {
        EvidencePoints  // 증거 위치 버튼
    }

    protected override bool IsSorting => true;
    public override UIName ID => UIName.MiniMapPopUpUI;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        NpcHouseNameSetting();
    }

    private void NpcHouseNameSetting()
    {
        int idCount = 0;
        foreach (Transform child in GetObject((int)GameObjects.EvidencePoints).transform)
        {
            TMP_Text text = child.GetChild(0).GetComponent<TMP_Text>();
            // NPC 집일 때 변경
            if (text.text.Contains("NPC"))
            {
                // TMP변경
                //text.text = NoneCharacterManager.Instance.GetNpcNameToID(idCount) + " 집";
                text.text = NoneCharacterManager.Instance.TempLLMNpcNames[idCount] + " 집";
                idCount += 1;
            }
        }
    }

    private void OnClickExitButton(PointerEventData data)
    {
        UIManager.Instance.CloseUI(this);
    }
}
