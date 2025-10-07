using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MagnetStage : StageManager
{
    public Material[] arr_blackMat;
    public int flikerTime = 5;
    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas2("Episode5-1");

        stageTitle = "Episode05";
        stageSubTitle = "Magnet Land";

    }

    public override void PlayCutscene(TimelineAsset _timeline)
    {
        base.PlayCutscene(_timeline);
    }

    /// <summary>
    /// 컷 씬 종료, 상호작용 시작
    /// 변수 초기화
    /// /// </summary>
    public override void EndCutScene()
    {
        Debug.Log(gameObject.name + "EndCutscene: " + currentTimeline);
        m_director.Stop();
        // gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(false);
        gameMgr.statGame = GameStatus.INTERACTION;


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

        StartCoroutine(gameMgr.LateFunc(() => gameMgr.uiMgr.UIStageTitleFade(stageTitle, stageSubTitle), 2f));

        AstarScan(Vector3.zero, new Vector3(10, 0, 10));
    }


    public override void EndStage()
    {
        base.EndStage();
    }

    //카메라 바라보기
    public void SignalLookCamera(Character _char)
    {
        _char.ChildTurnLook(gameMgr.arMainCamera.transform);
    }

    public void SignalCameraLookBack(Character  _char)
    {
        _char.ChildTurnBack();
    }

    //회상장면 하얗게 페이드
    public void SignalWhiteFade()
    {
        gameMgr.uiMgr.fadeCanvas.SetImageColor(new Color(1, 1, 1,1));
        gameMgr.uiMgr.fadeCanvas.StartFade(()=> { gameMgr.uiMgr.fadeCanvas.SetImageColor(new Color(0, 0, 0,1)); }, 4, 0.3f);
    }

    #region Signal Flikering

    /// <summary>
    /// 해당 캐릭터 깜빡이는 효과    /// </summary>
    /// <param name="_headerNum"></param>
    public void SignalLightningFlickering(int _headerNum)
    {
        StartCoroutine(LightningHitEffect(_headerNum, flikerTime));
    }

    public void SignalFlikeringKanto()
    {
        SignalLightningFlickering(0);
    }
    public void SignalFlikeringZino()
    {
        SignalLightningFlickering(1);
    }
    public void SignalFlikeringOodada()
    {
        SignalLightningFlickering(2);
    }
    public void SignalFlikeringCoco()
    {
        SignalLightningFlickering(3);
    }
    public void SignalFlikeringDoink()
    {
        SignalLightningFlickering(4);
    }
    public void SignalFlikeringTena()
    {
        SignalLightningFlickering(5);
    }
    #endregion

    /// <summary>
    /// 번개맞고 번쩍이는 효과
    /// </summary>
    /// <param name="_flickerTime"></param>
    /// <returns></returns>
    protected IEnumerator LightningHitEffect(int _headerNum, float _flickerTime = 5)
    {
        Material _main = arr_header[_headerNum].arr_skin[0].material;
        for (int j = 0; j < _flickerTime; j++)
        {
            for (int i = 0; i < arr_header[_headerNum].arr_skin.Length; i++)
            {
                arr_header[_headerNum].arr_skin[i].material = arr_blackMat[_headerNum];
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < arr_header[_headerNum].arr_skin.Length; i++)
            {
                arr_header[_headerNum].arr_skin[i].material = _main;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }


}
