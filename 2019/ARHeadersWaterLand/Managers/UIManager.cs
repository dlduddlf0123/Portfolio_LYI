using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.ReadOnly;

public class UIManager : MonoBehaviour {

    CSVparser parse = new CSVparser();
    GameManager gameMgr;
    SoundManager soundMgr;

    List<Button> list_buttons;
    List<TextMeshProUGUI> list_readyTexts;
    List<Text> list_resultTexts;
    List<Text> list_menuTexts;
    List<Text> list_settingTexts;
    List<Text> list_goalTexts;
    List<Text> list_tutorialTexts;

    //child(0) : Game UI
    public RectTransform gameUI { get; set; }
    public Image game_img_clearGauge { get; set; }
    public Image game_img_timeGauge { get; set; }
    public Text game_missiles { get; set; }
    public TextMeshProUGUI game_text_countDown { get; set; }
    Animator game_text_anim { get; set; }
    public Image game_img_countDown { get; set;  }
    public Button game_bt_menu { get; set; }

    //child(1) : Result UI
    public RectTransform resultUI { get; set; }
    public Image result_img_bg { get; set; }
    public Image result_img_text { get; set; }
    public Button result_bt_retry { get; set; }
    public Button result_bt_exit { get; set; }
    public Button result_bt_next { get; set; }
    public Text result_text_retry { get; set; }
    public Text result_text_menu { get; set; }
    public Text result_text_next { get; set; }

    //child(2) : Menu UI(=Stop)
    public RectTransform menuUI { get; set; }
    public Button menu_bt_close { get; set; }
    public RectTransform menu_bt_bgm { get; set; }
    public RectTransform menu_bt_sfx { get; set; }
    public Button menu_bt_retry { get; set; }
    public Button menu_bt_exit { get; set; }
    public Button menu_bt_test { get; set; }
    public Text menu_text_bgm_on { get; set; }
    public Text menu_text_bgm_off { get; set; }
    public Text menu_text_sfx_on { get; set; }
    public Text menu_text_sfx_off { get; set; }

    //child(3) : Title UI
    public RectTransform titleUI { get; set; }
    public RectTransform title_content { get; set; }
    public Button title_bt_left { get; set; }
    public Button title_bt_right { get; set; }
    List<GameObject> list_levels { get; set; }
    public Button title_bt_research { get; set; }
    public Button title_bt_setting { get; set; }

    //child(4) : Ready UI
    public RectTransform readyUI { get; set; }
    public Button ready_bt_play { get; set; }
    public Slider ready_slider_rotate { get; set; }
    public Slider ready_slider_scale { get; set; }
    public Slider ready_slider_height { get; set; }

    //child(5) : Goal UI
    public RectTransform goalUI { get; set; }
    public Text goal_text_time { get; set; }
    public Text goal_text_missile { get; set; }
    public Text goal_text_enemy { get; set; }
    public Button goal_bt_ok { get; set; }
    public Button goal_bt_exit { get; set; }

    //child(6) : Cheat UI
    public RectTransform cheatUI { get; set; }
    public Button cheat_bt_close { get; set; }
    public Transform cheat_layout { get; set; }
    public Slider cheat_slider_moveSpeed { get; set; }
    public Slider cheat_slider_shotSpeed { get; set; }
    public Slider cheat_slider_shotCoolDown { get; set; }
    public Slider cheat_slider_numHeader { get; set; }
    public Slider cheat_slider_numEnemy { get; set; }

    //child(7) : Tutorial UI
    public Button tutorialUI { get; set; }
    public Text tutorial_text_clear { get; set; }
    public Text tutorial_text_time { get; set; }
    public Text tutorial_text_bullet { get; set; }

    //Child(8) : WarningUI
    public RectTransform warningUI { get; set; }
    Button warning_ok;

    //child(11) : Setting UI
    public RectTransform settingUI { get; set; }
    public Button setting_bt_close { get; set; }
    public RectTransform setting_bt_bgm { get; set; }
    public RectTransform setting_bt_sfx { get; set; }
    public Button setting_bt_credits { get; set; }
    public Button setting_bt_language { get; set; }
    public Text setting_text_bgm_on { get; set; }
    public Text setting_text_bgm_off { get; set; }
    public Text setting_text_sfx_on { get; set; }
    public Text setting_text_sfx_off { get; set; }


    //child(12) : Credit UI
    public RectTransform creditUI { get; set; }
    public Button credit_bt_close { get; set; }

    public int playTime { get; set; }   //현재 남은 게임 시간
    public float maxClearGauge { get; set; } //클리어 게이지의 최대량
    public int max_count_level { get; set; }

    public bool bgmOn { get; set; }
    public bool sfxOn { get; set; }

