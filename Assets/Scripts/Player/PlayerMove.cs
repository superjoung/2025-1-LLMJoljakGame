using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DefineEnum.GameModeDefine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpeed = 5f;
    public float Gravity = 10f;
    public float CameraXSpeed = 8f;
    public float CameraYSpeed = 8f;
    public GameObject PlayerHead;

    public bool CanMove = true; // UI 창 열었을 때
    public bool CanRotate = true; // Player 목 회전
    public bool CanPlayerAction
    {
        get
        {
            return _canPlayerAction;
        }

        set
        {
            CanMove = value;
            CanRotate = value;
            _canPlayerAction = value;
        }
    }


    private CharacterController _characterController;
    private Vector3 _playerMoveVector;
    private float _mouseX = 0f; // 좌우 회전값
    private float _mouseY = 0f; // 위아래 회전값을 담을 변수
    private bool _canPlayerAction;

    private void Start()
    {
        _characterController = gameObject.GetComponent<CharacterController>();
        CanPlayerAction = true;
    }

    private void Update()
    {
        if(GameManager.Instance.CurrentGameMode == GameFlowMode.FreeMoveMode || GameManager.Instance.CurrentGameMode == GameFlowMode.HearingMode)
            Move();
    }

    private void Move()
    {
        if (CanRotate)
        {
            _mouseX += Input.GetAxis("Mouse X") * CameraXSpeed;
            _mouseY += Input.GetAxis("Mouse Y") * CameraYSpeed;

            _mouseY = Mathf.Clamp(_mouseY, -50f, 30f);
            PlayerHead.transform.localEulerAngles = new Vector3(-_mouseY, 0, 0);
            transform.localEulerAngles = new Vector3(0, _mouseX, 0);
        }
        // Player가 땅에 닿지 않았다면
        if (_characterController.isGrounded)
        {
            _playerMoveVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            _playerMoveVector = _characterController.transform.TransformDirection(_playerMoveVector);
        }
        else _playerMoveVector.y -= Gravity * Time.deltaTime;

        if (CanMove) _characterController.Move(_playerMoveVector * Time.deltaTime * PlayerSpeed);
    }
}