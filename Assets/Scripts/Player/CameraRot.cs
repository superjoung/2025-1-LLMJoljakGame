using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRot : MonoBehaviour
{
    [SerializeField] private float mouseSpeed = 8f; //ȸ���ӵ�
    private float mouseY = 0f; //���Ʒ� ȸ������ ���� ����

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mouseY += Input.GetAxis("Mouse Y") * mouseSpeed;

        mouseY = Mathf.Clamp(mouseY, -50f, 30f);

        transform.localEulerAngles = new Vector3(-mouseY, 0, 0);
    }
}
