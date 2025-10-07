using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ep0_PinchInteraction : InteractionManager
{
    RayInteractObject m_ray;
    Character header;
    Collider m_collider;

    public bool isEnd = false;

    protected override void DoAwake()
    {
        m_ray = GetComponent<RayInteractObject>();
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;

        e_handIcon = HandIcon.PINCH;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Header") &&
            !isEnd)
        {
            EndInteraction();
        }
    }

    public override IEnumerator DialogWaitTime()
    {
        Kanto kanto = header.GetComponent<Kanto>();
        gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.gameObject.SetActive(true);
        while (gameMgr.statGame == GameStatus.INTERACTION &&
            arr_LoopDialog.Length > 0)
        {
            yield return new WaitForSeconds(5f);
            if (kanto.isDrag)
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
        header = gameMgr.currentEpisode.currentStage.arr_header[0];

        base.StartInteraction();

        header.GetComponent<Kanto>().isGrabbable = true;
        header.GetComponent<CharacterController>().enabled = false;

        if (header.GetComponent<Rigidbody>() == null)
        {
            Rigidbody _rb = header.gameObject.AddComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        header.transform.GetChild(1).gameObject.SetActive(true);

        list_guidePosition.Add(header.transform.position + Vector3.up * 2 * gameMgr.uiMgr.stageSize);
        list_guidePosition.Add(transform.position);

        PlayGuideParticle();

        m_collider.enabled = true;
        isEnd = false;
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();
        header.StopAllCoroutines();

        m_collider.enabled = false;
        isEnd = true;
        header.GetComponent<Kanto>().isGrabbable = false;
        header.StopAllCoroutines();
        header.GetComponent<Kanto>().isDrag = false;
        header.GetComponent<Rigidbody>().useGravity = true;
        header.transform.GetChild(1).gameObject.SetActive(false);
        gameMgr.handCtrl.manoHandMove.arr_handFollwer[1].ToggleHandEffect(false);

        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);
        header.Success();

        StopGuideParticle();

        StartCoroutine(gameMgr.LateFunc(() => base.EndInteraction(), 3f));
    }
}
