using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameFireWood : MiniGameManager
{
    public FireWoodSpawner spawner;
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
        typeMiniGame = MiniGameType.FIREWOOD;
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
        spawner.Spawn();
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

        if (gameScore > PlayerPrefs.GetInt("FireWoodHighScore", 0))
        {
            PlayerPrefs.SetInt("FireWoodHighScore", gameScore);
            stageUI.result_text_highScore.text = "HighScore: " + gameScore.ToString();
        }

        //점수에 따른 보상 부여
        if (gameScore > gradeCut[0])
        {
            //금
            //stageMgr.interactHeader.LikeChange(10);
            stageUI.ChangeGrade(0);
        }
        else if (gameScore > gradeCut[1])
        {
            //은
            // stageMgr.interactHeader.LikeChange(20);
            stageUI.ChangeGrade(1);
        }
        else if (gameScore > gradeCut[2])
        {
            //동
            //stageMgr.interactHeader.LikeChange(30);
            stageUI.ChangeGrade(2);
        }
        else
        {
            //실패 or 실망
            // stageMgr.interactHeader.LikeChange(-10);
            stageUI.ChangeGrade(3);
        }

    }
    #endregion
}
