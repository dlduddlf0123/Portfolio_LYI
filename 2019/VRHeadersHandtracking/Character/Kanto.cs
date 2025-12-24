using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kanto : Character
{

    //  public AnimationCurve curve = new AnimationCurve();
    protected override void DoAwake()
    {
        StatusInit();
        currentTarget = null;

        //Navigations
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.enabled = true;
        mNavAgent.isStopped = false;
    }


    void Start()
    {
        StatusInit();

        list__FriendDialog = gameMgr.dialogMgr.ReadDialogDatas("CSV/KantoFriendDialog");
        list__NormalDialog = gameMgr.dialogMgr.ReadDialogDatas("CSV/KantoNormalDialog");
        list__HateDialog = gameMgr.dialogMgr.ReadDialogDatas("CSV/KantoHateDialog");

        headerCanvas.list__CurrentDialog = list__HateDialog;
        AI_Move(0);
    }

    protected override void StatusInit()
    {
        Debug.Log(this.gameObject.name + "Init");
        Status.header = Headers.KANTO;
        Status.likeState = LikeState.HATE;
        statAnim = AnimState.IDLE;

        Status.moveSpeed = 2f;
        Status.minSpeed = 0.1f;
        Status.maxSpeed = 4f;

        Status.maxHp = 5;
        Status.hp = Status.maxHp;

        Status.likeMeter = 0;
        Status.hunger = 0;
        Status.favoriteFoodNum = 0;
        Status.likeTaste = 0;

        isAction = false;
        isDie = false;
        isHit = false;
        isGround = true;
        mAnimator.SetBool("isGround", true);
        isJudged = false;
        isStart = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        mNavAgent.isStopped = true;
        mNavAgent.speed = Status.moveSpeed;
        StatusInit();
    }

    /// <summary>
    /// 현재 호감도에 따라 상태 변경
    /// 호감도 상태가 변경되면 애니메이션 또한 변경
    /// 상태가 변경될 때 대사 출력
    /// </summary>
    //protected override void ChangeLikeState()
    //{
    //    Debug.Log((int)(Status.likeMeter / 20));
    //    switch ((int)(Status.likeMeter / 20))
    //    {
    //        case 0:
    //            statLike = LikeState.HATE;
    //            //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_hate") as RuntimeAnimatorController;
    //            headerCanvas.ShowText(5, 2);
    //            break;
    //        //case 1:
    //        //    statLike = LikeState.ALERT;
    //        //    break;
    //        case 2:
    //            statLike = LikeState.NORMAL;
    //            headerCanvas.ShowText(5, 0);
    //            //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_normal") as RuntimeAnimatorController;
    //            break;
    //        //case 3:
    //        //    statLike = LikeState.GOOD;
    //        //    break;
    //        case 4:
    //            statLike = LikeState.FRIEND;
    //            headerCanvas.ShowText(5, 1);
    //            //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_friend") as RuntimeAnimatorController;
    //            break;
    //    }
    //    Debug.Log(gameObject.name + "Like =" + statLike);
    //}
    protected override void ChangeLikeState()
    {
        LikeState _before = statLike;
        if (Status.likeMeter >= 100 && statLike != LikeState.FRIEND)
        {
            Status.likeMeter = 0;
            if ((int)statLike < 2)
            {
                statLike++;
            }
        }
        else if (Status.likeMeter <=0 && statLike != LikeState.HATE)
        {
            Status.likeMeter = 100;
            if (statLike > 0)
            {
                statLike--;
            }
        }

        int _random = Random.Range(0, 2);

        Debug.Log(statLike);
        switch (statLike)
        {
            case LikeState.HATE:
                headerCanvas.list__CurrentDialog = list__HateDialog;
                headerCanvas.ShowText(4, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_hate") as RuntimeAnimatorController;
                break;
            case LikeState.NORMAL:
                headerCanvas.list__CurrentDialog = list__NormalDialog;
                headerCanvas.ShowText((_before < statLike)?3:4, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_normal") as RuntimeAnimatorController;
                break;
            case LikeState.FRIEND:
                headerCanvas.list__CurrentDialog = list__FriendDialog;
                headerCanvas.ShowText(3, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_friend") as RuntimeAnimatorController;
                break;
        }
        //soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip("Sounds/SFX/level_up"));
        gameMgr.uiMgr.SetCommandUI((int)statLike);  //커맨드 UI 변경

        Debug.Log(gameObject.name + " Like =" + statLike);
    }

    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:run/3:stay/4:call/5:jump/d:clear</param>
    public override void AI_Move(int _type)
    {
        //게임 중단일 경우 하지않음
        if (gameMgr.statGame == GameState.GAMEOVER) { return; }
        if (currentAI != null) { StopCoroutine(currentAI); }

        //호감도 상태에 따른 랜덤 대사 출력하기
        int _random = Random.Range(0, 2);
        //AI 동작들
        switch (_type)
        {
            case 0: //배회
                headerCanvas.ShowText(0, _random);
                currentAI = StartCoroutine(PatrolMove());
                break;
            case 1: //맞았을 때
                currentAI = StartCoroutine(Hit());
                break;
            case 2: //달릴 때
                headerCanvas.ShowText(0, _random);
                currentAI = StartCoroutine(Run());
                break;
            case 3: //가만히 있을 때, 정지할 때
                currentAI = StartCoroutine(Idle());
                break;
            case 4: //부르기
                currentAI = StartCoroutine(Call());
                break;
            case 5: //점프
                currentAI = StartCoroutine(base.Jump());
                break;
            default: //깨끗해졌을 때
                currentAI = StartCoroutine(Clean());
                break;
        }
    }



    #region Animation Settings
    /// <summary>
    /// 애니메이션 변경 시 호출, 현재 AnimState에 따른 애니메이션 적용
    /// bool type Anims
    /// </summary>
    public override void SetAnim()
    {
        //Debug.Log(this.gameObject.name + "AI:" + statAnim);
        switch (statAnim)
        {
            case AnimState.IDLE:
                mAnimator.SetBool("isWalk", false);
                mAnimator.SetBool("isRun", false);
                mAnimator.SetBool("isRun2", false);
                break;
            case AnimState.PATROL:
                mAnimator.SetBool("isWalk", true);
                mAnimator.SetBool("isRun", false);
                mAnimator.SetBool("isRun2", false);
                break;
            case AnimState.HIT:
                mAnimator.SetBool("isWalk", false);
                mAnimator.SetBool("isRun", false);
                mAnimator.SetBool("isRun2", false);
                break;
            case AnimState.RUN:
                mAnimator.SetBool("isWalk", false);
                mAnimator.SetBool("isRun", true);
                mAnimator.SetBool("isRun2", false);
                break;
            case AnimState.HAPPY:
                mAnimator.SetBool("isWalk", false);
                mAnimator.SetBool("isRun", false);
                mAnimator.SetBool("isRun2", true);
                break;
            case AnimState.CLEAN:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// trigger type Anims
    /// </summary>
    /// <param name="_num">0:Jump/1:Stretch/2:Shake/3:Hit</param>
    public override void SetAnim(int _num)
    {
        mAnimator.SetBool("isWalk", false);
        mAnimator.SetBool("isRun", false);
        mAnimator.SetBool("isRun2", false);
        mAnimator.SetBool("isMove", false);
        switch (_num)
        {
            case 0:
                mAnimator.SetTrigger("isJump");
                break;
            case 1:
                mAnimator.SetTrigger("isStretch");
                break;
            case 2:
                mAnimator.SetTrigger("isShake");
                break;
            case 3:
                mAnimator.SetTrigger("isHit");
                break;
            case 4:
                mAnimator.SetTrigger("tAir");
                break;
            case 5:
                mAnimator.SetTrigger("tNext");
                break;
            default:
                break;
        }
    }

    public override void AnimationEnd()
    {
        bodyColls.SetActive(true);
        AI_Move(3);
    }
    #endregion


}
