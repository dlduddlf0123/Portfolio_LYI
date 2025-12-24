using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveHandTracking;
using System.IO;

public enum GameState
{
    NONE = 0,
    PLAYING,
    GAMEOVER,
    CLEAR,
}


/// <summary>
/// 1. 어플을 시작했을 때에 행동을 설정한다.
/// 2. 게임을 시작했을 때의 행동을 설정한다.
/// 3. 게임 내에서 동적으로 변하는 오브젝트를 알고있다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public Character[] headers;
    public Character selectHeader;
    //public GameObject stage;
    public UIManager uiMgr;
    public SoundManager soundMgr;
    public AppleManager appleMgr;
    public DialogManager dialogMgr;

    public GameState statGame;

    public Camera mainCam;
    
    public HandInteract[] hand;
    GameObject[] menu = new GameObject[2];

    public ParticleSystem[] particles;

    public Coroutine currentCoroutine = null;

    public GameObject miniGameTable;

    public float timeRecord;

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

    // Start is called before the first frame update
    void Awake()
    {
        statGame = GameState.NONE;
        timeRecord = PlayerPrefs.GetFloat("TimeRecord", 90f);
        particles = new ParticleSystem[this.transform.GetChild(0).childCount];
        for (int idx = 0; idx < particles.Length; idx++)
        {
            particles[idx] = this.transform.GetChild(0).GetChild(idx).GetComponent<ParticleSystem>();
        }
    }
    private void Start()
    {
        selectHeader = headers[0];
        soundMgr.PlayBgm(Resources.Load<AudioClip>("Sounds/BGM/Casual Theme Loop #2"));
        for (int i = 0; i < 2; i++)
        {
            menu[i] = hand[i].transform.GetChild(0).gameObject;
        }
    }

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
    /// <summary>
    /// Play Effect & SFX on Hand Interact
    /// </summary>
    /// <param name="_pos">Which Hand to play</param>
    /// <param name="_particle">Particle to Play</param>
    /// <param name="_path">SFX path</param>
    public void HandEffect(int _hand)
    {
        PlayEffect(hand[_hand].transform.position, particles[0]);
        soundMgr.PlaySfx(hand[_hand].transform.position, soundMgr.LoadClip("Sounds/SFX/jump_15"));
    }
    public void HandEffect(HandInteract _hand)
    {
        PlayEffect(_hand.transform.position, particles[0]);
        soundMgr.PlaySfx(_hand.transform.position, soundMgr.LoadClip("Sounds/SFX/jump_15"));
    }

    #region HandController


    //검지 손가락
    public void ActionLeftPoint(int state)
    {
        if (state == 1)
        {
            if (selectHeader.isAction == true) { return; }

            selectHeader.SetSecondFinger(hand[0]);
        }
        else if (state == 2)
        {
            selectHeader.SetPaper(hand[0]);
        }
    }
    public void ActionRightPoint(int state)
    {
        if (state == 1)
        {
            if (selectHeader.isAction == true) { return; }

            selectHeader.SetSecondFinger(hand[1]);
        }
        else if (state == 2)
        {
            selectHeader.SetPaper(hand[1]);
        }
    }

    //주먹쥐기
    public void ActionLeftFist(int state)
    {
        if (state == 1)
        {
            if (selectHeader.isAction == true) { return; }
            selectHeader.SetRock(hand[0]);
        }
        else if (state == 2)
        {
            selectHeader.SetPaper(hand[0]);
        }
    }
    public void ActionRightFist(int state)
    {
        if (state == 1)
        {
            if (selectHeader.isAction == true) { return; }
            selectHeader.SetRock(hand[1]);
        }
        else if (state == 2)
        {
            selectHeader.SetPaper(hand[1]);
        }
    }

    //주먹펴기
    public void ActionFive(int state)
    {
        selectHeader.SetPaper(hand[0]);
    }

    //Like 따봉
    public void ActionLeftLike(int state)
    {
        StartCoroutine(hand[0].ActionDetachHand());
    }
    public void ActionRightLike(int state)
    {
        StartCoroutine(hand[1].ActionDetachHand());
    }

    //OK
    public void ActionOK(int state)
    {

    }

    //양손주먹
    public void ActionDoubleFist(int state)
    {
        if (state == 2)
        {
            PlayEffect(hand[0].transform.position, particles[0]);
            PlayEffect(hand[1].transform.position, particles[0]);
            soundMgr.PlaySfx(hand[1].transform.position, soundMgr.LoadClip("Sounds/SFX/jump_15"));
            selectHeader.SetDoubleRock();
        }
    }
    //양손검지
    public void ActionDoublePoint(int state)
    {
        if (state == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                PlayEffect(hand[i].transform.position, particles[0]);
                menu[i].SetActive(true);
                menu[i].transform.SetParent(null);
                menu[i].transform.LookAt(mainCam.transform);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                PlayEffect(hand[i].transform.position, particles[0]);
                menu[i].SetActive(false);
                menu[i].transform.SetParent(hand[i].transform);
                menu[i].transform.localPosition = Vector3.zero;
            }
        }
       
    }
    //따봉 -> 주먹(기폭 스위치 누르듯)
    public void ActionBoom(int state)
    {

    }
    #endregion

}
