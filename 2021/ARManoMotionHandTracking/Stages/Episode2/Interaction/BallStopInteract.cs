using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStopInteract : InteractionManager
{
    Collider m_coll;


    protected override void DoAwake()
    {
        m_coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION)
        {
            gameMgr.uiMgr.worldCanvas.StartTimer(m_coll.transform.position, gameMgr.waitTime, () =>
            {
                gameMgr.currentEpisode.currentStage.EndInteraction();
            });

            StopGuideParticle();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION)
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();
            PlayGuideParticle();
        }
    }


    public override void StartInteraction()
    {
        base.StartInteraction();

        gameMgr.currentEpisode.currentStage.SetInteractionPos();

        m_coll.enabled = true;

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        m_coll.enabled = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();
        StopGuideParticle();

        base.EndInteraction();
    }


}
