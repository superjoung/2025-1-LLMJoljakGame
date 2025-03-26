using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpeed = 5f;
    public float Gravity = 10f;
    public float CameraXSpeed = 8f;

    private CharacterController _characterController;
    private Vector3 _playerMoveVector;
    private float _mouseX = 0f;

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
        transform.localEulerAngles = new Vector3(0, _mouseX, 0);
        // Player°¡ ¶¥¿¡ ´êÁö ¾Ê¾Ò´Ù¸é
        if (_characterController.isGrounded)
        {
            _playerMoveVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            _playerMoveVector = _characterController.transform.TransformDirection(_playerMoveVector);
        }
        else _playerMoveVector.y -= Gravity * Time.deltaTime;

        _characterController.Move(_playerMoveVector * Time.deltaTime * PlayerSpeed);
    }
}
