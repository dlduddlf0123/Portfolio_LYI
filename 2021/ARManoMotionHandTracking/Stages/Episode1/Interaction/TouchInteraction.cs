using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInteraction : InteractionManager
{
    Collider coll;

    public bool isOff = true;

    protected override void DoAwake()
    {
        coll = GetComponent<Collider>();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION)
        {
            gameMgr.uiMgr.worldCanvas.StartTimer(gameMgr.handCtrl.manoHandMove.transform.position + Vector3.up * 0.1f, gameMgr.waitTime, () =>
            {
                gameMgr.currentEpisode.currentStage.arr_header[0].Success("Thank you!");
                StartCoroutine(gameMgr.LateFunc(() => gameMgr.currentEpisode.currentStage.EndInteraction()));
                //GetComponent<Renderer>().enabled = true;
                if (transform.childCount > 0)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            });
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        coll.enabled = true;
        //transform.position = gameMgr.currentEpisode.currentStage.header.transform.GetChild(0).position + Vector3.up * 3f;
        ////손 높이 고정
        //gameMgr.handCtrl.isHandFix = true;
        //gameMgr.handCtrl.handCollHeight = gameMgr.currentEpisode.currentStage.header.transform.position.y + 3f;
    }

    public override void EndInteraction()
    {
        //gameMgr.handCtrl.isHandFix = false;
        coll.enabled = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();

        base.EndInteraction();

        gameObject.SetActive(isOff);
    }
}
