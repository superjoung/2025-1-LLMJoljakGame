using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpeed = 5f;
    public float Gravity = 10f;
    public float CameraXSpeed = 8f;
    public float CameraYSpeed = 8f;

    private CharacterController _characterController;
    private Vector3 _playerMoveVector;
    private float _mouseX = 0f; // 좌우 회전값
    private float mouseY = 0f; // 위아래 회전값을 담을 변수

    private void Start()
    {
        _characterController = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        _mouseX += Input.GetAxis("Mouse X") * CameraXSpeed;
        mouseY += Input.GetAxis("Mouse Y") * CameraYSpeed;

        mouseY = Mathf.Clamp(mouseY, -50f, 30f);
        transform.localEulerAngles = new Vector3(-mouseY, _mouseX, 0);
        // Player가 땅에 닿지 않았다면
        if (_characterController.isGrounded)
        {
            _playerMoveVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            _playerMoveVector = _characterController.transform.TransformDirection(_playerMoveVector);
        }
        else _playerMoveVector.y -= Gravity * Time.deltaTime;

        _characterController.Move(_playerMoveVector * Time.deltaTime * PlayerSpeed);
    }
}
