using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using GoogleARCore;
using UnityEngine.SceneManagement;
using Project.ReadOnly;
using System.IO;

public enum GameState
{
    NONE = 0,
    TITLE,
    DIALOG,
    READY,
    PLAYING,
    STOP,
    RESULT,
}

public class GameManager : MonoBehaviour
{

    //각종 클래스 호출
    CSVparser parse = new CSVparser();
    public UIManager uiMgr;
    public MissileManager missileMgr;
    public DialogManager dialogMgr;
    public SoundManager soundMgr;

    //AssetBundles
    public AssetBundle b_prefabs { get; set; }
    public AssetBundle b_sprites { get; set; }
    public AssetBundle b_sounds { get; set; }
    public AssetBundle b_csvdata { get; set; }
    public AssetBundle b_animator { get; set; }

    //ARcore Objects(Don't Active in Play)
    public GameObject arCore;
    public GameObject planeGenerator;
    public GameObject planeDiscovery;
    public GameObject pointCloud;

    public GameState gameState;

    //캐릭터들 관련 변수들
    public GameObject stage; // 스테이지 오브젝트 받기
    public GameObject characters { get; set; }
    public GameObject fleePoints { get; set; }
    public GameObject spawnPoints { get; set; }
    public FXV.FXVShield water { get; set; }
    public GameObject enemyPool { get; set; }

    public List<Character> list_Headers { get; set; }
    public List<Character> list_Enemies { get; set; }
    public List<Transform> list_FleePoints { get; set; }
    public List<Transform> list_SpawnPoints { get; set; }


    /*
   public string sDevice = "";
   public string sLanguage = "English";	//language 0: korean 1: english 2: japanese 3: chinese
   public string sCountry = "A1";          // ISO 3166-1 alpha-2
   public string sMarket = "google";
   public string sMarket_id = "market_id";
   public string sMarket_token = "market_token";
   */

    public int language; //0:korean 1: english
    public bool isDemo { get; set; }


    //게임 플레이 변수
    public bool isClear { get; set; } //게임 클리어 여부

    public int limit_playTime { get; set; }       //게임 제한시간 0=비활성화
    public int limit_missiles { get; set; }        //총알 개수 0=비활성화
    public int AI_Level { get; set; }               //활성화 할 특수 동작들
    public int limit_headers { get; set; }        //대가리들의 수
    public int limit_enemies { get; set; }       //적의 최대 수
    public int current_enemies { get; set; }   //현재 적 수

    public int currentLevel { get; set; } //난이도,라운드,스테이지 어떤 레벨을 고를 것인가
    public int clearLevel { get; set; }//클리어한 최대 레벨, 세이브 데이터

    public int tutorialNum { get; set; }   //튜토리얼 진행 여부
    public int dialogNum { get; set; }      //몇 번째 대화 

    public float sumHp { get; set; }    //총 게이지 양 설정

    public static bool isNull { get { return null == s_instance; } }


