using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.GameModeDefine;
using System.Linq;
using LLM;
using Unity.VisualScripting;

public partial class GameManager : Singleton<GameManager>
{
    public int Days
    {
        get
        {
            return _days;
        }
        set
        {
            _days = value;
            EvidenceCount = 2;
        }
    }
    public bool IsMorning { get { return Days % 2 == 1; } }
    public ParentPrefabs ParentPrefabs;
    public Transform PlayerSpawnPosition;

    // 증거 테이블 데이터
    public List<Dictionary<string, object>> EvidenceDatas = new List<Dictionary<string, object>>();
    // 증거 테이블 저장
    //public List<Clue> TempClueDatas = new List<Clue>();
    public List<Clue> ClueDatas = new List<Clue>();

    // 플레이어 증거 담아두기
    public List<string> EvidenceInventory = new List<string>();
        
    protected PlayerMainScreenUI _playerMainScreenUI;

    private bool _isMapOpened = false;
    private int _days = 1;

    private void Awake()
    {
        //StartCoroutine(LLMConnectManager.Instance.GetGameSetup(null));
        base.Awake();
        EvidenceDatas.Clear();
        EvidenceDatas = CSVReader.Read("CSV/EvidenceDatas");
        ParentPrefabs = GameObject.Find("PrefabsBox").GetComponent<ParentPrefabs>();
        NoneCharacterManager.Instance.NoneCharacterAwake();
        _playerMainScreenUI = UIManager.Instance.ShowSceneUI<PlayerMainScreenUI>();
    }

    private void Start()
    {
        Init();
    }

    public void SetClueData(List<Clue> clues)
    {
        ClueDatas = clues;
        CreateClues();
        SetClueInSpot();
    }
    
    // 임시 증거 생성부분 삭제될 예정
    private void CreateClues()
    {
        List<string> locations = new List<string>() { "숲", "성당", "광장", "우물", "집" };
        List<string> ids = new List<string>();
        for(int i = 0; i < EvidenceDatas.Count; i++)
        {
            ids.Add(EvidenceDatas[i]["Evidence_ID"].ToString());
            //Debug.Log($"[INFO]GameManager(CreateClues) - 증거 {ids[i]}가 등록되었습니다.");
        }

        //TempClueDatas.Add(new Clue());
        //TempClueDatas.Add(new Clue());
        //TempClueDatas.Add(new Clue());
        
        // 증거 클래스에 등록
        foreach(Clue child in ClueDatas)
        {
            child.id = ids[Random.Range(0, ids.Count)];
            child.location = locations[Random.Range(0, locations.Count)];
            Debug.Log($"[INFO]GameManager(CreateClues) - 증거 {child.id}가 {child.location}에 배치되었습니다.");
        }
    }

    public void SetClueInSpot()
    {
        // TempClueDatas에서 단서들 전부 안에다 넣어주기
        foreach(Clue child in ClueDatas)
        {
            foreach(Transform EvidenceSpot in ParentPrefabs.NpcEvidenceBox.transform)
            {
                // 증거를 숨겨야하는 장소와 같다면
                if (EvidenceSpot.GetComponent<EvidenceSpotAttachData>().Name.Contains(child.location))
                {
                    Transform currentSpot = EvidenceSpot.GetChild(0);
                    if(EvidenceSpot.childCount > 1)
                    {
                        currentSpot = EvidenceSpot.GetChild(Random.Range(0, currentSpot.childCount));
                    }

                    // 랜덤으로 뽑은 자식들 컨텐츠 중 Evidence 증거 cs 붙이기
                    EvidenceObjectAttachData data = currentSpot.GetChild(Random.Range(0, currentSpot.childCount)).AddComponent<EvidenceObjectAttachData>();
                    // 증거 아이디 넣어주기
                    data.EvidenceID = child.id;
                    data.gameObject.tag = "Evidence";
                }
            }
        }
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
        if (Input.GetKeyDown(KeyCode.R) && (CurrentGameMode == GameFlowMode.FreeMoveMode || CurrentGameMode == GameFlowMode.EvidenceMode))
        {
            // 바로 종료
            Timer = 300;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            NoneCharacterManager.Instance.NoneCharacterStart();
        }
    }

    public void SetLoadingActive(bool isActive)
    {
        if (!isActive)
        {
            NoneCharacterManager.Instance.NoneCharacterStart();
        }

        Time.timeScale = isActive ? 0 : 1;
        GameObject loadingUI = GameObject.Find("LoadingUI");
        if (loadingUI)
        {
            loadingUI.transform.Find("Loading").gameObject.SetActive(isActive);
        }
    }
}
