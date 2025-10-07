using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBubble : MiniGameManager
{
    public GameObject bubblePrefab;

    public static Queue<GameObject> bubbleQueue = new Queue<GameObject>();

    public Transform bubbleDestination;
    public Transform[] array = new Transform[6];

    public float bubbleDelay;
    public Transform startpos;

    public int gameDifficulty;
   

    public int[] randomdestination = new int[2] { 0, 0 }; //버블 포지션 리스폰 위치

    public int combo;

    protected override void DoAwake()
    {
        Initialize(7);
    }



    #region 오브젝트풀링 관련 함수
    private void Initialize(int initCount)
    {
        for(int i =0;i<initCount;i++)
        {
            bubbleQueue.Enqueue(CreateNewObject());
        }
    }

    private GameObject CreateNewObject()
    {
        Debug.Log(startpos);
        var newObj = Instantiate(bubblePrefab);
        newObj.transform.position = startpos.position;
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        newObj.GetComponent<Bubble>().bubbleMgr = this;
        newObj.GetComponent<Bubble>().startPos = startpos.position;
        return newObj;
    }

    public void GetObject(int bubbleCount)
    {
        for (int i = 0; i < bubbleCount; i++)
        {
            if (bubbleQueue.Count > 0)
            {
                var bubble = bubbleQueue.Dequeue();
                bubble.gameObject.GetComponent<Bubble>().destination = randomdestination[i];
                bubble.gameObject.SetActive(true);
            }
            else
            {
                GameObject newObj = CreateNewObject();
                newObj.gameObject.SetActive(true);
            }
        }
        
    }

    public void ReturnObject(GameObject obj)
    {
        
        obj.gameObject.SetActive(false);
        bubbleQueue.Enqueue(obj);
    }
    #endregion

    IEnumerator BubbleSpawn()  //난이도 설정된 상태로 게임 시작
    {
        while(currentTime > 3)
        {
            MakeBubble();
            yield return new WaitForSeconds(bubbleDelay=Random.Range(0.5f,2.0f));
        }
    }

    public void MakeBubble() // 버블 생성
    {
        int bubbleCount = Random.Range(1, 3);
        if(bubbleCount ==1)
        {
            randomdestination[0] = Random.Range(0, 6);
            GetObject(bubbleCount);
        }
        else if(bubbleCount ==2)
        {          
            randomdestination[0] = Random.Range(0, 6);
            randomdestination[1] = Random.Range(0, 6);
            GetObject(bubbleCount);
        }
    }

    #region 상속받은 함수들(게임 루틴 관련)
    /// <summary>
    /// 게임모드 시작!(버튼)
    /// </summary>
    public override void PlayStart()
    {
        typeMiniGame = MiniGameType.BUBBLE;

        base.PlayStart();


        bubbleDestination.position = new Vector3(bubbleDestination.position.x,
            gameMgr.mainCam.transform.position.y - 0.3f,
            bubbleDestination.position.z);
    }

    /// <summary>
    /// 카운트다운 후 게임플레이 시작
    /// </summary>
    public override void GameStart()
    {
        Debug.Log(this.gameObject.name + " GameStart!!");
        StartCoroutine(BubbleSpawn());
        gameScore = 0;
        combo = 0;
    }

    /// <summary>
    /// 게임 종료, 결과창, 보상 부여, 데이터 저장 등
    /// </summary>
    public override void GameOver()
    {
        statMiniGame = MiniGameState.RESULT;
        Debug.Log(this.gameObject.name + " GameOver!!");

        //결과 창 활성화
        stageUI.stage_gameUI.SetActive(false);
        stageUI.stage_resultUI.SetActive(true);

        stageUI.result_text_score.text = "Score: " + gameScore.ToString();

        if (gameScore > PlayerPrefs.GetInt("BubbleHighScore", 0))
        {
            PlayerPrefs.SetInt("BubbleHighScore", gameScore);
            stageUI.result_text_highScore.text = "HighScore: " + gameScore.ToString();
        }

        //점수에 따른 보상 부여
        if (gameScore > gradeCut[0])
        {
            //금
            stageMgr.interactHeader.LikeChange(30);
            stageUI.ChangeGrade(0);
        }
        else if (gameScore > gradeCut[1])
        {
            //은
            stageMgr.interactHeader.LikeChange(20);
            stageUI.ChangeGrade(1);
        }
        else if (gameScore > gradeCut[2])
        {
            //동
            stageMgr.interactHeader.LikeChange(10);
            stageUI.ChangeGrade(2);
        }
        else
        {
            //실패 or 실망
             stageMgr.interactHeader.LikeChange(-10);
            stageUI.ChangeGrade(3);
        }

    }
    #endregion
}
