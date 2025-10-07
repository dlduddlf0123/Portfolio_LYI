using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MiniGameType
{
    NONE = -1,
    BUBBLE,
    BASKET,
    FIREWOOD,
    COOK,
    DEFENSE,
    STAR,
}

public enum MiniGameState
{
    NONE,
    READY,
    PLAYING,
    RESULT,
}

/// <summary>
/// 방울 터트리기 놀이
/// 게임 플레이 관련 데이터 관리 클래스
/// </summary>
public class MiniGameManager : PlayManager
{
    public StageManager stageMgr;
    public StageUI stageUI;
    public Transform startTr;
    public Transform UiTr;

    public MiniGameState statMiniGame = MiniGameState.NONE;
    public MiniGameType typeMiniGame = MiniGameType.NONE;

    public int gameScore = 0;
    public int limitTime = 10;
    public int currentTime = 0;

    public int[] gradeCut = new int[3];

    protected override void DoAwake() { }

    /// <summary>
    /// 게임모드 시작!(버튼)
    /// </summary>
    public override void PlayStart()
    {
        //이동, 게임모드 변경
        if (gameMgr.statGame == GameState.MINIGAME) { return; }

        base.PlayStart();
        gameMgr.statGame = GameState.MINIGAME;
        stageMgr.currentMiniGame = typeMiniGame;

        //UI호출
        stageUI.StageUIInit(this);
        stageUI.gameObject.SetActive(true);

        stageMgr.PlayEnd();
        stageMgr.MovePlayer(startTr);

        Debug.Log(gameObject.name + " PlayStart");
        //UI호출
        //stageUI.StageUIInit(this);
    }

    /// <summary>
    /// 게임모드 종료!(버튼)
    /// </summary>
    public override void PlayEnd()
    {
        if (gameMgr.statGame == GameState.INTERACTION) { return; }

        base.PlayEnd();
        gameMgr.statGame = GameState.INTERACTION;
        stageMgr.currentMiniGame = MiniGameType.NONE;
        statMiniGame = MiniGameState.NONE;
        
        stageUI.StageUIInit(this);
        stageUI.gameObject.SetActive(false);

        stageMgr.PlayStart();
        stageMgr.MovePlayer(stageMgr.interactionTransform);
        
        Debug.Log(gameObject.name + " PlayEnd");
    }

    /// <summary>
    /// 카운트다운 후 게임플레이 시작
    /// </summary>
    public virtual void GameStart()
    {
        gameScore = 0;
        stageUI.game_text_score.text = gameScore.ToString();

    }

    /// <summary>
    /// 게임 종료, 결과창, 데이터 저장 등
    /// </summary>
    public virtual void GameOver()
    {


    }

    /// <summary>
    /// 게임 시작 전 시간 세기
    /// </summary>
    /// <returns></returns>
    public IEnumerator CountDown()
    {
        if (statMiniGame == MiniGameState.READY)
        {
            yield break;
        }
        statMiniGame = MiniGameState.READY;

        //카운트다운 UI 활성화
        stageUI.stage_tutorialUI.SetActive(false);
        stageUI.stage_countDownUI.SetActive(true);

        stageUI.countDown_img_count[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        stageUI.countDown_img_count[0].gameObject.SetActive(false);
        stageUI.countDown_img_count[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        stageUI.countDown_img_count[1].gameObject.SetActive(false);

        //게임 UI 활성화
        stageUI.stage_countDownUI.SetActive(false);
        stageUI.stage_gameUI.SetActive(true);

        //게임 시작
        statMiniGame = MiniGameState.PLAYING;
        stageUI.StartCoroutine(stageUI.GameTimer(this, limitTime));
        GameStart();
    }


    /// <summary>
    /// 스코어를 올려준다
    /// </summary>
    /// <param name="_point"></param>
    public void GetScore(int _point)
    {
        gameScore += _point;
        stageUI.game_text_score.text = "Score: " + gameScore.ToString();
    }

}
