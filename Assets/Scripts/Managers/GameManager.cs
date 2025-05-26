using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DefineEnum.GameModeDefine;
using System.Linq;

public partial class GameManager : Singleton<GameManager>
{
    public int Days = 1;
    public bool IsMorning { get { return Days % 2 == 1; } }
    public ParentPrefabs ParentPrefabs;

    // 증거 테이블
    public List<Dictionary<string, object>> EvidenceDatas = new List<Dictionary<string, object>>();

    protected PlayerMainScreenUI _playerMainScreenUI;

    private bool _isMapOpened = false;

    private void Start()
    {
        StartCoroutine(LLMConnectManager.Instance.GetGameSetup());
        EvidenceDatas = CSVReader.Read("CSV/EvidenceDatas");
        ParentPrefabs = GameObject.Find("PrefabsBox").GetComponent<ParentPrefabs>();
        NoneCharacterManager.Instance.NoneCharacterStart();
        _playerMainScreenUI = UIManager.Instance.ShowSceneUI<PlayerMainScreenUI>();
        Init();
    }

    private void Update()
    {
        ModeUpdate();
        InputKeyUpdate();
    }

    private void Init()
    {
        ModeInit();
    }

    public List<string> GetEvidenceDatas(string EvidenceID)
    {
        List<string> datas = new List<string>();
        int id = int.Parse(EvidenceID.Split("_")[1]) - 1;

        datas.Add(EvidenceDatas[id]["Evidence_Rare"].ToString());
        datas.Add(EvidenceDatas[id]["Evidence_Name"].ToString());
        datas.Add(EvidenceDatas[id]["Evidence_Ex"].ToString());
        return datas.ToList();
    }

    private void InputKeyUpdate()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            CurrentGameMode = GameFlowMode.EvidenceMode;
        }

        // 미니맵 펼치기 버튼 클릭시
        if (Input.GetKeyDown(KeyCode.M) && CurrentGameMode == GameFlowMode.FreeMoveMode)
        {
            if (!_isMapOpened)
            {
                UIManager.Instance.ShowPopupUI<MiniMapPopUpUI>();
            }
            else
            {
                UIManager.Instance.CloseUI(UIName.MiniMapPopUpUI);
            }
            _isMapOpened = !_isMapOpened;
        }

        // 탐색 종료 버튼
        if (Input.GetKeyDown(KeyCode.R) && CurrentGameMode == GameFlowMode.FreeMoveMode)
        {
            // 바로 종료
            Timer = 300;
        }
    }
}
