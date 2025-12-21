using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ReadOnly; //custum namespace

public enum UIWindow
{
    TITLE,
    READY,
    MAIN,
    GAME,
    SETTING,
    SELECT,
    WARNING,
}

public class UIManager : MonoBehaviour
{
    GameManager gameMgr;

    //에디터에서 스테이지 선택 스킵
    public ARObjectSelect debugStarter;
    public int debugStageNum = 0;
    public bool isSkipInEditor = true;


    //Additional Canvas Objects
    public WorldCanvas worldCanvas;
    public Fade fadeCanvas;
    public EndingCredits endingCreditCanvas;
    public Canvas stageTitleCanvas; //When stage starts, Fading title UI display
    Text txt_stageTitle_title;
    Text txt_stageTitle_sub;

    //UIManager Canvas Objects
    public UITitle ui_title;
    public UIReady ui_ready;
    //public UIMain ui_main;
    public UIGame ui_game;
    public UISetting ui_setting;
    public  UIPause ui_pause;
    public UIHandCalibration ui_handCalibration;
    //public RectTransform ui_select;
    //public RectTransform ui_payment;
    public RectTransform ui_warning;

    public UIWindow ui_currentWindow;
    public List<UIWindow> list_lastWindow = new List<UIWindow>();

    public GameObject stageSelect;
    public GameObject shadowPlane;

    #region GetComponent 정리
    //TitleUI
    //Button title_startButton;
    //AudioSource title_audio;
    //AudioClip clip_bgm_title;

    ////MainUI
    //public Button main_btn_ARreset { get; set; }
    //AudioSource main_audio;
    //AudioClip clip_bgm_main;

    ////ReadyUI
    //Button ready_btn_back;
    //public Button ready_btn_start { get; set; }
    //Slider ready_slider_scale;
    //public Button ready_btn_handCalibration { get; set; }

    ////GameUI
    //public Button game_btn_back { get; set; }
    //public Button game_btn_skip { get; set; }
    //public Button game_btn_pause { get; set; }
    //public Text game_txt_dialog { get; set; }
    //public CanvasGroup game_grp_frame { get; set; }

    //public Text debug_txt_scene { get; set; }
    //public Text debug_txt_timeline { get; set; }
    //public Text debug_txt_interaction { get; set; }

    //SettingUI
    //public Text setting_txt_bgm { get; set; }
    //public Text setting_txt_sfx { get; set; }
    //public Text setting_txt_language { get; set; }

    //public Slider setting_slider_BGM { get; set; }
    //public Slider setting_slider_SFX { get; set; }
    //public Dropdown setting_drop_language { get; set; }
    //public Button setting_btn_close { get; set; }
    //public Button setting_btn_ARreset { get; set; }
    //public Button setting_btn_exit { get; set; }
    //public Toggle setting_toggle_handIcon { get; set; }
    //public Toggle setting_toggle_handOcclusion { get; set; }

    ////HandCalibrationUI
    //public Button calibration_btn_close { get; set; }
    //public Slider calibration_slider_handDepth { get; set; }
    //public Slider calibration_slider_right { get; set; }
    //public Slider calibration_slider_front { get; set; }
    //public Image calibration_img_center { get; set; }

    ////PauseUI
    //public Button pause_btn_resume { get; set; }
    //public Button pause_btn_restart { get; set; }
    //public Button pause_btn_exit { get; set; }

    //SelectUI
    //    public StageSelectItem[] arr_select_item_stage { get; set; }
    //    AudioSource select_audio;

    //PaymentUI
    //public Text payment_txt_description { get; set; }
    ////test
    //public Text uiText;
    //public Text guestureText;
    //public Slider Zslider;
    //public Text zText;
    //public Text txt_dialog;
    //public Text txt_placedpos;
    #endregion

    //WarningUI
    public Button warning_btn_start { get; set; }


    Coroutine ui_currentCoroutine = null;

    public float stageSize = 1f;
    public int select_episodeNum = 0;

