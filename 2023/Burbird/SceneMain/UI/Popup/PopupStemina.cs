using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace Burbird
{
    public class PopupStemina : MonoBehaviour
    {
        GameManager gameMgr;

        [SerializeField]
        private Text txt_title;
        [SerializeField]
        private Button btn_close;
        [SerializeField]
        private Button btn_ad;
        [SerializeField]
        private Button btn_diamond;

        public delegate void OnClose();
        public OnClose onClose;

        public int price = 600;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        private void Start()
        {
            btn_close.onClick.AddListener(Button_Close);
            btn_ad.onClick.AddListener(Button_WatchAD);
            btn_diamond.onClick.AddListener(Button_Diamond);
        }

        /// <summary>
        /// 창 닫기
        /// </summary>
        public void Button_Close()
        {
            gameObject.SetActive(false);

            if (onClose != null)
            {
                onClose.Invoke();
            }
        }

        /// <summary>
        /// 광고 재생 후 보상으로 스테미나 20 지급
        /// </summary>
        public void Button_WatchAD()
        {
            gameMgr.dataMgr.GetStemina(20);
        }


        /// <summary>
        /// 다이아몬드 소모 후 보상으로 스테미나 20 지급
        /// </summary>
        public void Button_Diamond()
        {
            //gameMgr.dataMgr.UseDiamond(price);
            gameMgr.dataMgr.GetStemina(20);
        }
    }
}