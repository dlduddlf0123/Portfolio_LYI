using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cloud : Character
{
    public bool isGrabbable = false;

    protected override void DoAwake()
    {

    }
    private void Start()
    {
        m_sharedMat = arr_skin[0].material;
        m_sharedMat.SetFloat("_ChangeHeight", 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    //gameMgr.uiMgr.worldCanvas.StartTimer(transform.position + Vector3.up * 0.1f, 2f, () => Thankyou());
        //    if (gameMgr.statGame == GameStatus.CUTSCENE &&
        //        isTouched == false)
        //    {
        //        isTouched = true;
        //        m_animator.applyRootMotion = true;
        //        gameMgr.currentEpisode.currentStage.m_director.Pause();

        //        Failure("만지지마!");

        //        StartCoroutine(Touch());
        //    }
        //    if (isGrabbable)
        //    {
        //        gameMgr.uiMgr.worldCanvas.StartTimer(transform.position, 1f, () => transform.parent = collision.gameObject.transform);
        //    }
        //}
    }

    private void OnTriggerEnter(Collider coll)
    {
        //if (isNavMove && coll.gameObject.CompareTag("End"))
        //{
        //    isNavMove = false;
        //    coll.enabled = false;
        //}
    }

    public void LookRotate(Transform _tr)
    {
        if (currentAI != null)
        {
            StopCoroutine(currentAI);
        }
        currentAI = StartCoroutine(LookAt(_tr));
    }
    protected IEnumerator LookAt(Transform _tr)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_tr.position - transform.position), t);
            yield return new WaitForSeconds(0.01f);
        }
    }
}