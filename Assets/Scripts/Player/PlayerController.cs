using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove _playerMove;
    private bool _canAction // 상호작용키를 누를 수 있는지 확인
    {
        get
        {
            if (!_startTalk && !_startHearing)
            {
                return true;
            }
            return false;
        }
    }
    private bool _startTalk = false;    // Player가 대화를 시작했는지
    private bool _startHearing = false; // Player가 심문을 시작했는지

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
        if (_canAction && NoneCharacterManager.Instance.CanTalkNpcCount != 0)
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

        if(_startTalk)
        {
            NPCTalkUpdate();
        }
        else if (_startHearing)
        {
            NPCHearingUpdate();
        }
    }

    private void NPCTalkStart()
    {
        _startTalk = true;
        _playerMove.CanMove = false; // 화면은 돌아갈 수 있도록 설정

        // TEMP : 임의로 여러개의 NPC가 겹처있을 때 하나를 지정해서 대화 시도 추후 다중 대화 기능이 필요하다면 수정 필요
        int ID = NoneCharacterManager.Instance.CanStartTalkNpcs[0];
        NoneCharacterManager.Instance.TalkStartWithPlayer(ID);
    }

    private void NPCTalkUpdate()
    {

    }

    private void NPCHearingStart()
    {
        _startHearing = true;
        _playerMove.CanMove = false;
    }

    private void NPCHearingUpdate()
    {

    }
}
