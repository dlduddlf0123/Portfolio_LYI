using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    PlayManager ingameMgr;
    [Header("- In-Game UI")]
    public GameObject inGameUI;
    public Image img_hpGauge;
    public Image img_feverGuage;
    public Text txt_score;
    public Text txt_combo;
    public Button bt_left;
    public Button bt_right;
    public Button bt_attack;
    public TextMeshProUGUI feverTime;
    public Button bt_pause;
    public Button bt_leftTouch;
    public Button bt_rightTouch;
    public Button bt_item;
    //In-Game Result UI
    [Header("- Result UI")]
    public RectTransform resultUI;
    public Text score;
    public Button bt_restart;
    public Text rank;
    //HitGuideCanvas UI
    [Header("- HitGuide UI")]
    public GameObject HitGuide;
    public GameObject circleGuidePool;
    public Queue<GameObject> circleGuide = new Queue<GameObject>(); //원 작아지는 이펙트 모아놓는 큐(오브젝트 풀링으로 쓸것임)
    [Range(0, 1)] public float[] hitGuideSize = new float[2];//뒤에 노트랑 앞에 노트랑 게임뷰에서 크기가 다름 -> 가이드 크기가 달라져야됨(왼,오)
    public GameObject currentHitGuide;
    //Pause UI
    [Header("- Pause UI")]
    public RectTransform pauseUI;
    public Button pause_bt_start;
    public Button pause_bt_exit;
    public Button pause_bt_retry;
    //CountDown
    [Header("- CoundDown")]
    public TextMeshProUGUI CountDown;





    public float canvaswidth { get; set; }
    public float canvasheight { get; set; }

    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
        canvaswidth = Screen.width;
        canvasheight = Screen.height;
        for(int i =0;i<5;i++)
        {
            circleGuide.Enqueue(circleGuidePool.transform.GetChild(i).gameObject);
            circleGuidePool.transform.GetChild(i).gameObject.SetActive(false);
        }
        //In-game UI 
        bt_pause.onClick.AddListener(() => { pauseUI.gameObject.SetActive(true); ingameMgr.Pause(); });
        bt_item.onClick.AddListener(() => { ingameMgr.player.mAnimator.SetTrigger("isItem");bt_item.gameObject.SetActive(false); });

        //Pause UI
        pause_bt_start.onClick.AddListener(() => { pauseUI.gameObject.SetActive(false);ingameMgr.Play(); bt_pause.interactable = false; });
        pause_bt_exit.onClick.AddListener(() => { ingameMgr.gameMgr.LoadScene(0); ingameMgr.gameMgr.uiMgr.musicSelectUI.gameObject.SetActive(true);Time.timeScale = 1; AudioListener.pause = false; });
        pause_bt_retry.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);Time.timeScale = 1;AudioListener.pause = false; });
    }

    void Start()
    {
        UIInit();
    }

    public void UIInit()
    {
        img_hpGauge.fillAmount = 1.0f;
        img_feverGuage.fillAmount = 0;
        SetScoreText(0);
        SetComboText(0);
        txt_combo.gameObject.SetActive(false);
    }    

    public void SetHPGauge(int _hp)
    {
        img_hpGauge.fillAmount = _hp *0.01f;
    }

    public void SetFeverGuage(int _fever)
    {
        img_feverGuage.fillAmount = _fever * 0.066f;
    }

    public void SetScoreText(int _score)
    {
        txt_score.text = "" + _score;
    }

    public void SetComboText(int _combo)
    {
        txt_combo.gameObject.GetComponent<Animation>().Play();
        if (_combo == 0)
        {
            txt_combo.gameObject.SetActive(false);
        }
        else
        {
            txt_combo.gameObject.SetActive(true);
            txt_combo.text = "Combo\n" + _combo;
        }
    }

    public void ShowResult()
    {
        inGameUI.SetActive(false);
        resultUI.gameObject.SetActive(true);
        ingameMgr.player.gameObject.SetActive(false);
        score.text = ""+ingameMgr.score;
        GetRank();
        if(ingameMgr.gameMgr.highScore < ingameMgr.score)
        {
            ingameMgr.gameMgr.highScore = ingameMgr.score;
            ingameMgr.gameMgr.uiMgr.select_score.text = "High Score : " + ingameMgr.gameMgr.highScore;
        }
        bt_restart.onClick.AddListener(() => { ingameMgr.gameMgr.LoadScene(0); ingameMgr.gameMgr.uiMgr.musicSelectUI.gameObject.SetActive(true); });
        //랭크랑 리스타트 버튼 구현하기
    }
    
    public void GetRank()
    {
        if(ingameMgr.score>=9500 )
        {
            rank.text = "S";
        }
        else if(ingameMgr.score>=9000)
        {
            rank.text = "A";
        }
        else if(ingameMgr.score>=8000)
        {
            rank.text = "B";
        }
        else if(ingameMgr.score>=7000)
        {
            rank.text = "C";
        }
        else
        {
            rank.text = "D";
        }
    }
    public IEnumerator FeverTimeUI()
    {
        feverTime.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        feverTime.gameObject.SetActive(false);
    }

}