    // Use this for initialization
    void Awake () {
        gameMgr = GameManager.Instance;
        soundMgr = gameMgr.soundMgr;
        list_buttons = new List<Button>();
        list_readyTexts = new List<TextMeshProUGUI>();
        list_resultTexts = new List<Text>();
        list_menuTexts = new List<Text>();
        list_settingTexts = new List<Text>();
        list_goalTexts = new List<Text>();
        list_tutorialTexts = new List<Text>();

        //Game UI
        gameUI = this.transform.GetChild(0).GetComponent<RectTransform>();
        game_img_clearGauge = gameUI.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        game_img_timeGauge = gameUI.transform.GetChild(1).GetChild(1).GetComponent<Image>();
        game_missiles = gameUI.transform.GetChild(2).GetComponent<Text>();
        game_text_countDown = gameUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        game_img_countDown = game_text_countDown.transform.GetChild(0).GetComponent<Image>();
        game_text_anim = game_img_countDown.GetComponent<Animator>();
        game_bt_menu = gameUI.transform.GetChild(4).GetComponent<Button>();
        list_buttons.Add(game_bt_menu);

        //Result UI
        resultUI = this.transform.GetChild(1).GetComponent<RectTransform>();
        result_img_bg = resultUI.GetChild(0).GetComponent<Image>();
        result_img_text = result_img_bg.transform.GetChild(0).GetComponent<Image>();
        result_bt_retry = result_img_bg.transform.GetChild(1).GetComponent<Button>();
        result_bt_exit = result_img_bg.transform.GetChild(2).GetComponent<Button>();
        result_bt_next = result_img_bg.transform.GetChild(3).GetComponent<Button>();
        list_buttons.Add(result_bt_retry);
        list_buttons.Add(result_bt_exit);
        list_buttons.Add(result_bt_next);
        Text resultretryText = result_bt_retry.transform.GetChild(0).GetComponent<Text>();
        Text resultexitText = result_bt_exit.transform.GetChild(0).GetComponent<Text>();
        Text resultnextText = result_bt_next.transform.GetChild(0).GetComponent<Text>();
        list_resultTexts.Add(resultretryText);
        list_resultTexts.Add(resultexitText);
        list_resultTexts.Add(resultnextText);

        //Menu UI
        menuUI = this.transform.GetChild(2).GetComponent<RectTransform>();
        menu_bt_close = menuUI.transform.GetChild(1).GetComponent<Button>();
        menu_bt_bgm = menuUI.transform.GetChild(2).GetComponent<RectTransform>();
        menu_text_bgm_on = menu_bt_bgm.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        menu_text_bgm_off = menu_bt_bgm.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        menu_bt_sfx = menuUI.transform.GetChild(3).GetComponent<RectTransform>();
        menu_text_sfx_on = menu_bt_sfx.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        menu_text_sfx_off = menu_bt_sfx.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        menu_bt_retry = menuUI.transform.GetChild(4).GetComponent<Button>();
        menu_bt_exit = menuUI.transform.GetChild(5).GetComponent<Button>();
        menu_bt_test = menuUI.transform.GetChild(6).GetComponent<Button>();
        list_buttons.Add(menu_bt_close);
        Button bgmOn = menu_bt_bgm.transform.GetChild(0).GetComponent<Button>();
        Button bgmOff = menu_bt_bgm.transform.GetChild(1).GetComponent<Button>();
        Button sfxOn = menu_bt_sfx.transform.GetChild(0).GetComponent<Button>();
        Button sfxOff = menu_bt_sfx.transform.GetChild(1).GetComponent<Button>();
        list_buttons.Add(bgmOn);
        list_buttons.Add(bgmOff);
        list_buttons.Add(sfxOn);
        list_buttons.Add(sfxOff);
        list_buttons.Add(menu_bt_retry);
        list_buttons.Add(menu_bt_exit);
        list_buttons.Add(menu_bt_test);
        Text bgmText = menuUI.GetChild(7).GetChild(0).GetComponent<Text>();
        Text sfxText = menuUI.GetChild(8).GetChild(0).GetComponent<Text>();
        list_menuTexts.Add(bgmText);
        list_menuTexts.Add(sfxText);
        list_menuTexts.Add(menu_text_bgm_on);
        list_menuTexts.Add(menu_text_sfx_on);
        Text menuCloseText = menu_bt_close.transform.GetChild(0).GetComponent<Text>();
        Text menuRetryText = menu_bt_retry.transform.GetChild(0).GetComponent<Text>();
        Text menuExitText = menu_bt_exit.transform.GetChild(0).GetComponent<Text>();
        list_menuTexts.Add(menuCloseText);
        list_menuTexts.Add(menuRetryText);
        list_menuTexts.Add(menuExitText);


        //Title UI
        titleUI = this.transform.GetChild(3).GetComponent<RectTransform>();
        title_content = titleUI.GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        title_bt_left = titleUI.GetChild(1).GetComponent<Button>();
        title_bt_right = titleUI.GetChild(2).GetComponent<Button>();
        title_bt_research = titleUI.GetChild(3).GetComponent<Button>();
        title_bt_setting = titleUI.GetChild(4).GetComponent<Button>();
        list_levels = new List<GameObject>();
        list_buttons.Add(title_bt_left);
        list_buttons.Add(title_bt_right);
        list_buttons.Add(title_bt_research);
        list_buttons.Add(title_bt_setting);

        //Ready UI
        readyUI = this.transform.GetChild(4).GetComponent<RectTransform>();
        ready_bt_play = readyUI.GetChild(0).GetComponent<Button>();
        ready_slider_rotate = readyUI.transform.GetChild(1).GetComponent<Slider>();
        ready_slider_scale = readyUI.transform.GetChild(2).GetComponent<Slider>();
        ready_slider_height = readyUI.transform.GetChild(3).GetComponent<Slider>();
        list_buttons.Add(ready_bt_play);
        TextMeshProUGUI scaleText = ready_slider_scale.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI rotateText = ready_slider_rotate.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI heightText = ready_slider_height.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        list_readyTexts.Add(scaleText);
        list_readyTexts.Add(rotateText);
        list_readyTexts.Add(heightText);


        //Goal UI
        goalUI = this.transform.GetChild(5).GetComponent<RectTransform>();
        goal_bt_ok = goalUI.GetChild(1).GetComponent<Button>();
        goal_text_time = goalUI.GetChild(2).GetChild(0).GetComponent<Text>();
        goal_text_missile = goalUI.GetChild(2).GetChild(1).GetComponent<Text>();
        goal_text_enemy = goalUI.GetChild(2).GetChild(2).GetComponent<Text>();
        goal_bt_exit = goalUI.GetChild(3).GetComponent<Button>();
        list_buttons.Add(goal_bt_ok);
        list_buttons.Add(goal_bt_exit);
        list_goalTexts.Add(goal_text_time);
        list_goalTexts.Add(goal_text_missile);
        list_goalTexts.Add(goal_text_enemy);
        Text goalOk = goal_bt_ok.transform.GetChild(0).GetComponent<Text>();
        list_goalTexts.Add(goalOk);


        //Cheat UI
        cheatUI = this.transform.GetChild(6).GetComponent<RectTransform>();
        cheat_bt_close = cheatUI.transform.GetChild(1).GetComponent<Button>();
        cheat_layout = cheatUI.transform.GetChild(2).GetComponent<Transform>();
        cheat_slider_moveSpeed = cheat_layout.GetChild(0).GetComponent<Slider>();
        cheat_slider_shotSpeed = cheat_layout.GetChild(1).GetComponent<Slider>();
        cheat_slider_shotCoolDown = cheat_layout.GetChild(2).GetComponent<Slider>();
        cheat_slider_numHeader = cheat_layout.GetChild(3).GetComponent<Slider>();
        cheat_slider_numEnemy = cheat_layout.GetChild(4).GetComponent<Slider>();

        //Tutorial UI
        tutorialUI = this.transform.GetChild(7).GetComponent<Button>();
        tutorial_text_clear = tutorialUI.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        tutorial_text_time = tutorialUI.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        tutorial_text_bullet = tutorialUI.transform.GetChild(2).GetChild(0).GetComponent<Text>();
        list_buttons.Add(tutorialUI);
        list_tutorialTexts.Add(tutorial_text_clear);
        list_tutorialTexts.Add(tutorial_text_time);
        list_tutorialTexts.Add(tutorial_text_bullet);

        //Warning UI
        warningUI = this.transform.GetChild(8).GetComponent<RectTransform>();
        warning_ok = warningUI.GetChild(2).GetComponent<Button>();
        list_buttons.Add(warning_ok);


        //Setting UI
        settingUI = this.transform.GetChild(11).GetComponent<RectTransform>();
        setting_bt_close = settingUI.transform.GetChild(1).GetComponent<Button>();
        setting_bt_bgm = settingUI.transform.GetChild(2).GetComponent<RectTransform>();
        setting_text_bgm_on = setting_bt_bgm.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        setting_text_bgm_off = setting_bt_bgm.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        setting_bt_sfx = settingUI.transform.GetChild(3).GetComponent<RectTransform>();
        setting_text_sfx_on = setting_bt_sfx.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        setting_text_sfx_off = setting_bt_sfx.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        setting_bt_credits = settingUI.transform.GetChild(4).GetComponent<Button>();
        setting_bt_language = settingUI.transform.GetChild(5).GetComponent<Button>();

        Button bgmOn2 = setting_bt_bgm.transform.GetChild(0).GetComponent<Button>();
        Button bgmOff2 = setting_bt_bgm.transform.GetChild(1).GetComponent<Button>();
        Button sfxOn2 = setting_bt_sfx.transform.GetChild(0).GetComponent<Button>();
        Button sfxOff2 = setting_bt_sfx.transform.GetChild(1).GetComponent<Button>();
        list_buttons.Add(bgmOn2);
        list_buttons.Add(bgmOff2);
        list_buttons.Add(sfxOn2);
        list_buttons.Add(sfxOff2);
        list_buttons.Add(setting_bt_language);
        list_buttons.Add(setting_bt_credits);
        list_buttons.Add(setting_bt_close);

        Text bgmText2 = settingUI.GetChild(6).GetChild(0).GetComponent<Text>();
        Text sfxText2 = settingUI.GetChild(7).GetChild(0).GetComponent<Text>();
        list_settingTexts.Add(bgmText2);
        list_settingTexts.Add(sfxText2);
        list_settingTexts.Add(setting_text_bgm_on);
        list_settingTexts.Add(setting_text_sfx_on);
        Text settingCloseText = setting_bt_close.transform.GetChild(0).GetComponent<Text>();
        Text settingCreditsText = setting_bt_credits.transform.GetChild(0).GetComponent<Text>();
        Text settingLanguageText = setting_bt_language.transform.GetChild(0).GetComponent<Text>();
        list_settingTexts.Add(settingCloseText);
        list_settingTexts.Add(settingCreditsText);
        list_settingTexts.Add(settingLanguageText);

        //Credit UI
        creditUI = this.transform.GetChild(12).GetComponent<RectTransform>();
        credit_bt_close = creditUI.GetChild(1).GetComponent<Button>();

        //Active False All Contents
        gameUI.gameObject.SetActive(false);
        resultUI.gameObject.SetActive(false);
        menuUI.gameObject.SetActive(false);
        titleUI.gameObject.SetActive(false);
        readyUI.gameObject.SetActive(false);
        warningUI.gameObject.SetActive(true);
        settingUI.gameObject.SetActive(false);
        creditUI.gameObject.SetActive(false);
    }

