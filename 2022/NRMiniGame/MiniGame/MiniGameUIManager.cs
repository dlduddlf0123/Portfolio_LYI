using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;

public enum MiniGameUIStat
{
    NONE = 0,
    TUTORIAL,
    COUNTDOWN,
    GAME,
    RESULT,
}


/// <summary>
/// 각종 미니게임 시 사용될 UI
/// 게임 시작, 결과창
/// 카운트 다운 등을 표시한다.
/// 각 게임이 바뀔 때 마다 지정된 위치로 이동된다.
/// </summary>
public class MiniGameUIManager : MonoBehaviour
{
    GameManager gameMgr;
    MiniGameManager miniGameMgr;

    public MiniGameUI currentMiniGameUI; // 현재 게임의 UI

    //게임 이름, 설명 보관 리스트
    List<List<object>> list__csv_title = new List<List<object>>();

    AudioSource m_audioSource;

    //TutorialUI
    public GameObject stage_tutorialUI;
    TextMeshProUGUI tutorial_text_title;
    Text tutorial_text_tutorial;
    Text tutorial_text_grade;

    Button tutorial_btn_exit;
    Button tutorial_btn_start;

    //CountDownUI
    public GameObject stage_countDownUI;
    GameObject[] countDown_img_count;
    TextMeshProUGUI countDown_text_stage;

    //ResultUI
    public GameObject stage_resultUI;
    public Text result_text_score { get; set; }

    Button result_btn_retry;
    Button result_btn_exit;

    //결과창 등급 이미지
    public Sprite[] arr_sprite_grade;
    //Raycast 입력 받을 오브젝트
    public NRRayInteract[] arr_NRray;

    public MiniGameUIStat statMiniGameUI;
    void Awake()
    {
        gameMgr = GameManager.Instance;
        miniGameMgr = gameMgr.miniGameMgr;

        //list_miniGameText = GameManager.Instance.dialogMgr.ReadDialogDatas(ReadOnly.Defines.TEXT_UI_MINIGAME);

        m_audioSource = GetComponent<AudioSource>();

        //TutorialUI
        tutorial_text_title = stage_tutorialUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tutorial_text_tutorial = stage_tutorialUI.transform.GetChild(1).GetComponent<Text>();
        tutorial_text_grade = stage_tutorialUI.transform.GetChild(2).GetComponent<Text>();
        tutorial_btn_exit = stage_tutorialUI.transform.GetChild(3).GetComponent<Button>();
        tutorial_btn_start = stage_tutorialUI.transform.GetChild(4).GetComponent<Button>();

        //CountDownUi
        countDown_img_count = new GameObject[4];
        for (int i = 0; i < countDown_img_count.Length; i++)
        {
            countDown_img_count[i] = stage_countDownUI.transform.GetChild(i).gameObject;
        }
        countDown_text_stage = stage_countDownUI.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

       //ResultUI
       //result_text_score = stage_resultUI.transform.GetChild(1).GetComponent<Text>();
        //result_text_highScore = stage_resultUI.transform.GetChild(2).GetComponent<Text>();
        //result_img_grade = stage_resultUI.transform.GetChild(3).GetChild(0).GetComponent<Image>();
        result_btn_retry = stage_resultUI.transform.GetChild(0).GetComponent<Button>();
        result_btn_exit = stage_resultUI.transform.GetChild(1).GetComponent<Button>();


        list__csv_title = gameMgr.csvMgr.ReadCSVDatas("Title");
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDisable();
    }


    /// <summary>
    ///페이드 이후 호출
    ///UI 위치 설정 및 해당 게임 설명 데이터 불러오기
    /// </summary>
    public void UIInit(MiniGame miniGame)
    {
        Debug.Log("StageUI Init!");

        //transform.position = miniGameMgr.UiTr.position;
        //transform.rotation = miniGameMgr.UiTr.rotation;

        if ((int)miniGame.typeMiniGame <= list__csv_title.Count)
        {
            //CSV파일 읽어오기
            tutorial_text_title.text = list__csv_title[(int)miniGame.typeMiniGame][0].ToString();
            tutorial_text_tutorial.text = list__csv_title[(int)miniGame.typeMiniGame][1].ToString();
        }

        currentMiniGameUI = miniGame.miniGameUI;


        tutorial_text_grade.text =
            miniGame.gradeCut[0].ToString() + "\n" +
            miniGame.gradeCut[1].ToString() + "\n" +
            miniGame.gradeCut[2].ToString() + "\n";

        tutorial_btn_start.onClick.AddListener(() => StartCoroutine(CountDown()));    //게임플레이 시작
        tutorial_btn_exit.onClick.RemoveAllListeners();
        tutorial_btn_exit.onClick.AddListener(() => miniGame.EndMiniGame());
        arr_NRray[0].pointerClick = tutorial_btn_start.onClick;
        arr_NRray[1].pointerClick = tutorial_btn_exit.onClick;


        result_btn_retry.onClick.AddListener(RetryButton);
        result_btn_exit.onClick.RemoveAllListeners();
        result_btn_exit.onClick.AddListener(() => miniGame.EndMiniGame()); //게임모드 종료
        arr_NRray[2].pointerClick = result_btn_retry.onClick;
        arr_NRray[3].pointerClick = result_btn_exit.onClick;

        //창 활성화
        UIActive(MiniGameUIStat.TUTORIAL);
    }

