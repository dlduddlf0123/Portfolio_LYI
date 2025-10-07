using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using ReadOnly;

public enum GameState
{
    NONE = 0,
    LOADING,
    INTERACTION,
    TRAVEL,
    CONTEST,
    HOUSE,
    CUTSCENE,
    MINIGAME,
    GAMEOVER,
}

public class GameManager : MonoBehaviour
{
    public SoundManager soundMgr;
    public DialogManager dialogMgr;

    public PlayManager[] playMgr;   //각종 게임모드 / 각 모드마다 게임 내용이 다름
    public PlayManager currentPlay; //현재 게임모드

    public SunMove sunMove; //시간
    public MenuUI menuUI;

    public PlayerHand[] hand;
    public Camera mainCam;

    public GameState statGame;
    public Coroutine currentCoroutine = null;

    //AssetBundles
    public AssetBundle b_prefabs { get; set; }
    public AssetBundle b_sprites { get; set; }
    public AssetBundle b_sounds { get; set; }
    public AssetBundle b_csvdata { get; set; }
    public AssetBundle b_animator { get; set; }

    public ParticleSystem[] particles;  //ParticleObjects

    public Canvas fadeCanvas;
    public Image fadeImg;
    
    //Save Datas
    public int language; //0:korean 1: english


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
        if (FindObjectsOfType(typeof(GameManager)).Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //if (Application.systemLanguage == SystemLanguage.Korean)
        //    language = PlayerPrefs.GetInt("Language", 0);
        //else
        //    language = PlayerPrefs.GetInt("Language", 1);

        b_prefabs = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/prefabs");
        b_sounds = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/sounds");
        b_csvdata = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/csv");
        b_sprites = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/sprites");
        // b_animator = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "animator"));

        //statGame = GameState.INTERACTION;

        particles = new ParticleSystem[this.transform.GetChild(0).childCount];

        for (int idx = 0; idx < particles.Length; idx++)
        {
            particles[idx] = this.transform.GetChild(0).GetChild(idx).GetComponent<ParticleSystem>();
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //soundMgr.PlayBgm(soundMgr.LoadClip(Defines.SOUND_BGM_STAGE1));
    }


    #region SceneMoveFunc

    public void LoadScene(int _sceneNum)
    {
        if (statGame == GameState.LOADING)
        {
            return;
        }
        StartCoroutine(ChangeScene(_sceneNum));
    }


    //Scene 전환시 호출, 비동기 로딩 후 로딩이 끝나면 전환
    public IEnumerator ChangeScene(int _sceneNum)
    {
        statGame = GameState.LOADING;
        var async_operation = SceneManager.LoadSceneAsync(1); //1: LoadingScene

        while (SceneManager.GetActiveScene().buildIndex != 1)
            yield return null;

        //로딩 전에 씬 비활성화
        async_operation.allowSceneActivation = false;
        //다음 씬 번호 입력
        async_operation = SceneManager.LoadSceneAsync(_sceneNum);

        //로딩 중일때
        while (async_operation.progress < 0.9f)
            yield return null;

        async_operation.allowSceneActivation = true;
    }

    public void Fade(bool _fadeIn, float _fadingSpeed = 1f)
    {
        StartCoroutine(FadeInOut(_fadeIn, _fadingSpeed));
    }

    /// <summary>
    /// FadeIn, Out
    /// </summary>
    /// <returns></returns>
    public IEnumerator Fading(float _fadingSpeed = 3f, float _fadedTime = 0.5f, UnityEngine.Events.UnityAction _action = null)
    {
        yield return FadeInOut(true, _fadingSpeed);
        yield return new WaitForSeconds(_fadedTime);
        if (_action != null)
        {
            _action.Invoke();
        }

        yield return FadeInOut(false, _fadingSpeed);
    }

    /// <summary>
    /// FadeIn, Out
    /// </summary>
    /// <param name="_fadeIn"> 어두워질건지? </param>
    /// <param name="_fadingSpeed"> 색 변화 속도 배속 </param>
    /// <returns></returns>
    public IEnumerator FadeInOut(bool _fadeIn, float _fadingSpeed = 1f)
    {
        fadeCanvas.worldCamera = mainCam;
        float currentTime = 0f;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime * _fadingSpeed;

            fadeImg.color = (_fadeIn) ?
            Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), currentTime) :
            Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), currentTime);

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 다음날로 이동
    /// </summary>
    /// <returns></returns>
    public IEnumerator NextDay()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        float _fadeTime = 3f;
        //Valve.VR.SteamVR_Fade.Start(Color.black, _fadeTime);
        //다음날 넘어가는 효과음
        //다음날 UI 출력하기
        yield return new WaitForSeconds(_fadeTime);

        sunMove.dateTime = new System.DateTime(sunMove.dateTime.Year, sunMove.dateTime.Month, sunMove.dateTime.Day + 1, 6, 0, 0);

        sunMove.transform.rotation = Quaternion.Euler(0, -100, 0);
        sunMove.statTime = TimeState.MORNING;

        //Valve.VR.SteamVR_Fade.Start(Color.clear, _fadeTime);
    }
    #endregion

    #region 이펙트 관련 함수
    /// <summary>
    /// 파티클 재생 함수
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_go"></param>
    public void PlayEffect(Vector3 _position, ParticleSystem _p)
    {
        _p.transform.position = _position;
        _p.Play();
    }
    public void PlayEffect(Transform _position, ParticleSystem _p,string _path)
    {
        _p.transform.position = _position.position;
        _p.Play();
        soundMgr.PlaySfx(_position, soundMgr.LoadClip(_path));
    }
    /// <summary>
    /// Play Effect & SFX on Hand Interact
    /// </summary>
    /// <param name="_pos">Which Hand to play</param>
    /// <param name="_particle">Particle to Play</param>
    /// <param name="_path">SFX path</param>
    public void HandEffect(int _hand)
    {
        PlayEffect(hand[_hand].transform.position, particles[0]);
        soundMgr.PlaySfx(hand[_hand].transform, soundMgr.LoadClip(Defines.SOUND_SFX_CLICK));
    }
    public void HandEffect(PlayerHand _hand, int _particle = 0, string _clip = Defines.SOUND_SFX_CLICK)
    {
        PlayEffect(_hand.transform.position, particles[_particle]);
        soundMgr.PlaySfx(_hand.transform, soundMgr.LoadClip(_clip));
    }
    #endregion

    #region 손모양 함수들



    ////검지 손가락
    //public void ActionLeftPoint(int state)
    //{
    //    if (state == 1)
    //    {
    //        if (selectHeader.isAction == true) { return; }

    //        selectHeader.SetSecondFinger(hand[0]);
    //    }
    //    else if (state == 2)
    //    {
    //        selectHeader.SetPaper(hand[0]);
    //    }
    //}
    //public void ActionRightPoint(int state)
    //{
    //    if (state == 1)
    //    {
    //        if (selectHeader.isAction == true) { return; }

    //        selectHeader.SetSecondFinger(hand[1]);
    //    }
    //    else if (state == 2)
    //    {
    //        selectHeader.SetPaper(hand[1]);
    //    }
    //}

    ////주먹쥐기
    //public void ActionLeftFist(int state)
    //{
    //    if (state == 1)
    //    {
    //        if (selectHeader.isAction == true) { return; }
    //        selectHeader.SetRock(hand[0]);
    //    }
    //    else if (state == 2)
    //    {
    //        selectHeader.SetPaper(hand[0]);
    //    }
    //}
    //public void ActionRightFist(int state)
    //{
    //    if (state == 1)
    //    {
    //        if (selectHeader.isAction == true) { return; }
    //        selectHeader.SetRock(hand[1]);
    //    }
    //    else if (state == 2)
    //    {
    //        selectHeader.SetPaper(hand[1]);
    //    }
    //}

    ////주먹펴기
    //public void ActionFive(int state)
    //{
    //    selectHeader.SetPaper(hand[0]);
    //}

    ////Like 따봉
    //public void ActionLeftLike(int state)
    //{
    //    StartCoroutine(hand[0].ActionDetachHand());
    //}
    //public void ActionRightLike(int state)
    //{
    //    StartCoroutine(hand[1].ActionDetachHand());
    //}

    ////양손주먹
    //public void ActionDoubleFist(int state)
    //{
    //    if (state == 2)
    //    {
    //        PlayEffect(hand[0].transform.position, particles[0]);
    //        PlayEffect(hand[1].transform.position, particles[0]);
    //        soundMgr.PlaySfx(hand[1].transform, soundMgr.LoadClip(Defines.SOUND_SFX_CLICK));
    //        selectHeader.SetDoubleRock();
    //    }
    //}
    ////양손검지
    //public void ActionDoublePoint(int state)
    //{
    //    if (state == 1)
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            PlayEffect(hand[i].transform.position, particles[0]);
    //            menu[i].SetActive(true);
    //            menu[i].transform.SetParent(null);
    //            menu[i].transform.LookAt(mainCam.transform);
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            PlayEffect(hand[i].transform.position, particles[0]);
    //            menu[i].SetActive(false);
    //            menu[i].transform.SetParent(hand[i].transform);
    //            menu[i].transform.localPosition = Vector3.zero;
    //        }
    //    }
       
    //}
    ////따봉 -> 주먹(기폭 스위치 누르듯)
    //public void ActionBoom(int state)
    //{

    //}
    #endregion

}
