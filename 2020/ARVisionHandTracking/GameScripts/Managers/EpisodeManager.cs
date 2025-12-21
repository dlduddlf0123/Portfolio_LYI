using System.Collections;
using System.Collections.Generic;
using RogoDigital.Lipsync;
using UnityEngine;


/// <summary>
/// 총 6개 존재, 각 에피소드의 스테이지 들을 보관하는 오브젝트, 현재 스테이지 역시 보유/// </summary>
public class EpisodeManager : MonoBehaviour
{
    GameManager gameMgr;
    public StageManager[] arr_stage;
    public StageManager currentStage;

    public LipSyncData[] arr_voice { get; set; }

    public int episodeNum = 0;
    public int currentStageNum = 0;

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        arr_voice = gameMgr.b_arr_voice[3].LoadAllAssets<LipSyncData>();
    }

    /// <summary>
    /// 특정 스테이지 활성화  
    /// </summary>
    public void ActiveStage(int _stageNum)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            arr_stage[i].episodeMgr = this;

            if (i != _stageNum)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        currentStage = arr_stage[_stageNum];
        currentStageNum = _stageNum;

        gameMgr.uiMgr.fadeCanvas.StartFade(() =>
        {
            currentStage.gameObject.SetActive(true);
            currentStage.StartStage();
            gameMgr.uiMgr.shadowPlane.SetActive(false);
        });
    }

    /// <summary>
    /// 스테이지 클리어 시 바로 다음스테이지로 이동
    /// </summary>
    public void NextStage()
    {
        if (arr_stage.Length-1 <= currentStageNum)
        {
            currentStage.EndStage();
            return;
        }
        currentStageNum++;
        gameMgr.uiMgr.shadowPlane.SetActive(false);
        ActiveStage(currentStageNum);
    }

    public void EndStage()
    {
        gameMgr.statGame = GameStatus.SELECT;
        gameMgr.uiMgr.stageSelect.SetActive(true);
        Destroy(gameObject);
    }
}
