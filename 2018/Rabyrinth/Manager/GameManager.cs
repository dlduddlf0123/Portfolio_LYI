using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using InternetTime;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Rabyrinth.ReadOnlys;

public class GameManager : MonoSingleton<GameManager>
{
    public string cognitoIdentityPoolString;
    public string[] ddbTableName;
    public Canvas Background;
    public Transform MagicPoolTr;

    public int testScene;

    public PlayerCamera playerCamera { get; private set; }
    public Camera Main_Cam { get; private set; }
    public Camera UI_Cam { get; private set; }
    public MainUI Main_UI { get; private set; }
    public PlayerCharacter Player { get; private set; }
    public DataManager PlayData { get; private set; }
    public AWSManager AWS_Mgr { get; private set; }
    public SpawnManager spawnManager { get; private set; }
    public SkillManager skillManager { get; private set; }
    public List<GameObject> listDntDestroy { get; private set; }

    public string PlayerName { get; private set; }
    public string UnauthID { get; private set; }

    public string EventName { get; set; }

    private DisableSystemUI systemUI;

    //public int floor { get; set; }

    //public int nCurrentKPM { get; set; }
    public int nCurrentDeadNPC { get; set; }
    public float fCurrentStagePlayTime  { get; set; }

    public bool isPlay { get; set; }
    public bool isEvent { get; set; }
    //private float deltaTime;

    public int isRanking;

    protected override void DoAwake()
    {
        // 스크린이 잠금되지 않도록 변경
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //소프트키 비활성화
#if UNITY_ANDROID && !UNITY_EDITOR
        systemUI = new DisableSystemUI();
        systemUI.DisableNavUI();
        StartCoroutine(SystemUIRun());
#endif
        // 스크린 비율 9:16으로 고정
        //Screen.SetResolution(Screen.width, (Screen.width * 9) / 16, true);
        //Screen.SetResolution(1920, 1080, true);
        PlayerName = "";
        UnauthID = "";
        EventName = "";
        //floor = 1;
        //nCurrentKPM = 50;
        nCurrentDeadNPC = 0;
        fCurrentStagePlayTime = 0.0f;
        isPlay = false;

        //카메라 생성 및 DontDestroy설정
        Main_Cam = (Instantiate(Resources.Load(Defines.MAIN_CAM_PATH)) as GameObject).GetComponent<Camera>();
        UI_Cam = (Instantiate(Resources.Load(Defines.UI_CAM_PATH)) as GameObject).GetComponent<Camera>();
        playerCamera = Main_Cam.GetComponent<PlayerCamera>();
        DontDestroyOnLoadAndAdd(Main_Cam.gameObject);
        DontDestroyOnLoadAndAdd(UI_Cam.gameObject);

        //로고 생성 (Fade ani)
        Instantiate(Resources.Load(Defines.UI_LOGO_PATH));

        //UI 생성 및 DontDestroy설정
        Main_UI = (Instantiate(Resources.Load(Defines.UI_MAIN_PATH)) as GameObject).GetComponent<MainUI>();
        DontDestroyOnLoadAndAdd(Main_UI.gameObject);

        //Main UI 및 Backgound에 worldCam 지정
        Main_UI.GetComponent<Canvas>().worldCamera = UI_Cam;
        Background.worldCamera = UI_Cam;

        // 매직풀
        DontDestroyOnLoadAndAdd(MagicPoolTr.gameObject);

        //AWS 및 PlayerData 매니저 클래스 추가를 위한 콜백 액션정의
        Action action = () =>
        {
            AWS_Mgr = this.gameObject.AddComponent<AWSManager>();
            PlayData = this.gameObject.AddComponent<DataManager>();
            spawnManager = this.gameObject.AddComponent<SpawnManager>();
            skillManager = this.gameObject.AddComponent<SkillManager>();
        };

        // 네트워크 연결 체크 후 상단 Class 추가 실행
        StartCoroutine(WaitConnetNetwork(action));

        // 플레이어 캐릭터 생성
        StartCoroutine(CreatePC());
    }
    public int DamageReduse(int Atk, int Def)
    {
        if(Def < Atk)
            return Atk - (int)((float)Atk * ((float)Def / (float)Atk) / 2.0f);

        float D_DIV_A = (float)Def / (float)Atk;

        if (D_DIV_A > 31.0f)
            return 1;

        float pow = Mathf.Pow(2.0f, D_DIV_A);

        float damage = (float)Atk * (pow - 1.0f) / pow;

        return Atk - (int)damage;
    }

