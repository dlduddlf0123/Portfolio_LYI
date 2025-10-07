using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 공 날리기 상호작용
/// 특정 지점 도착 시 조준 시작, 5초 뒤 각도대로 발사
/// 골 링으로 들어가면 성공, 빗나가면 실패
/// 실패 시 공이 원래 위치로 돌아가 다시 도전
/// </summary>
public class BallPushInteract : InteractionManager
{
    public Rigidbody ball;
    public LineRenderer aimLine;

    Collider aimColl;

    public float fireForce = 100f;
    float aimDistance = 0.0f;

    protected override void DoAwake()
    {
        aimColl = GetComponent<Collider>();
        aimColl.enabled = false;

        aimLine.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION)
        {
            //gameMgr.uiMgr.worldCanvas.StartTimer(aimColl.transform.position, gameMgr.waitTime, () =>
            //{
            //    gameMgr.currentEpisode.currentStage.EndInteraction();
            //});

            aimColl.enabled = false;

            aimLine.enabled = true;
            aimLine.SetPosition(0, this.transform.position);

            StopGuideParticle();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION)
        {
            //gameMgr.uiMgr.worldCanvas.StopTimer();
        }
    }

    void FireReady()
    {
        aimColl.enabled = true;
        aimLine.enabled = false;

        PlayGuideParticle();
    }

    IEnumerator BallAim()
    {
        float t = 0f;
        while (t < 5)
        {
            t += 0.01f;

            //Line Update
            aimLine.SetPosition(1, gameMgr.handCtrl.handFollower.transform.position);
            aimDistance = Vector3.Distance(transform.position, gameMgr.handCtrl.handFollower.transform.position);

            yield return new WaitForSeconds(0.01f);
        }

        //Fire
        BallFire();
    }


    void BallFire()
    {
        aimLine.enabled = false;
        StartCoroutine(FailCheck());

        ball.AddForce((transform.position - gameMgr.handCtrl.handFollower.transform.position).normalized * fireForce);

    }

    //발사 후 일정시간 경과 시 실패
    IEnumerator FailCheck()
    {
        yield return new WaitForSeconds(3f);
        GoalFailure();
    }

    public void GoalSuccess()
    {
        StopAllCoroutines();

        EndInteraction();
    }

    void GoalFailure()
    {
        ball.transform.localPosition = Vector3.zero;
        FireReady();
    }


    public override void StartInteraction()
    {
        base.StartInteraction();

        gameMgr.currentEpisode.currentStage.SetInteractionPos();

        list_guidePosition.Add(transform.position);

        FireReady();
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        aimColl.enabled = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();
        StopGuideParticle();

        base.EndInteraction();
    }


}