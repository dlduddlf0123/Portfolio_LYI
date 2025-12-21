using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogoDigital.Lipsync;
using ReadOnly;

/// <summary>
/// 1스테이지의 스테이지 관리자.
/// </summary>
public class RainStage : StageManager
{

    protected override void DoAwake()
    {
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

        stageTitle = "Forest";
        stageSubTitle = "비내리는 숲 속 풍경 \n 멀리서 들려오는 칸토의 비명이 점점 가까워 진다";
    }

    public void SignalLookCamera()
    {
        arr_header[0].TurnLook(Camera.main.transform);
    }

    public void SignalCactusNavigation(Transform _tr)
    {
        header.m_animator.applyRootMotion = true;
        m_director.Pause();
        header.SetAnim(1);
        header.MoveCharacter(_tr.position, 1, () =>
        {
            header.m_animator.applyRootMotion = false;
            //gameMgr.currentEpisode.currentStage.arr_header[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            m_director.Play();
        });
    }
    public void SignalThornNavigation(Transform _tr)
    {
        header.m_animator.applyRootMotion = true;
        m_director.Pause();
        header.SetAnim(1);
        header.MoveCharacter(_tr.position, 1, () =>
        {
            header.transform.position = _tr.position;
            header.transform.rotation = _tr.rotation;
            header.m_animator.applyRootMotion = false;
            m_director.Play();
        });
    }

    public override void StartStage()
    {
        base.StartStage();

        AstarScan();
        StartCoroutine(gameMgr.LateFunc(() => gameMgr.uiMgr.StageTitleFade(stageTitle, stageSubTitle), 2f));
        
    }


    public override void EndStage()
    {
        base.EndStage();
    }

}
