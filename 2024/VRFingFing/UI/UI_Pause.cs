using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using VRTokTok.Manager;

namespace VRTokTok.UI
{
    /// <summary>
    /// 9/25/2023-LYI
    /// 일시정지 UI
    /// </summary>
    public class UI_Pause : MonoBehaviour
    {
        PlaySceneManager playMgr;

        public Oculus.Interaction.PokeInteractable interactable;

        [Header("Sound")]
        [SerializeField]
        Toggle tog_bgm;
        [SerializeField]
        Toggle tog_sfx;

        [Header("Button")]
        [SerializeField]
        Button btn_menu;
        [SerializeField]
        Button btn_restart;
        [SerializeField]
        Button btn_continue;

        private void Awake()
        {
            playMgr = GameManager.Instance.playMgr;
        }

        //private void OnEnable()
        //{
        //    Time.timeScale = 0;
        //}
        //private void OnDisable()
        //{
        //    Time.timeScale = 1;
        //}

        private void Start()
        {
            tog_bgm.onValueChanged.AddListener(BGMToggle);
            tog_sfx.onValueChanged.AddListener(SFXToggle);

            btn_menu.onClick.AddListener(MenuButton);
            btn_restart.onClick.AddListener(RestartButton);
            btn_continue.onClick.AddListener(ContinueButton);

            PauseLoad();

            //if (playMgr.statPlay != PlayStatus.PLAY)
            //{
            //    gameObject.SetActive(false);
            //}
        }

        public void PauseLoad()
        {
            GameManager.Instance.soundMgr.bgmVolume = ES3.Load(Constants.Sound.BGM_VOLUME, 1f);
            GameManager.Instance.soundMgr.sfxVolume = ES3.Load(Constants.Sound.SFX_VOLUME, 1f);

            tog_bgm.isOn = (GameManager.Instance.soundMgr.bgmVolume == 1) ? true : false;
            tog_sfx.isOn = (GameManager.Instance.soundMgr.sfxVolume == 1) ? true : false;
        }

        public void BGMToggle(bool isActive)
        {
            GameManager.Instance.soundMgr.bgmVolume = isActive ? 1 : 0;
            ES3.Save(Constants.Sound.BGM_VOLUME, GameManager.Instance.soundMgr.bgmVolume);
        }
        public void SFXToggle(bool isActive)
        {
            GameManager.Instance.soundMgr.sfxVolume = isActive ? 1 : 0;
            ES3.Save(Constants.Sound.SFX_VOLUME, GameManager.Instance.soundMgr.sfxVolume);
        }

        public void MenuButton()
        {
            //GameManager.Instance.loadMgr.LoadScene(SceneStatus.MENU);
        }

        public void RestartButton()
        {
            playMgr.currentStage.RestartStage(false);
        }

        public void ContinueButton()
        {
            interactable.enabled = false;
            this.transform.GetChild(0).gameObject.SetActive(false);
        }


    }
}