    public void UIDisable()
    {
        stage_tutorialUI.SetActive(false);
        arr_NRray[0].gameObject.SetActive(false);
        arr_NRray[1].gameObject.SetActive(false);

        stage_countDownUI.SetActive(false);
        countDown_img_count[0].gameObject.SetActive(false);
        countDown_img_count[1].gameObject.SetActive(false);
        countDown_img_count[2].gameObject.SetActive(false);

        if (currentMiniGameUI != null)
        {
            currentMiniGameUI.gameObject.SetActive(false);
        }

        stage_resultUI.SetActive(false);
        arr_NRray[2].gameObject.SetActive(false);
        arr_NRray[3].gameObject.SetActive(false);
    }

    /// <summary>
    /// Mini Game UI Active
    /// </summary>
    /// <param name="currentUI">/1: 설명/2: 카운트/3: 게임/4: 결과/</param>
    public void UIActive(MiniGameUIStat currentUI)
    {
        statMiniGameUI = currentUI;

        m_audioSource.clip = gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_BGM_MINIGAME_TUTORIAL);
        m_audioSource.Play();
        gameMgr.soundMgr.ChangeBGMAudioSource(m_audioSource,false);
        UIDisable();

        switch (currentUI)
        {
            case MiniGameUIStat.NONE:
                break;
            case MiniGameUIStat.TUTORIAL:
                stage_tutorialUI.SetActive(true);
                arr_NRray[0].gameObject.SetActive(true);
                arr_NRray[1].gameObject.SetActive(true);
                break;
            case MiniGameUIStat.COUNTDOWN:
                m_audioSource.Stop();
                stage_countDownUI.SetActive(true);
                break;
            case MiniGameUIStat.GAME:
                currentMiniGameUI.gameObject.SetActive(true);
                break;
            case MiniGameUIStat.RESULT:
                stage_resultUI.SetActive(true);
                m_audioSource.clip = gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_BGM_MINIGAME_RESULT);
                m_audioSource.Play();

                arr_NRray[2].gameObject.SetActive(true);
                arr_NRray[3].gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }



    void RetryButton()
    {
        UIActive(MiniGameUIStat.GAME);

        StartCoroutine(CountDown());
    }


    public void ChangeSpriteImg(Image _img, Sprite _sprite)
    {
        _img.sprite = _sprite;
    }


    /// <summary>
    /// 등급 이미지 바꾸기
    /// </summary>
    /// <param name="grade"></param>
    public void ResultChangeGrade(int grade)
    {
        //switch (grade)
        //{
        //    case 0:
        //        //금
        //        ChangeSpriteImg(result_img_grade, arr_sprite_grade[3]);
        //        break;
        //    case 1:
        //        //은
        //        ChangeSpriteImg(result_img_grade, arr_sprite_grade[2]);
        //        break;
        //    case 2:
        //        //동
        //        ChangeSpriteImg(result_img_grade, arr_sprite_grade[1]);
        //        break;
        //    default:
        //        //실패
        //        ChangeSpriteImg(result_img_grade, arr_sprite_grade[0]);
        //        break;
        //}
    }

    /// <summary>
    /// 게임 시작 전 시간 세기
    /// </summary>
    /// <returns></returns>
    public IEnumerator CountDown(bool isClear = false)
    {
        if (gameMgr.statGame == GameStatus.READY)
        {
            yield break;
        }

        Debug.Log("MiniGameUIManager: CountDown()");
        gameMgr.statGame = GameStatus.READY;

        for (int i = 0; i < countDown_img_count.Length; i++)
        {
            countDown_img_count[i].SetActive(false);
        }
        //카운트다운 UI 활성화
        UIActive(MiniGameUIStat.COUNTDOWN);


        if (isClear)
        {
            countDown_img_count[3].gameObject.SetActive(true);
            //클리어 효과음, 애니

            yield return new WaitForSeconds(1f);
            countDown_img_count[3].gameObject.SetActive(false);
        }
        
        countDown_text_stage.text = miniGameMgr.currentMiniGame.stageNum.ToString();
        countDown_img_count[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countDown_img_count[0].gameObject.SetActive(false);
        countDown_img_count[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countDown_img_count[1].gameObject.SetActive(false);
        countDown_img_count[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countDown_img_count[2].gameObject.SetActive(false);

        //게임 UI 활성화
        UIActive(MiniGameUIStat.GAME);

        //게임 시작
        gameMgr.statGame = GameStatus.GAMEPLAY;

        miniGameMgr.currentMiniGame.StartMiniGame();
    }


}