using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnly;

/// <summary>
/// 2스테이지의 스테이지 관리자.
/// </summary>
public class CircleStage : StageManager
{
    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_EPISODE_21);
        bgm_stage = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_MAIN);

        stageTitle = "Circle";
        stageSubTitle = "동글동글 \n 뭐든지 동그래!";

    }

    /// <summary>
    /// 컷 씬 종료, 상호작용 시작
    /// 변수 초기화    /// </summary>
    public override void EndCutScene()
    {
        Debug.Log(gameObject.name + "EndCutscene: " + currentTimeline);
        m_director.Stop();
        // gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(false);
        gameMgr.statGame = GameStatus.GAME;

        for (int i = 0; i < arr_header.Length; i++)
        {
            arr_header[i].StopAllCoroutines();
            if (currentTimeline < list_endPos.Count)
            {
                arr_header[i].transform.position = list_endPos[currentTimeline].GetChild(i).transform.position;
                arr_header[i].transform.rotation = list_endPos[currentTimeline].GetChild(i).transform.rotation;
            }
            //arr_header[i].transform.localRotation = Quaternion.Euler(arr_header[i].transform.localRotation.eulerAngles + arr_header[i].transform.GetChild(0).localRotation.eulerAngles);
            arr_header[i].transform.GetChild(0).localRotation = Quaternion.identity;
            //arr_header[i].transform.localPosition += arr_header[i].transform.GetChild(0).localPosition *0.5f;
            arr_header[i].transform.GetChild(0).localPosition = Vector3.zero;

            arr_header[i].SetAnim(0);
        }


        currentDialog = 0;
        currentTimeline++;

        StartInteraction();
    }

    public override void StartStage()
    {
        base.StartStage();

        StartCoroutine(gameMgr.LateFunc(() => gameMgr.uiMgr.StageTitleFade(stageTitle, stageSubTitle), 2f));

        AstarScan(Vector3.zero, new Vector3(10, 0, 10));
    }


    public override void EndStage()
    {
        base.EndStage();
    }


}
