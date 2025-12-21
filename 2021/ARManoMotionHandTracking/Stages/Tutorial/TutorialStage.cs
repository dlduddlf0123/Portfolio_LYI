using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnly;
using UnityEngine.Animations;


public class TutorialStage : StageManager
{
    public GameObject handIconHighlightCanvas;
    public GameObject depthMaskWall;

    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_TUTORIAL);

        bgm_stage = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_MAIN);

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

    public void SignalHighlightHandIcon(bool _isActive)
    {
        handIconHighlightCanvas.SetActive(_isActive);

        if (currentInteraction == 5)
        {
            gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.SCREEN);
        }
        else
        {
            gameMgr.uiMgr.ui_game.ChangeHandIcon(list_interaction[currentInteraction].e_handIcon);
        }
    }

    public override void StartStage()
    {
        base.StartStage();

       // AstarScan();
        StartCoroutine(gameMgr.LateFunc(() =>
        {
            gameMgr.uiMgr.UIStageTitleFade(stageTitle, stageSubTitle);
        }, 2f));
    }

    public override void EndStage()
    {
        Debug.Log(gameObject.name + "EndStage");
        currentTimeline = 0;
        currentDialog = 0;
        currentInteraction = 0;

        gameMgr.uiMgr.fadeCanvas.StartFade(() =>
        {
            gameMgr.isTutorial = true;
            PlayerPrefs.SetInt("isTutorial", System.Convert.ToInt32(gameMgr.isTutorial));

            gameMgr.currentEpisode = null;

            gameMgr.SetStage(gameMgr.uiMgr.select_episodeNum);
            gameMgr.GameStart();
            Destroy(episodeMgr.gameObject);
        });
    }

}