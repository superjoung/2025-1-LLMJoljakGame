using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NPCAttachData : MonoBehaviour
{
    public int ID;
    public Transform UIPos;
    public Transform UINeck;

    public bool IsTalkStart = false;  // 대화 시작
    public bool CanTalkStart = false; // 대화 시작 가능 (범위 안에 들어왔을 때)

    private void Start()
    {
        ID = int.Parse(gameObject.name.Split("_")[1]);
        CharacterTalkPopUpUI popUpUI =  UIManager.Instance.ShowNPCUI<CharacterTalkPopUpUI>(UIPos);
        popUpUI.NpcID = ID;
        UINeck.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CanTalkStart = true;
            Debug.Log($"{ID} NCP에게 {other.gameObject.name} In.");
            UINeck.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            UINeck.LookAt(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CanTalkStart = false;
            Debug.Log($"{ID} NCP에게 {other.gameObject.name} Out.");
            UINeck.gameObject.SetActive(false);
        }
    }
}
