using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System.IO;

using AroundEffect;
using VRTokTok;
using VRTokTok.Manager;

using OVR;

public enum GameStatus
{
    MENU = 0,
    LOADING = 1,
    SELECT,
    GAME,
}

/// <summary>
/// 7/6/2023-LYI
/// 게임 데이터 관리 클래스
/// 현재 게임 진행 상태 관리
/// 각종 데이터 홀드
/// 
/// Don'tDestroyOnLoad
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    public AddressableManager addressableMgr;
    public SoundManager soundMgr;
    public ObjectPoolingManager objPoolingMgr;
    public DataManager dataMgr;

    [Header("PlayManager")]
    public PlaySceneManager playMgr;
    public TableManager tableMgr;


    [Header("OVRCamera")]
    public OVRManager ovrMgr;
    public Camera mainCam;
    public Fade fade;

    [Header("Properties")]
    public GameStatus statGame;

    public ParticleSystem[] arr_particles;

    public Camera pcCam;

    //Save Datas
    public int language; //0:korean 1: english
    public bool isTutorial = false; //True인 경우 실행 시 튜토리얼 진행

    //싱글톤 
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

    // Start is called before the first frame update
    void Awake()
    {
        if (FindObjectsOfType(typeof(GameManager)).Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {

        soundMgr.PlayBgm(soundMgr.bgmSource.clip);

        //statScene = (SceneStatus)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        //게임 상태 초기화
        // ChangeGameStat(GameStatus.MENU);

    }


    public void OnAddreessableLoadComplete()
    {
       addressableMgr.isLoadComplete = true;


        tableMgr.TableElementsInit();

        // dataMgr.SetStageDatas();
        dataMgr.SetMixedStageData();

        playMgr.cheeringSeat.CheckCharacterLock();

        //soundMgr.ChangeSceneBGM(statGame);

        //Application.targetFrameRate = 90;

        //if (OVRManager.display != null)
        //{
        //    OVRManager.display.displayFrequency = 90f;
        //}

        //GameManager.Instance.dataMgr.DataManagerInit();
        //GameManager.Instance.invenMgr.LoadInventoryDatasAfterAddressable();

        //6/24/2024-LYI
        //튜토리얼 변수 추가
        isTutorial = ES3.Load<bool>(Constants.ES3.IS_TUTORIAL, true);
        if (isTutorial == true)
        {
            if (ES3.Load<bool>("9001", false) == true)
            {
                isTutorial = false;
                ES3.Save<bool>(Constants.ES3.IS_TUTORIAL, isTutorial);
            }
        }
       
        playMgr.tokMgr.HandInit();

#if UNITY_EDITOR
        if (addressableMgr.isStartDebug)
        {
            //load test
            ChangeGameStat(GameStatus.GAME);
            GameStart(addressableMgr.debugStageNum);
        }
        else
        {
            if (isTutorial)
            {
                ChangeGameStat(GameStatus.GAME);
                GameStart(1000);
            }
            else
            {
                ChangeGameStat(GameStatus.MENU);
            }
        }
#else

            if (isTutorial)
            {
                ChangeGameStat(GameStatus.GAME);
                GameStart(1000);
            }
            else
            {
             ChangeGameStat(GameStatus.MENU);
            }
#endif
    }


    /// <summary>
    /// 11/13/2023-LYI
    /// 게임 상태 바뀔 때의 처리 변경
    /// 사운드, ui 등 상태변경
    /// </summary>
    /// <param name="stat"></param>
    public void ChangeGameStat(GameStatus stat)
    {
        Debug.Log("ChangeGameStat: " + stat.ToString());

        statGame = stat;
        switch (stat)
        {
            case GameStatus.MENU:
                break;
            case GameStatus.LOADING:
                break;
            case GameStatus.SELECT:
                break;
            case GameStatus.GAME:
                break;
            default:
                break;
        }

        soundMgr.ChangeSceneBGM(statGame);
        tableMgr.ChangeTableUIStatus();
    }


    /// <summary>
    /// 7/13/2023-LYI
    /// 메인메뉴에서 게임 시작 시 호출
    /// 씬 이동, 플레이매니저 할당
    /// 플레이매니저에서 스테이지 시작하도록 호출
    /// </summary>
    public void GameStart(int stageNum = 0)
    {
       // loadMgr.LoadScene(SceneStatus.GAME, () => FindPlayManager(stageNum));

        playMgr.LoadStage(stageNum);
    }

    /// <summary>
    /// 8/23/2023-LYI
    /// 씬 이동 이후 호출
    /// 해당 씬의 플레이 매니저 할당 및 게임 시작 함수 호출
    /// </summary>
    void FindPlayManager(int stageNum = 0)
    {
        if (playMgr == null)
        {
            playMgr = FindObjectOfType<PlaySceneManager>();
        }

        playMgr.LoadStage(stageNum);
    }

    public void EndTutorial()
    {
        isTutorial = false;
        ES3.Save<bool>(Constants.ES3.IS_TUTORIAL, isTutorial);
    }

    /// <summary>
    /// 7/13/2023-LYI
    /// 게임 종료 시 호출
    /// 메인메뉴로 나가기?
    /// </summary>
    //public void OnPlayEnd()
    //{
    //    tableMgr.OnTableMenu();
    //   // loadMgr.LoadScene("Main");

    //}


#region 이펙트 관련 함수
    /// <summary>
    /// 파티클 재생 함수
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_go"></param>
    //public void PlayEffect(Vector3 _position, ParticleSystem _p)
    //{
    //    _p.transform.position = _position;
    //    _p.Play();
    //}
    //public void PlayEffect(Transform _position, ParticleSystem _p, string _path)
    //{
    //    _p.transform.position = _position.position;
    //    _p.Play();
    //    soundMgr.PlaySfx(_position, soundMgr.LoadClip(_path));
    //}


    //public void PlayParticleEffect(Vector3 _pos, GameObject _go)
    //{
    //    ParticleSystem _particle = Instantiate(_go).GetComponent<ParticleSystem>();
    //    _particle.transform.position = _pos;

    //    if (_particle.transform.childCount != 0)
    //    {
    //        ParticleSystem[] arr_particle = _particle.GetComponentsInChildren<ParticleSystem>();

    //        for (int index = 0; index < arr_particle.Length; index++)
    //        {
    //            arr_particle[index].Play();
    //        }
    //    }
    //    else
    //    {
    //        _particle.Play();
    //    }

    //    Destroy(_particle.gameObject, _particle.main.duration + 1);
    //}
    //public void PlayParticleEffect(Vector3 _pos, string _path)
    //{
    //    ParticleSystem _particle = Instantiate(b_prefab.LoadAsset<GameObject>(_path)).GetComponent<ParticleSystem>();
    //    _particle.transform.position = _pos;
    //    //_particle.transform.localScale = Vector3.one * uiMgr.stageSize;

    //    if (_particle.transform.childCount != 0)
    //    {
    //        ParticleSystem[] arr_particle = _particle.GetComponentsInChildren<ParticleSystem>();

    //        for (int index = 0; index < arr_particle.Length; index++)
    //        {
    //            arr_particle[index].Play();
    //        }
    //    }
    //    else
    //    {
    //        _particle.Play();
    //    }

    //    Destroy(_particle.gameObject, _particle.main.duration + 1);
    //}
#endregion

    public IEnumerator LateFunc(UnityAction action, float time = 1f)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }
    public IEnumerator LateFrameFunc(UnityAction action, float frame = 1f)
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        for (int i = 0; i < frame; i++)
        {
            yield return wait;
        }
        action.Invoke();
    }


}
