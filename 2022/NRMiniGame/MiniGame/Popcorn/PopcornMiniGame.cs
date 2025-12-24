using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using System;
public class PopcornMiniGame : MiniGame
{
    [Header("Son")]
    PopcornMiniGameUI popcornUI;

    //originalPrefab
    public PopcornPrefab prefab_ball; //복사할 원본 팝콘

    Queue<PopcornPrefab> queue_popcornPool = new Queue<PopcornPrefab>();

    public Transform cornPos;
    Transform[] arr_spawnPos;

    public Transform[] arr_ballParent; //0:Disable 1:Active

    public Transform startTr;

    Coroutine currentCoroutine = null;

    public float spawnTime = 0.3f;

    public int currentPopcorn = 0;
    public int maxPopcorn = 0;
    protected override void DoAwake()
    {
        arr_spawnPos = new Transform[cornPos.childCount];

        for (int i = 0; i < cornPos.childCount; i++)
        {
            arr_spawnPos[i] = cornPos.GetChild(i);
        }

        popcornUI = miniGameUI.GetComponent<PopcornMiniGameUI>();

        stageNum = PlayerPrefs.GetInt("PopcornStage", 1);
        list__csv_stage = gameMgr.csvMgr.ReadCSVDatas("Popcorn");
    }

    public override void GameInit()
    {
        base.GameInit();

        //PlayerPrefs.SetInt("PopcornStage", stageNum);

        currentPopcorn = 0;

        limitTime = Convert.ToInt32(list__csv_stage[stageNum][0]);
        maxPopcorn = Convert.ToInt32(list__csv_stage[stageNum][1]);

        Debug.Log("Stage:" + stageNum + "/LimitTime:" + limitTime + "/MaxPopcorn:" + maxPopcorn);
    }


    void BallInit(PopcornPrefab _go)
    {
        _go.CornInit();

        _go.transform.SetParent(arr_ballParent[0]);
        queue_popcornPool.Enqueue(_go);
        _go.GetComponent<Rigidbody>().velocity = Vector3.zero;

        _go.gameObject.SetActive(false);
    }

    public override void GetScore(int score)
    {
        gameScore += score;
        miniGameUI.GameUIChangeScoreText(gameScore);
    }

    public override void LoseScore(int score)
    {
        gameScore -= score;
        if (gameScore < 0)
        {
            gameScore = 0;
        }
        miniGameUI.GameUIChangeScoreText(gameScore);
    }

    /// <summary>
    /// 옥수수를 팝콘으로 만들었을 때
    /// 스코어 획득, 카운트, 게임 종료 체크
    /// 오브젝트 풀로 되돌리기
    /// </summary>
    /// <param name="_go"></param>
    void CornPop(PopcornPrefab _go)
    {
        currentPopcorn++;
        if (currentPopcorn >= maxPopcorn)
        {
            currentPopcorn = maxPopcorn;
            popcornUI.GameUIChangePopcornText(currentPopcorn, maxPopcorn);

            ResultMiniGame(true);
        }
        else
        {
            popcornUI.GameUIChangePopcornText(currentPopcorn, maxPopcorn);
        }


        GetScore(100);

        BallInit(_go);
    }

    /// <summary>
    /// 공 뿌리기
    /// </summary>
    void SpamBall()
    {
        StartCoroutine(SpawnBall());
    }

    IEnumerator SpawnBall()
    {
        while (gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            if (queue_popcornPool.Count < 30)
            {
                ActiveBall();
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    void ActiveBall()
    {
        if (queue_popcornPool.Count == 0)
        {
            PopcornPrefab _go = Instantiate(prefab_ball);
            _go.transform.SetParent(arr_ballParent[1]);
            _go.transform.position = arr_spawnPos[UnityEngine.Random.Range(0, arr_spawnPos.Length)].position;

            _go.onPop = () => CornPop(_go);
            _go.onDestroy = () => BallInit(_go);
        }
        else
        {
            PopcornPrefab _go = queue_popcornPool.Dequeue();
            _go.transform.SetParent(arr_ballParent[1]);
            _go.transform.position = arr_spawnPos[UnityEngine.Random.Range(0, arr_spawnPos.Length)].position;
            _go.gameObject.SetActive(true);
        }
    }

    void ActiveMultiBall(int ballNum)
    {
        for (int i = 0; i < ballNum; i++)
        {
            ActiveBall();
        }
    }

    /// <summary>
    /// 게임 끝날 때 활성화 된 것들 다 비활성화 시키기
    /// </summary>
    void DisableActiveBall()
    {
        GameObject[] arr_activeBalls = new GameObject[arr_ballParent[1].childCount];
        for (int i = 0; i < arr_activeBalls.Length; i++)
        {
            arr_activeBalls[i] = arr_ballParent[1].GetChild(i).gameObject;
        }
        for (int i = 0; i < arr_activeBalls.Length; i++)
        {
            arr_activeBalls[i].transform.SetParent(arr_ballParent[0]);
            arr_activeBalls[i].SetActive(false);
        }
    }

    public override void ResultMiniGame(bool isSuccess)
    {
        DisableActiveBall();
        base.ResultMiniGame(isSuccess);
    }
    public override void ClearStage()
    {
        DisableActiveBall();
        base.ClearStage();
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();

        popcornUI.GameUIChangePopcornText(currentPopcorn, maxPopcorn);
        //최대 팝콘 개수만큼 팝콘 뿌리기
        ActiveMultiBall(maxPopcorn);
       
    }

    public override void EndMiniGame()
    {
        StopAllCoroutines();

        base.EndMiniGame();
    }

}