    void Start()
    {
        //Debug.Log("UI_MANAGER_START");

        //titleUI.onClick.AddListener(delegate () { gameMgr.GameStart(gameMgr.currentLevel); });
        title_bt_research.onClick.AddListener(() => { gameMgr.LoadScene(0);  });
        title_bt_setting.onClick.AddListener(() => { settingUI.gameObject.SetActive(true); });

        //Game UI
        game_img_clearGauge.fillAmount = 0.0f;
        game_img_timeGauge.fillAmount = 1.0f;
        game_bt_menu.onClick.AddListener(() => { GameMenuToggle(true);});

        //Result UI
        result_bt_retry.onClick.AddListener(ResultRetryButton);
        result_bt_exit.onClick.AddListener(ResultMenuButton);
        result_bt_next.onClick.AddListener(ResultNextButton);

        //Menu UI
        menu_bt_close.onClick.AddListener(delegate () { GameMenuToggle(false); });
        //BGM ON
        gameMgr.soundMgr.bgmVolume = 1.0f;
        menu_bt_bgm.transform.GetChild(0).gameObject.SetActive(true);
        menu_bt_bgm.transform.GetChild(1).gameObject.SetActive(false);
        //Debug.Log("BGM ON");
        bgmOn = true;
        //SFX ON
        gameMgr.soundMgr.sfxVolume = 1.0f;
        menu_bt_sfx.transform.GetChild(0).gameObject.SetActive(true);
        menu_bt_sfx.transform.GetChild(1).gameObject.SetActive(false);
        //Debug.Log("SFX ON");
        sfxOn = true;

        menu_bt_retry.onClick.AddListener(ResultRetryButton);
        menu_bt_exit.onClick.AddListener(MenuExitButton);
        menu_bt_test.onClick.AddListener(() => {
            cheatUI.gameObject.SetActive(true);
            menuUI.gameObject.SetActive(false); });

        //Ready UI
        ready_bt_play.onClick.AddListener(PlayButton);
        ready_slider_rotate.onValueChanged.AddListener(delegate { gameMgr.stage.transform.parent.localRotation = Quaternion.Euler(0, ready_slider_rotate.value, 0); });
        ready_slider_scale.onValueChanged.AddListener(delegate { gameMgr.stage.transform.parent.localScale = new Vector3(ready_slider_scale.value, ready_slider_scale.value, ready_slider_scale.value); });
        ready_slider_height.onValueChanged.AddListener(delegate { gameMgr.stage.transform.parent.localPosition = new Vector3(gameMgr.stage.transform.parent.localPosition.x, ready_slider_height.value, gameMgr.stage.transform.parent.localPosition.z); });

        //GoalUI
        goal_bt_ok.onClick.AddListener(() =>
            {
                goalUI.gameObject.SetActive(false);

                //게임 UI 활성화
                gameUI.gameObject.SetActive(true);

                Debug.Log("!Tutorial Number =" + gameMgr.tutorialNum);
                if (gameMgr.tutorialNum < 3
                && gameMgr.tutorialNum == gameMgr.currentLevel-1)
                {
                    tutorialUI.gameObject.SetActive(true);
                    tutorialUI.transform.GetChild(gameMgr.tutorialNum).gameObject.SetActive(true);
                }
                else
                    StartCoroutine(CountDown());
            });
        goal_bt_exit.onClick.AddListener(() =>
        {
            Application.OpenURL("market:details?id=com.SavanaFactory.Savanavana");
            goalUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(false);
            titleUI.gameObject.SetActive(true);
            soundMgr.PlayBgm(soundMgr.bgm_title);
            //TitleLevelUnLock();
        });

        //cheatUI
        cheat_bt_close.onClick.AddListener(() => {
            cheatUI.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        });
        cheat_slider_moveSpeed.maxValue = 5f;
        cheat_slider_moveSpeed.minValue = 0.1f;
        cheat_slider_moveSpeed.value = 1f;
        cheat_slider_moveSpeed.onValueChanged.AddListener(delegate {
            for (int i = 0; i < gameMgr.limit_headers+1; i++)
            {
                gameMgr.list_Headers[i].accel = cheat_slider_moveSpeed.value;
                gameMgr.list_Headers[i].bezierCurve.moveSpeed = cheat_slider_moveSpeed.value;
            } });

        cheat_slider_shotSpeed.minValue = 0.1f;
        cheat_slider_shotSpeed.maxValue = 5.0f;
        cheat_slider_shotSpeed.value = 1f;
        cheat_slider_shotSpeed.onValueChanged.AddListener(delegate { gameMgr.missileMgr.missileSpeed *= cheat_slider_shotSpeed.value; });

        cheat_slider_shotCoolDown.minValue = 0f;
        cheat_slider_shotCoolDown.maxValue = 2f;
        cheat_slider_shotCoolDown.value = 0.3f;
        cheat_slider_shotCoolDown.onValueChanged.AddListener(delegate { gameMgr.missileMgr.shotDelay = cheat_slider_shotCoolDown.value; });
        //cheat_slider_numHeader.onValueChanged.AddListener(delegate { });
        //cheat_slider_numEnemy.onValueChanged.AddListener(delegate { });


        tutorialUI.onClick.AddListener(() =>
        {
            if (gameMgr.tutorialNum >= 0)
            {
                tutorialUI.gameObject.SetActive(false);
                tutorialUI.transform.GetChild(gameMgr.tutorialNum).gameObject.SetActive(false);
                gameMgr.tutorialNum++;
                PlayerPrefs.SetInt("TutorialNum", gameMgr.tutorialNum);
            }
            else
            {
                tutorialUI.gameObject.SetActive(false);
                tutorialUI.transform.GetChild(gameMgr.tutorialNum).gameObject.SetActive(false);
                gameMgr.tutorialNum = -1;
                PlayerPrefs.SetInt("TutorialNum", gameMgr.tutorialNum);
            }
            StartCoroutine(CountDown());
        });


        //WarningUI
        warning_ok.onClick.AddListener(
            () => {
                warningUI.gameObject.SetActive(false);
                gameMgr.arCore.SetActive(true);
            }
        );

        //Setting UI
        setting_bt_close.onClick.AddListener(() => settingUI.gameObject.SetActive(false));
        //BGM ON
        gameMgr.soundMgr.bgmVolume = 1.0f;
        setting_bt_bgm.transform.GetChild(0).gameObject.SetActive(true);
        setting_bt_bgm.transform.GetChild(1).gameObject.SetActive(false);
        //Debug.Log("BGM ON");
        bgmOn = true;
        //SFX ON
        gameMgr.soundMgr.sfxVolume = 1.0f;
        setting_bt_sfx.transform.GetChild(0).gameObject.SetActive(true);
        setting_bt_sfx.transform.GetChild(1).gameObject.SetActive(false);
        //Debug.Log("SFX ON");
        sfxOn = true;

        setting_bt_credits.onClick.AddListener(() => creditUI.gameObject.SetActive(true));
        setting_bt_language.onClick.AddListener(() => {LanguageButton(); });
        
        if (gameMgr.language == 0)
            setting_bt_language.GetComponent<Image>().sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_BUTTON_KOREAN);
        else
            setting_bt_language.GetComponent<Image>().sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_BUTTON_ENGLISH);

        credit_bt_close.onClick.AddListener(() => creditUI.gameObject.SetActive(false));

        max_count_level = gameMgr.LevelCount()-1;    //생성할 버튼 개수 = 최대 레벨 개수 - 1 (level글자 빼고)

        //일자 스크롤 형 레벨 선택 창
        for (int i = 1; i < max_count_level +1; i++)
        {
            int _level = i; //!지역 변수로 i를 저장하지 않고 넘길경우 i의 최대값이 넘어가게 된다!
            GameObject button = Instantiate(gameMgr.b_prefabs.LoadAsset(Defines.PREFAB_BUTTON_LEVEL), title_content) as GameObject;
            button.gameObject.name = "Level " + i;
            button.transform.GetChild(0).GetComponent<Text>().text = "Level " + i;
            button.GetComponent<Button>().onClick.AddListener(() => { TitleLevelButton(_level); soundMgr.PlaySfx(button.transform.position, soundMgr.sfx_bt); });
            button.GetComponent<Button>().interactable = false;
            button.transform.GetChild(1).gameObject.SetActive(true); // lock 이미지
            list_levels.Add(button);
            
        }

        TitleLevelUnLock();
        
        foreach (Button _bt in list_buttons)
        {
            _bt.onClick.AddListener(() => soundMgr.PlaySfx(_bt.transform.position, soundMgr.sfx_bt));
        }

        ChangeTextLanguage();

    }//Start End

    //게이지 증/감
    //대가리들이 구름에 묻을 때 마다 증가한다
    //게이지 총량은 대가리들 숫자에 비례한다
    //기본 3대, 캐릭터에 따라 더 많을 수도 있다.
    //총 20스테이지, 대가리들 총 5마리?
    public void GaugePlus(int _damage)
    {
        //게이지 변경
        game_img_clearGauge.fillAmount += _damage / gameMgr.sumHp;
    }


    //만약 지형에 따라 다시 지저분해 질 수 있는 경우 사용한다
    //진흙에 묻은 대가리마다 씻기는 양이 다르므로 어떤 타입인지 확인한다
    //모두 체력 3으로 동일하므로 통일한다
    public void GaugeMinus(int _heal)
    {
        game_img_clearGauge.fillAmount -= _heal / gameMgr.sumHp;
        /*switch (_head)
        {
            case Headers.GIRRAFE:
                //게이지 변경
                game_img_clearGauge.fillAmount -= _heal / gameMgr.sumHp;
                break;
            case Headers.ZEBRA:
                //게이지 변경
                game_img_clearGauge.fillAmount -= _heal / gameMgr.sumHp;
                break;
            case Headers.RHINO:
                game_img_clearGauge.fillAmount -= _heal / gameMgr.sumHp;
                break;
            default:
                break;
        }*/
    }


    /// <summary>
    /// 텍스트를 CSV파일에 맞춰 변경하기
    /// </summary>
    public void ChangeTextLanguage()
    {
        //SetArrayText(0, list_readyTexts);(Textpro에서 한글이 깨짐)
        SetArrayText(1, list_resultTexts);
        SetArrayText(2, list_menuTexts);
        SetArrayText(3, list_goalTexts);
        SetArrayText(4, list_tutorialTexts);
        SetArrayText(6, list_settingTexts);

        menu_text_bgm_on.text = ReadTextData(2, 2);
        menu_text_bgm_off.text = ReadTextData(2, 3);
        menu_text_sfx_on.text = ReadTextData(2, 2);
        menu_text_sfx_off.text = ReadTextData(2, 3);

        Text warningText = warningUI.GetChild(1).GetChild(1).GetComponent<Text>();
        warningText.text = ReadTextData(5, 0);
        SplitText(warningText);

        setting_text_bgm_on.text = ReadTextData(2, 2);
        setting_text_bgm_off.text = ReadTextData(2, 3);
        setting_text_sfx_on.text = ReadTextData(2, 2);
        setting_text_sfx_off.text = ReadTextData(2, 3);

    }


    /// <summary>
    /// 배열 내 텍스트 변경
    /// </summary>
    /// <param name="_ui">0: ready 1: result 2: menu 3: goal 4: tutorial</param>
    /// <param name="_list"></param>
    public void SetArrayText(int _ui, List<Text> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i].text = ReadTextData(_ui, i);
        }
        if (_ui == 4)
        {
            string t;
            for (int i = 0; i < _list.Count; i++)
            {
                _list[i].text = null;
                t = ReadTextData(_ui, i);
                string[] _t = t.Split('@');
                for (int j = 0; j < _t.Length; j++)
                {
                    _list[i].text += _t[j];
                    if (j < _t.Length - 1)
                    {
                        _list[i].text += '\n';
                    }
                }
            }
        }
    }
    //OverLoading
    public void SetArrayText(int _ui, List<TextMeshProUGUI> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i].text = ReadTextData(_ui, i);
        }
    }
    //Spit Text with'@'
    public void SplitText(Text txt)
    {
        string t = txt.text;
        txt.text = null;
        string[] _t = t.Split('@');
        for (int j = 0; j < _t.Length; j++)
        {
            txt.text += _t[j];
            if (j < _t.Length - 1)
            {
                txt.text += '\n';
            }
        }
    }

    /// <summary>
    /// CSV 파일 텍스트 읽어오기
    /// </summary>
    /// <param name="_ui">어떤 UI인지</param>
    /// <param name="_text">몇 번째 텍스트 인지</param>
    /// <returns>돌려줄 텍스트</returns>
    public string ReadTextData(int _ui, int _text)
    {
        Table table = parse.ParsingCSV(Defines.CSV_UITEXT_DATA_KOR);
        switch (gameMgr.language)
        {
            case 0:
                table = parse.ParsingCSV(Defines.CSV_UITEXT_DATA_KOR);
                break;
            case 1:
                table = parse.ParsingCSV(Defines.CSV_UITEXT_DATA_ENG);
                break;
        }
        string _data = table.Row[_ui].Col[_text+1].ToString();
        return _data;
    }


    //------------------------------버튼 동작 함수들------------------------------------
    //ReadyUI
    /// <summary>
    /// 게임 시작 버튼, 타이틀 화면을 활성화 및 맵 탐색기능을 정지시킨다
    /// </summary>
    public void PlayButton()
    {
        gameMgr.DeActivateARCoreSupport();
    }

    //GameUI
    /// <summary>
    /// 게임 메뉴를 여는 버튼(일시정지 버튼)
    /// </summary>
    /// <param name="_isOpen">true:메뉴창 열기/false:메뉴창 닫기</param>
    public void GameMenuToggle(bool _isOpen)
    {
        //준비중(로딩,카운트다운) 중에 사용하지 않음
        if (gameMgr.gameState == GameState.READY) return;
        menuUI.gameObject.SetActive(_isOpen);

        //시간 정지
        if (_isOpen == true
            && gameMgr.gameState == GameState.PLAYING)
        {
            gameMgr.gameState = GameState.STOP;
            Debug.Log("GameState : " + gameMgr.gameState);
            gameMgr.missileMgr.gameObject.SetActive(false);
            Time.timeScale = 0.0f;
        }
        else if (_isOpen == false
            && gameMgr.gameState == GameState.STOP)
        {
            Time.timeScale = 1.0f;
            gameMgr.gameState = GameState.PLAYING;
            Debug.Log("GameState : " + gameMgr.gameState);
            gameMgr.missileMgr.gameObject.SetActive(true);
        }
    }

    //ResultUI
    /// <summary>
    /// 현재 스테이지 재 시작
    /// </summary>
    public void ResultRetryButton()
    {
        //스테이지 재 시작
        gameMgr.GameOver(false);
        gameMgr.GameStart(gameMgr.currentLevel);
    }

    /// <summary>
    /// 스테이지 선택창으로 이동
    /// </summary>
    public void ResultMenuButton()
    {
        resultUI.gameObject.SetActive(false);
        titleUI.gameObject.SetActive(true);
        soundMgr.PlayBgm(soundMgr.bgm_title);
        TitleLevelUnLock();
    }

    /// <summary>
    /// 다음 스테이지 시작
    /// </summary>
    public void ResultNextButton()
    {
        if (gameMgr.currentLevel >= max_count_level)
        {
            return;
        }
        //현재 스테이지와 최대 클리어 스테이지를 비교
        if (gameMgr.currentLevel > gameMgr.clearLevel)
        {
            //Msgbox(3f,txt);
            Debug.Log("현재 스테이지를 클리어 해야 다음 스테이지로 갈 수 있습니다.");
            Debug.Log("현재 클리어레벨: " + gameMgr.clearLevel);
            return;
        }
        else
        {
            //다음 스테이지 호출
            gameMgr.currentLevel++;
            gameMgr.GameStart(gameMgr.currentLevel);
        }
    }
    

    //Menu UI
    //BGM 버튼
    public void MenuBGMToggle()
    {
        if (bgmOn == true)
        {
            gameMgr.soundMgr.bgmSource.Stop();
            menu_bt_bgm.transform.GetChild(0).gameObject.SetActive(false);
            menu_bt_bgm.transform.GetChild(1).gameObject.SetActive(true);
            setting_bt_bgm.transform.GetChild(0).gameObject.SetActive(false);
            setting_bt_bgm.transform.GetChild(1).gameObject.SetActive(true);
            Debug.Log("BGM OFF");
            bgmOn = false;
        }
        else
        {
            gameMgr.soundMgr.bgmSource.Play();
            menu_bt_bgm.transform.GetChild(0).gameObject.SetActive(true);
            menu_bt_bgm.transform.GetChild(1).gameObject.SetActive(false);
            setting_bt_bgm.transform.GetChild(0).gameObject.SetActive(true);
            setting_bt_bgm.transform.GetChild(1).gameObject.SetActive(false);
            Debug.Log("BGM ON");
            bgmOn = true;
        }
    }

    //SFX 버튼
    public void MenuSFXToggle()
    {
        if (sfxOn == true)
        {
            gameMgr.soundMgr.sfxVolume = 0.0f;
            menu_bt_sfx.transform.GetChild(0).gameObject.SetActive(false);
            menu_bt_sfx.transform.GetChild(1).gameObject.SetActive(true);
            setting_bt_sfx.transform.GetChild(0).gameObject.SetActive(false);
            setting_bt_sfx.transform.GetChild(1).gameObject.SetActive(true);
            Debug.Log("SFX OFF");
            sfxOn = false;
        }
        else
        {
            gameMgr.soundMgr.sfxVolume = 1.0f;
            menu_bt_sfx.transform.GetChild(0).gameObject.SetActive(true);
            menu_bt_sfx.transform.GetChild(1).gameObject.SetActive(false);
            setting_bt_sfx.transform.GetChild(0).gameObject.SetActive(true);
            setting_bt_sfx.transform.GetChild(1).gameObject.SetActive(false);
            Debug.Log("SFX ON");
            sfxOn = true;
        }
    }

    /// <summary>
    /// 나가기 버튼 클릭 시
    /// </summary>
    public void MenuExitButton()
    {
        Time.timeScale = 1.0f;
        gameMgr.GameOver(false);
        resultUI.gameObject.SetActive(false);
        titleUI.gameObject.SetActive(true);
        soundMgr.PlayBgm(soundMgr.bgm_title);
        TitleLevelUnLock();
    }

    ///// <summary>
    ///// 스크롤 뷰가 버튼 위치에 있을 때 위치를 맞춰준다
    ///// </summary>
    //public IEnumerator TitleContent()
    //{
    //    Debug.Log("!Title!");
    //    while (titleUI.gameObject.activeSelf == true
    //        && title_content.localPosition.x % 300 <= -10)
    //    {
    //        title_content.transform.localPosition = new Vector3(
    //            Vector3.Lerp(title_content.localPosition,
    //            -list_levels[Mathf.Abs((int)(title_content.localPosition.x / 300))].transform.localPosition,
    //            30f * Time.deltaTime).x, 0, 0);
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    Debug.Log("!Title! End");
    //}
    ///// <summary>
    ///// 콘텐츠 내의 레벨 버튼이 딱딱 떨어지도록 이동한다
    ///// </summary>
    //public IEnumerator TitleLeftButton()
    //{
    //    int _listNum = Mathf.Abs((int)((title_content.localPosition += new Vector3(300, 0, 0)).x / 300));
    //    Vector3 _target = -list_levels[_listNum].transform.localPosition;
    //    Debug.Log("!Left!"); 
    //    while (title_content.localPosition.x %300 <= -10)
    //    {
    //        title_content.transform.localPosition = new Vector3(
    //            Vector3.Lerp(title_content.transform.localPosition,
    //            _target,
    //            30f * Time.deltaTime).x, 0, 0);
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    Debug.Log("!Left!End");
    //}

    //public void TitleRightButton()
    //{
    //    title_content.transform.localPosition -= new Vector3(300, 0, 0);
    //}

    //Title UI
    /// <summary>
    /// 레벨 버튼 클릭 시 레벨 값 전달
    /// </summary>
    /// <param name="_level">레벨 버튼의 레벨값</param>
    void TitleLevelButton(int _level)
    {
        gameMgr.currentLevel = _level;
        Debug.Log("Game Start Level: " + gameMgr.currentLevel);
        gameMgr.GameStart(_level);
    }

    /// <summary>
    /// 타이틀 화면에서 레벨 잠금 해제
    /// </summary>
    public void TitleLevelUnLock()
    {
        if (list_levels.Count < gameMgr.clearLevel)
        {
            Debug.Log("클리어 레벨이 최대 레벨 수보다 많음");
            return;
        }

        for (int i = 0; i <= gameMgr.clearLevel; i++)
        {
            list_levels[i].GetComponent<Button>().interactable = true;
            list_levels[i].transform.GetChild(1).gameObject.SetActive(false);

            //select_list_levels[i].GetComponent<Button>().interactable = true;
            //select_list_levels[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    //Goal UI
    /// <summary>
    /// 목표치 텍스트로 보여주기
    /// </summary>
    public void ShowGoal(int _t, int _m, int _e)
    {
        if (gameMgr.currentLevel >= 11 && gameMgr.isDemo == true)
        {
            goalUI.gameObject.SetActive(true);
            goal_text_time.gameObject.SetActive(true);
            goal_text_missile.gameObject.SetActive(false);
            goal_text_enemy.gameObject.SetActive(false);

            if (gameMgr.language == 0)
                goal_text_time.text = "Savanavana를 플레이 해주셔서 감사합니다 새로운 캐릭터들과 스토리, 게임을 플레이 하고 싶으시면 플레이스토어에서 유료 버전을 구매해 주시기 바랍니다.";
            else
                goal_text_time.text = "Thank you for playing Savanavana. If you want to play new characters, stories, and games, you can purchase a paid version from the Play Store.";

            goal_text_time.fontSize = 48;
            goal_bt_ok.gameObject.SetActive(false);
            goal_bt_exit.gameObject.SetActive(true);
        }
        else
        {
            gameMgr.gameState = GameState.READY;
            goalUI.gameObject.SetActive(true);
            goal_text_time.gameObject.SetActive(true);
            goal_text_missile.gameObject.SetActive(true);
            goal_text_enemy.gameObject.SetActive(true);
            goal_bt_ok.gameObject.SetActive(true);
            goal_bt_exit.gameObject.SetActive(false);

            goal_text_time.text = "Time : " + _t;
            goal_text_time.fontSize = 100;
            goal_text_missile.text = "Missile : " + _m;
            goal_text_enemy.text = "Enemy : " + _e;

            if (_t == 0)
            {
                goal_text_time.gameObject.SetActive(false);
            }
            if (_m == 0)
            {
                goal_text_missile.gameObject.SetActive(false);
            }
            if (_e == 0)
            {
                goal_text_enemy.gameObject.SetActive(false);
            }

            if (_t == 0
                && _m == 0
                && _e == 0)
            {
                goal_text_time.text = "No Limits";
                goal_text_time.gameObject.SetActive(true);
            }
        }

        soundMgr.PlayBgm(null);
    }

    //SettingUI
    //Change Language
    public void LanguageButton()
    {
        switch (gameMgr.language)
        {
            case 0:
                gameMgr.language++;
                PlayerPrefs.SetInt("Language", gameMgr.language);
                setting_bt_language.GetComponent<Image>().sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_BUTTON_ENGLISH);
                ChangeTextLanguage();
                break;
            case 1:
                gameMgr.language = 0;
                PlayerPrefs.SetInt("Language", gameMgr.language);
                setting_bt_language.GetComponent<Image>().sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_BUTTON_KOREAN);
                ChangeTextLanguage();
                break;
        }
    }

    //--------------------------------------Coroutines------------------------------------------
    //시작 시 카운트다운
    public IEnumerator CountDown()
    {
        //캐릭터 활성화
        gameMgr.SetActiveCharacters(true);

        float countTime = 1.0f; //카운트 텀
        game_text_countDown.gameObject.SetActive(true);
        game_text_countDown.text = gameMgr.currentLevel.ToString();
        game_img_countDown.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXT_LEVEL) as Sprite;
        game_img_countDown.transform.localPosition = new Vector3(0, 300, 0);
        yield return new WaitForSeconds(countTime);
        game_text_countDown.text = " ";
        game_text_countDown.gameObject.SetActive(false);
        game_text_countDown.gameObject.SetActive(true);
        game_img_countDown.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXT_READY) as Sprite;
        game_img_countDown.transform.localPosition = new Vector3(0, 100, 0);
        //gameMgr.soundMgr.PlaySfx(transform.position, Resources.Load<AudioClip>(Defines.SOUND_SFX_COUNT) as AudioClip);
        yield return new WaitForSeconds(countTime);
        game_img_countDown.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXT_START) as Sprite;
        game_text_countDown.gameObject.SetActive(false);
        game_text_countDown.gameObject.SetActive(true);
        //gameMgr.soundMgr.PlaySfx(transform.position, Resources.Load<AudioClip>(Defines.SOUND_SFX_START) as AudioClip);
        //게임 시작!
        //컨트롤 관련 호출
        gameMgr.gameState = GameState.PLAYING;
        Debug.Log("GameState : " + gameMgr.gameState);

        //미사일 활성화
        gameMgr.missileMgr.gameObject.SetActive(true);

        //BGM 재생
        gameMgr.soundMgr.PlayBgm(gameMgr.soundMgr.bgm_gamePlay);
        
        //시간이 감소하기 시작한다
        if (gameMgr.limit_playTime != 0)
        {
            StartCoroutine(GameTimer());
        }

        yield return new WaitForSeconds(countTime);
        game_text_countDown.gameObject.SetActive(false);
    }

    //게임 시간 세어주는 코루틴
    public IEnumerator GameTimer()
    {
        int playTime = gameMgr.limit_playTime; //게임 오버까지의 남은 시간 

        //게임이 플레이 상태이거나 게임 시간이 다 되기 전까지 반복
        //게이지 줄어들기
        while (gameMgr.gameState == GameState.PLAYING
            && playTime > 0)
        {
            playTime--; //1초마다 1씩 감소
            game_img_timeGauge.fillAmount -= 1/ (float)gameMgr.limit_playTime;
            yield return new WaitForSeconds(1.0f);
        }

        //시간이 다 지났을 때
        if(playTime <= 0)
        {
            gameMgr.gameState = GameState.RESULT;
            gameMgr.isClear = false;
            gameMgr.GameOver(gameMgr.isClear);
        }
    }

}
