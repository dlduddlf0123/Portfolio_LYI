using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornInteraction : InteractionManager
{
    int hp = 7;
    bool isHit = false;
    float hitTime = 1f;
    Coroutine currentCoroutine;
    protected override void DoAwake()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
           gameMgr.statGame == GameStatus.GAME &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 2)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine  = StartCoroutine(HitTimer());
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //gameMgr.uiMgr.worldCanvas.StopTimer();
            if (isHit)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                hp--;
                transform.GetChild(0).GetChild(hp).gameObject.SetActive(false);
                //이펙트, 사운드 넣을 것
                
                if (hp <= 0)
                {
                    EndInteraction();
                }
            }
        }
    }

    IEnumerator HitTimer()
    {
        isHit = true;
        yield return new WaitForSeconds(hitTime);

        isHit = false;
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
        }

        gameMgr.currentEpisode.currentStage.header.ChangeIdleAnimation(4);

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        //gameMgr.handCtrl.isHandFix = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();

        base.EndInteraction();

        gameObject.SetActive(false);
    }
}