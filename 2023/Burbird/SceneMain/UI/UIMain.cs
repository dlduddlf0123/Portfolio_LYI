using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;
using BackEnd;

namespace Burbird
{
    public class UIMain : MonoBehaviour
    {
        GameManager gameMgr;

        [SerializeField]
        UISetting ui_setting;

        //Menu Bar
        RectTransform main_bar_menu;
        Button main_btn_shop;
        Button main_btn_equip;
        Button main_btn_world;
        Button main_btn_ability;
        Button main_btn_event;

        //Money Bar
        RectTransform money_bar;
        Button btn_stemina;
        Text txt_stemina;
        Text txt_timerStemina;

        Button btn_coin;
        Text txt_coin;

        Button btn_diamond;
        Text txt_diamond;

        Button btn_setting;

        //Popup
        RectTransform popup_stemina;

        public Coroutine c_timeTick = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;

            main_bar_menu = transform.GetChild(0).GetComponent<RectTransform>();
            main_btn_shop = main_bar_menu.GetChild(0).GetComponent<Button>();
            main_btn_equip = main_bar_menu.GetChild(1).GetComponent<Button>();
            main_btn_world = main_bar_menu.GetChild(2).GetComponent<Button>();
            main_btn_ability = main_bar_menu.GetChild(3).GetComponent<Button>();
            main_btn_event = main_bar_menu.GetChild(4).GetComponent<Button>();

            money_bar = transform.GetChild(1).GetComponent<RectTransform>();
            btn_stemina = money_bar.GetChild(0).GetComponent<Button>();
            txt_stemina = btn_stemina.transform.GetChild(1).GetComponent<Text>();
            txt_timerStemina = btn_stemina.transform.GetChild(2).GetComponent<Text>();

            btn_coin = money_bar.GetChild(1).GetComponent<Button>();
            txt_coin = btn_coin.transform.GetChild(1).GetComponent<Text>();

            btn_diamond = money_bar.GetChild(2).GetComponent<Button>();
            txt_diamond = btn_diamond.transform.GetChild(1).GetComponent<Text>();

            btn_setting = money_bar.GetChild(3).GetComponent<Button>();
            ui_setting.gameObject.SetActive(false);

            popup_stemina = transform.GetChild(2).GetComponent<RectTransform>();

        }

        private void Start()
        {
            main_btn_shop.onClick.AddListener(() => gameMgr.uiMgr.SetUIActive(UIWindow.SHOP));
            main_btn_equip.onClick.AddListener(() => gameMgr.uiMgr.SetUIActive(UIWindow.EQUIPMENT));
            main_btn_world.onClick.AddListener(() => gameMgr.uiMgr.SetUIActive(UIWindow.WORLD));
            main_btn_ability.onClick.AddListener(() => gameMgr.uiMgr.SetUIActive(UIWindow.UPGRADE));
            main_btn_event.onClick.AddListener(() => gameMgr.uiMgr.SetUIActive(UIWindow.EVENT));
           
            btn_setting.onClick.AddListener(SettingButton);

            ActiveSteminaTimer();
        }


        public void ActiveSteminaTimer()
        {
            if (c_timeTick != null)
            {
                StopCoroutine(c_timeTick);
            }

            c_timeTick = StartCoroutine(SteminaTick());

        }

                
        
        IEnumerator SteminaTick()
        {
            DateTime currentTime = gameMgr.timeMgr.CheckWebTime();
            string startTime = DateTime.Now.ToString("HH:mm:ss");

            int maxStamina = 100;
            int stamina = 0;
            float recoveryTime = 20 * 60; // recovery time in seconds (20 minutes)
            float lastRecoveryTime = 0f;

            float t = 0;
            while (true)
            {
                if (stamina < maxStamina)
                {
                    // check how much time has passed since last recovery
                    float timePassed = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerSecond - lastRecoveryTime;

                    // check if enough time has passed for recovery
                    if (timePassed >= recoveryTime)
                    {
                        // recover one stamina point
                        stamina += 1;
                        // clamp stamina to max value
                        stamina = Mathf.Clamp(stamina, 0, maxStamina);
                        // update last recovery time
                        lastRecoveryTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerSecond;
                    }
                    ChangeTimerStemina(timePassed);
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void ChangeTimerStemina(float time)
        {
            float minutes, seconds;
            minutes = time / 60;
            seconds = time % 60;

            //시간 받아와서 시계형식으로 변형하여 표시
            txt_timerStemina.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        /// <summary>
        /// MainUI 상단 바 재화들 새로고침
        /// </summary>
        public void RefreshMainUI()
        {
            ChangeSteminaText(gameMgr.dataMgr.Stemina, gameMgr.dataMgr.MaxStemina);
            ChangeCoinText(gameMgr.dataMgr.Coin);
            ChangeDiamondText(gameMgr.dataMgr.Diamond);
        }
        public void ChangeSteminaText(int value, int maxValue)
        {
            txt_stemina.text = value + " / " + maxValue;
        }
        public void ChangeCoinText(int value)
        {
            txt_coin.text = value.ToString();
        }
        public void ChangeDiamondText(int value)
        {
            txt_diamond.text = value.ToString();
        }



        public void ToggleGameObject(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }
        public void TogglePopupStemina()
        {
            ToggleGameObject(popup_stemina.gameObject);
        }

        public void SettingButton()
        {
            ui_setting.gameObject.SetActive(true);
            ui_setting.OpenSettingUI();

        }


    } //class UIMain
}//namespace Burbird