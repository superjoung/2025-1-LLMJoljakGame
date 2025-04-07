using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DefineEnum.GameModeDefine;

public partial class GameManager : MonoBehaviour
{
    public int Days = 1;
    public bool IsMorning { get { return Days % 2 == 1; } }
    public ParentPrefabs ParentPrefabs;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(nameof(GameManager));
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private static GameManager _instance;

    private void Start()
    {
        ParentPrefabs = GameObject.Find("PrefabsBox").GetComponent<ParentPrefabs>();
        NoneCharacterManager.Instance.NpcSpawn();
        UIManager.Instance.ShowSceneUI<PlayerMainScreenUI>();
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
