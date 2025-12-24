using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum GameState
{
    NONE = 0,
    PLAY,
    CLEAR,
    CUTSCENE,
    GAMEOVER,
}


/// <summary>
/// 1. 어플을 시작했을 때에 행동을 설정한다.
/// 2. 게임을 시작했을 때의 행동을 설정한다.
/// 3. 게임 내에서 동적으로 변하는 오브젝트를 알고있다.
/// 4. 게임의 현재 진행상태를 알고있다.(스테이지 몇)
/// </summary>
public class GameManager : MonoBehaviour
{
    public SoundManager soundMgr;
    public DialogManager dialogMgr;
    public MissileManager missileMgr;
    public StageManager stageMgr;

    //AssetBundles
    public AssetBundle b_prefabs { get; set; }
    public AssetBundle b_sprites { get; set; }
    public AssetBundle b_sounds { get; set; }
    public AssetBundle b_csvdata { get; set; }
    public AssetBundle b_animator { get; set; }

    //Player
    public GameObject playerPrefab;
    public Player player;
    public MyRightHand rightHand;
    public MyLeftHand leftHand;
    public Camera mainCam;
    public ParticleSystem effectTouch;
    public GameObject laser;

    public GameState statGame;
    public Character[] arr_headers;

    //Save Datas
    public int language; //0:korean 1: english
    public int clearLevel { get; set; }//클리어한 최대 레벨, 세이브 데이터

    //싱글톤 선언
    private static GameManager s_instance;
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
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            player  = Instantiate(playerPrefab).GetComponent<Player>();
            leftHand = player.transform.GetChild(0).GetChild(1).GetComponent<MyLeftHand>();
            rightHand = player.transform.GetChild(0).GetChild(2).GetComponent<MyRightHand>();
            mainCam = player.transform.GetChild(0).GetChild(3).GetComponent<Camera>();
        }

        if (Application.systemLanguage == SystemLanguage.Korean)
            language = PlayerPrefs.GetInt("Language", 0);
        else
            language = PlayerPrefs.GetInt("Language", 1);


        // b_prefabs = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/prefabs"));
        b_sounds = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/sounds");
        b_csvdata = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/csv");
        b_sprites = AssetBundle.LoadFromFile("AssetBundles/StandaloneWindows/sprites");
       // b_animator = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "animator"));

        DontDestroyOnLoad(this);

        statGame = GameState.NONE;

    }

    private void Start()
    {
        //rightHand.gameMgr = this;
        rightHand.laser = laser;
    }

    public void PlayEffect(ParticleSystem _effect, Vector3 _pos)
    {
        _effect.transform.position = _pos;
        _effect.Play();
    }

    public void ChangeScene(string _sceneName)
    {
        Valve.VR.SteamVR_LoadLevel.Begin(_sceneName);
    }

}
