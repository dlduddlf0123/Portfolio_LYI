using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    public Text setting_txt_bgm;
    public Slider setting_slider_BGM;
    public Text setting_txt_sfx;
    public Slider setting_slider_SFX;

    public Text setting_txt_language;
    public Dropdown setting_drop_language;

    public Toggle setting_toggle_handIcon;
    public Toggle setting_toggle_handOcclusion;
    public Toggle setting_toggle_tutorial;
    public Toggle setting_toggle_education;

    public Button setting_btn_close;
    public Button setting_btn_ARreset;
    public Button setting_btn_exit;

    void Start()
    {

        //setting_btn_close = ui_setting.GetChild(0).GetChild(2).GetComponent<Button>(); //not scroll

        ////Setting UI scroll contents
        //Transform _setting_scrollContent = ui_setting.GetChild(0).GetChild(1).GetChild(0).GetChild(0);
        //Transform _setting_bg1 = _setting_scrollContent.GetChild(0);
        //Transform _setting_bg2 = _setting_scrollContent.GetChild(1);

        //setting_txt_bgm = _setting_bg1.GetChild(0).GetChild(0).GetComponent<Text>();
        //setting_slider_BGM = _setting_bg1.GetChild(0).GetChild(1).GetComponent<Slider>();

        //setting_txt_sfx = _setting_bg1.GetChild(1).GetChild(0).GetComponent<Text>();
        //setting_slider_SFX = _setting_bg1.GetChild(1).GetChild(1).GetComponent<Slider>();

        //setting_txt_language = _setting_bg2.GetChild(0).GetChild(0).GetComponent<Text>();
        //setting_drop_language = _setting_bg2.GetChild(0).GetChild(1).GetComponent<Dropdown>();

        //setting_toggle_handIcon = _setting_bg2.GetChild(1).GetComponent<Toggle>();
        //setting_toggle_handOcclusion = _setting_bg2.GetChild(2).GetComponent<Toggle>();

        //setting_btn_ARreset = _setting_scrollContent.GetChild(2).GetComponent<Button>();
        //setting_btn_exit = _setting_scrollContent.GetChild(3).GetComponent<Button>();

        //setting_slider_BGM.value = gameMgr.soundMgr.bgmVolume;
        //setting_slider_SFX.value = gameMgr.soundMgr.sfxVolume;

        //setting_slider_BGM.onValueChanged.AddListener(delegate
        //{
        //    gameMgr.soundMgr.bgmVolume = setting_slider_BGM.value;
        //    gameMgr.soundMgr.bgmSource.volume = gameMgr.soundMgr.bgmVolume;
        //});
        //setting_slider_SFX.onValueChanged.AddListener(delegate
        //{
        //    gameMgr.soundMgr.sfxVolume = setting_slider_SFX.value;
        //});


        //setting_drop_language.value = (int)gameMgr.currentLanguage;
        //setting_drop_language.onValueChanged.AddListener(delegate
        //{
        //    gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0);
        //    gameMgr.currentLanguage = (GameLanguage)setting_drop_language.value;
        //    gameMgr.ChangeLanguage(gameMgr.currentLanguage);
        //});

        //setting_btn_close.onClick.AddListener(() => UISettingButtonClose());

        //setting_btn_ARreset.onClick.AddListener(() => UIGameReanchorButton());
        //setting_btn_exit.onClick.AddListener(() => UIPauseButtonExit());

        //setting_toggle_handIcon.isOn = gameMgr.handCtrl.toggleHandIcon;
        //setting_toggle_handIcon.onValueChanged.AddListener((bool _isOn) =>
        //{
        //    gameMgr.handCtrl.ToggleHandIcon(_isOn);
        //});
        //setting_toggle_handOcclusion.isOn = gameMgr.handCtrl.toggleHandOcclusion;
        //setting_toggle_handOcclusion.onValueChanged.AddListener((bool _isOn) =>
        //{
        //    gameMgr.handCtrl.ToggleHandOcclusion(_isOn);
        //});
    }

}