    //싱글톤 선언
    private static GameManager s_instance = null;
    public static GameManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return s_instance;
        }
    }

    void Awake()
    {
        if (s_instance != null && this != s_instance)
        {
            Debug.LogError("Cannot have two instances.");
            Destroy(gameObject);
            return;
        }
        s_instance = this;

        b_prefabs = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "prefabs"));
        b_sounds = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sounds"));
        b_csvdata = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "csvdata"));
        b_sprites = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sprites"));
        b_animator = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "animator"));

        //Scene 전환시 삭제 방지
        // DontDestroyOnLoad(this);

        gameState = GameState.NONE;
        currentLevel = 1;
        clearLevel = 49;// PlayerPrefs.GetInt("ClearLevel",0);
        tutorialNum = 0;// PlayerPrefs.GetInt("TutorialNum",0);
        language = PlayerPrefs.GetInt("Language", 0);

        //---------------!!데모버전 설정!!(중요)-------------
        isDemo = false;
        //----------------------------------------------------


        sumHp = 0.0f;

        list_Headers = new List<Character>();
        list_FleePoints = new List<Transform>();
        list_SpawnPoints = new List<Transform>();
        list_Enemies = new List<Character>();

        uiMgr = transform.GetChild(0).gameObject.GetComponent<UIManager>();
        //missileMgr = uiMgr.transform.GetChild(5).gameObject.GetComponent<MissileManager>();
        soundMgr = this.GetComponent<SoundManager>();

        missileMgr.gameObject.SetActive(false);
        LevelInit();
    }

    /// <summary>
    /// PlayScene으로 갈 때 호출
    /// </summary>
    public void CreateManager()
    {
        if (missileMgr == null)
        {
            missileMgr = (Instantiate(b_prefabs.LoadAsset<GameObject>(Defines.PREFAB_MISSILEMANAGER), this.transform) as GameObject).GetComponent<MissileManager>();
        }
        if (uiMgr == null)
        {
            uiMgr = (Instantiate(b_prefabs.LoadAsset<GameObject>(Defines.PREFAB_UI_CANVAS), this.transform) as GameObject).GetComponent<UIManager>();
        }
    }

    /// <summary>
    /// Scene전환 함수
    /// </summary>
    /// <param name="_scene">SceneNumber</param>
    public void LoadScene(int _scene)
    {
        Time.timeScale = 1.0f;
        if (_scene == 1) { CreateManager(); }

        ////Scene 전환시 게임매니져 하위 객체들 비활성화
        //missileMgr.gameObject.SetActive(false);

        //uiMgr.gameUI.gameObject.SetActive(false);
        //uiMgr.resultUI.gameObject.SetActive(false);
        //uiMgr.menuUI.gameObject.SetActive(false);
        //planeGenerator.SetActive(true);
        //uiMgr.titleUI.gameObject.SetActive(false);

        SceneManager.LoadScene(_scene);
    }

    /// <summary>
    /// deactivate ARcores Support Objects, Don't Need In Play
    /// </summary>
    public void DeActivateARCoreSupport()
    {
        SetStage();

        stage.transform.parent = null;

        pointCloud.SetActive(false);
        planeGenerator.SetActive(false);
        planeDiscovery.SetActive(false);
    }

    /// <summary>
    /// 난이도 초기화 함수
    /// </summary>
    public void LevelInit()
    {
        Debug.Log("LevelData Initialize :" + currentLevel);
        isClear = false;

        limit_playTime = ReadLevelData(currentLevel, 1);
        Debug.Log("Limit Time :" + ReadLevelData(currentLevel, 1));

        limit_missiles = ReadLevelData(currentLevel, 2);
        Debug.Log("Limit Missiles :" + ReadLevelData(currentLevel, 2));

        AI_Level = ReadLevelData(currentLevel, 3);
        Debug.Log("AI_LEVEL :" + ReadLevelData(currentLevel, 3));

        limit_headers = ReadLevelData(currentLevel, 4);
        Debug.Log("Limit Headers :" + ReadLevelData(currentLevel, 4));

        limit_enemies = ReadLevelData(currentLevel, 5);
        Debug.Log("Limit Enemies :" + ReadLevelData(currentLevel, 5));
        current_enemies = limit_enemies;

        dialogNum = ReadLevelData(currentLevel, 6);
        Debug.Log("Dialog :" + ReadLevelData(currentLevel, 6));
    }

    /// <summary>
    /// 게임 UI 초기화
    /// </summary>
    public void GameUIInit()
    {
        Debug.Log("GameUI Initialize");

        uiMgr.game_img_clearGauge.fillAmount = 0.0f;     //게이지
        uiMgr.game_img_timeGauge.fillAmount = 1.0f;
        //suiMgr.game_txt_timer.text = limit_PlayTime.ToString();   //시간
        //방울
        missileMgr.missileCount = limit_missiles;
        uiMgr.game_missiles.text = limit_missiles.ToString();
    }


    /// <summary>
    /// 지형 생성시 호출, Stage하위 정보를 GameManager의 데이터로 저장한다
    /// </summary>
    public void SetStage()
    {
        //UI매니저의 요소를 쓰기에 Awake() 이후 작동해야 한다
        uiMgr.titleUI.gameObject.SetActive(true);
        uiMgr.TitleLevelUnLock();
        uiMgr.readyUI.gameObject.SetActive(false);

        //stage
        characters = stage.transform.GetChild(0).gameObject;
        fleePoints = stage.transform.GetChild(1).gameObject;
        spawnPoints = stage.transform.GetChild(2).gameObject;
        water = stage.transform.GetChild(3).GetComponent<FXV.FXVShield>();
        enemyPool = spawnPoints.transform.GetChild(0).gameObject;

        for (int i = 0; i < characters.transform.childCount; i++)
        {
            list_Headers.Add(characters.transform.GetChild(i).GetComponent<Character>());
        }

        for (int i = 0; i < fleePoints.transform.childCount; i++)
        {
            list_FleePoints.Add(fleePoints.transform.GetChild(i));
        }

        for (int i = 0; i < spawnPoints.transform.childCount; i++)
        {
            list_SpawnPoints.Add(spawnPoints.transform.GetChild(i));
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject _enemy = (GameObject)Instantiate(b_prefabs.LoadAsset(Defines.PREFAB_ENEMY_CLOUD, typeof(GameObject)), enemyPool.transform);
            _enemy.name = "enemy " + "[" + i + "]";
            list_Enemies.Add(_enemy.GetComponent<Character>());
            _enemy.SetActive(false);
        }

        soundMgr.PlayBgm(soundMgr.bgm_title);
    }

    /// <summary>
    /// 해당 스테이지에 맞는 캐릭터 활성화 여부 설정
    /// </summary>
    /// <param name="_isActive">true or false</param>
    public void SetActiveCharacters(bool _isActive)
    {
        fleePoints.transform.position = stage.transform.position;

        //세팅 전 체력 초기화
        sumHp = 0.0f;
        //캐릭터 보이기
        //레벨에 따라 캐릭터 숫자 결정
        for (int i = 0; i <= limit_headers; i++)
        {
            if (list_Headers.Count > limit_headers)
            {
                list_Headers[i].StopAllCoroutines();
                list_Headers[i].gameObject.SetActive(_isActive);
                sumHp += list_Headers[i].Status.maxHp;
                list_Headers[i].AI_Move(0);
            }
        }

        for (int i = 0; i < limit_enemies; i++)
        {
            if (list_Enemies.Count > limit_enemies)
            {
                list_Enemies[i].StopAllCoroutines();
                list_Enemies[i].gameObject.SetActive(_isActive);
                list_Enemies[i].AI_Move(0);
            }
        }

    }

    /// <summary>
    /// 레벨과 타입을 넣으면 레벨에 해당되는 값을 리턴
    /// </summary>
    /// <param name="_level"> 현재 스테이지 레벨값, 0보다 커야함</param>
    /// <param name="_type">0: Level, 1: Time, 2: Missiles, 3: AI</param>
    /// <returns></returns>
    public int ReadLevelData(int _level, int _type)
    {
        Table table = parse.ParsingCSV(Defines.CSV_LEVEL_DATA);
        int _data = Convert.ToInt32(table.Row[_level].Col[_type]);
        return _data;
    }

    //총 레벨 수를 돌려준다
    public int LevelCount()
    {
        Table table = parse.ParsingCSV(Defines.CSV_LEVEL_DATA);
        int _data = table.Row.Count;
        return _data;
    }

    /// <summary>
    /// 실제 게임 시작시 사용할 동작, 이 함수 호출 뒤에 바로 게임이 시작된다.
    /// </summary>
    /// <param name="_level"> 현재 레벨,단계,스테이지</param>
    public void GameStart(int _level)
    {
        SetStage();
        //AR코어 인식 후 필드를 생성, 게임 시작 버튼을 누르면 작동된다
        gameState = GameState.READY;
        Debug.Log("GameState : " + gameState);
        Debug.Log("CurrentLevel : " + currentLevel);

        //시간 활성화
        Time.timeScale = 1.0f;

        //타이틀 숨기기
        uiMgr.titleUI.gameObject.SetActive(false);
        //결과창 숨기기
        uiMgr.resultUI.gameObject.SetActive(false);
        //메뉴 숨기기
        uiMgr.menuUI.gameObject.SetActive(false);

        //레벨값, 스테이지 관련 변수 변화
        LevelInit();
        //플레이어,UI 초기화
        //레벨 변수 받아서 레벨에 맞는 난이도 정하기
        GameUIInit();

        //시간 제한 비활성화 시
        if (limit_playTime == 0)
        {
            uiMgr.game_img_timeGauge.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            uiMgr.game_img_timeGauge.transform.parent.gameObject.SetActive(true);
        }
        //총알 제한 비활성화 시
        if (limit_missiles == 0)
        {
            uiMgr.game_missiles.gameObject.SetActive(false);
        }
        else
        {
            uiMgr.game_missiles.gameObject.SetActive(true);
        }

        if (dialogNum == 0)
        {
            //클리어 목표 보여주기
            uiMgr.ShowGoal(limit_playTime, limit_missiles, limit_enemies);
        }
        else
        {
            //대화
            dialogMgr.gameObject.SetActive(true);
            dialogMgr.StartDialog(dialogNum);
        }

    }

    //게이지가 가득 찼고 모든 적을 죽였을 경우 클리어
    public void CheckGameClear()
    {
        if (uiMgr.game_img_clearGauge.fillAmount >= 1.0f
            && current_enemies <= 0)
        {
            isClear = true;
            StartCoroutine(ReduceGameOver());
        }
    }

    IEnumerator ReduceGameOver()
    {
        yield return new WaitForSeconds(1.0f);
        GameOver(isClear);
    }

    /// <summary>
    /// 게임 종료시 결과창, 클리어 여부에 따라 내용이 달라진다
    /// </summary>
    /// <param name="_isClear"> true:성공/false:실패 </param>
    public void GameOver(bool _isClear)
    {
        //게임 종료 시(성공, 실패) 결과창
        //Init 함수들 호출필요 
        //모든 것이 초기화 되어 재시작 시에도 정상적으로 작동 되어야 한다
        //게임 상태 변경
        gameState = GameState.RESULT;
        Debug.Log("GameState : " + gameState);
        //진행중인 코루틴을 모두 종료한다.
        StopAllCoroutines();
        //스테이지 종료 시 캐릭터로 인한 오류 제거
        for (int i = 0; i < list_Headers.Count; i++)
        {
            list_Headers[i].StopAllCoroutines();
        }
        uiMgr.StopAllCoroutines();
        missileMgr.StopAllCoroutines();
        //캐릭터 숨기기
        SetActiveCharacters(false);

        //미사일 숨기기
        missileMgr.gameObject.SetActive(false);
        //게임 UI 숨기기
        uiMgr.gameUI.gameObject.SetActive(false);
        //결과창 띄우기
        uiMgr.resultUI.gameObject.SetActive(true);
        //메뉴 숨기기
        uiMgr.menuUI.gameObject.SetActive(false);

        //BGM 종료
        soundMgr.bgmSource.Stop();

        if (_isClear == true)
        {
            //클리어 시
            uiMgr.result_img_text.sprite = b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXT_VICTORY) as Sprite;
            soundMgr.PlaySfx(this.transform.position, b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_WIN) as AudioClip);
            if (clearLevel < currentLevel)
            {
                clearLevel = currentLevel;
                PlayerPrefs.SetInt("ClearLevel", clearLevel);
                Debug.Log("Clear Level : " + clearLevel);
            }
        }
        else
        {
            //클리어 실패 시
            uiMgr.result_img_text.sprite = b_sprites.LoadAsset<Sprite>(Defines.SPRITE_TEXT_DEFEAT) as Sprite;
            soundMgr.PlaySfx(this.transform.position, b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_LOSE) as AudioClip);
        }
    }

    //Instance 중복 시 삭제되면서 비워준다
    private void OnDestroy()
    {
        if (isNull == false) { s_instance = null; }
    }
}
