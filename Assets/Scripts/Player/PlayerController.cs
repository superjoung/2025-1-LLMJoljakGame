using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.GameModeDefine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove _playerMove;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _playerMove = GetComponent<PlayerMove>();
    }

    // Player Input 확인
    private void Update()
    {
        // 게임 모드에 따라 업데이트 시작
        PlayerControllerUpdate();
    }

    private void PlayerControllerUpdate()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameFlowMode.FreeMoveMode:
                _playerMove.CanPlayerAction = true;
                if (NoneCharacterManager.Instance.CanTalkNpcCount != 0)
                {
                    // 대화키를 눌렀을 때
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        NPCTalkStart();
                    }
                    // 심문키를 눌렀을 때
                    else if (Input.GetKeyDown(KeyCode.Q))
                    {
                        NPCHearingStart();
                    }
                }
                break;
            case GameFlowMode.TalkMode:
                NPCTalkUpdate();
                break;
            case GameFlowMode.HearingMode:
                NPCHearingUpdate();
                break;
        }
    }

    private void NPCTalkStart()
    {
        // TEMP : 임의로 여러개의 NPC가 겹처있을 때 하나를 지정해서 대화 시도 추후 다중 대화 기능이 필요하다면 수정 필요
        int ID = NoneCharacterManager.Instance.CanStartTalkNpcs[0];
        NoneCharacterManager.Instance.TalkStartWithPlayer(ID);
        GameManager.Instance.CurrentGameMode = GameFlowMode.TalkMode;
        _playerMove.CanPlayerAction = false;
    }

    private void NPCTalkUpdate()
    {

    }

    private void NPCHearingStart()
    {
        int ID = NoneCharacterManager.Instance.CanStartTalkNpcs[0];
        NoneCharacterManager.Instance.GetNpcToID(ID).transform.LookAt(transform.position);
        GameManager.Instance.HearingNpcID = ID;
        GameManager.Instance.CurrentGameMode = DefineEnum.GameModeDefine.GameFlowMode.HearingMode;
        _playerMove.CanPlayerAction = false;
    }

    private void NPCHearingUpdate()
    {

    }
}
