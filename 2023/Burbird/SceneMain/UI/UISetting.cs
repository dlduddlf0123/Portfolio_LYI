using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace Burbird
{
    /// <summary>
    /// 3/15/2023-LYI
    /// 메인메뉴 설정창 
    /// </summary>
    public class UISetting : MonoBehaviour
    {
        GameManager gameMgr;

        [SerializeField]
        Button btn_close;

        [SerializeField]
        Button btn_bgmToggle;
        [SerializeField]
        Button btn_sfxToggle;

        [SerializeField]
        Button btn_practice;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        void Start()
        {
            btn_close.onClick.AddListener(CloseButton);

            btn_practice.onClick.AddListener(PracticeButton);

            btn_bgmToggle.onClick.AddListener(SettingBGMToggle);
            btn_sfxToggle.onClick.AddListener(SettingSFXToggle);
        }

        public void OpenSettingUI()
        {
            SettingUIInit();
        }

        public void SettingUIInit()
        {
            bool isBGMOn = gameMgr.soundMgr.bgmVolume == 0 ? false : true;
            bool isSFXOn = gameMgr.soundMgr.sfxVolume == 0 ? false : true;

            ChangeOnOffButton(btn_bgmToggle, isBGMOn);
            ChangeOnOffButton(btn_sfxToggle, isSFXOn);
        }

        #region Button Listners
        void CloseButton()
        {
            gameObject.SetActive(false);
        }

        void PracticeButton()
        {
            gameMgr.PracticeStart();
        }

        void SettingBGMToggle()
        {
            if (gameMgr.soundMgr.bgmVolume == 0)
            {
                Debug.Log("BGM On");
                gameMgr.soundMgr.bgmVolume = 1;
                ChangeOnOffButton(btn_bgmToggle, true);
            }
            else
            {
                Debug.Log("BGM Off");
                gameMgr.soundMgr.bgmVolume = 0;
                ChangeOnOffButton(btn_bgmToggle, false);
            }
            gameMgr.soundMgr.bgmSource.volume = gameMgr.soundMgr.bgmVolume;
            gameMgr.soundMgr.SaveVolume();
        }
        void SettingSFXToggle()
        {
            if (gameMgr.soundMgr.sfxVolume == 0)
            {
                Debug.Log("SFX On");
                gameMgr.soundMgr.sfxVolume = 1;
                ChangeOnOffButton(btn_sfxToggle, true);
            }
            else
            {
                Debug.Log("SFX Off");
                gameMgr.soundMgr.sfxVolume = 0;
                ChangeOnOffButton(btn_sfxToggle, false);
            }

            gameMgr.soundMgr.SaveVolume();
        }
        #endregion

        /// <summary>
        /// 3/15/2023-LYI
        /// 버튼 누를 시 이미지 변경
        /// </summary>
        /// <param name="button">이미지를 변경할 버튼</param>
        /// <param name="isOn"></param>
        void ChangeOnOffButton(Button button, bool isOn)
        {
            if (isOn)
            {
                button.image.color = new Color32(49, 122, 255, 255);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ON";
            }
            else
            {
                button.image.color = new Color32(207, 0, 28, 255);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "OFF";
            }
        }

    }
}