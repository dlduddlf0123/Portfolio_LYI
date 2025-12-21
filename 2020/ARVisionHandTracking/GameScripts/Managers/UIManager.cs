using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ReadOnly;

public enum UIWindow
{
    TITLE,
    READY,
    MAIN,
    GAME,
    SETTING,
}

public class UIManager : MonoBehaviour
{
    GameManager gameMgr;
    public ARObjectSelect debugStarter;
    public int debugStageNum = 0;

    public WorldCanvas worldCanvas;
    public Fade fadeCanvas;
    public Canvas stageTitleCanvas;
    Text txt_stageTitle_title;
    Text txt_stageTitle_sub;

    public RectTransform ui_title;
    public RectTransform ui_main;
    public RectTransform ui_ready;
    public RectTransform ui_game;
    public RectTransform ui_setting;

    public UIWindow ui_currentWindow;
    public List<UIWindow> list_lastWindow = new List<UIWindow>();

    public GameObject stageSelect;
    public GameObject shadowPlane;

    //TitleUI
    Button title_startButton;
    AudioSource title_audio;
    AudioClip clip_bgm_title;

    //MainUI
    public Button main_btn_ARreset { get; set; }
    AudioSource main_audio;
    AudioClip clip_bgm_main;

    //ReadyUI
    Button ready_btn_back;
    public Button ready_btn_start { get; set; }
    Slider ready_btn_scale;

    //GameUI
    public Button game_btn_back { get; set; }
    public Button game_btn_skip { get; set; }

    public Text debug_txt_scene { get; set; }
    public Text debug_txt_timeline{ get; set; }
    public Text debug_txt_interaction { get; set; }

    //SettingUI
    public Text setting_txt_bgm { get; set; }
    public Text setting_txt_sfx { get; set; }
    public Text setting_txt_language { get; set; }
    
    public Slider setting_slider_BGM { get; set; }
    public Slider setting_slider_SFX { get; set; }
    public Dropdown setting_drop_language { get; set; }
    public Button setting_btn_close { get; set; }

    public Slider setting_sliderZ { get; set; }
    public Slider setting_sliderRight { get; set; }
    public Slider setting_sliderFront { get; set; }

    public Text uiText;
    public Text guestureText;
    public Slider Zslider;
    public Text zText;

    public Text txt_dialog;

    Coroutine ui_currentCoroutine = null;

    public float stageSize = 1f;
    /// <summary>
    /// UI 오브젝트들의 초기화, 할당 등
    /// </summary>
    private void Awake()
    {
        gameMgr = GameManager.Instance;

        txt_stageTitle_title = stageTitleCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        txt_stageTitle_sub = stageTitleCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>();

        //TitleUI
        title_startButton = ui_title.GetChild(1).GetComponent<Button>();
        title_audio = ui_title.GetComponent<AudioSource>();
        clip_bgm_title = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_TITLE);

        //ReadyUI
        ready_btn_back = ui_ready.GetChild(0).GetComponent<Button>();
        ready_btn_start = ui_ready.GetChild(1).GetComponent<Button>();
        ready_btn_scale = ui_ready.GetChild(3).GetComponent<Slider>();

