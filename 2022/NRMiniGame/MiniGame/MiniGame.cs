using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using Pathfinding;

public class MiniGame : MonoBehaviour
{
    [Header("MiniGame")]
    public GameManager gameMgr;
    public MiniGameUI miniGameUI;

    public List<ParticleSystem> list_guideParticle;
    protected List<Vector3> list_guidePosition = new List<Vector3>();

    //사운드
    public AudioSource m_audioSource { get; set; }
    protected AudioClip bgm_stage;

    //지형 관련
    public AstarPath ApathMgr;
    protected RecastGraph recastGraph;

    //난이도 보관 리스트
    protected List<List<object>> list__csv_stage = new List<List<object>>();

    //현재 미니게임의 유형
    public MiniGameType typeMiniGame = MiniGameType.NONE;
    public HandIcon e_handIcon; //손 모양

    public UnityAction action_miniGameEnd;

    public Transform particlePool;

    protected string stageTitle = null;

    //게임 스코어, 최고기록
    public int gameScore = 0;
    public int highScore = 0;

    //제한시간, 현재 시간 (시간 제한이 없을 수 있음)
    public int limitTime = 10;
    public int currentTime = 0;

    //목숨(유형에 따라 없을 수 있음)
    public int maxLife = 3;
    public int currentLife = 3;

    //점수 등급 컷
    public int[] gradeCut = new int[3];

    //현재 스테이지 번호
    public int stageNum = 1;

