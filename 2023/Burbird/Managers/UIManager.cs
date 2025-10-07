using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ReadOnly;

namespace Burbird
{
    public enum UIWindow
    {
        NONE = 0,
        WORLD,
        WORLDSELECT,
        SHOP,
        EQUIPMENT,
        UPGRADE,
        EVENT,
    }

    public class UIManager : MonoBehaviour
    {
        GameManager gameMgr;

        public UIMain ui_main;
        public UIWorld ui_world;
        public RectTransform ui_worldSelect;
        public UIShop ui_shop;
        public UIEquip ui_equip;
        public RectTransform ui_upgrade;
        public RectTransform ui_event;

        public GameObject equip_character;

        public UIWindow ui_currentWindow;
        public List<UIWindow> list_lastWindow = new List<UIWindow>();

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            gameMgr.uiMgr = this;

        }


        /// <summary>
        /// UI 오브젝트들의 기능 부여
        /// </summary>
        void Start()
        {
            List<Button[]> list_button = new List<Button[]>();

            for (int num = 0; num < transform.childCount; num++)
            {
                list_button.Add(transform.GetChild(num).GetComponentsInChildren<Button>());
            }

            foreach (var btn in list_button)
            {
                for (int num = 0; num < btn.Length; num++)
                {
                    // btn[num].onClick.AddListener(() => gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0));
                }
            }

            gameMgr.dataMgr.LocalLoadPlayerStat();
            ui_main.RefreshMainUI();

            SetUIActive(UIWindow.WORLD, false);
        }

        /// <summary>
        /// 뒤로가기 버튼 기능
        /// </summary>
        public void CallLastWindow()
        {
            SetUIActive(list_lastWindow[list_lastWindow.Count - 1], true);
            list_lastWindow.RemoveAt(list_lastWindow.Count - 1);
        }

        //각 장면에 해당하는 UI 활성화 함수
        public void SetUIActive(UIWindow sceneUI, bool isFade = true, bool isBack = false)
        {
            if (sceneUI == ui_currentWindow)
            {
                return;
            }
            if (isFade)
            {
                gameMgr.loader.fade.StartFade(() => { UIActive(sceneUI, isBack); });
            }
            else
            {
                UIActive(sceneUI, isBack);
            }
        }

        void UIActive(UIWindow sceneUI, bool isBack)
        {
            ui_world.gameObject.SetActive(false);
            ui_worldSelect.gameObject.SetActive(false);
            ui_shop.gameObject.SetActive(false);
            ui_equip.gameObject.SetActive(false);
            equip_character.SetActive(false);
            ui_upgrade.gameObject.SetActive(false);
            ui_event.gameObject.SetActive(false);

            if (!isBack)
            {
                list_lastWindow.Add(ui_currentWindow);
                if (list_lastWindow.Count > 10)
                {
                    list_lastWindow.RemoveAt(0);
                }
            }

            ui_currentWindow = sceneUI;

            switch (sceneUI)
            {
                case UIWindow.WORLD:
                    ui_world.gameObject.SetActive(true);
                    break;
                case UIWindow.WORLDSELECT:
                    ui_worldSelect.gameObject.SetActive(true);
                    break;
                case UIWindow.SHOP:
                    ui_shop.gameObject.SetActive(true);
                    break;
                case UIWindow.EQUIPMENT:
                    ui_equip.gameObject.SetActive(true);
                    equip_character.SetActive(true);


                    gameMgr.invenChecker.SortInventory(true);
                    gameMgr.invenChecker.RedrawInventory();
                    gameMgr.invenChecker.RefreshEquipStat();
                    break;
                case UIWindow.UPGRADE:
                    ui_upgrade.gameObject.SetActive(true);
                    break;
                case UIWindow.EVENT:
                    ui_event.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
            RefreshUIElements();
        }

        public void RefreshUIElements()
        {
            ui_main.RefreshMainUI();
            ui_world.RefreshWorldUI();
        }

        public void ToggleDebug(Button button)
        {
            gameMgr.isDebug = !gameMgr.isDebug;
            button.transform.GetChild(0).GetComponent<Text>().text = "Debug\n" + gameMgr.isDebug.ToString();
        }
    }

}