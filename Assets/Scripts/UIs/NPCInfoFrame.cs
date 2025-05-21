using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCInfoFrame : BaseUI
{
    enum Texts
    {
        NPCName
    }
    enum Images
    {
        NPCImage
    }
    enum Buttons
    {
        NPCButton
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.NPCTalkPanelUI;

    public int id = -1;

    public void Start()
    {
        Init();
    }

    public override void Init()
    {   
        base.Init();
        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        GetText((int)Texts.NPCName).text = NoneCharacterManager.Instance.GetNpcNameToID(id);

        GetButton((int)Buttons.NPCButton).gameObject.BindEvent(SelectTalkNpc);
        gameObject.GetComponent<Canvas>().sortingOrder = 1;
    }

    private void SelectTalkNpc(PointerEventData data)
    {
        NoneCharacterManager.Instance.CurrentTalkNpcID = id;
        NoneCharacterManager.Instance.PlayerLookAtToNpc(NoneCharacterManager.Instance.GetNpcToID(id));
    }
}
