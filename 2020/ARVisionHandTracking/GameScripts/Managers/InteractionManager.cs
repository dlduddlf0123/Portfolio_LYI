using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 상호작용의 시작과 종료를 관리한다.
/// 이 클래스를 상속받아 실제 상호작용 플레이를 작동시킨다.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public GameManager gameMgr;

    public string[] arr_LoopDialog;

    public  List<ParticleSystem> list_guideParticle = new List<ParticleSystem>();
    protected List<Vector3> list_guidePosition = new List<Vector3>();

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        DoAwake();
    }

    protected virtual void DoAwake() { }


    public IEnumerator DialogTime()
    {
       
        gameMgr.currentEpisode.currentStage.header.headerCanvas.gameObject.SetActive(true);
        while (gameMgr.statGame == GameStatus.GAME &&
            arr_LoopDialog.Length > 0)
        {
            int rand = Random.Range(0, arr_LoopDialog.Length);
            gameMgr.currentEpisode.currentStage.header.headerCanvas.ShowText(arr_LoopDialog[rand]);
            yield return new WaitForSeconds(15f);
        }
    }

    public void MakeGuideParticle()
    {
        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            if (list_guideParticle.Count < list_guidePosition.Count)
            {
                list_guideParticle.Add(Instantiate(gameMgr.b_stagePrefab.LoadAsset<GameObject>("InteractionEffect")).GetComponent<ParticleSystem>());
                list_guideParticle[i].transform.parent = transform;
            }
        }
    }

    public virtual void PlayGuideParticle()
    {
        MakeGuideParticle();

        for (int i = 0; i < list_guideParticle.Count; i++)
        {
            list_guideParticle[i].transform.position = list_guidePosition[i];
            list_guideParticle[i].Play();
            list_guideParticle[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
    }

    public void StopGuideParticle()
    {
        if (list_guideParticle.Count == 0)
        {
            return;
        }
        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            if (list_guideParticle[i] != null)
                list_guideParticle[i].Stop();
        }
    }

    public virtual void StartInteraction()
    {
        Debug.Log(gameObject.name + "StartInteraction");

        gameMgr.statGame = GameStatus.GAME;
        gameMgr.handCtrl.handColl.SetActive(true);
        gameMgr.handCtrl.handFollower.SetActive(true);

        if (arr_LoopDialog.Length != 0)
        {
            StartCoroutine(DialogTime());
        }
    }

    public virtual void EndInteraction()
    {
        Debug.Log(gameObject.name + "EndInteraction");
        gameMgr.statGame = GameStatus.CUTSCENE;
        //gameMgr.handCtrl.handColl.SetActive(false);

        //gameMgr.uiMgr.fadeCanvas.StartFade(()=> gameMgr.currentEpisode.currentStage.PlayCutscene(gameMgr.currentEpisode.currentStage.currentCutscene));
        gameMgr.currentEpisode.currentStage.PlayCutscene(gameMgr.currentEpisode.currentStage.currentTimeline);
        gameMgr.currentEpisode.currentStage.currentInteraction++;

    }
}
