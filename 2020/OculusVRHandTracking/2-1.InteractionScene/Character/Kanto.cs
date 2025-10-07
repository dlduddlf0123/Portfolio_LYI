using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ReadOnly;

public class Kanto : Character
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

        list___dialog_kor.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.DIALOG_KANTO_KOR));
        list___dialog_kor.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.DIALOG_KANTO_KOR));
        list___dialog_kor.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.DIALOG_KANTO_KOR));
        list___dialog_eng.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.DIALOG_KANTO_KOR));
        list___dialog_eng.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.DIALOG_KANTO_KOR));
        list___dialog_eng.Add(gameMgr.dialogMgr.ReadDialogDatas(Defines.DIALOG_KANTO_KOR));

        //Navigations
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.enabled = true;
        mNavAgent.isStopped = false;

        ChangeLikeState();
        ChangeEnergyState();
        ChangeHungerState();


        AI_Move(0);
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

        ChangeMovePoint(0);
    }

    private void OnDisable()
    {
        Stop();
        StatusInit();
    }

    //private void Update()
    //{
    //    GetHunger(-Time.deltaTime);
    //    GetEnergy(-Time.deltaTime*0.3f);
    //}
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Header"))
        {
            AI_Move(0);
        }
    }

    /// <summary>
    /// 현재 호감도에 따라 상태 변경
    /// 호감도 상태가 변경되면 애니메이션 또한 변경
    /// 상태가 변경될 때 대사 출력
    /// </summary>
    protected override void ChangeLikeState()
    {
        LikeState _before = statLike;

        float likeMax = 100f;
        switch (statLike)
        {
            case LikeState.HATE:
                likeMax = 50;
                break;
            case LikeState.NORMAL:
                likeMax = 100;
                break;
            case LikeState.FRIEND:
                likeMax = 100;
                break;
        }

        if (Status.likeMeter >= likeMax && (int)statLike != 2)
        {
            Status.likeMeter = 0;
            if ((int)statLike < 2)
            {
                statLike++;
            }
        }
        else if (Status.likeMeter <= 0 && (int)statLike != 0)
        {
            Status.likeMeter = likeMax;
            if (statLike > 0)
            {
                statLike--;
            }
        }

        switch (gameMgr.language)
        {
            case 0:
                headerCanvas.list__currentDialog = list___dialog_kor[(int)statLike];
                break;
            case 1:
                headerCanvas.list__currentDialog = list___dialog_eng[(int)statLike];
                break;
            default:
                break;
        }
        int _random = Random.Range(0, 2);

        Debug.Log(statLike);
        switch (statLike)
        {
            case LikeState.HATE:
                headerCanvas.ShowText(4, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_hate") as RuntimeAnimatorController;
                break;
            case LikeState.NORMAL:
                headerCanvas.ShowText((_before < statLike) ? 3 : 4, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_normal") as RuntimeAnimatorController;
                break;
            case LikeState.FRIEND:
                headerCanvas.ShowText(3, _random);
                //mAnimator.runtimeAnimatorController = Resources.Load("Animator/Kanto_friend") as RuntimeAnimatorController;
                break;
        }
        //soundMgr.PlaySfx(this.transform.position, soundMgr.LoadClip("Sounds/SFX/level_up"));

        //gameMgr.uiMgr.SetCommandUI((int)statLike);  //커맨드 UI 변경

        Debug.Log(gameObject.name + " Like =" + statLike);
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

        if (statEnergy == EnergyState.BLACKOUT)
        {
            _type = 5;
        }

        //if (statHunger == HungerState.FULL &&
        //    statAnim == AnimState.CALL &&
        //    statLike >= LikeState.NORMAL &&
        //    gameMgr.questMgr.dic_quest["Poop"].questStep == 0)
        //{
        //    Stop();
        //    gameMgr.questMgr.StartQuest("Poop");
        //    return;
        //}
        if (Status.hungerMeter > 100) 
        {
            Stop();
            Status.hungerMeter = 100;
            StartCoroutine(Pooping());
        }

        //if (gameMgr.questMgr.dic_quest["Poop"].statQuest != QuestStatus.CLEAR &&
        //    gameMgr.questMgr.dic_quest["Poop"].questStep == 2 &&
        //    statAnim == AnimState.CALL)
        //{
        //    currentAI = StartCoroutine(HateSmell());
        //    return;
        //}

        base.AI_Move(_type);
    }



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
