using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


/// <summary>
/// 절벽에서 손으로 캐릭터 올려주는 상호작용
/// 집어서 절벽 위에 두면 다음 장면 이동
/// /// </summary>
public class CliffInteraction : InteractionManager
{
    Character header;
   public  PositionConstraint wallConst;

    public bool isEnd = false;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Header"))
    //    {
    //        if (!other.GetComponent<Kanto>().isDrag)
    //        {
    //            EndInteraction();
    //        }
    //    }
    //}


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

        wallConst.constraintActive = false;
        gameMgr.arPlaneMgr.GetComponent<PositionConstraint>().constraintActive = false;
        //gameMgr.currentEpisode.currentStage.GetComponent<RainStage>().cameraConstraint.constraintActive = false;

        header.GetComponent<Kanto>().isGrabbable = true;
        header.GetComponent<CharacterController>().enabled = false;

        Rigidbody _rb = header.gameObject.AddComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        header.transform.GetChild(1).gameObject.SetActive(true);

        list_guidePosition.Add(header.transform.position + Vector3.up);
        list_guidePosition.Add(transform.GetChild(0).position);

        PlayGuideParticle();
        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.PINCH);

        isEnd = false; 
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        gameMgr.handCtrl.manoHandMove.arr_handFollwer[1].ToggleHandEffect(false);
        isEnd = true;
        header.GetComponent<Kanto>().isGrabbable = false;
        header.StopAllCoroutines();


        header.transform.position =  transform.GetChild(0).position;
        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);
        header.Success();

        StopGuideParticle();

      StartCoroutine(  gameMgr.LateFunc(()=>base.EndInteraction(),3f));
    }

}

