using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MagnetStage : StageManager
{
    public Texture[] arr_blackTex;

    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas2("Episode5-1");

        stageTitle = "Magnet";
        stageSubTitle = "번쩍번쩍 번개\n 붙어서 떨어지지않아";

    }

    public override void PlayCutscene(TimelineAsset _timeline)
    {
        base.PlayCutscene(_timeline);
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

    /// <summary>
    /// 해당 캐릭터 깜빡이는 효과    /// </summary>
    /// <param name="_header"></param>
    public void SignalLightningFlickering(Character _header)
    {
        StartCoroutine(LightningHitEffect(_header));
    }

    /// <summary>
    /// 번개맞고 번쩍이는 효과
    /// </summary>
    /// <param name="_flickerTime"></param>
    /// <returns></returns>
    protected IEnumerator LightningHitEffect(Character _header, float _flickerTime = 5)
    {
        Texture _main = _header.arr_skin[0].material.mainTexture;
        for (int j = 0; j < _flickerTime; j++)
        {
            for (int i = 0; i < _header.arr_skin.Length; i++)
            {
                _header.arr_skin[i].material.mainTexture = arr_blackTex[i];
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < _header.arr_skin.Length; i++)
            {
                _header.arr_skin[i].material.mainTexture = _main;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
