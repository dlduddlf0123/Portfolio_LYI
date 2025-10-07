using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TirePunkInteract : InteractionManager
{
    Collider m_coll;
    public GameObject bandAid;

    protected override void DoAwake()
    {
        m_coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 0)
        {
            gameMgr.uiMgr.worldCanvas.StartTimer(m_coll.transform.position, gameMgr.waitTime, () =>
            {
                gameMgr.currentEpisode.currentStage.EndInteraction();
            });
            bandAid.SetActive(true);

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
            bandAid.SetActive(false);
        }
    }


    public override void StartInteraction()
    {
        base.StartInteraction();

        gameMgr.currentEpisode.currentStage.SetInteractionPos();

        transform.position = gameMgr.currentEpisode.currentStage.list_endPos[1].position;
        transform.rotation = Quaternion.Euler(0, -90, 0);
        transform.GetChild(0).localRotation = Quaternion.identity;
        transform.GetChild(0).gameObject.SetActive(true);

        m_coll.enabled = true;
        bandAid.gameObject.SetActive(false);

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