    /// <summary>
    /// UI 오브젝트들의 초기화, 할당 등
    /// </summary>
    private void Awake()
    {
        gameMgr = GameManager.Instance;

        txt_stageTitle_title = stageTitleCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        txt_stageTitle_sub = stageTitleCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>();

        #region Getcomponent 축소 작업
        ////TitleUI
        //title_startButton = ui_title.GetChild(1).GetComponent<Button>();
        //title_audio = ui_title.GetComponent<AudioSource>();
        //clip_bgm_title = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_TITLE);

        ////ReadyUI
        //ready_btn_back = ui_ready.GetChild(0).GetComponent<Button>();
        //ready_btn_start = ui_ready.GetChild(1).GetComponent<Button>();
        //ready_slider_scale = ui_ready.GetChild(3).GetComponent<Slider>();
        //ready_btn_handCalibration = ui_ready.GetChild(4).GetComponent<Button>();

        //MainUI
        //main_btn_ARreset = ui_main.GetChild(1).GetComponent<Button>();
        //main_audio = ui_main.GetComponent<AudioSource>();
        //clip_bgm_main = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_SELECT);

        //GameUI
        //game_btn_back = ui_game.GetChild(1).GetComponent<Button>();
        //game_btn_skip = ui_game.GetChild(2).GetComponent<Button>();
        //game_btn_pause = ui_game.GetChild(3).GetComponent<Button>();
        //game_txt_dialog = ui_game.GetChild(4).GetComponent<Text>();
        //game_grp_frame = ui_game.GetChild(5).GetComponent<CanvasGroup>();

        //debug_txt_scene = ui_game.GetChild(6).GetComponent<Text>();
        //debug_txt_timeline = ui_game.GetChild(7).GetComponent<Text>();
        //debug_txt_interaction = ui_game.GetChild(8).GetComponent<Text>();

        ////SettingUI
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


        //HandCalibrationUI
        //calibration_btn_close = ui_handCalibration.GetChild(0).GetComponent<Button>();
        //calibration_slider_handDepth = ui_handCalibration.GetChild(1).GetComponent<Slider>();
        //calibration_slider_right = ui_handCalibration.GetChild(2).GetComponent<Slider>();
        //calibration_slider_front = ui_handCalibration.GetChild(3).GetComponent<Slider>();
        //calibration_img_center = ui_handCalibration.GetChild(4).GetComponent<Image>();


        //SelectUI
        //arr_select_item_stage = new StageSelectItem[ui_select.transform.GetChild(0).GetChild(0).GetChild(0).childCount];
        //for (int i = 0; i < arr_select_item_stage.Length; i++)
        //{
        //    arr_select_item_stage[i] = ui_select.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<StageSelectItem>();
        //}
        //select_audio = ui_select.GetComponent<AudioSource>();

        //PaymentUI
        //payment_txt_description = ui_payment.transform.GetChild(2).GetComponent<Text>();
        #endregion
    }


    /// <summary>
    /// UI 오브젝트들의 기능 부여
    /// </summary>
    // Use this for initialization
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
                Button _btn = btn[num];
                btn[num].onClick.AddListener(() =>
                {
                    _btn.interactable = false;
                    StartCoroutine(gameMgr.LateFunc(() => _btn.interactable = true,0.5f));
                    gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0);
                });
            }
        }

        //TitleUI
        ui_title.title_startButton.onClick.AddListener(() => UITitleButtonStart());

        //ReadyUI
        ui_ready.ready_btn_start.onClick.RemoveAllListeners();
        ui_ready.ready_btn_start.onClick.AddListener(() => { UIReadyButtonStart(); });

#if !UNITY_EDITOR
        ui_ready.ready_btn_start.gameObject.SetActive(false);
