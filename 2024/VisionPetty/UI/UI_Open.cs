using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using MoreMountains.Feedbacks;

namespace AroundEffect
{

    /// <summary>
    /// 8/7/2024-LYI
    /// test name
    /// attach somthing can open UI
    /// clickable?
    /// </summary>
    public class UI_Open : MonoBehaviour
    {
        public UnityEvent onAction;

        public Popup_Inventory openUI;

        public MMF_Player mmf_active;
        public MMF_Player mmf_deactive;

        private void Awake()
        {
            if (openUI != null)
            {
                openUI.gameObject.SetActive(false);
            }
        }
        void Start()
        {
            //onAction.AddListener(()=>openUI.gameObject.SetActive(true));
        }

        public void OpenUI()
        {
            openUI.gameObject.SetActive(true);
            openUI.OnUIOpen();
        }

    }
}