        //MainUI
        main_btn_ARreset = ui_main.GetChild(1).GetComponent<Button>();
        main_audio = ui_main.GetComponent<AudioSource>();
        clip_bgm_main = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_SELECT);

        //GameUI
        game_btn_back = ui_game.GetChild(1).GetComponent<Button>();
        game_btn_skip = ui_game.GetChild(2).GetComponent<Button>();

        debug_txt_scene = ui_game.GetChild(3).GetComponent<Text>();
        debug_txt_timeline = ui_game.GetChild(4).GetComponent<Text>();
        debug_txt_interaction = ui_game.GetChild(5).GetComponent<Text>();

        //SettingUI
        setting_txt_bgm = ui_setting.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        setting_txt_sfx = ui_setting.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        setting_txt_language = ui_setting.GetChild(1).GetChild(0).GetComponent<Text>();

        setting_slider_BGM = ui_setting.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Slider>();
        setting_slider_SFX = ui_setting.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Slider>();
        setting_drop_language = ui_setting.GetChild(1).GetChild(1).GetComponent<Dropdown>();
        setting_btn_close = ui_setting.GetChild(0).GetChild(2).GetComponent<Button>();


        setting_sliderZ = ui_setting.GetChild(2).GetComponent<Slider>();
        setting_sliderRight = ui_setting.GetChild(3).GetComponent<Slider>();
        setting_sliderFront = ui_setting.GetChild(5).GetComponent<Slider>();
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
                btn[num].onClick.AddListener(() => gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0));
            }
        }

        Zslider.onValueChanged.AddListener(delegate
        {
           GameManager.Instance.arPlaneMgr.transform.localScale = Vector3.one * Zslider.value;
            zText.text = "MapSize: " + Zslider.value;
        });

        //TitleUI
        title_startButton.onClick.AddListener(() => TitleButtonStart());

        //MainUI
        main_btn_ARreset.onClick.AddListener(() => { GameReanchorButton(); });


        //ReadyUI
        ready_btn_back.onClick.AddListener(() => { SetUIActive(UIWindow.TITLE); });
        ready_btn_start.onClick.AddListener(() => { ReadyButtonStart(); });
        ready_btn_scale.onValueChanged.AddListener(delegate
        {
            stageSize = ready_btn_scale.value;
        });

        //GameUI
        game_btn_back.onClick.AddListener(() => { GameButtonBack(); });
        game_btn_skip.onClick.AddListener(() => {
            if (gameMgr.statGame == GameStatus.CUTSCENE)
            {
                gameMgr.currentEpisode.currentStage.EndCutScene();
            }
            else if (gameMgr.statGame == GameStatus.GAME)
            {
                gameMgr.currentEpisode.currentStage.EndInteraction();
            }
        });
        gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(false);
         

        debug_txt_scene.gameObject.SetActive(gameMgr.isDebug);
        debug_txt_timeline.gameObject.SetActive(gameMgr.isDebug);
        debug_txt_interaction.gameObject.SetActive(gameMgr.isDebug);

        //SettingUI
        setting_slider_BGM.value = gameMgr.soundMgr.bgmVolume;
        setting_slider_SFX.value = gameMgr.soundMgr.sfxVolume;

        setting_slider_BGM.onValueChanged.AddListener(delegate
        {
            gameMgr.soundMgr.bgmVolume = setting_slider_BGM.value;
            gameMgr.soundMgr.bgmSource.volume = gameMgr.soundMgr.bgmVolume;
        });
        setting_slider_SFX.onValueChanged.AddListener(delegate
        {
            gameMgr.soundMgr.sfxVolume = setting_slider_SFX.value;
        });


        setting_drop_language.value = (int)gameMgr.currentLanguage;
        setting_drop_language.onValueChanged.AddListener(delegate
        {
            gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0);
            gameMgr.currentLanguage = (GameLanguage)setting_drop_language.value;
            gameMgr.ChangeLanguage(gameMgr.currentLanguage);
        });

        setting_btn_close.onClick.AddListener(() => gameMgr.soundMgr.SaveVolume());


        setting_sliderZ.value = gameMgr.handCtrl.handPosZ;
        setting_sliderRight.value = gameMgr.handCtrl.handPosRight;
        setting_sliderFront.value = gameMgr.handCtrl.handPosFront;
        setting_sliderZ.transform.GetChild(0).GetComponent<Text>().text = "HandPosZ: " + setting_sliderZ.value;
        setting_sliderRight.transform.GetChild(0).GetComponent<Text>().text = "HandPosRight: " + setting_sliderRight.value;
        setting_sliderFront.transform.GetChild(0).GetComponent<Text>().text = "HandPosFront: " + setting_sliderFront.value;
        setting_sliderZ.onValueChanged.AddListener(delegate {
            gameMgr.handCtrl.handPosZ = setting_sliderZ.value;
            setting_sliderZ.transform.GetChild(0).GetComponent<Text>().text = "HandPosZ: " + setting_sliderZ.value;
        });
        setting_sliderRight.onValueChanged.AddListener(delegate {
            gameMgr.handCtrl.handPosRight = setting_sliderRight.value;
            setting_sliderRight.transform.GetChild(0).GetComponent<Text>().text = "HandPosRight: " + setting_sliderRight.value;
        });
        setting_sliderFront.onValueChanged.AddListener(delegate
        {
            gameMgr.handCtrl.handPosFront = setting_sliderFront.value;
            setting_sliderFront.transform.GetChild(0).GetComponent<Text>().text = "HandPosFront: " + setting_sliderFront.value;
        });
        // setting_btn_back.onClick.AddListener(()=>sett)

        SetUIActive(UIWindow.TITLE, false);
    }


    public void SetDialogText(string _text)
    {
        txt_dialog.text = _text;
        txt_dialog.transform.parent.gameObject.SetActive(true);
        if (ui_currentCoroutine == null)
        {
            ui_currentCoroutine = StartCoroutine(UITimer(txt_dialog.transform.parent.gameObject, 3f));
        }

    }


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
        ui_main.gameObject.SetActive(false);
        ui_ready.gameObject.SetActive(false);
        ui_game.gameObject.SetActive(false);
        ui_setting.gameObject.SetActive(false);

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
                ui_currentWindow = UIWindow.TITLE;

                gameMgr.soundMgr.ChangeBGMAudioSource(title_audio);
                gameMgr.soundMgr.PlayBgm(clip_bgm_title);
                gameMgr.ARSessionSetActive(false);
                break;
            case UIWindow.READY:
                ui_ready.gameObject.SetActive(true);
                ui_currentWindow = UIWindow.READY;
                break;
            case UIWindow.MAIN:
                ui_main.gameObject.SetActive(true);
                ui_currentWindow = UIWindow.MAIN;

                gameMgr.soundMgr.ChangeBGMAudioSource(main_audio);
                gameMgr.soundMgr.PlayBgm(clip_bgm_main);
                break;
            case UIWindow.GAME:
                ui_game.gameObject.SetActive(true);
                game_btn_back.gameObject.SetActive(false);
                ui_currentWindow = UIWindow.GAME;
                break;
            case UIWindow.SETTING:
                ui_setting.gameObject.SetActive(true);
                ui_currentWindow = UIWindow.SETTING;
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// AR 설정 시작
    /// 지면 인식, 영역 설정을 한다.
    /// 다음은 스테이지 선택
    /// </summary>
    public void TitleButtonStart()
    {
        gameMgr.ARSessionSetActive(true);   //AR 활성화

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
#if UNITY_EDITOR
        SetUIActive(UIWindow.GAME, false);
        debugStarter.gameMgr = gameMgr;
        debugStarter.lastSelect = debugStarter.transform.GetChild(0).GetComponent<ARSelectableObject>();
        debugStarter.SetStage(debugStageNum);
#else
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
#endif
    }


    /// <summary>
    /// 영역 설정 이후 시작 버튼 클릭 시
    /// </summary>
    public void ReadyButtonStart()
    {
        SetUIActive(UIWindow.MAIN, false);

        gameMgr.statGame = GameStatus.SELECT;

        //스테이지 선택 오브젝트 활성화
        stageSelect.SetActive(true);
        stageSelect.transform.parent.position = gameMgr.planeGenerator.placedPos;
        stageSelect.transform.parent.rotation = gameMgr.planeGenerator.placedRot;

        //for (int i = 1; i < stageSelect.transform.parent.childCount; i++)
        //{
        //    stageSelect.transform.parent.GetChild(i).GetComponent<ARObjectSelect>().PlayGuideParticle();
        //}

        //stageSelect.transform.parent.localScale *= gameMgr.uiMgr.stageSize;

        //아무것도 없을 시 그림자 생성용 바닥 활성화
        shadowPlane.SetActive(true);
        shadowPlane.transform.position = gameMgr.planeGenerator.placedPos;
        shadowPlane.transform.rotation = gameMgr.planeGenerator.placedRot;

        //생성된 바닥 기즈모 제거
        gameMgr.planeGenerator.isPlaced = true;
        gameMgr.planeGenerator.SetAllPlanesActive(false);
        gameMgr.arPlaneMgr.enabled = false; //바닥 생성 비활성화

        game_btn_back.gameObject.SetActive(true); //뒤로가기 버튼 활성화

        gameMgr.handCtrl.handColl.SetActive(true); //손 상호작용 활성화
        gameMgr.handCtrl.handFollower.SetActive(true);
    }

    /// <summary>
    /// 앵커 초기화, 재설정. 기존 활성화 오브젝트 비 활성화 후 포지션 조정
    /// /// </summary>
    public void GameReanchorButton()
    {
        SetUIActive(UIWindow.READY, false);

        gameMgr.statGame = GameStatus.ARPLANE;
        //스테이지 선택 오브젝트 활성화
        stageSelect.SetActive(false);
        //아무것도 없을 시 그림자 생성용 바닥 활성화
        shadowPlane.SetActive(false);

        //생성된 바닥 기즈모 제거
        gameMgr.planeGenerator.isPlaced = false;
        gameMgr.planeGenerator.SetAllPlanesActive(true);
        gameMgr.arPlaneMgr.enabled = true; //바닥 생성 비활성화

        game_btn_back.gameObject.SetActive(false); //뒤로가기 버튼 활성화
    }

    public void GameButtonBack()
    {
        stageSelect.SetActive(true);
        gameMgr.statGame = GameStatus.SELECT;

        gameMgr.currentEpisode.currentStage.gameObject.SetActive(false);

        Destroy(gameMgr.currentEpisode.currentStage.gameObject, 3);
        //game_btn_back.gameObject.SetActive(false);
    }


    /// <summary>
    /// 스테이지 시작 시 타이틀 이미지 페이드
    /// 각 스테이지에 맞는 설명 노출
    /// </summary>
    public void StageTitleFade(string _title, string _sub)
    {
        txt_stageTitle_title.text = _title;
        txt_stageTitle_sub.text = _sub;

        fadeCanvas.StartTitleFade(null, 2, 2);
    }

    public void ToggleDebug(Button _button)
    {
        gameMgr.isDebug = !gameMgr.isDebug;
        _button.transform.GetChild(0).GetComponent<Text>().text = "Debug\n" + gameMgr.isDebug.ToString();
        debug_txt_scene.gameObject.SetActive(gameMgr.isDebug);
        debug_txt_timeline.gameObject.SetActive(gameMgr.isDebug);
        debug_txt_interaction.gameObject.SetActive(gameMgr.isDebug);
    }
}
