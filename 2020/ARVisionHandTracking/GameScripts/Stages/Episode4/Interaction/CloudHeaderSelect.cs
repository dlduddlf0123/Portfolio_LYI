using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CloudHeaderSelect : ARObjectSelect
{
    public TimelineAsset[] arr_timeline;

    public string titleString;
    public float waitTime = 1f;
    public int correctNum = 4;

    private void Start()
    {
        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            int j = i;
            arr_arSelectables[j].action.AddListener(() =>
            {
                PlayTimeline(arr_timeline[j]);
                HeaderSelectStop();
                arr_arSelectables[j].gameObject.SetActive(false);
            });
            list_guidePosition.Add(arr_arSelectables[j].transform.position);
        }

        HeaderSelectStop();
    }

    /// <summary>
    /// 선택한 오브젝트 액션 실행
    /// </summary>
    /// <returns></returns>
    public override IEnumerator DisableObject()
    {
        gameMgr.uiMgr.worldCanvas.StopTimer();

        lastSelect.SetTriggerAnimation(3);
        lastSelect.isSelected = false;
        lastSelect.isTimer = false;

        isDisable = true;
        yield return new WaitForSeconds(0.5f);
        isDisable = false;

        lastSelect.action.Invoke();
        lastSelect = null;
        // gameObject.SetActive(false);
    }

    /// <summary>
    /// 상호작용 시작 시 캐릭터 위치 정렬    /// </summary>
    public void HeaderSelectReady()
    {
        PlayGuideParticle();

        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].GetComponent<Collider>().enabled = true;
            gameMgr.currentEpisode.currentStage.arr_header[i].transform.position = new Vector3(
                arr_arSelectables[i].transform.position.x,
                gameMgr.currentEpisode.currentStage.arr_header[i].transform.position.y,
                arr_arSelectables[i].transform.position.z);
        }
    }
    public void HeaderSelectStop()
    {
        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            arr_arSelectables[i].GetComponent<Collider>().enabled = false;
        }
    }
    public override void PlayGuideParticle()
    {
        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            if (list_guideParticle[i] != null)
            {
                list_guideParticle[i].transform.position = list_guidePosition[i];
                list_guideParticle[i].Play();
                list_guideParticle[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
        }
    }

    //버튼동작
    /// <summary>
    /// 클릭한 오브젝트에 해당하는 타임라인 재생
    /// 각 타임라인 끝에 트리거 달아서 다음 동작 시킬 것 - EndCutScene 비슷한 거
    /// </summary>
    public void PlayTimeline(TimelineAsset _timeline)
    {
        Debug.Log("CloudPlayTimeline: " + _timeline.name);
        StopGuideParticle();
        gameMgr.currentEpisode.currentStage.PlayCutscene(_timeline);
    }

    /// <summary>
    /// 타임라인 플레이 이후 상호작용 끝나는지 체크 후 선택 상태로 돌아가기
    /// </summary>
   public void CheckTimelineEndSignal()
    {
        gameMgr.currentEpisode.currentStage.m_director.Stop();

        Debug.Log("CheckTimelineEndSignal");

        gameMgr.statGame = GameStatus.GAME;

        gameMgr.currentEpisode.currentStage.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < arr_arSelectables.Length; i++)
        {
            if (arr_arSelectables[i].gameObject.activeSelf == false)
            {
                list_guidePosition[i] = Vector3.zero;
                list_guideParticle[i] = null;
            }
        }

        int endCount = 0;
        for (int i = 0; i < list_guideParticle.Count; i++)
        {
            if ( list_guideParticle[i] == null)
            {
                endCount++;
            }
        }
        if (list_guideParticle.Count == endCount)
        {
            EndInteraction();
        }
        else
        {
            HeaderSelectReady();
        }
    }

    public override void StartInteraction()
    {
        MakeGuideParticle();
        base.StartInteraction();
        HeaderSelectReady();
    }

    public override void EndInteraction()
    {
        base.EndInteraction();
    }

}