    //Scene 전환시 호출, 비동기 로딩 후 로딩이 끝나면 전환
    public IEnumerator ChangeScene(int _index) // (parameter : scene index) 
    {
        if (spawnManager.listActives != null)
            spawnManager.listActives.Clear();

        if (Player != null)
        {
            if (Player.nvAgent != null)
                Player.nvAgent.enabled = false;

            Player.StopAllCoroutines();
            Player.gameObject.SetActive(false);
        }

        var async_operation = SceneManager.LoadSceneAsync(1);

        while (SceneManager.GetActiveScene().buildIndex != 1)
            yield return null;

        Main_UI.TargetHpBar.gameObject.SetActive(false);
        Main_UI.SetLoading(true);

        //로딩 전에 씬 비활성화
        async_operation.allowSceneActivation = false;
        //다음 씬 번호 입력
        async_operation = SceneManager.LoadSceneAsync(_index);

        //로딩 중일때
        while (async_operation.progress < 0.9f)
            yield return null;

        async_operation.allowSceneActivation = true;
    }

    // 소프트키 비활성 기능 코루틴
    private IEnumerator SystemUIRun()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            systemUI.Run();
#if UNITY_ANDROID && !UNITY_EDITOR
            systemUI.Run();
#endif
        }
    }

    //프레임 표시를 위한 게임 시간체크
    //void Update()
    //{
    //    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    //}

    // GUI로 현재 프레임 표시
    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 50;
    //    style.normal.textColor = Color.white;
    //    string text = Main_UI.Log.text;
    //    GUI.Label(rect, text, style);
    //}

    // 게임 재시작을 위한 DontDestroy 객체 지정 및 저장
    public void DontDestroyOnLoadAndAdd(GameObject _gameObject)
    {
        DontDestroyOnLoad(_gameObject);

        if (listDntDestroy == null)
            listDntDestroy = new List<GameObject>();

        listDntDestroy.Add(_gameObject);
    }

    // 게임 재시작
    public void GameRestart()
    {
        foreach (var Object in listDntDestroy)
            Destroy(Object);

        Instantiate(Resources.Load(Defines.MANAGER_GAMERESTARTER));
    }

    // 네트워크 연결 상태를 체크해서 boolean return
    public bool CheckNetwork()
    {
        if (Application.internetReachability.Equals(NetworkReachability.NotReachable))
            return false;
        else
            return true;
    }

    //로고 액션 완료 후 Logo 클래스 에서 호출한다.
    public void LogoEnd()
    {
        StartCoroutine(WaitSceneChange());
    }
    private IEnumerator WaitSceneChange()
    {
        yield return new WaitForSeconds(2.0f);
        Main_UI.gameObject.SetActive(true);

        if (!CheckNetwork())
        {
            Action action = () => { StartCoroutine(StartGame()); };
            StartCoroutine(WaitConnetNetwork(action));
        }
        else
        {
            StartCoroutine(StartGame());
        }

    }

    private IEnumerator StartGame()
    {
        while(PlayData.GameData == null ||
            PlayData.PlayerData == null) /*||
            PlayData.lRankingData == null)*/
            yield return null;

        if (testScene != 0)
            StartCoroutine(ChangeScene(testScene));
        else
            StartCoroutine(ChangeScene(UnityEngine.Random.Range(0, 2) != 0 ? 2 : 5));
    }

    //플레이어 캐릭터를 생성한다. 플레이어 데이터가 초기화 될때까지 기다린 후 생성.
    private IEnumerator CreatePC()
    {
        while(PlayData == null)
            yield return null;

        Player = (Instantiate(Resources.Load(Defines.CHARACTER_PLAYER_PATH)) as GameObject).GetComponent<PlayerCharacter>();
        DontDestroyOnLoadAndAdd(Player.gameObject);
    }

    //인터넷 연결을 기다린다. 연결 시도 10초 이상 대기시 게임 재시작
    private IEnumerator WaitConnetNetwork(Action _action1 = null, Action _action2 = null)
    {
        int timeSecond = 0;

        while (!CheckNetwork())
        {
            if (timeSecond > 9)
            {
                if (_action2 != null)
                    _action2();
                else
                    Main_UI.popUpController.PopUpMiddle("네트워크 연결 실패. 게임을 재시작 합니다.", () => { GameRestart(); });
            }

            yield return new WaitForSeconds(1.0f);
            timeSecond++;
        }

        if(_action1 != null)
            _action1();
    }

    //private IEnumerator WaitSceneLoad(int _index)
    //{
    //    while (SceneManager.GetActiveScene().buildIndex != _index)
    //        yield return null;
    //}

    //게임 종료시 자동호출
    private void OnApplicationQuit() // 앱 종료 시 호출
    {
        //if (!isQuit)
        //{
        //    Application.CancelQuit();
        //    GameQuit();
        //}
    }

    //잠금, 활성시 자동호출 (잠금 true, 활성 false)
    private void OnApplicationPause(bool pause) // 앱 활성 및 비활성 시 호출
    {
        if (pause)
        {
            //if (PlayData.PlayerData != null)
            //    AWS_Mgr.SavePlayerDataSecu(PlayData.PlayerData, true);
        }
        else
        {
            // 게임 활성화
            #if UNITY_ANDROID && !UNITY_EDITOR
            // 소프트키 비활성
            systemUI.DisableNavUI();
#endif
            // 선물 시간 활성
            ActiveGameTime();
        }
    }

    //NTP에서 시간 획득(UTC)
    private bool GetNTP(out DateTime _dt)
    {
        SNTPClient client;
        try
        {
            client = new SNTPClient(Defines.NTP_PATH);
            client.Connect(false);
        }
        catch
        {
            //Debug.Log("NetWork Don't Connect");
            _dt = new DateTime(); //임시
            //_dt = syncPlayerData.gameOff_DateTime;
            //isNetWork = false;
            //timeText.text = "Check Network!";
            return false;
        }
        _dt = client.TransmitTimestamp;
        //isNetWork = true;
        //Debug.Log("NetWork Connect");
        return true;
    }

    private void ActiveGameTime()
    {
        //if (SceneManager.GetActiveScene().buildIndex != (int)Defines.Scene.Play)
        //    return;

        //Obj_GiftButton.GetComponent<Button>().enabled = false;

        //DateTime currentTime;
        //GetNTP(out currentTime);

        //TimeSpan TS = currentTime - syncPlayerData.gameOff_DateTime;
        //double TimeDiff = TS.TotalSeconds > 0 ? TS.TotalSeconds : 0;

        //syncPlayerData.gameOff_DateTime = currentTime;
        //syncPlayerData.getGift_RemaningTime -= TimeDiff;

        //if (syncPlayerData.getGift_RemaningTime <= 0)
        //{
        //    Obj_GiftButton.GetComponent<Button>().enabled = true;
        //    syncPlayerData.getGift_RemaningTime = 0.0f;
        //}
        //else
        //{
        //    Obj_GiftButton.GetComponent<Button>().enabled = false;
        //}

        //if (isUpdateTime != true)
        //    StartCoroutine(UpdateGiftTime());
    }
}
