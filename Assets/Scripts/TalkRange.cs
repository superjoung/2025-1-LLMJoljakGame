using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkRange : MonoBehaviour
{
    public NPCFixAttachData FixAttachData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FixAttachData.CanTalkStart = true;
            Debug.Log($"{FixAttachData.Id} NCP에게 {other.gameObject.name} In.");
            FixAttachData.UINeck.gameObject.SetActive(true);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        UINeck.LookAt(other.transform.position);
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FixAttachData.CanTalkStart = false;
            Debug.Log($"{FixAttachData.Id} NCP에게 {other.gameObject.name} Out.");
            FixAttachData.UINeck.gameObject.SetActive(false);
        }
    }
}
