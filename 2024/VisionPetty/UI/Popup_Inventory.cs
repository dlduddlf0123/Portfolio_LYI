using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MoreMountains.Feedbacks;

namespace AroundEffect
{


    public class Popup_Inventory : MonoBehaviour
    {

        [SerializeField] Button btn_close;
        //Inventory Item

        public MMF_Player mmf_open;
        public MMF_Player mmf_close;

        void Start()
        {
            btn_close.onClick.AddListener(ButtonClose);
        }

        public void PopupInit()
        {

        }


        public void ButtonClose()
        {
            this.gameObject.SetActive(false);
        }


        /// <summary>
        /// 8/7/2024-LYI
        /// 팝업이 활성화 될 때 위치 및 효과?
        /// </summary>
        public virtual void OnUIOpen()
        {
            mmf_open.PlayFeedbacks();
        }
        public virtual void OnUIClose()
        {
            mmf_close.PlayFeedbacks();
        }


    }
}