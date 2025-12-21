using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyInteraction : InteractionManager
{
    Character header;

    public Transform endPos;
    public float speed = 1f;

    bool isHeaderOn = false;

    protected override void DoAwake()
    {
        header = gameMgr.currentEpisode.currentStage.arr_header[0];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Header") &&
            gameMgr.statGame == GameStatus.INTERACTION &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 7)
        {
            if (header.GetComponent<Kanto>().isGrabbable)
            {
                header.GetComponent<Kanto>().isGrabbable = false;

                header.transform.SetParent(transform);
                header.transform.localPosition = Vector3.up * 0.1f;
                header.transform.localRotation = Quaternion.identity;

                isHeaderOn = true;
                StartCoroutine(Up());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            EndInteraction();
        }
    }

    IEnumerator Up()
    {
        while (Vector3.Distance(transform.localPosition, endPos.localPosition) > 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, gameMgr.handCtrl.handFollower.transform.position.y*2, transform.localPosition.z), Time.deltaTime * speed);
            yield return new WaitForSeconds(0.0167f);
        }

        EndInteraction();

    }

    public override IEnumerator DialogWaitTime()
    {
        gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.gameObject.SetActive(true);
        while (gameMgr.statGame == GameStatus.INTERACTION &&
            arr_LoopDialog.Length > 0)
        {
            yield return new WaitForSeconds(5f);
            if (isHeaderOn)
            {
                gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.ShowText(arr_LoopDialog[1], 5);
            }
            else
            {
                gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.ShowText(arr_LoopDialog[0], 5);
            }
            yield return new WaitForSeconds(5f);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        isHeaderOn = false;
        header.GetComponent<Kanto>().isGrabbable = true;
        transform.localPosition = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        header.GetComponent<Kanto>().isGrabbable = false;
        isHeaderOn = false;

        StopGuideParticle();
        base.EndInteraction();
    }

}
