using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCooking : MiniGameManager
{
    Collider selectCollider;

    protected override void DoAwake()
    {
        selectCollider = GetComponent<Collider>();


    }

    #region 상속받은 함수들(게임 루틴 관련)
    /// <summary>
    /// 게임모드 시작!(버튼)
    /// </summary>
    public override void PlayStart()
    {
        typeMiniGame = MiniGameType.COOK;
        selectCollider.enabled = false;

        base.PlayStart();

    }

    public override void PlayEnd()
    {
        base.PlayEnd();

        selectCollider.enabled = true;
    }


    /// <summary>
    /// 카운트다운 후 게임플레이 시작
    /// </summary>
    public override void GameStart()
    {
        Debug.Log(this.gameObject.name + " GameStart!!");
        base.GameStart();

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

        if (gameScore > PlayerPrefs.GetInt("CookHighScore", 0))
        {
            PlayerPrefs.SetInt("CookHighScore", gameScore);
            stageUI.result_text_highScore.text = "HighScore: " + gameScore.ToString();
        }

        //점수에 따른 보상 부여
        if (gameScore > 3000)
        {
            //동
            //stageMgr.interactHeader.LikeChange(10);
        }
        else if (gameScore > 5000)
        {
            //은
            // stageMgr.interactHeader.LikeChange(20);
        }
        else if (gameScore > 10000)
        {
            //금
            //stageMgr.interactHeader.LikeChange(30);
        }
        else
        {
            //실패 or 실망
            // stageMgr.interactHeader.LikeChange(-10);
        }

    }
    #endregion
}
