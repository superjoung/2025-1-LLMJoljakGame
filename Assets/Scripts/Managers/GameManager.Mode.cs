using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.GameModeDefine;

public partial class GameManager
{
    // 필요 세팅 변수
    public string EvidenceSpotName = "";
    public int HearingNpcID = -1;
    
    [SerializeField] private GameObject FreeMovePlayer;
    [SerializeField] private GameObject World;
    [SerializeField] private GameObject HearingRoom;
    private List<GameObject> _destoryGameobjects = new List<GameObject>();
    private GameObject _saveEvidenceSpot = null;

    public GameFlowMode CurrentGameMode
    {
        get
        {
            return _currentGameMode;
        }
        set
        {
            if (CheckModeChange(value))
            {
                ModeChange(_currentGameMode, value);
                _currentGameMode = value;
            }
            else
            {
                Debug.LogWarning("[WARN]GameManager.Mode(CurrentGameMode) - 다음 모드로 변경할 수 없습니다.");
            }
        }
    }

    private GameFlowMode _currentGameMode = GameFlowMode.None;

    // 모드 클래스 초기 실행 시 필요한 함수
    private void ModeInit()
    {
        CurrentGameMode = GameFlowMode.FreeMoveMode;
    }

    // 현재 모드와 바뀔 모드를 입력받아 모드의 시작과 끝에 필요한 값들을 넣어줌
    private void ModeChange(GameFlowMode currentMode, GameFlowMode changeMode)
    {
        // 모드 변경 전 파괴되어야하는 오브젝트 파괴
        foreach(GameObject child in _destoryGameobjects)
        {
            DestroyImmediate(child);
        }

        if (_currentGameMode != GameFlowMode.None)
        {
            ModeEnd(currentMode);
        }

        ModeInit(changeMode);
    }

    // 해당 모드가 끝날 때 필요한 코드
    private void ModeEnd(GameFlowMode mode)
    {
        switch (mode)
        {
            case GameFlowMode.FreeMoveMode:
                FreeMoveEnd();
                break;
            case GameFlowMode.HearingMode:
                HearingEnd();
                break;
            case GameFlowMode.EvidenceMode:
                EvidenceEnd();
                break;
            case GameFlowMode.TalkMode:
                TalkModeEnd();
                break;
            default:
                Debug.LogWarning("[WARN]GameManager(ModeEnd) - 해당 mode는 존재하지 않습니다.");
                break;
        }
    }
    // 해당 모드가 시작될 때 필요한 코드
    private void ModeInit(GameFlowMode mode)
    {
        switch (mode)
        {
            case GameFlowMode.FreeMoveMode:
                FreeMoveInit();
                break;
            case GameFlowMode.HearingMode:
                HearingInit();
                break;
            case GameFlowMode.EvidenceMode:
                EvidenceInit();
                break;
            case GameFlowMode.TalkMode:
                TalkModeInit();
                break;
            default:
                Debug.LogWarning("[WARN]GameManager(ModeInit) - 해당 mode는 존재하지 않습니다.");
                break;
        }
    }

    private bool CheckModeChange(GameFlowMode mode)
    {
        bool canChange = true;
        switch (mode)
        {
            case GameFlowMode.EvidenceMode:
                if(EvidenceSpotName == "") canChange = false;
                break;
            case GameFlowMode.HearingMode:
                if(HearingNpcID == -1) canChange = false;
                break;
        }
        Debug.Log($"[INFO]GameManager.Mode(CheckModeChange) - {mode} 변환 {canChange}");
        return canChange;
    }

    #region Evidence
    private void EvidenceInit()
    {
        // 해당 지역 이름과 동일한 게임 오브젝트 확인
        foreach(Transform child in ParentPrefabs.NpcCameraBox.transform)
        {
            string spotName = child.name.Split("_")[1];
            if(spotName == EvidenceSpotName) // 증거수집을 원하는 지역 이름과 같을 경우
            {
                _saveEvidenceSpot = child.gameObject;
                // 각 올바른 변수끼리 연결
                GameObject player = ResourceManager.Instance.Instantiate("Player/EvidencePlayer", child.GetChild(1).position, null);
                PlayerEvidenceMove move = player.GetComponent<PlayerEvidenceMove>();
                child.GetChild(0).gameObject.SetActive(true);
                move.UseCamera = child.GetChild(0).GetComponent<Camera>();
                move.ClickPos = child.GetChild(2);

                _destoryGameobjects.Add(player);
            }
        }
    }

    private void EvidenceEnd()
    {
        _saveEvidenceSpot.transform.GetChild(0).gameObject.SetActive(false);
        EvidenceSpotName = "";
    }
    #endregion

    #region Hearing
    private void HearingInit()
    {
        FreeMovePlayer.SetActive(true);
        foreach(GameObject child in NoneCharacterManager.Instance.NpcList)
        {
            if (int.Parse(child.name.Split("_")[1]) != HearingNpcID)
            {
                child.SetActive(false);
            }
        }
        GameObject Npc = NoneCharacterManager.Instance.GetNpcToID(HearingNpcID);
        ResourceManager.Instance.Instantiate("HearingRoom", Npc.transform.GetChild(3).position, null);
    }
    private void HearingEnd()
    {
        foreach (GameObject child in NoneCharacterManager.Instance.NpcList)
        {
            if (int.Parse(child.name.Split("_")[1]) != HearingNpcID)
            {
                child.SetActive(true);
            }
        }
        HearingNpcID = -1;
    }
    #endregion

    #region FreeMove
    private void FreeMoveInit()
    {
        FreeMovePlayer.SetActive(true);
    }
    private void FreeMoveEnd()
    {
        FreeMovePlayer.SetActive(false);
    }
    #endregion
    #region Talking
    private void TalkModeInit()
    {
        FreeMovePlayer.SetActive(true);
        _playerMainScreenUI.ShowChatUI();
    }
    private void TalkModeEnd()
    {
        _playerMainScreenUI.HideChatUI();
    }
    #endregion
}
