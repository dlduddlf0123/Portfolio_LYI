using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tena : Character
{
    public bool isGrabbable = false;

    protected override void DoAwake()
    {
        arr_skin = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            arr_feet[i].footSound = gameMgr.b_sound.LoadAsset<AudioClip>("jump_15");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //gameMgr.uiMgr.worldCanvas.StartTimer(transform.position + Vector3.up * 0.1f, 2f, () => Thankyou());
            if (gameMgr.statGame == GameStatus.CUTSCENE &&
                isTouched == false)
            {
                isTouched = true;
                m_animator.applyRootMotion = true;
                gameMgr.currentEpisode.currentStage.m_director.Pause();

                Failure("만지지마!");

                StartCoroutine(Touch());
            }
            if (isGrabbable)
            {
                gameMgr.uiMgr.worldCanvas.StartTimer(transform.position, gameMgr.waitTime, () => transform.parent = collision.gameObject.transform);
            }
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (isNavMove && coll.gameObject.CompareTag("End"))
        {
            isNavMove = false;
            coll.enabled = false;
        }
    }

}