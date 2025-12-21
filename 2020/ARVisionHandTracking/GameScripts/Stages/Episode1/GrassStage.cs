using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogoDigital.Lipsync;
using ShadedTechnology.GrassPhysics;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using ReadOnly;

/// <summary>
/// 1스테이지의 스테이지 관리자.
/// </summary>
public class GrassStage : StageManager
{
    public GrassPhysicsArea grassPhysics;
    public GrassTrailEffect grassEffect { get; set; }

    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_EPISODE_12);
        bgm_stage = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_MAIN);

        stageTitle = "Grass";
        stageSubTitle = "광활한 풀숲 \n 모든 것이 풀에 묻혀있다";

        grassEffect = (GrassTrailEffect)grassPhysics.postProcessProfile.postProcesses[0];
        grassEffect.recoverySpeed = 0.1f;
    }

    private void Start()
    {
        arr_header[1].gameObject.SetActive(false);
    }

    public void SignalCameraLook()
    {
        float posZ1, posZ2;

       posZ1 = arr_header[0].transform.GetChild(0).localPosition.z * 2;
         posZ2 = arr_header[1].transform.GetChild(0).localPosition.z * 2;
        StartCoroutine(gameMgr.LateFrameFunc(() =>
        {

            arr_header[0].transform.localPosition -= Vector3.right * posZ1;
            arr_header[1].transform.localPosition += Vector3.right * posZ2;

            arr_header[0].TurnLook(Camera.main.transform);
            arr_header[1].TurnLook(Camera.main.transform);

        }));
    }

    public void SignalCameraLookBack()
    {
        arr_header[0].TurnBack();
        arr_header[1].TurnBack();
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

        header.StopAllCoroutines();
        header.transform.position = list_endPos[currentTimeline].position;
        header.transform.rotation = list_endPos[currentTimeline].rotation;
        header.transform.GetChild(0).localPosition = Vector3.zero;
        header.transform.GetChild(0).localRotation = Quaternion.identity;

        header.SetAnim(0);

        for (int i = 1; i < arr_header.Length; i++)
        {
            arr_header[i].StopAllCoroutines();
            arr_header[i].transform.rotation = arr_header[i].transform.GetChild(0).rotation;
            arr_header[i].transform.GetChild(0).localRotation = Quaternion.identity;
            arr_header[i].transform.position += arr_header[i].transform.GetChild(0).localPosition;
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

        AstarScan(Vector3.zero,Vector3.up);
    }
    

    public override void EndStage()
    {
        base.EndStage();
    }

}
