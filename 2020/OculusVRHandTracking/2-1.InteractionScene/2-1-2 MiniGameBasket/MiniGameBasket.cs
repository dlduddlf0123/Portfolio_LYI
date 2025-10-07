using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 과일 받기 놀이
/// 일정 높이 위에서 x좌표 방향으로 과일 생성, 중력에 따라 떨어진다
/// 바구니를 들고 과일을 받으면 스코어 상승
/// 일정 시간동안 획득한 과일에 따라 최종 스코어 계산
/// 획득한 과일로 먹이를 줄 때 쓸 수 있다. or 그냥 친밀도 증가
/// </summary>
public class MiniGameBasket : MiniGameManager
{
    public Transform spawner;
    Transform[] spawnPoints;

    public BasketCharacter header;

    public GameObject apple;

    public GameObject basket;
    public Transform[] basketPoint;
    List<GameObject> list_basket = new List<GameObject>();

    public Queue<GameObject> q_apple = new Queue<GameObject>();

    public float randomTimeMin = 0.5f;
    public float randomTimeMax = 2f;

    protected override void DoAwake()
    {
        spawnPoints = spawner.GetComponentsInChildren<Transform>();
        basketPoint = basket.transform.GetComponentsInChildren<Transform>();

        header.gameObject.SetActive(false);
    }

    public void AddApple(GameObject _apple)
    {
        _apple.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _apple.GetComponent<Rigidbody>().isKinematic = true;
        _apple.GetComponent<Collider>().enabled = false;
        _apple.transform.position = basketPoint[list_basket.Count].position;
        //gameObject.tag = "Basket";
        list_basket.Add(_apple);
    }

    public void ResetBasket()
    {
        for (int i = 0; i < list_basket.Count; i++)
        {
            list_basket[i].GetComponent<FallingFruit>().Init();
        }
        list_basket.Clear();
    }

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    public void GameInit()
    {
        if (q_apple.Count  <= 1)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject _apple = Instantiate(apple);
                _apple.GetComponent<FallingFruit>().basket = this;
                q_apple.Enqueue(_apple);
            }
        }
    }


    IEnumerator StartSpawn()
    {
        while (currentTime > 3)
        {
            int randomPos = Random.Range(1, spawnPoints.Length);
            Vector3 randomVector = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
            float randomTime = Random.Range(randomTimeMin, randomTimeMax);

            if (q_apple.Count == 0)
            {
                GameObject _apple = Instantiate(apple);
                _apple.GetComponent<FallingFruit>().basket = this;
                _apple.transform.position = spawnPoints[randomPos].position+ randomVector;
                _apple.SetActive(true);
            }
            else
            {
                GameObject _apple = q_apple.Dequeue();
                _apple.transform.position = spawnPoints[randomPos].position+ randomVector;
                _apple.SetActive(true);
            }

            yield return new WaitForSeconds(randomTime);
        }
    }


    public override void PlayStart()
    {
        typeMiniGame = MiniGameType.BASKET;
        base.PlayStart();


        header.gameObject.SetActive(true);
    }

    public override void PlayEnd()
    {
        stageMgr.MovePlayer(stageMgr.interactionTransform);
        base.PlayEnd();


        header.gameObject.SetActive(false);
    }



    /// <summary>
    /// 카운트다운 후 게임플레이 시작
    /// </summary>
    public override void GameStart()
    {
        base.GameStart();

        Debug.Log(this.gameObject.name + " GameStart!!");
        StartCoroutine(StartSpawn());
    }

    /// <summary>
    /// 게임 종료, 결과창, 보상 부여, 데이터 저장 등
    /// </summary>
    public override void GameOver()
    {
        base.GameOver();

        statMiniGame = MiniGameState.RESULT;
        Debug.Log(this.gameObject.name + " GameOver!!");

        

        //결과 창 활성화
        stageUI.stage_gameUI.SetActive(false);
        stageUI.stage_resultUI.SetActive(true);

        stageUI.result_text_score.text = "Score: " + gameScore.ToString();

        if (gameScore > PlayerPrefs.GetInt("BasketHighScore", 0))
        {
            PlayerPrefs.SetInt("BasketHighScore", gameScore);
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




}
