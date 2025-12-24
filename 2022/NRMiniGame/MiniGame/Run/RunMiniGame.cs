using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using System;

/// <summary>
/// 달리기 게임
/// 캐릭터 조작부
/// 맵 생성부
/// </summary>
public class RunMiniGame : MiniGame
{
    [Header("Son")]
    RunMiniGameUI runUI;


    public Transform startTr;

    Coroutine currentCoroutine = null;

    protected override void DoAwake()
    {

        runUI = miniGameUI.GetComponent<RunMiniGameUI>();

        stageNum = PlayerPrefs.GetInt("RunStage", 1);
        list__csv_stage = gameMgr.csvMgr.ReadCSVDatas("Run");
    }

    public override void GameInit()
    {
        base.GameInit();

        //PlayerPrefs.SetInt("RunStage", stageNum);
        limitTime = Convert.ToInt32(list__csv_stage[stageNum][0]);

        Debug.Log("Stage:" + stageNum + "/LimitTime:" + limitTime);
    }


    void BallInit(PopcornPrefab _go)
    {
        _go.CornInit();

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



    public override void ResultMiniGame(bool isSuccess)
    {
        base.ResultMiniGame(isSuccess);
    }
    public override void ClearStage()
    {
        base.ClearStage();
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();

    }

    public override void EndMiniGame()
    {
        StopAllCoroutines();

        base.EndMiniGame();
    }

}
