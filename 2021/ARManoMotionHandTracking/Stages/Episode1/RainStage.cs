using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogoDigital.Lipsync;
using ReadOnly;
using UnityEngine.Animations;

/// <summary>
/// 1스테이지의 스테이지 관리자.
/// </summary>
public class RainStage : StageManager
{
    public PositionConstraint cameraConstraint;
    public GameObject depthMaskWall;

    public AudioSource m_rainAudio;
    Light mainLight;

    protected override void DoAwake()
    {
        cameraConstraint = gameMgr.arMainCamera.transform.parent.GetComponent<PositionConstraint>();

        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_EPISODE_11);

        bgm_stage = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_MAIN);

        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        list__currentLipData[i].Add(Resources.Load<LipSyncData>("LipSyncs/Stage1/TimeLine" + (i + 1) + "/LipSync" +( j + 1)));

        //    }

        //}

        stageTitle = "Episode01";
        stageSubTitle = "Flower Land";

        mainLight = gameMgr.mainLight;
    }

    private void Start()
    {
        currentDialog = 0;
    }

    public void SignalLookCamera()
    {
        arr_header[0].transform.position -= Vector3.right * arr_header[0].transform.GetChild(0).localPosition.z * 0.5f * gameMgr.uiMgr.stageSize;
        arr_header[0].transform.GetChild(0).localPosition = Vector3.zero;
        arr_header[0].TurnLook(gameMgr.arMainCamera.transform);
    }

    public void SignalCactusNavigation(Transform _tr)
    {
        arr_header[0].m_animator.applyRootMotion = true;
        m_director.Pause();
        arr_header[0].SetAnim(1);
        arr_header[0].MoveCharacter(_tr.position, 1, () =>
        {
            arr_header[0].m_animator.applyRootMotion = false;
            //gameMgr.currentEpisode.currentStage.arr_header[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            m_director.Play();
        });
    }
    public void SignalThornNavigation(Transform _tr)
    {
        arr_header[0].m_animator.applyRootMotion = true;
        m_director.Pause();
        arr_header[0].SetAnim(1);
        arr_header[0].MoveCharacter(_tr.position, 1, () =>
        {
            StartCoroutine(SmoothTransformReset(0, _tr));
            arr_header[0].m_animator.applyRootMotion = false;
            m_director.Play();
        });
    }

    public void ActiveDepthMaskWall(bool _isActive)
    {
        depthMaskWall.SetActive(_isActive);
    }

    public override void StartStage()
    {
        base.StartStage();

        ActiveDepthMaskWall(true);
        AstarScan();

        Vector3 _temp = gameMgr.arMainCamera.transform.position;
        cameraConstraint.constraintActive = true;
        depthMaskWall.GetComponent<PositionConstraint>().constraintActive = true;

        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = arr_header[0].transform.GetChild(0);
        source.weight = 1;
        cameraConstraint.SetSource(0, source);


        mainLight = gameMgr.mainLight;
        mainLight.intensity = 0;

        gameMgr.soundMgr.ChangeBGMAudioSource(m_rainAudio);


        StartCoroutine(gameMgr.LateFrameFunc(() => cameraConstraint.translationOffset = (_temp - gameMgr.arMainCamera.transform.position)));
        StartCoroutine(gameMgr.LateFunc(() =>
        {
            gameMgr.uiMgr.UIStageTitleFade(stageTitle, stageSubTitle);
        }, 2f));
    }

    public override void EndStage()
    {
        cameraConstraint.constraintActive = false;
        gameMgr.arMainCamera.transform.parent.localPosition = Vector3.zero;

        ActiveDepthMaskWall(false);
        base.EndStage();
    }

}
