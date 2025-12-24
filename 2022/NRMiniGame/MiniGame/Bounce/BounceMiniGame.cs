using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
/// <summary>
/// 손으로 대가리들을 튕겨서 건너편으로 건너가게 하는 게임
/// 중간에 장애물 등장
/// 튕기는 방식 AddForce? 그냥 물리 머테리얼? Bounce 
/// 시작할 때 캐릭터 점프는 애드포스로
/// </summary>
public class BounceMiniGame : MiniGame
{
    [Header("Son")]
    //originalPrefab
     BounceBallPrefab[] prefab_ball; //캐릭터 공들

    public GameObject[] arr_platform; //0:start, 1:end
    public Transform startTr;

    Coroutine currentCoroutine = null;

    public int ballNum = 0; //현재 공 번호(캐릭터) 0~5
    public int goalPosition = 0;
    public int obstacles = 0;

    protected override void DoAwake()
    {
        stageNum = PlayerPrefs.GetInt("BounceStage", 1);
        list__csv_stage = gameMgr.csvMgr.ReadCSVDatas("Bounce");

        prefab_ball = new BounceBallPrefab[6];
        for (int i = 0; i < prefab_ball.Length; i++)
        {
            prefab_ball[i] = transform.GetChild(0).GetChild(i).GetComponent<BounceBallPrefab>();
            prefab_ball[i].transform.position = startTr.position;
            prefab_ball[i].transform.rotation = Quaternion.Euler(Vector3.up * 90); //오른쪽을 향하게

            prefab_ball[i].onDamage = LoseLife;
            prefab_ball[i].onEnd = BallArrived;

            prefab_ball[i].gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 게임 시작 시 호출
    /// 현재 스테이지의 제한시간, 골 위치값, 장애물 위치값 적용, 공 변경
    /// </summary>
    public override void GameInit()
    {
        base.GameInit();

        //PlayerPrefs.SetInt("BounceStage", stageNum);

        limitTime = System.Convert.ToInt32(list__csv_stage[stageNum][0]);
        goalPosition = System.Convert.ToInt32(list__csv_stage[stageNum][1]);
        obstacles = System.Convert.ToInt32(list__csv_stage[stageNum][2]);

        ballNum = (stageNum % 6);

        Debug.Log("Stage:" + stageNum + "/LimitTime:" + limitTime +
            "/GoalPosition:" + goalPosition + "/Obstacles:" + obstacles);

        arr_platform[0].SetActive(true); //시작 플랫폼 보여주기

        ChangeEndPosition();
        ChangeObstacle();

        BallInit();
    }


    /// <summary>
    /// 현재 공 번호에 따른 공 변경
    /// 공 상태 초기화
    /// </summary>
    void BallInit()
    {
        for (int i = 0; i < prefab_ball.Length; i++)
        {
            prefab_ball[i].gameObject.SetActive(false);
        }
        prefab_ball[ballNum].transform.position = startTr.position;
        prefab_ball[ballNum].transform.rotation = Quaternion.Euler(Vector3.up * 90); //오른쪽을 향하게
        prefab_ball[ballNum].GetComponent<Rigidbody>().velocity = Vector3.zero;
        prefab_ball[ballNum].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        prefab_ball[ballNum].gameObject.SetActive(true);
    }

    /// <summary>
    /// 포지션 번호에 따른 도착 플랫폼 위치, 크기 난이도 조정
    /// </summary>
    void ChangeEndPosition()
    {
        switch (goalPosition)
        {
            case 0:
                arr_platform[1].transform.localPosition = new Vector3(5, -0.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(3, 1, 3);
                break;
            case 1:
                arr_platform[1].transform.localPosition = new Vector3(5, -2.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(3, 1, 3);
                break;
            case 2:
                arr_platform[1].transform.localPosition = new Vector3(5, 2.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(3, 1, 3);
                break;
            case 3:
                arr_platform[1].transform.localPosition = new Vector3(5, -0.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(2, 1, 2);
                break;
            case 4:
                arr_platform[1].transform.localPosition = new Vector3(5, -0.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(2, 1, 2);
                break;
            case 5:
                arr_platform[1].transform.localPosition = new Vector3(5, -2.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(2, 1, 2);
                break;
            case 6:
                arr_platform[1].transform.localPosition = new Vector3(5, 2.5f,0);
                arr_platform[1].transform.localScale = new Vector3(2, 1, 2);
                break;
            case 7:
                arr_platform[1].transform.localPosition = new Vector3(5, -0.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(1, 1, 1);
                break;
            case 8:
                arr_platform[1].transform.localPosition = new Vector3(5, 2.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(1, 1, 1);
                break;
            case 9:
                arr_platform[1].transform.localPosition = new Vector3(5, -0.5f, 0);
                arr_platform[1].transform.localScale = new Vector3(1, 1, 1);
                break;
            default:
                break;
        }

        Debug.Log("GoalPosition[" + goalPosition + "]: " + arr_platform[1].transform.localPosition);
    }

    /// <summary>
    /// 장애물 번호에 따른 장애물 위치, 크기, 갯수, 움직임 등 변경
    /// </summary>
    void ChangeObstacle()
    {
        switch (obstacles)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            default:
                break;
        }
        Debug.Log("Obstacles[" + obstacles + "]: ");
    }

    /// <summary>
    /// 피격 모션, 위로 떠오르기
    /// 바닥에 떨어졌을 때 호출
    /// </summary>
    public override void LoseLife()
    {
        base.LoseLife();

        arr_platform[0].SetActive(true);

        BallInit();
    }

    /// <summary>
    /// 공이 반대편에 도착한 경우 호출
    /// 다음 공 준비, 추가 점수
    /// </summary>
    public void BallArrived()
    {
        ResultMiniGame(true);
    }

    public override void ResultMiniGame(bool isSuccess)
    {
        prefab_ball[ballNum].gameObject.SetActive(false);
        base.ResultMiniGame(isSuccess);
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