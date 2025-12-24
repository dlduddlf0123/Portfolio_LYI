using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DegururuMiniGame : MiniGame
{
    [Header("Son")]

    PhysicsProjectile physics = new PhysicsProjectile();

    //originalPrefab
    public GameObject[] prefab_ball;

    Queue<GameObject> queue_ballPool = new Queue<GameObject>();
    public Transform[] arr_spawnPos;
    public Transform[] arr_ballParent;
    public Transform[] arr_endPos;

    Coroutine currentCoroutine = null;


    public float spawnTime = 1f;

    protected override void DoAwake()
    {
        stageNum = PlayerPrefs.GetInt("DegururuStage", 1);
       list__csv_stage = gameMgr.csvMgr.ReadCSVDatas("Degururu");
    }

    public override void GameInit()
    {
        base.GameInit();

        //PlayerPrefs.SetInt("DegururuStage", stageNum);

        limitTime = System.Convert.ToInt32(list__csv_stage[stageNum][0]);
        spawnTime = (float)System.Convert.ToDouble(list__csv_stage[stageNum][1]);

        Debug.Log("Stage:" + stageNum + "/LimitTime:" + limitTime + "/SpawnTime:" + spawnTime);
    }


    void BallInit(GameObject go)
    {
        go.transform.SetParent(arr_ballParent[0]);
        queue_ballPool.Enqueue(go);
        go.GetComponent<Rigidbody>().velocity = Vector3.zero;
        go.GetComponent<Rigidbody>().isKinematic = false;
        go.GetComponent<DegururuBallPrefab>().isFall = false;
        go.gameObject.SetActive(false);
    }

    void BallDamage(GameObject go)
    {
        LoseLife();

        BallInit(go);
    }

    /// <summary>
    /// 공 뿌리기
    /// </summary>
    void SpamBall()
    {
        StartCoroutine(SpawnBall());
    }

    void ActiveBall()
    {
        if (queue_ballPool.Count == 0)
        {
            GameObject go = Instantiate(prefab_ball[Random.Range(0, 6)]);

            int rand = Random.Range(0, arr_spawnPos.Length - 1);
            go.transform.position = arr_spawnPos[rand].position;
            go.transform.SetParent(arr_ballParent[1]);

            go.GetComponent<DegururuBallPrefab>().endPos = arr_endPos[rand].position;

            go.GetComponent<DegururuBallPrefab>().onFall = () => BallDamage(go);
            go.GetComponent<DegururuBallPrefab>().onGoal = () => BallInit(go);
        }
        else
        {
            GameObject go = queue_ballPool.Dequeue();

            int rand = Random.Range(0, arr_spawnPos.Length - 1);
            go.transform.position = arr_spawnPos[rand].position;
            go.transform.SetParent(arr_ballParent[1]);

            go.GetComponent<DegururuBallPrefab>().endPos = arr_endPos[rand].position;

            go.gameObject.SetActive(true);
        }
    }

    IEnumerator SpawnBall()
    {
        while (gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            ActiveBall();
            yield return new WaitForSeconds(spawnTime);
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
    public override IEnumerator LimitTimer(int limitTime)
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
            ResultMiniGame(true);
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


        //list_guidePosition.Add(transform.position);
        //PlayGuideParticle();

        //gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.BACK);

        SpamBall();

    }

    public override void EndMiniGame()
    {
        StopAllCoroutines();

        base.EndMiniGame();
    }

}
