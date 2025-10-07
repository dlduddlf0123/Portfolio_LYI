using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketCharacter : Character
{
    //  public AnimationCurve curve = new AnimationCurve();
    protected override void DoAwake()
    {
        statAnim = AnimState.IDLE;
        currentTarget = null;
    }


    protected override void Start()
    {
        base.Start();

        StatusInit();

        //Navigations
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.enabled = true;
        mNavAgent.isStopped = false;
    }

    public override void Stop()
    {
        StopAllCoroutines();

        if (currentAI != null)
        {
            StopCoroutine(currentAI);
            currentAI = null;
        }
        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;
    }

    protected override void StatusInit()
    {
        Debug.Log(this.gameObject.name + "Init");
        Status.header = Headers.KANTO;
        statAnim = AnimState.IDLE;

        Status.moveSpeed = 2f;
        Status.minSpeed = 0.1f;
        Status.maxSpeed = 4f;

        Status.maxHp = 5;
        Status.hp = Status.maxHp;

        statLike = LikeState.HATE;   //(LikeState)PlayerPrefs.GetInt("LikeState", 0);
        statHunger = HungerState.FULL;
        statEnergy = EnergyState.ENERGETIC;
        Status.likeMeter = 0;   //PlayerPrefs.GetInt("LikeMeter", 0);
        Status.hungerMeter = 100;
        Status.energyMeter = 100;

        Status.likeFood = FoodType.GRASS;
        Status.hateFood = FoodType.MEAT;
        Status.likeTaste = 20;  //10~19 = 과일 //20~29 = 채소, 유제품 // 30~39 = 육류
        Status.hateTaste = 30;

        isAction = false;
        isDie = false;
        isHit = false;
    }

    private void OnDisable()
    {
        Stop();
        StatusInit();
    }

    /// <summary>
    /// AI: 03
    /// 대기 동작
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Idle()
    {
        mAnimator.SetBool("IdleMove", false);
        statAnim = AnimState.IDLE;
        PlayBoolAnimation();

        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;

        yield return new WaitForSeconds(5f);

    }

    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:run/3:stay/4:call/5:sleep/d:clear</param>
    public override void AI_Move(int _type)
    {
        //게임 중단일 경우 하지않음
        if (gameMgr.statGame == GameState.GAMEOVER || statAnim == AnimState.LYING || isEvent || isAction) { return; }
        if (currentAI != null) { StopCoroutine(currentAI); currentAI = null; }

        base.AI_Move(_type);
    }

    public override void NextAI(bool _choose)
    { }


    #region Animation Settings
        /// <summary>
        /// 애니메이션 변경 시 호출, 현재 AnimState에 따른 애니메이션 적용
        /// bool type Anims
        /// </summary>
    public override void PlayBoolAnimation()
    {
        mAnimator.SetBool("isMove", false);
        mAnimator.SetBool("IdleMove", false);
        switch (statAnim)
        {
            case AnimState.IDLE:
                mAnimator.SetFloat("TriggerNum", Random.Range(0, 2));
                break;
            case AnimState.PATROL:
                mAnimator.SetBool("isMove", true);
                mAnimator.SetFloat("Speed", 0);
                break;
            case AnimState.RUN:
                mAnimator.SetBool("isMove", true);
                mAnimator.SetFloat("Speed", 2);
                mAnimator.SetFloat("MoveNum", 0);
                break;
            case AnimState.CALL:
                mAnimator.SetBool("isMove", true);
                mAnimator.SetFloat("Speed", 2);
                mAnimator.SetFloat("MoveNum", 1);
                break;
            case AnimState.SLEEP:
                mAnimator.SetFloat("StateNum", 0);
                mAnimator.SetTrigger("StateChange");
                break;
            case AnimState.LYING:
                mAnimator.SetFloat("StateNum", 1);
                mAnimator.SetTrigger("StateChange");
                break;
            case AnimState.CLEAN:
                mAnimator.SetBool("isMove", true);
                mAnimator.SetFloat("Speed", 2);
                mAnimator.SetFloat("MoveNum", -1);
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