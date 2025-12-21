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

    public List<ParticleSystem> list_guideParticle = new List<ParticleSystem>();
    public Transform particlePool;

    public int episodeNum = 0;
    public int currentStageNum = 0;

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        if (episodeNum < gameMgr.b_arr_voice.Length)
        {
            arr_voice = gameMgr.b_arr_voice[episodeNum].LoadAllAssets<LipSyncData>();
        }
        particlePool = transform.GetChild(transform.childCount-1);
    }

    /// <summary>
    /// 특정 스테이지 활성화  
    /// </summary>
    public void ActiveStage(int _stageNum)
    {
        for (int i = 0; i < arr_stage.Length; i++)
        {
            arr_stage[i].episodeMgr = this;

            if (i != _stageNum)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        currentStage = arr_stage[_stageNum];
        currentStageNum = _stageNum;

        currentStage.gameObject.SetActive(true);
    }

    /// <summary>
    /// 스테이지 클리어 시 바로 다음스테이지로 이동
    /// </summary>
    public void NextStage()
    {
        currentStage.EndStage();

        currentStageNum++;
//        gameMgr.uiMgr.shadowPlane.SetActive(false);
        ActiveStage(currentStageNum);
        gameMgr.uiMgr.fadeCanvas.StartFade(() =>
        {
            currentStage.gameObject.SetActive(true);
            currentStage.StartStage();
        });
    }

    /// <summary>
    /// 스테이지 엔딩 이후 동작
    /// </summary>
    public void EndEpisode()
    {
        gameMgr.currentEpisode = null;
        gameMgr.uiMgr.endingCreditCanvas.gameObject.SetActive(false);
        gameMgr.uiMgr.ui_game.game_btn_skip.gameObject.SetActive(false);

        gameMgr.uiMgr.SetUIActive(UIWindow.TITLE, false); //UI 변경

        Destroy(gameObject);
    }
}
