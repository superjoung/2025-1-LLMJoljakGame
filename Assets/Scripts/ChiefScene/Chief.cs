using System;
using UnityEngine;

namespace ChiefScene
{
    // 사건을 생성하고 UI를 호출시켜 안내해주는 Chief
    public class Chief : MonoBehaviour
    {
        public void Awake()
        {
            StartCoroutine(LLMConnectManager.Instance.GetGameSetup(ExplainGameSetup));
        }

        public void Start()
        {   
            DestroyAllManagers();
        }

        private void DestroyAllManagers()
        {
            GameManager.Instance.DestroySelf();
            NoneCharacterManager.Instance.DestroySelf();
        }

        private void ExplainGameSetup()
        {
            Debug.Log($"{LLMConnectManager.Instance.GetVillageName()}: {LLMConnectManager.Instance.GetEventDescription()}");
        }
    }
}