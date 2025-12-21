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

    GrassActor[] arr_grassActor;

    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_EPISODE_12);

        stageTitle = "Episode01";
        stageSubTitle = "Flower Land";

        grassEffect = (GrassTrailEffect)grassPhysics.postProcessProfile.postProcesses[0];
        grassEffect.recoverySpeed = 0.1f;

        arr_grassActor = GetComponentsInChildren<GrassActor>();
    }

    private void Start()
    {
        for (int i = 0; i < arr_grassActor.Length; i++)
        {
            arr_grassActor[i].radius *= gameMgr.uiMgr.stageSize;
        }
        arr_header[1].gameObject.SetActive(false);
    }

    public void SignalCameraLook()
    {
        //  float posZ1, posZ2;

        //posZ1 = arr_header[0].transform.GetChild(0).localPosition.z * 0.45f * gameMgr.uiMgr.stageSize;
        //posZ2 = arr_header[1].transform.GetChild(0).localPosition.z * 0.45f * gameMgr.uiMgr.stageSize;

        //arr_header[0].transform.localPosition -= Vector3.right * posZ1;
        //arr_header[1].transform.localPosition += Vector3.right * posZ2;

        for (int i = 0; i < arr_header.Length; i++)
        {
            if (arr_header[i].gameObject.activeSelf)
            {
                arr_header[i].TurnLook(gameMgr.arMainCamera.transform);
            }
        }
    }

    public void SignalCameraLookBack()
    {
        for (int i = 0; i < arr_header.Length; i++)
        {
            if (arr_header[i].gameObject.activeSelf)
            {
                arr_header[i].TurnBack();
            }
        }
    }

    /// <summary>
    /// 컷 씬 종료, 상호작용 시작
    /// 변수 초기화    /// </summary>
    public override void EndCutScene()
    {
        Debug.Log(gameObject.name + "EndCutscene: " + currentTimeline);
        m_director.Stop();
        // gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(false);
        gameMgr.statGame = GameStatus.INTERACTION;

        arr_header[0].StopAllCoroutines();

        StartCoroutine(SmoothTransformReset(0, list_endPos[currentTimeline]));
        arr_header[0].transform.GetChild(0).localPosition = Vector3.zero;
        arr_header[0].transform.GetChild(0).localRotation = Quaternion.identity;

        arr_header[0].SetAnim(0);

        for (int i = 1; i < arr_header.Length; i++)
        {
            arr_header[i].StopAllCoroutines();

            StartCoroutine(SmoothRotReset(i, arr_header[i].transform.GetChild(0).rotation));
            arr_header[i].transform.GetChild(0).localRotation = Quaternion.identity;

            StartCoroutine(SmoothPosReset(i, arr_header[i].transform.position + arr_header[i].transform.GetChild(0).localPosition));
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

        AstarScan(Vector3.zero,Vector3.up);
    }
    

    public override void EndStage()
    {
        base.EndStage();
    }

    public override void StartInteraction()
    {
        if (list_interaction.Count <= currentInteraction)
        {
            EndStage();
            return;
        }

        list_interaction[currentInteraction].gameObject.SetActive(true);
        list_interaction[currentInteraction].StartInteraction();
    }
    public override void EndInteraction()
    {
        list_interaction[currentInteraction].EndInteraction();
        // list_interaction[currentInteraction].gameObject.SetActive(false);
    }
}
