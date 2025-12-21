using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnly;
using RogoDigital.Lipsync;
using UnityEngine.Timeline;

/// <summary>
/// 4스테이지의 스테이지 관리자.
/// </summary>
public class DesertStage : StageManager
{
    public Character[] arr_cloud;

    public List<List<AudioClip>> list__voice = new List<List<AudioClip>>();
    public ColorChanger[] arr_colorChanger;

    protected override void DoAwake()
    {
        //씬에서 사용될 대사 호출
        list__timelineDialog = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_EPISODE_41);
        //bgm_stage = gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_EPISODE_MAIN);


        stageTitle = "Episode04";
        stageSubTitle = "Colorful Cloud Land";

        arr_cloud = transform.GetChild(2).GetChild(0).GetComponentsInChildren<Character>();
    }

    public override void PlayCutscene(int _sceneNum)
    {
        base.PlayCutscene(_sceneNum);

        transform.GetChild(0).gameObject.SetActive(false);
    }
    public override void PlayCutscene()
    {
        base.PlayCutscene();

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void PlayCutscene(TimelineAsset _timeline)
    {
        base.PlayCutscene(_timeline);
        transform.GetChild(0).gameObject.SetActive(false);
    }
    /// <summary>
    /// 컷 씬 종료, 상호작용 시작
    /// 변수 초기화    /// </summary>
    public  override void EndCutScene()
    {
        Debug.Log(gameObject.name + "EndCutscene: " + currentTimeline);
        m_director.Stop();
        // gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(false);
        gameMgr.statGame = GameStatus.INTERACTION;

        for (int i = 0; i < arr_header.Length; i++)
        {
            arr_header[i].StopAllCoroutines();
            arr_header[i].transform.localRotation = arr_header[i].transform.GetChild(0).rotation;
            arr_header[i].transform.GetChild(0).rotation = Quaternion.identity;
            arr_header[i].transform.localPosition += arr_header[i].transform.GetChild(0).localPosition;
            arr_header[i].transform.GetChild(0).localPosition = Vector3.zero;

            arr_header[i].SetAnim(0);
        }
        transform.GetChild(0).gameObject.SetActive(true);


        currentDialog = 0;
        currentTimeline++;

        StartInteraction();
    }

    public override void StartStage()
    {
        base.StartStage();

        StartCoroutine(gameMgr.LateFunc(() => gameMgr.uiMgr.UIStageTitleFade(stageTitle, stageSubTitle), 2f));

        AstarScan(Vector3.zero,new Vector3(10,0,10));
    }


    public override void EndStage()
    {
        base.EndStage();
    }


    public void SignalChangeColor(int _changeNum)
    {
        arr_colorChanger[_changeNum].StartCoroutine(arr_colorChanger[_changeNum].ChangeColor());
        arr_colorChanger[_changeNum+1].StartCoroutine(arr_colorChanger[_changeNum+1].ChangeColor());
    }

    public void ChangeColorSignal(ColorChanger _character)
    {
        _character.StartCoroutine(_character.ChangeColor());
    }


    public void AbsorbColor(ColorChanger _absorber, ColorChanger _victim)
    {
        _absorber.StartCoroutine(_absorber.ChangeColor());
        _victim.StartCoroutine(_victim.ChangeColor());
    }

    public void CheckMoveEnd()
    {
        int moveEndCount = 0;
        for (int i = 0; i < arr_cloud.Length; i++)
        {
            if (!arr_cloud[i].isNavMove)
                moveEndCount++;
        }

        if (moveEndCount >= arr_cloud.Length-1)
        {
            m_director.Play();
        }
    }

    public void DoinkMove(Transform _tr)
    {
        //m_director.Pause();

        arr_header[0].SetAnim(2);
        arr_header[0].MoveCharacter(_tr.position, 3,()=>
        {
            arr_header[0].transform.localPosition = new Vector3(-10, 0, 9);
            arr_header[0].transform.localRotation = Quaternion.Euler(0, -90, 0);
        });
    }

    public void MassCloudMove1(Transform _tr)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 1; i < arr_cloud.Length; i++)
        {
            sum += arr_cloud[i].transform.position;
        }
        sum /= arr_cloud.Length-1;


        for (int i = 1; i < arr_cloud.Length; i++)
        {
            arr_cloud[i].MoveCharacter(arr_cloud[i].transform.position + (_tr.position - sum), 3);
        }
    }

    public void MassCloudMove2(Transform _tr)
    {
       // m_director.Pause();

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < arr_cloud.Length; i++)
        {
            sum += arr_cloud[i].transform.position;
        }
        sum /= arr_cloud.Length;

        
        for (int i = 0; i < arr_cloud.Length; i++)
        {
            arr_cloud[i].MoveCharacter(arr_cloud[i].transform.position + (_tr.position - sum),3, () =>
            {
            //    CheckMoveEnd();
            });
        }

    }
    //캐릭터 바라보기
    public void MassCloudLookAt()
    {
        for (int i = 0; i < arr_cloud.Length-1; i++)
        {
            arr_cloud[i].GetComponent<Cloud>().LookRotate(arr_cloud[arr_cloud.Length - 1].transform);
        }
    }

}
