using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornInteraction : InteractionManager
{
    public AudioClip sfx_thorn;
    public GameObject particle_thorn;

    int hp = 7;
    bool isHit = false;
    float hitTime = 1f;
    Coroutine currentCoroutine;

    protected override void DoAwake()
    {

    }


    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Index") &&
           gameMgr.statGame == GameStatus.INTERACTION &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 2 &&
            gameMgr.handCtrl.manoHandMove.isPinch)
        {
            hitTime = 1;
            if (!isHit)
            {
                currentCoroutine = StartCoroutine(HitTimer());
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Index") &&
            gameMgr.handCtrl.manoHandMove.isPinch)
        {
            //gameMgr.uiMgr.worldCanvas.StopTimer();
            if (isHit)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = null;
                }
                hp--;
                transform.GetChild(0).GetChild(hp).gameObject.SetActive(false);
                
                if (hp <= 0)
                {
                    EndInteraction();
                }
            }
            isHit = false;
        }
    }

    IEnumerator HitTimer()
    {
        isHit = true;
        float t = 0;
        Vector3 half;
        Vector3 startPos = transform.GetChild(0).GetChild(hp-1).position;

        //이펙트, 사운드 넣을 것
        gameMgr.soundMgr.PlaySfx(transform.position, sfx_thorn);
        gameMgr.PlayParticleEffect(transform.position, particle_thorn);
        while (t < hitTime && gameMgr.handCtrl.manoHandMove.isPinch)
        {
            t += 0.01f;

            half =
            gameMgr.handCtrl.manoHandMove.finger_thumb.transform.position -
            gameMgr.handCtrl.manoHandMove.finger_index.transform.position;

            transform.GetChild(0).GetChild(hp-1).gameObject.transform.position = gameMgr.handCtrl.manoHandMove.finger_index.transform.position + half * 0.5f;

            yield return new WaitForSeconds(0.01f);
        }

        transform.GetChild(0).GetChild(hp-1).position = startPos;
        isHit = false;
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
        }

        gameMgr.currentEpisode.currentStage.arr_header[0].ChangeIdleAnimation(4);

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();

        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.PINCH);
    }

    public override void EndInteraction()
    {
        //gameMgr.handCtrl.isHandFix = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();
        StopGuideParticle();
        base.EndInteraction();

        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);
        gameObject.SetActive(false);
    }
}