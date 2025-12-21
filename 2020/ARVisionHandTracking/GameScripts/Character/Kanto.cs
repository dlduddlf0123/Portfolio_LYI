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

    public bool isGrabbable = false;


    protected override void DoAwake()
    {
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
    }

    private void OnTriggerEnter(Collider collision)
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
                gameMgr.currentEpisode.currentStage.currentDialog--;
                Failure("만지지마!");

                StartCoroutine(Touch());
            }
            if (isGrabbable)
            {
                gameMgr.uiMgr.worldCanvas.StartTimer(transform.position, gameMgr.waitTime, () => transform.parent = collision.gameObject.transform);
            }
        }
        if (isNavMove && collision.gameObject.CompareTag("End"))
        {
            isNavMove = false;
            collision.enabled = false;
        }
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
