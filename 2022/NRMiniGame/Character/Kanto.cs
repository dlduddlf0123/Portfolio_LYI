using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;

public class Kanto : Character
{
    //public int loopTime = 10;
    //public float talkTime = 0.3f;
    //public bool isTalking = false;

    public bool isTouchable = false;
    public bool isGrabbable = false;
    public bool isDrag = false;
    Vector3 startVec;

    Coroutine currentCoroutine = null;

    protected override void DoAwake()
    {
        characterColor = new Color32(255, 179, 0, 255);
       // arr_texture[0] = gameMgr.b_sprite.LoadAsset<Texture>("Giraffe");
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            arr_feet[i].footSound = gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_WALK02);
        }

        m_sharedMat = arr_skin[0].sharedMaterial;
        m_sharedMat.SetFloat("_ChangeHeight", 1);
        //m_sharedMat.mainTexture = arr_texture[0];

        // StartCoroutine(ChangeColor(true));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isGrabbable &&
            !isDrag &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 7)
        {
            transform.position = gameMgr.currentEpisode.currentStage.list_endPos[7].position;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 10)
        {
            //gameMgr.uiMgr.worldCanvas.StartTimer(transform.position + Vector3.up * 0.1f, 2f, () => Thankyou());
            if (gameMgr.statGame == GameStatus.CHARACTERSELECT &&
                isTouched == false)
            {
                //isTouched = true;
                //m_animator.applyRootMotion = true;
                //gameMgr.currentEpisode.currentStage.m_director.Pause();
                //gameMgr.currentEpisode.currentStage.currentDialog--;
                //Failure("만지지마!");

                //StartCoroutine(Touch());
            }

            if (gameMgr.statGame == GameStatus.GAMEPLAY &&
                isTouched == false &&
                collision.CompareTag("Index") &&
                !isGrabbable &&
                !gameMgr.handCtrlL.NRHandMove.isPinch &&
                isTouchable)
            {
                isTouched = true;
                PlayJustTrigger(2);
                if (headerCanvas == null)
                {
                    return;
                }
                headerCanvas.gameObject.SetActive(true);
                headerCanvas.ShowText("Don't touch me!");

                currentCoroutine = StartCoroutine(gameMgr.LateFunc(() => isTouched = false, 2));
            }

            if (isGrabbable &&
                gameMgr.handCtrlL.NRHandMove.isPinch &&
                collision.CompareTag("Index") &&
                !isDrag)
            {
                startVec = transform.position;
                isDrag = true;

                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(HandGrab());
            }
        }

        if (isNavMove && collision.gameObject.CompareTag("End"))
        {
            isNavMove = false;
            collision.enabled = false;
        }
    }

    IEnumerator HandGrab()
    {
        GetComponent<Rigidbody>().useGravity = false;
        gameMgr.handCtrlL.NRHandMove.arr_handFollwer[1].ToggleHandEffect(true);
        while (gameMgr.handCtrlL.NRHandMove.isPinch)
        {
            Vector3 half =
            gameMgr.handCtrlL.NRHandMove.finger_thumb.transform.position -
            gameMgr.handCtrlL.NRHandMove.finger_index.transform.position ;
            transform.position = gameMgr.handCtrlL.NRHandMove.finger_index.transform.position + half * 0.5f;
            yield return new WaitForSeconds(0.01f);
        }
        isDrag = false;
        GetComponent<Rigidbody>().useGravity = true;
        gameMgr.handCtrlL.NRHandMove.arr_handFollwer[1].ToggleHandEffect(false);

        currentCoroutine = StartCoroutine(gameMgr.LateFunc(() =>
        {
            if (isGrabbable)
            {
                transform.position = startVec;
                StartCoroutine(HitEffect());
            }
        }, 2));
        
    }
    //public void Talking(int _times)
    //{
    //    if (isTalking)
    //    {
    //        return;
    //    }
    //    StartCoroutine(Talk(_times));
    //}
    //public IEnumerator Talk(int _times)
    //{
    //    isTalking = true;
    //    int t = 0;
    //    while (t < _times )
    //    {
    //        int rand = Random.Range(0, 6);
    //        yield return new WaitForSeconds(talkTime * 0.3f);
    //        m_skin.SetBlendShapeWeight(rand, 50);
    //        yield return new WaitForSeconds(talkTime);
    //        m_skin.SetBlendShapeWeight(rand, 0);
    //        t++;
    //    }

    //    isTalking = false;
    //}

    //public void ChangeMouth(float _wave)
    //{
    //    if (_wave == 0)
    //    {
    //        m_skin.SetBlendShapeWeight(0, 0);
    //        return;
    //    }
    //    //float a = Mathf.Abs(_wave) * 1000;
    //    //if (a < 50)
    //    //{
    //    //    m_skin.SetBlendShapeWeight(0, 0);
    //    //    return;
    //    //}
    //    m_skin.SetBlendShapeWeight(0, _wave);
    //}


    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        gameMgr.uiMgr.worldCanvas.StopTimer();
    //    }
    //}

}
