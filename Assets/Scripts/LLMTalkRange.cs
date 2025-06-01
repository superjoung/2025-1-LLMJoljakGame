using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLMTalkRange : MonoBehaviour
{
    public NPCAttachData AttachData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AttachData.CanTalkStart = true;
            Debug.Log($"{AttachData.ID} NCP에게 {other.gameObject.name} In.");
            AttachData.UINeck.gameObject.SetActive(true);
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
            AttachData.CanTalkStart = false;
            Debug.Log($"{AttachData.ID} NCP에게 {other.gameObject.name} Out.");
            AttachData.UINeck.gameObject.SetActive(false);
        }
    }
}
