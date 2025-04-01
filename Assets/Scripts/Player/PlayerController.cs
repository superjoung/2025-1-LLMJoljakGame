using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove _playerMove;
    private bool _canAction = true;
    private bool _startTalk = false;
    private bool _startHearing = false;

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
                _canAction = false;
                _startTalk = true;
                _playerMove.CanMove = false;

                NPCTalkStart();
            }
            // 심문키를 눌렀을 때
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                _canAction = false;
                _startHearing = true;
                _playerMove.CanMove = false;

                NPCHearingStart();
            }
        }

        if(_startTalk)
        {
            NPCTalkUpdate();
        }
        else if (_startHearing)
        {
            NPCHearingStart();
        }
    }

    private void NPCTalkStart()
    {
        int ID = NoneCharacterManager.Instance.CanStartTalkNpcs[0];
        GameObject npc = NoneCharacterManager.Instance.GetNpcToID(ID);
        npc.transform.LookAt(gameObject.transform.position);
    }

    private void NPCTalkUpdate()
    {

    }

    private void NPCHearingStart()
    {

    }

    private void NPCHearingUpdate()
    {

    }
}