    public bool isScan = false;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_BGM_MINIGAME_PLAY);

        //list_guideParticle = episodeMgr.list_guideParticle;


        DoAwake();
    }

    protected virtual void DoAwake() { }

    /// <summary>
    /// 초기화, 난이도 관련 데이터 확인
    /// </summary>
    public virtual void GameInit()
    {
        Debug.Log(gameObject.name + ": GameInit()");

        if (stageNum > list__csv_stage.Count)
        {
            stageNum = list__csv_stage.Count;
        }

        gameScore = 0;
        highScore = gameMgr.miniGameMgr.LoadScore();

        currentLife = maxLife;

        currentTime = 0;
        limitTime = 10;
        

        if (isScan)
        {
            //네비게이션 스캔
            AstarScan();
        }
    }


    public virtual void AstarScan()
    {
        recastGraph = ApathMgr.data.recastGraph;
        recastGraph.SnapForceBoundsToScene();
        ApathMgr.graphs[0].Scan();
    }

    public virtual void AstarScan(Vector3 _center, Vector3 _size)
    {
        recastGraph = ApathMgr.data.recastGraph;
        recastGraph.SnapForceBoundsToScene();
        recastGraph.forcedBoundsCenter += _center;
        recastGraph.forcedBoundsSize += _size;
        ApathMgr.graphs[0].Scan();
    }


    public void MakeGuideParticle()
    {
        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            if (list_guideParticle.Count < list_guidePosition.Count)
            {
                list_guideParticle.Add(Instantiate(gameMgr.b_stagePrefab.LoadAsset<GameObject>("InteractionEffect")).GetComponent<ParticleSystem>());
            }
          //  list_guideParticle[i].transform.parent = episodeMgr.particlePool;
            list_guideParticle[i].transform.localScale *= gameMgr.uiMgr.stageSize;

        }
    }

    public virtual void PlayGuideParticle()
    {
        MakeGuideParticle();

        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            list_guideParticle[i].transform.position = list_guidePosition[i];
            list_guideParticle[i].transform.localScale = Vector3.one * 0.3f;
            list_guideParticle[i].Play();
            list_guideParticle[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
    }

    public virtual void StopGuideParticle()
    {
        if (list_guideParticle.Count == 0)
        {
            return;
        }
        for (int i = 0; i < list_guideParticle.Count; i++)
        {
            if (list_guideParticle[i] != null)
                list_guideParticle[i].Stop();
        }
    }

    //게임 시간 세어주는 코루틴
    public IEnumerator GameTimer()
    {
        currentTime = 0;

        //게임이 플레이 상태이거나 게임 시간이 다 되기 전까지 반복
        while (gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            currentTime++; //1초마다 1씩 증가
            //game_img_timeGauge.fillAmount -= 1 / (float)gameMgr.limit_playTime;

            miniGameUI.GameUIChangeTimerText(currentTime);
            yield return new WaitForSeconds(1.0f);
        }
    }


    //게임 시간 세어주는 코루틴
    public virtual IEnumerator LimitTimer(int limitTime)
    {
        currentTime = limitTime; //게임 오버까지의 남은 시간 

        //게임이 플레이 상태이거나 게임 시간이 다 되기 전까지 반복
        //게이지 줄어들기
        while (currentTime > 0 &&
            gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            currentTime--; //1초마다 1씩 감소
            //game_img_timeGauge.fillAmount -= 1 / (float)gameMgr.limit_playTime;

            miniGameUI.GameUIChangeTimerText(currentTime);
            yield return new WaitForSeconds(1.0f);
        }

        //시간이 다 지났을 때
        if (currentTime <= 0 &&
            gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            ResultMiniGame(false);
        }
    }


    public virtual void GetScore(int score)
    {
        gameScore += score;
        miniGameUI.GameUIChangeScoreText(gameScore);
    }

    public virtual void LoseScore(int score)
    {
        gameScore -= score;
        if (gameScore < 0)
        {
            gameScore = 0;
        }
        miniGameUI.GameUIChangeScoreText(gameScore);
    }

    public virtual void GetLife()
    {
        Debug.Log("Get Life()");
        currentLife++;
        if (currentLife > maxLife)
        {
            currentLife = maxLife;
        }

        miniGameUI.GameUIChangeLifeIcon(currentLife);
    }
    public virtual void LoseLife()
    {
        Debug.Log("Lose Life()");
        currentLife--;
        if (currentLife < 1)
        {
            currentLife = 0;
            ResultMiniGame(false);
        }

        miniGameUI.GameUIChangeLifeIcon(currentLife);
    }

    /// <summary>
    /// 해당 스테이지를 성공했을 때 호출
    /// </summary>
    public virtual void ClearStage()
    {
        Debug.Log("Stage Clear!");
        stageNum++;

        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);
        //클리어 글자 표시 이후 다음 레벨 시작
        gameMgr.miniGameMgr.miniGameUIMgr.UIActive(MiniGameUIStat.COUNTDOWN);

        StartCoroutine(gameMgr.miniGameMgr.miniGameUIMgr.CountDown(true));

    }


    /// <summary>
    /// 미니게임 결과 성공
    /// </summary>
    public virtual void ResultMiniGame(bool isSuccess)
    {
        gameMgr.statGame = GameStatus.RESULT;
        Debug.Log(this.gameObject.name + "Result");

        StopAllCoroutines();

        miniGameUI.stage_gameUI.SetActive(false);

        //결과 창 표시 성공 / 실패
        if (isSuccess)
        {
            ClearStage();
        }
        else
        {
            //결과 창 활성화
            gameMgr.miniGameMgr.miniGameUIMgr.UIActive(MiniGameUIStat.RESULT);

            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_FAILURE);
            //gameMgr.miniGameMgr.miniGameUIMgr.result_text_score.text = "Score: " + gameScore.ToString();
           
            SaveScore();

            //점수에 따른 보상 부여
            if (gameScore > gradeCut[0])
            {
                //금
                gameMgr.miniGameMgr.miniGameUIMgr.ResultChangeGrade(0);
            }
            else if (gameScore > gradeCut[1])
            {
                //은
                gameMgr.miniGameMgr.miniGameUIMgr.ResultChangeGrade(1);
            }
            else if (gameScore > gradeCut[2])
            {
                //동
                gameMgr.miniGameMgr.miniGameUIMgr.ResultChangeGrade(2);
            }
            else
            {
                //실패 or 실망
                gameMgr.miniGameMgr.miniGameUIMgr.ResultChangeGrade(3);
            }


            //스코어 컷 등

        }
    }

    public virtual void SaveScore()
    {
        //최고기록 체크, 최고기록 축하 폭죽, 기록 저장
        if (gameScore > PlayerPrefs.GetInt("BubbleHighScore", 0))
        {
            PlayerPrefs.SetInt("BubbleHighScore", gameScore);
            //gameMgr.miniGameMgr.miniGameUIMgr.result_text_highScore.text = "HighScore: " + gameScore.ToString();
        }
    }

    /// <summary>
    /// Init,Scan,GameStatus,BGM,Icon
    /// </summary>
    public virtual void StartMiniGame()
    {
        //게임 상태 변경
        Debug.Log(gameObject.name + "StartMiniGame");
        gameMgr.statGame = GameStatus.GAMEPLAY;

        //BGM 변경
        gameMgr.soundMgr.ChangeBGMAudioSource(m_audioSource,false);
        miniGameUI.stage_gameUI.SetActive(true);

        miniGameUI.GameUIChangeLifeIcon(maxLife);
        miniGameUI.GameUIChangeTimerText(limitTime);

        GameInit();

        //제한시간 있으면 시간 감소
        if (limitTime > 1)
        {
            StartCoroutine(LimitTimer(limitTime));
        }
        else
        {
            //일반 시간 측정(난이도 조정, 스코어링)
            StartCoroutine(GameTimer());
        }
    }


    /// <summary>
    /// BGM,Icon,Destroy
    /// 미니게임 삭제 뒤 메뉴로 나가기
    /// </summary>
    public virtual void EndMiniGame()
    {
        Debug.Log(gameObject.name + "EndMiniGame");
        gameMgr.statGame = GameStatus.MENU;
        gameMgr.nrUiMgr.UIActive(NRUIActive.MAINMENU);

        //gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.NONE);

        m_audioSource.Stop();

        if (action_miniGameEnd != null)
        {
            action_miniGameEnd.Invoke();
        }

        gameMgr.miniGameMgr.DestroyMiniGame();
    }
}
