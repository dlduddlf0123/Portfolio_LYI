using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainInteraction : InteractionManager
{
    Collider coll;
    public ParticleSystem rainParticle;
    public GameObject umbrella;

    protected override void DoAwake()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.GAME &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 0)
        {
            gameMgr.uiMgr.worldCanvas.StartTimer(gameMgr.handCtrl.handColl.transform.position + Vector3.up * 0.1f, gameMgr.waitTime, () =>
                 {
                     gameMgr.currentEpisode.currentStage.EndInteraction();
                 });
            umbrella.SetActive(true);
            umbrella.transform.position = gameMgr.handCtrl.handColl.transform.position + Vector3.up * 2f;
            StopGuideParticle();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.GAME)
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();
            PlayGuideParticle();
            umbrella.SetActive(false);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        coll.enabled = true;
        //transform.position = gameMgr.currentEpisode.currentStage.header.transform.GetChild(0).position + Vector3.up * 3f;
        //손 높이 고정
        //gameMgr.handCtrl.isHandFix = true;
        //  gameMgr.handCtrl.handCollHeight = gameMgr.currentEpisode.currentStage.header.transform.position.y + 3f;

        //칸토 동작 변경
        gameMgr.currentEpisode.currentStage.header.ChangeIdleAnimation(4);
        umbrella.gameObject.SetActive(false);

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();
        gameMgr.currentEpisode.currentStage.header.SetAnim(0);
        coll.enabled = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();
        rainParticle.Stop();
        StopGuideParticle();
        base.EndInteraction();
    } 


}
