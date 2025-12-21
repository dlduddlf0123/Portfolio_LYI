using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ep0_HandLiftInteraction : InteractionManager
{
    Collider m_collider;

    public GameObject handShadow;

    float maxHeight = 2f;
    float maxXZPos = 3f;

    bool isLift = false;

    Coroutine currentCoroutine = null;

    protected override void DoAwake()
    {
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;

        list_guidePosition.Add(stageMgr.arr_header[0].transform.position + Vector3.up *2* gameMgr.uiMgr.stageSize);
        e_handIcon = HandIcon.FRONT;

        handShadow.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") &&
            gameMgr.handCtrl.manoHandMove.handSide == HandSide.Palmside &&
            !isLift)
        {
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(LiftHeader());
            }
        }
    }


    /// <summary>
    /// 손으로 캐릭터 들어올리기
    /// 좌표는 모두 상대 좌표로 해야 한다./// </summary>
    /// <returns></returns>
    IEnumerator LiftHeader()
    {
        Debug.Log("StartLift");
        isLift = true;
        gameMgr.handCtrl.manoHandMove.arr_handFollwer[0].ToggleHandEffect(true);

        while (isLift &&
            gameMgr.handCtrl.handFollower.transform.position.y < maxHeight &&
              Vector3.Distance(
            new Vector3(gameMgr.handCtrl.handFollower.transform.position.x, 0, gameMgr.handCtrl.handFollower.transform.position.z),
            new Vector3(transform.position.x, 0, transform.position.z)) < maxXZPos)
        {
            if (gameMgr.handCtrl.handFollower.transform.position.y > stageMgr.transform.position.y)
            {
                stageMgr.arr_header[0].transform.position = new Vector3(
                    stageMgr.arr_header[0].transform.position.x,
                    gameMgr.handCtrl.handFollower.transform.position.y,
                    stageMgr.arr_header[0].transform.position.z);
                handShadow.transform.position = stageMgr.arr_header[0].transform.position -  Vector3.up * 1.5f;
            }

            yield return new WaitForSeconds(0.01f);
        }

        Debug.Log("EndLift");
        isLift = false;
        gameMgr.handCtrl.manoHandMove.arr_handFollwer[0].ToggleHandEffect(false);

        if (stageMgr.arr_header[0].transform.position.y > maxHeight - 1.5f)
        {
            EndInteraction();
        }
        else
        {
            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_FAILURE);
        }

        currentCoroutine = null;
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        PlayGuideParticle();

        gameMgr.handCtrl.ToggleHandOcclusion(false);

        m_collider.enabled = true;
        handShadow.SetActive(true);

    }

    public override void EndInteraction()
    {
        StopGuideParticle();

        gameMgr.handCtrl.ToggleHandOcclusion(gameMgr.uiMgr.ui_setting.setting_toggle_handOcclusion.isOn);

        m_collider.enabled = false;
        handShadow.SetActive(false);

        base.EndInteraction();
    }

}
