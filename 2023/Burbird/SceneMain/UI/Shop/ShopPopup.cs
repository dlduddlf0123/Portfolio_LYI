using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Events;

namespace Burbird
{
    public class ShopPopup : MonoBehaviour
    {
        public UIShop ui_shop;

        IAPButton iap_yes;
        Text txt_popup;
        Button btn_yes;
        Button btn_no;

        public UnityEvent evt_yes = null;
        private void Awake()
        {
            txt_popup = transform.GetChild(0).GetComponent<Text>();
            btn_yes = transform.GetChild(1).GetComponent<Button>();
            btn_no = transform.GetChild(2).GetComponent<Button>();
        }

        void Start()
        {
            btn_yes.onClick.AddListener(YesButton);
            btn_no.onClick.AddListener(NoButton);
        }

        void ClosePopUp()
        {
            ui_shop.currentItem = null;
            evt_yes.RemoveAllListeners();
            this.gameObject.SetActive(false);
        }
        void YesButton()
        {
            if (evt_yes != null)
            {
                Debug.Log("Purchase Complete");
                evt_yes.Invoke();
                ClosePopUp();
            }
        }
        void NoButton()
        {
            ClosePopUp();
        }



    }
}