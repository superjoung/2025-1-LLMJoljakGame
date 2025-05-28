using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class EvidenceMiniMapPopUpUI : BaseUI
{
    enum GameObjects
    {
        EvidencePoints  // 증거 위치 버튼
    }

    protected override bool IsSorting => true;
    public override UIName ID => UIName.EvidenceMiniMapPopUpUI;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
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
                child.name = NoneCharacterManager.Instance.GetNpcNameToID(idCount) + "_" + idCount.ToString();
                text.text = NoneCharacterManager.Instance.GetNpcNameToID(idCount) + " 집";

                //child.name = NoneCharacterManager.Instance.TempLLMNpcNames[idCount] + "_" + idCount.ToString();
                //text.text = NoneCharacterManager.Instance.TempLLMNpcNames[idCount] + " 집";
                idCount += 1;
            }
            child.gameObject.BindEvent(OnClickEvidenceButton);
        }
    }

    private void OnClickEvidenceButton(PointerEventData data)
    {
        GameManager.Instance.EvidenceSpotName = EventSystem.current.currentSelectedGameObject.name.Split("_")[1];
        GameManager.Instance.CurrentGameMode = DefineEnum.GameModeDefine.GameFlowMode.EvidenceMode;
        UIManager.Instance.CloseUI(this);
    }
}
