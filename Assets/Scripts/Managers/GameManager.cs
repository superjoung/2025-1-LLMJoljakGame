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

    private void Start()
    {
        EvidenceDatas = CSVReader.Read("CSV/EvidenceDatas");
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

    public List<string> GetEvidenceDatas(string EvidenceID)
    {
        List<string> datas = new List<string>();
        int id = int.Parse(EvidenceID.Split("_")[1]) - 1;

        datas.Add(EvidenceDatas[id]["Evidence_Rare"].ToString());
        datas.Add(EvidenceDatas[id]["Evidence_Name"].ToString());
        datas.Add(EvidenceDatas[id]["Evidence_Ex"].ToString());
        return datas.ToList();
    }
}
