using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DefineEnum.GameModeDefine;

public partial class GameManager : Singleton<GameManager>
{
    public int Days = 1;
    public bool IsMorning { get { return Days % 2 == 1; } }
    public ParentPrefabs ParentPrefabs;

    protected PlayerMainScreenUI _playerMainScreenUI;

    private void Start()
    {
        StartCoroutine(LLMConnectManager.Instance.GetGameSetup());
        ParentPrefabs = GameObject.Find("PrefabsBox").GetComponent<ParentPrefabs>();
        NoneCharacterManager.Instance.NpcSpawn();
        NoneCharacterManager.Instance.FixNPCSpawn();
        _playerMainScreenUI = UIManager.Instance.ShowSceneUI<PlayerMainScreenUI>();
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            CurrentGameMode = GameFlowMode.EvidenceMode;
        }
    }

    private void Init()
    {
        ModeInit();
    }
}
