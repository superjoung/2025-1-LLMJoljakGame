using System;
using UnityEngine;

namespace ChiefScene
{
    // 사건을 생성하고 UI를 호출시켜 안내해주는 Chief
    public class Chief : MonoBehaviour
    {
        private ChiefTalkPanelUI _chiefTalkPanelUI;
        
        public void Awake()
        {
            StartCoroutine(LLMConnectManager.Instance.GetGameSetup(LoadChiefStatement));
            _chiefTalkPanelUI = gameObject.GetComponentInChildren<ChiefTalkPanelUI>();
        }

        public void Start()
        {   
            DestroyAllManagers();
        }

        private void DestroyAllManagers()
        {
            if(FindObjectOfType<GameManager>()) GameManager.Instance.DestroySelf();
            if(FindObjectOfType<NoneCharacterManager>()) NoneCharacterManager.Instance.DestroySelf();
        }

        private void LoadChiefStatement()
        {
            StartCoroutine(LLMConnectManager.Instance.GetChiefStatement(SetChiefDialogue));
        }

        private void SetChiefDialogue(string statement)
        {
            _chiefTalkPanelUI.FinishLoading();
            _chiefTalkPanelUI.ShowText(statement);
        }
    }
}