#endif

        ui_ready.ready_slider_scale.onValueChanged.AddListener(delegate
        {
            stageSize = ui_ready.ready_slider_scale.value;
            ui_ready.ready_slider_scale.transform.GetChild(3).GetComponent<Text>().text = "StageSize: " + stageSize;
        });

        //GameUI
        ui_game.game_btn_skip.onClick.AddListener(() =>
        {
            if (gameMgr.statGame == GameStatus.CUTSCENE)
            {
                gameMgr.currentEpisode.currentStage.EndCutScene();
            }
            else if (gameMgr.statGame == GameStatus.INTERACTION)
            {
                gameMgr.currentEpisode.currentStage.EndInteraction();
            }
        });
        ui_game.game_btn_skip.gameObject.SetActive(false);
        ui_game.game_btn_pause.onClick.AddListener(() => { UIGameButtonPause(); });
        ui_game.game_txt_dialog.gameObject.SetActive(false);

        ui_game.debug_txt_scene.gameObject.SetActive(gameMgr.isDebug);
        ui_game.debug_txt_timeline.gameObject.SetActive(gameMgr.isDebug);
        ui_game.debug_txt_interaction.gameObject.SetActive(gameMgr.isDebug);

        // SettingUI
        ui_setting.setting_slider_BGM.value = gameMgr.soundMgr.bgmVolume;
        ui_setting.setting_slider_SFX.value = gameMgr.soundMgr.sfxVolume;

        ui_setting.setting_slider_BGM.onValueChanged.AddListener(delegate
        {
            gameMgr.soundMgr.bgmVolume = ui_setting.setting_slider_BGM.value;
            gameMgr.soundMgr.bgmSource.volume = gameMgr.soundMgr.bgmVolume;
        });
        ui_setting.setting_slider_SFX.onValueChanged.AddListener(delegate
        {
            gameMgr.soundMgr.sfxVolume = ui_setting.setting_slider_SFX.value;
        });


        ui_setting.setting_drop_language.value = (int)gameMgr.currentLanguage;
        ui_setting.setting_drop_language.onValueChanged.AddListener(delegate
        {
            gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0);
            gameMgr.currentLanguage = (GameLanguage)ui_setting.setting_drop_language.value;
            gameMgr.ChangeLanguage(gameMgr.currentLanguage);
        });

        ui_setting.setting_btn_close.onClick.AddListener(() => UISettingButtonClose());
        ui_setting.setting_btn_ARreset.onClick.AddListener(() => UISettingReanchorButton());
        ui_setting.setting_btn_exit.onClick.AddListener(() => UISettingReanchorButton());

        ui_setting.setting_toggle_handIcon.isOn = gameMgr.handCtrl.toggleHandIcon;
        ui_setting.setting_toggle_handIcon.onValueChanged.AddListener((bool _isOn) =>
        {
            gameMgr.handCtrl.ToggleHandIcon(_isOn);
        });
        ui_setting.setting_toggle_handOcclusion.isOn = gameMgr.handCtrl.toggleHandOcclusion;
        ui_setting.setting_toggle_handOcclusion.onValueChanged.AddListener((bool _isOn) =>
        {
            gameMgr.handCtrl.ToggleHandOcclusion(_isOn);
        });
        ui_setting.setting_toggle_tutorial.isOn = !gameMgr.isTutorial;
        ui_setting.setting_toggle_tutorial.onValueChanged.AddListener((bool _isOn) =>
        {
            gameMgr.isTutorial = !_isOn;
        });

        //PauseUI
        ui_pause.pause_btn_resume.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            ui_pause.gameObject.SetActive(false);
        });

        ui_pause.pause_btn_restart.onClick.AddListener(() => UIPauseButtonRestart());
        ui_pause.pause_btn_exit.onClick.AddListener(() => UISettingButtonExit());



        #region UI코드 정리 작업
        //Zslider.onValueChanged.AddListener(delegate
        //{
        //    GameManager.Instance.arPlaneMgr.transform.localScale = Vector3.one * Zslider.value;
        //    zText.text = "MapSize: " + Zslider.value;
        //});

        //MainUI
        //main_btn_ARreset.onClick.AddListener(() => { UIGameReanchorButton(); });

        ////HandCalibrationUI
        //ui_handCalibration.calibration_slider_handDepth.value = gameMgr.handCtrl.handPosZ;
        //ui_handCalibration.calibration_slider_right.value = gameMgr.handCtrl.handPosRight;
        //ui_handCalibration.calibration_slider_front.value = gameMgr.handCtrl.handPosFront;

        //ui_handCalibration.calibration_slider_handDepth.transform.GetChild(0).GetComponent<Text>().text = "HandPosZ: " + ui_handCalibration.calibration_slider_handDepth.value;
        //ui_handCalibration.calibration_slider_right.transform.GetChild(0).GetComponent<Text>().text = "HandPosRight: " + ui_handCalibration.calibration_slider_right.value;
        //ui_handCalibration.calibration_slider_front.transform.GetChild(0).GetComponent<Text>().text = "HandPosFront: " + ui_handCalibration.calibration_slider_front.value;
        //ui_handCalibration.calibration_slider_handDepth.onValueChanged.AddListener(delegate
        //{
        //    gameMgr.handCtrl.handPosZ = ui_handCalibration.calibration_slider_handDepth.value;
        //    ui_handCalibration.calibration_slider_handDepth.transform.GetChild(0).GetComponent<Text>().text = "HandPosZ: " + ui_handCalibration.calibration_slider_handDepth.value;
        //});
        //ui_handCalibration.calibration_slider_right.onValueChanged.AddListener(delegate
        //{
        //    gameMgr.handCtrl.handPosRight = ui_handCalibration.calibration_slider_right.value;
        //    ui_handCalibration.calibration_slider_right.transform.GetChild(0).GetComponent<Text>().text = "HandPosRight: " + ui_handCalibration.calibration_slider_right.value;
        //});
        //ui_handCalibration.calibration_slider_front.onValueChanged.AddListener(delegate
        //{
        //    gameMgr.handCtrl.handPosFront = ui_handCalibration.calibration_slider_front.value;
        //    ui_handCalibration.calibration_slider_front.transform.GetChild(0).GetComponent<Text>().text = "HandPosFront: " + ui_handCalibration.calibration_slider_front.value;
        //});

        //ui_handCalibration.calibration_btn_close.onClick.AddListener(() =>
        //{
        //    gameMgr.handCtrl.SaveHandPose();
        //    gameMgr.handCtrl.ToggleHandIcon(gameMgr.handCtrl.toggleHandIcon);
        //}); //손 현재 위치 저장



        //SelectUI
        //for (int i = 0; i < arr_select_btn_stage.Length; i++)
        //{
        //    int a = i;
        //    arr_select_btn_stage[i].onClick.AddListener(() =>
        //    {
        //        ARReady(a);
        //    });
        //}

        //PaymentUI

        #endregion

        SetUIActive(UIWindow.WARNING, false);
        SetDebug();
    }


    //public void SetDialogText(string _text)
    //{
    //    txt_dialog.text = _text;
    //    txt_dialog.transform.parent.gameObject.SetActive(true);
    //    if (ui_currentCoroutine == null)
    //    {
    //        ui_currentCoroutine = StartCoroutine(UITimer(txt_dialog.transform.parent.gameObject, 3f));
    //    }

    //}


    IEnumerator UITimer(GameObject _go, float _time)
    {
        yield return new WaitForSeconds(_time);
        _go.SetActive(false);
        ui_currentCoroutine = null;
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
    public void SetUIActive(UIWindow _sceneUI, bool _isFade = true, bool _isBack = false)
    {
        if (_isFade)
        {
            fadeCanvas.StartFade(() => { UIActive(_sceneUI, _isBack); });
        }
        else
        {
            UIActive(_sceneUI, _isBack);
        }
    }

    void UIActive(UIWindow _sceneUI, bool _isBack)
    {
        ui_title.gameObject.SetActive(false);
       // ui_main.gameObject.SetActive(false);
        ui_ready.gameObject.SetActive(false);
        ui_game.gameObject.SetActive(false);
        ui_setting.gameObject.SetActive(false);
      //  ui_select.gameObject.SetActive(false);
        ui_warning.gameObject.SetActive(false);

        if (!_isBack)
        {
            list_lastWindow.Add(ui_currentWindow);
            if (list_lastWindow.Count > 10)
            {
                list_lastWindow.RemoveAt(0);
            }
        }

        switch (_sceneUI)
        {
            case UIWindow.TITLE:
                ui_title.gameObject.SetActive(true);
                //ready_btn_start.gameObject.SetActive(false);

                gameMgr.soundMgr.ChangeBGMAudioSource(ui_title.title_audio);
               // gameMgr.soundMgr.PlayBgm(clip_bgm_title);
               // gameMgr.ARSessionSetActive(false);
                break;
            case UIWindow.READY:
                ui_ready.gameObject.SetActive(true);
                break;
            case UIWindow.GAME:
                ui_game.gameObject.SetActive(true);
                break;
            case UIWindow.SETTING:
                ui_setting.gameObject.SetActive(true);
                break;
            case UIWindow.WARNING:
                ui_warning.gameObject.SetActive(true);

                if (gameMgr.soundMgr.bgmSource!= null)
                {
                    gameMgr.soundMgr.bgmSource.Stop();
                }
                break;

            #region 더이상 사용되지 않는 UI
            //case UIWindow.MAIN:
            //    ui_main.gameObject.SetActive(true);
            //    ui_currentWindow = UIWindow.MAIN;

            //    gameMgr.soundMgr.ChangeBGMAudioSource(main_audio);
            //    gameMgr.soundMgr.PlayBgm(clip_bgm_main);
            //    break;
            //case UIWindow.SELECT:
            //    ui_select.gameObject.SetActive(true);

            //    gameMgr.soundMgr.ChangeBGMAudioSource(select_audio);
            //    gameMgr.soundMgr.PlayBgm();
            //    break;
            #endregion

            default:
                break;
        }

        ui_currentWindow = _sceneUI;
    }


    /// <summary>
    /// AR 설정 시작
    /// 지면 인식, 영역 설정을 한다.
    /// 다음은 스테이지 선택
    /// </summary>
    public void UITitleButtonStart()
    {
        gameMgr.uiMgr.ARReady(select_episodeNum);
        gameMgr.soundMgr.PlaySfx(transform.position, Defines.SOUND_SFX_EPISODE_IN, 1, 0);

        #region old
        // SetUIActive(UIWindow.SELECT);
        //planeGenerator.SetPlacePrefab(b_stagePrefab.LoadAsset<GameObject>("RainScene"));

        //     switch (_stageNum)
        //     {
        //case 0:
        //	gameMgr.planeGenerator.SetPlacePrefab(gameMgr.arr_stage[_stageNum]);
        //	break;
        //         default:
        //	gameMgr.planeGenerator.SetPlacePrefab(gameMgr.arr_stage[0]);
        //	break;
        //     }


        //에디터에서 바로 시작
        //        if (isSkipInEditor)
        //        {

        //#if UNITY_EDITOR
        //            SetUIActive(UIWindow.GAME, false);
        //            debugStarter.gameMgr = gameMgr;
        //            debugStarter.lastSelect = debugStarter.transform.GetChild(debugStageNum).GetComponent<ARSelectableObject>();
        //            debugStarter.SetStage(0);
        //#else
        //     StageSelect();
        //#endif
        //        }
        //        else
        //        {
        //            StageSelect();
        //        }
        #endregion
    }


    public void UIReadyButtonExit()
    {
        SetUIActive(UIWindow.TITLE);
        if (gameMgr.currentEpisode != null)
        {
            Destroy(gameMgr.currentEpisode.gameObject);
            gameMgr.currentEpisode = null;
        }
    }


    /// <summary>
    /// 영역 설정 이후 시작 버튼 클릭 시
    /// </summary>
    public void UIReadyButtonStart()
    {
        #region old
        //SetUIActive(UIWindow.MAIN, false);

        //gameMgr.statGame = GameStatus.SELECT;

        ////스테이지 선택 오브젝트 활성화
        //stageSelect.SetActive(true);
        //stageSelect.transform.parent.position = gameMgr.planeGenerator.placedPos;
        //stageSelect.transform.parent.rotation = gameMgr.planeGenerator.placedRot;

        ////for (int i = 1; i < stageSelect.transform.parent.childCount; i++)
        ////{
        ////    stageSelect.transform.parent.GetChild(i).GetComponent<ARObjectSelect>().PlayGuideParticle();
        ////}

        ////stageSelect.transform.parent.localScale *= gameMgr.uiMgr.stageSize;

        ////아무것도 없을 시 그림자 생성용 바닥 활성화
        //shadowPlane.SetActive(true);
        //shadowPlane.transform.position = gameMgr.planeGenerator.placedPos;
        //shadowPlane.transform.rotation = gameMgr.planeGenerator.placedRot;
        #endregion

        //생성된 바닥 기즈모 제거
        gameMgr.planeGenerator.isPlaced = true;
        gameMgr.planeGenerator.SetAllPlanesActive(false);
        gameMgr.arPlaneMgr.enabled = false; //바닥 생성 비활성화

        gameMgr.handCtrl.manoHandMove.gameObject.SetActive(true); //손 상호작용 활성화
        gameMgr.handCtrl.handFollower.gameObject.SetActive(true);

        gameMgr.soundMgr.PlaySfx(transform.position, Defines.SOUND_SFX_EPISODE_IN, 1,0);

        if (gameMgr.currentEpisode == null)
        {
            gameMgr.SetStage(select_episodeNum);
            gameMgr.GameStart();
        }
        else
        {
            gameMgr.currentEpisode.gameObject.SetActive(true);
            gameMgr.GameStart();
        }
    }



    public void UIGameButtonPause()
    {
        Time.timeScale = 0f;
        //ui_pause.gameObject.SetActive(true);

        for (int i = 0; i < gameMgr.currentEpisode.currentStage.arr_header.Length; i++)
        {
            gameMgr.currentEpisode.currentStage.arr_header[i].m_lipSync.Pause();
            gameMgr.currentEpisode.currentStage.arr_header[i].lipSound.mute = true;
        }

        ui_setting.gameObject.SetActive(true);
    }

    public void UIGameTimelineFrameToggle(bool _isOn)
    {
        StartCoroutine(ActGameTimelineFrameFade(_isOn));
    }
    IEnumerator ActGameTimelineFrameFade(bool _isOn)
    {
        float t = 0;
        if (_isOn)
        {
            while (t < 1)
            {
                t += 0.01f;
                ui_game.game_grp_frame.alpha = t;
                yield return new WaitForSeconds(0.01f);
            }
           ui_game.game_grp_frame.alpha = 1;
        }
        else
        {
            t = 1;
            while (t > 0)
            {
                t -= 0.01f;
                ui_game.game_grp_frame.alpha = t;
                yield return new WaitForSeconds(0.01f);
            }
            ui_game.game_grp_frame.alpha = 0;
        }
    }

    public void UISettingButtonClose()
    {
        Time.timeScale = 1f;
        gameMgr.handCtrl.SaveHandPose();
        gameMgr.soundMgr.SaveVolume();
        PlayerPrefs.SetInt("isTutorial", System.Convert.ToInt32(gameMgr.isTutorial));

        for (int i = 0; i < gameMgr.currentEpisode.currentStage.arr_header.Length; i++)
        {
            gameMgr.currentEpisode.currentStage.arr_header[i].m_lipSync.Resume();
            gameMgr.currentEpisode.currentStage.arr_header[i].lipSound.mute = false;
        }

        ui_setting.gameObject.SetActive(false);
    }
    /// <summary>
    /// 앵커 초기화, 재설정. 기존 활성화 오브젝트 비 활성화 후 포지션 조정
    /// /// </summary>
    public void UISettingReanchorButton()
    {
        Time.timeScale = 1f;

        SetUIActive(UIWindow.READY, false);

        gameMgr.statGame = GameStatus.ARPLANE;
        //스테이지 선택 오브젝트 활성화
        //   stageSelect.SetActive(false);
        //아무것도 없을 시 그림자 생성용 바닥 활성화
        //   shadowPlane.SetActive(false);

        //생성된 바닥 기즈모 제거
        gameMgr.planeGenerator.isPlaced = false;
        gameMgr.planeGenerator.placedPos = Vector3.zero;
        gameMgr.planeGenerator.placedRot = Quaternion.identity;
        gameMgr.planeGenerator.SetAllPlanesActive(true);
        gameMgr.arPlaneMgr.enabled = true; //바닥 생성 비활성화

        if (gameMgr.currentEpisode.episodeNum == 5)
        {
            gameMgr.uiMgr.fadeCanvas.StartFade(() =>
            {
                gameMgr.currentEpisode.EndEpisode();
            });
        }
        else
        {
            gameMgr.currentEpisode.currentStage.EndStage();
        }
    }

    /// <summary>
    /// 페이드 후 다시 처음부터
    /// </summary>
    public void UIPauseButtonRestart()
    {
        Time.timeScale = 1f;
    }

    public void UISettingButtonExit()
    {
        Time.timeScale = 1f;

        //ui_pause.gameObject.SetActive(false);
        if (gameMgr.currentEpisode != null)
        {
            if (gameMgr.currentEpisode.episodeNum == 5)
            {
                gameMgr.uiMgr.fadeCanvas.StartFade(() =>
                {
                    gameMgr.currentEpisode.EndEpisode();
                });
            }
            else
            {
                gameMgr.currentEpisode.currentStage.EndStage();
            }
        }
    }

    /// <summary>
    /// 스테이지 시작 시 타이틀 이미지 페이드
    /// 각 스테이지에 맞는 설명 노출
    /// </summary>
    public void UIStageTitleFade(string _title, string _sub)
    {
        txt_stageTitle_title.text = _title;
        txt_stageTitle_sub.text = _sub;

        fadeCanvas.StartTitleFade(null, 2, 2);
    }

    /// <summary>
    /// 디버그 모드 설정
    /// </summary>
    public void UIToggleDebug()
    {
        gameMgr.isDebug = !gameMgr.isDebug;
        //_button.transform.GetChild(0).GetComponent<Text>().text = "Debug\n" + gameMgr.isDebug.ToString();
        SetDebug();
    }
    public void SetDebug()
    {
        gameMgr.handCtrl.handFollower.GetComponent<Renderer>().enabled = gameMgr.isDebug;
        gameMgr.handCtrl.manoHandMove.palmCenter.GetComponent<Renderer>().enabled = gameMgr.isDebug;
        for (int i = 0; i < gameMgr.handCtrl.manoHandMove.arr_handFollwer.Length; i++)
        {
            gameMgr.handCtrl.manoHandMove.arr_handFollwer[i].GetComponent<Renderer>().enabled = gameMgr.isDebug;
        }

        ui_ready.ready_btn_handCalibration.gameObject.SetActive(gameMgr.isDebug);

        ui_game.debug_txt_scene.gameObject.SetActive(gameMgr.isDebug);
        ui_game.debug_txt_timeline.gameObject.SetActive(gameMgr.isDebug);
        ui_game.debug_txt_interaction.gameObject.SetActive(gameMgr.isDebug);
    }

    public void UIWarningButton()
    {
        SetUIActive(UIWindow.TITLE, false);
    }


    void StageSelect()
    {
        gameMgr.ARSessionSetActive(true);   //AR 활성화
        if (!gameMgr.planeGenerator.isPlaced)
        {
            gameMgr.statGame = GameStatus.ARPLANE;
            SetUIActive(UIWindow.READY);
            gameMgr.planeGenerator.SetAllPlanesActive(true);
        }
        else
        {
            gameMgr.currentEpisode.currentStage.StartStage();
        }
    }

    public void ARReady(int _num)
    {
        select_episodeNum = _num;
        gameMgr.ARSessionSetActive(true);   //AR 활성화

        if (!gameMgr.planeGenerator.isPlaced)
        {
            gameMgr.statGame = GameStatus.ARPLANE;
            SetUIActive(UIWindow.READY);
            gameMgr.planeGenerator.SetAllPlanesActive(true);
            //gameMgr.SetStage(select_episodeNum);
        }
        else
        {
            gameMgr.SetStage(select_episodeNum);
            gameMgr.GameStart();
        }

    }


}
