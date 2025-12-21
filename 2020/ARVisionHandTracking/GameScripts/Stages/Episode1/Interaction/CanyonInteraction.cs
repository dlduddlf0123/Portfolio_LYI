using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class CanyonInteraction : InteractionManager
{
    public AROcclusionManager occlusionMgr;
    public Transform Bridge;
    public Transform endPoint;
    Vector3 respawnPoint;

    Collider m_coll;
    public float fallingTime = 2f;

    float speed = 0.5f;

    bool isMove = false;
    public bool isOff = true;

    protected override void DoAwake()
    {
        occlusionMgr = Camera.main.gameObject.GetComponent<AROcclusionManager>();
        m_coll = GetComponent<Collider>();

        for (int i = 0; i < Bridge.childCount; i++)
        {
            Bridge.GetChild(i).GetComponent<InvisibleBridge>().bridgeNum = i;
        }

        respawnPoint = gameMgr.currentEpisode.currentStage.list_endPos[3].position;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Header") &&
            gameMgr.statGame == GameStatus.GAME &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 3)
        {
            StopAllCoroutines();

                collision.gameObject.transform.position = respawnPoint;
                collision.gameObject.GetComponent<Kanto>().StartBlink();
                StartCoroutine(DialogTime());

                isMove = false;
                gameMgr.currentEpisode.currentStage.header.SetAnim(0);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();
        }
    }
    public void StartMove()
    {
        if (!isMove &&
            gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(MoveToEnd());
            isMove = true;
        }
    }
    IEnumerator MoveToEnd()
    {
        Character header = gameMgr.currentEpisode.currentStage.arr_header[0];
        header.SetAnim(1); 
        while (header.transform.localPosition.z < endPoint.localPosition.z)
        {
            header.GetComponent<CharacterController>().SimpleMove(transform.forward *speed);
            yield return new WaitForSeconds(0.01f);
        }
        EndInteraction();
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
       // gameMgr.currentEpisode.currentStage.header.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;

        for (int i = 0; i < Bridge.childCount; i++)
        {
            list_guidePosition.Add(Bridge.GetChild(i).position + Vector3.up * 0.5f);
        }

        occlusionMgr.enabled = false;
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        //gameMgr.handCtrl.isHandFix = false;
        occlusionMgr.enabled = true;
        m_coll.enabled = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();
        StopGuideParticle();
        StopAllCoroutines();
        gameMgr.currentEpisode.currentStage.arr_header[0].StopAllCoroutines();

        base.EndInteraction();

        gameObject.SetActive(isOff);
    }
}

