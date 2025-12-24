using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 상호작용의 시작과 종료를 관리한다.
/// 이 클래스를 상속받아 실제 상호작용 플레이를 작동시킨다.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    [Header("Interaction Manager")]
    public GameManager gameMgr;
    public EpisodeManager episodeMgr;
    public StageManager stageMgr;

    public string[] arr_LoopDialog;

    public List<ParticleSystem> list_guideParticle;
    protected List<Vector3> list_guidePosition = new List<Vector3>();

    public HandIcon e_handIcon;

    [Space(5)]
    [Header("Child Interaction")]
    public TMPro.TextMeshPro txt_education;


    private void Awake()
    {
        gameMgr = GameManager.Instance;
        episodeMgr = gameMgr.currentEpisode;
        stageMgr = episodeMgr.currentStage;

        list_guideParticle = episodeMgr.list_guideParticle;

        if (txt_education != null)
        {
            txt_education.gameObject.SetActive(false);
        }

        DoAwake();
    }

    protected virtual void DoAwake() { }


    public virtual IEnumerator DialogWaitTime()
    {
        gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.gameObject.SetActive(true);
        while (gameMgr.statGame == GameStatus.GAMEPLAY &&
            arr_LoopDialog.Length > 0)
        {
            //  int rand = Random.Range(0, arr_LoopDialog.Length);
            for (int i = 0; i < arr_LoopDialog.Length; i++)
            {
                yield return new WaitForSeconds(5f);
                gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.ShowText(arr_LoopDialog[i], 5);
                yield return new WaitForSeconds(5f);
            }
        }
    }

    public void MakeGuideParticle()
    {
        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            if (list_guideParticle.Count < list_guidePosition.Count)
            {
                list_guideParticle.Add(Instantiate(gameMgr.b_stagePrefab.LoadAsset<GameObject>("InteractionEffect")).GetComponent<ParticleSystem>());
            }
            list_guideParticle[i].transform.parent = episodeMgr.particlePool;
            list_guideParticle[i].transform.localScale *= gameMgr.uiMgr.stageSize;

        }
    }

    public virtual void PlayGuideParticle()
    {
        MakeGuideParticle();

        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            list_guideParticle[i].transform.position = list_guidePosition[i];
            list_guideParticle[i].transform.localScale = Vector3.one * 0.3f;
            list_guideParticle[i].Play();
            list_guideParticle[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
    }

    public virtual void StopGuideParticle()
    {
        if (list_guideParticle.Count == 0)
        {
            return;
        }
        for (int i = 0; i < list_guideParticle.Count; i++)
        {
            if (list_guideParticle[i] != null)
                list_guideParticle[i].Stop();
        }
    }

    public virtual void StartInteraction()
    {
        Debug.Log(gameObject.name + "StartInteraction");

        gameMgr.statGame = GameStatus.GAMEPLAY;
        //gameMgr.handCtrl.manoHandMove.gameObject.SetActive(true);
        //gameMgr.handCtrl.handFollower.gameObject.SetActive(true);

        if (arr_LoopDialog.Length != 0)
        {
            StartCoroutine(DialogWaitTime());
        }

        gameMgr.uiMgr.UIGameTimelineFrameToggle(false);
        gameMgr.uiMgr.ui_game.ChangeHandIcon(e_handIcon);
    }

    public virtual void EndInteraction()
    {
        Debug.Log(gameObject.name + "EndInteraction");
        //gameMgr.handCtrl.handColl.SetActive(false);
        //gameMgr.handCtrl.handFollower.ToggleHandEffect(false);
        //gameMgr.handCtrl.manoHandMove.HandRayToggle(false);

        if (txt_education != null)
        {
            txt_education.gameObject.SetActive(false);
        }

        //gameMgr.uiMgr.fadeCanvas.StartFade(()=> gameMgr.currentEpisode.currentStage.PlayCutscene(gameMgr.currentEpisode.currentStage.currentCutscene));
        gameMgr.currentEpisode.currentStage.PlayCutscene(gameMgr.currentEpisode.currentStage.currentTimeline);
        gameMgr.currentEpisode.currentStage.currentInteraction++;

        gameMgr.uiMgr.UIGameTimelineFrameToggle(true);
        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.NONE);
    }
}