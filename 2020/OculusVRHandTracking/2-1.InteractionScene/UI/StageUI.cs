using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// 각종 미니게임 시 사용될 UI
/// 게임 시작, 결과창
/// 카운트 다운 등을 표시한다.
/// 각 게임이 바뀔 때 마다 지정된 위치로 이동된다.
/// </summary>
public class StageUI : MonoBehaviour
{
    GameManager gameMgr;
    public StageManager stageMgr;
    MiniGameManager miniGameMgr;

    List<List<object>> list_miniGameText = new List<List<object>>();

    //TutorialUI
    public GameObject stage_tutorialUI;
    Text tutorial_text_title;
    Text tutorial_text_tutorial;
    Text tutorial_text_grade;
    Button tutorial_btn_exit;
    Button tutorial_btn_next;
    Button tutorial_btn_start;

    //CountDownUI
    public GameObject stage_countDownUI;
    public Image[] countDown_img_count;

    //GameUI
    public GameObject stage_gameUI;
    TextMeshProUGUI game_text_time;
    public TextMeshProUGUI game_text_score { get; set; }

    //ResultUI
    public GameObject stage_resultUI;
    public Text result_text_score { get; set; }
    public Text result_text_highScore { get; set; }
    public Image result_img_grade { get; set; }
    Button result_btn_retry;
    Button result_btn_exit;

    void Awake()
    {
        list_miniGameText = GameManager.Instance.dialogMgr.ReadDialogDatas(ReadOnly.Defines.TEXT_UI_MINIGAME);

        //TutorialUI
        tutorial_text_title = stage_tutorialUI.transform.GetChild(0).GetComponent<Text>();
        tutorial_text_tutorial = stage_tutorialUI.transform.GetChild(1).GetComponent<Text>();
        tutorial_text_grade = stage_tutorialUI.transform.GetChild(2).GetComponent<Text>();
        tutorial_btn_exit = stage_tutorialUI.transform.GetChild(3).GetComponent<Button>();
        tutorial_btn_start = stage_tutorialUI.transform.GetChild(4).GetComponent<Button>();

        //CountDownUi
        countDown_img_count = new Image[2];
        countDown_img_count[0] = stage_countDownUI.transform.GetChild(0).GetComponent<Image>();
        countDown_img_count[1] = stage_countDownUI.transform.GetChild(1).GetComponent<Image>();

        //GameUI
        game_text_time = stage_gameUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        game_text_score = stage_gameUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //ResultUI
        result_text_score = stage_resultUI.transform.GetChild(1).GetComponent<Text>();
        result_text_highScore = stage_resultUI.transform.GetChild(2).GetComponent<Text>();
        result_img_grade = stage_resultUI.transform.GetChild(3).GetChild(0).GetComponent<Image>();
        result_btn_retry = stage_resultUI.transform.GetChild(4).GetComponent<Button>();
        result_btn_exit = stage_resultUI.transform.GetChild(5).GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameManager.Instance;


    }

