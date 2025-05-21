using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FixNpcSelectButtonFrame : BaseUI
{
    enum Texts
    {
        NpcName
    }
    enum Buttons
    {
        SelectButton
    }

    protected override bool IsSorting => false;
    public override UIName ID => UIName.FixNPCSelectButtonUI;

    public int Id = -1;

    public void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GetText((int)Texts.NpcName).text = NoneCharacterManager.Instance.GetNpcNameToID(Id);

        //GetButton((int)Buttons.SelectButton).gameObject.BindEvent(SelectTalkNpc);
        //gameObject.GetComponent<Canvas>().sortingOrder = 1;
    }
}