    /// <summary>
    ///페이드 이후 호출
    ///UI 위치 설정 및 해당 게임 설명 데이터 불러오기
    /// </summary>
    public void StageUIInit(MiniGameManager _miniGame)
    {
        Debug.Log("StageUI Init!");
        miniGameMgr = _miniGame;
        transform.position = miniGameMgr.UiTr.position;
        transform.rotation = miniGameMgr.UiTr.rotation;

        if (list_miniGameText.Count >= (int)_miniGame.typeMiniGame * 2 + 2)
        {
            //CSV파일 읽어오기
            tutorial_text_title.text = list_miniGameText[(int)_miniGame.typeMiniGame * 2][1].ToString();
            tutorial_text_tutorial.text = list_miniGameText[(int)_miniGame.typeMiniGame * 2 + 1][1].ToString();
        }
        tutorial_text_grade.text =
            _miniGame.gradeCut[0].ToString() + "\n" +
            _miniGame.gradeCut[1].ToString() + "\n" +
            _miniGame.gradeCut[2].ToString() + "\n";

        tutorial_btn_exit.onClick.AddListener(() => _miniGame.PlayEnd());
        tutorial_btn_start.onClick.AddListener(() => _miniGame.StartCoroutine(_miniGame.CountDown()));    //게임플레이 시작

        //게임 플레이 관련
        game_text_score.text = _miniGame.gameScore.ToString();
        game_text_time.text = _miniGame.limitTime.ToString();

        //결과창 관련
        switch (_miniGame.typeMiniGame)
        {
            case MiniGameType.BUBBLE:
                result_text_highScore.text = "HighScore: " + PlayerPrefs.GetInt("BubbleHighScore", 0).ToString();
                break;
            case MiniGameType.BASKET:
                result_text_highScore.text = "HighScore: " + PlayerPrefs.GetInt("BasketHighScore", 0).ToString();
                break;
            case MiniGameType.FIREWOOD:
                result_text_highScore.text = "HighScore: " + PlayerPrefs.GetInt("FireWoodHighScore", 0).ToString();
                break;
            case MiniGameType.COOK:
                result_text_highScore.text = "HighScore: " + PlayerPrefs.GetInt("CookHighScore", 0).ToString();
                break;
            case MiniGameType.DEFENSE:
                result_text_highScore.text = "HighScore: " + PlayerPrefs.GetInt("DefenseHighScore", 0).ToString();
                break;
            case MiniGameType.STAR:
                result_text_highScore.text = "HighScore: " + PlayerPrefs.GetInt("StarHighScore", 0).ToString();
                break;
        }
        result_btn_retry.onClick.AddListener(RetryButton);
        result_btn_exit.onClick.AddListener(() => _miniGame.PlayEnd()); //게임모드 종료

        //창 활성화
        stage_tutorialUI.SetActive(true);
        stage_countDownUI.SetActive(false);
        countDown_img_count[0].gameObject.SetActive(false);
        countDown_img_count[1].gameObject.SetActive(false);
        stage_gameUI.SetActive(false);
        stage_resultUI.SetActive(false);
    }

    void RetryButton()
    {
        stage_resultUI.SetActive(false);
        stage_gameUI.SetActive(true);

        miniGameMgr.StartCoroutine(miniGameMgr.CountDown());
    }

    public void ChangeSpriteImg(Image _img, Sprite _sprite)
    {
        _img.sprite = _sprite;
    }

    /// <summary>
    /// 등급 이미지 바꾸기
    /// </summary>
    /// <param name="_grade"></param>
    public void ChangeGrade(int _grade)
    {
        switch (_grade)
        {
            case 0:
                //금
                ChangeSpriteImg(result_img_grade, gameMgr.b_sprites.LoadAsset<Sprite>(ReadOnly.Defines.STAGEUI_IMG_GRADE1));
                break;
            case 1:
                //은
                ChangeSpriteImg(result_img_grade, gameMgr.b_sprites.LoadAsset<Sprite>(ReadOnly.Defines.STAGEUI_IMG_GRADE2));
                break;
            case 2:
                //동
                ChangeSpriteImg(result_img_grade, gameMgr.b_sprites.LoadAsset<Sprite>(ReadOnly.Defines.STAGEUI_IMG_GRADE3));
                break;
            default:
                //실패
                //ChangeSpriteImg(result_img_grade, gameMgr.b_sprites.LoadAsset<Sprite>(ReadOnly.Defines.STAGEUI_IMG_GRADE4));
                break;
        }
    }


    //게임 시간 세어주는 코루틴
    public IEnumerator GameTimer(MiniGameManager minigameMgr,int _limitTime)
    {
        minigameMgr.currentTime = _limitTime; //게임 오버까지의 남은 시간 

        //게임이 플레이 상태이거나 게임 시간이 다 되기 전까지 반복
        //게이지 줄어들기
        while (gameMgr.statGame == GameState.MINIGAME
            && minigameMgr.currentTime > 0)
        {
            minigameMgr.currentTime--; //1초마다 1씩 감소
            //game_img_timeGauge.fillAmount -= 1 / (float)gameMgr.limit_playTime;
            game_text_time.text = minigameMgr.currentTime.ToString();
            yield return new WaitForSeconds(1.0f);
        }

        //시간이 다 지났을 때
        if (minigameMgr.currentTime <= 0 &&
            gameMgr.statGame == GameState.MINIGAME)
        {
            miniGameMgr.GameOver();
        }
    }

}
