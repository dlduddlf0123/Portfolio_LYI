using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.ReadOnly;

public class CharGirrafe : Character
{
    //Call after Character.Awake()
    protected override void DoAwake()
    {
        StatusInit();

        statAnim = AnimState.IDLE;
        currentTarget = null;

        list__dialog_kor = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_DIALOG_KANTO_KOR);
        list__dialog_eng = gameMgr.dialogMgr.ReadDialogDatas(Defines.CSV_DIALOG_KANTO_ENG);

        //Navigations
        mNavAgent.speed = Status.moveSpeed * gameMgr.stage.transform.localScale.x;
        mNavAgent.enabled = true;
        mNavAgent.isStopped = false;
    }

    protected override void StatusInit()
    {
        Debug.Log(this.gameObject.name + "Init");
        Status.header = Headers.GIRRAFE;
        Status.moveSpeed = 2f;
        Status.findRange = 0.0f;
        Status.maxHp = 40;
        Status.hp = Status.maxHp;

        int rand = Random.Range(0, gameMgr.list_SpawnPoints.Count);
        spawnPoint = gameMgr.list_SpawnPoints[rand].localPosition;
        this.transform.localPosition = spawnPoint;
        
        isClean = false;
        isHit = false;
        isGround = false;
        headerCanvas.SetDialogLanguage();
    }

    private void Start()
    {
        StatusInit();
        this.transform.GetChild(0).GetComponent<WatchCamera>().GetHeader();
    }


    private void OnDisable()
    {
        Stop();
        StatusInit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Header"))
        {
            StopAllCoroutines();
            AI_Move(0);
        }

        if (other.CompareTag("ball"))
        {
            StopAllCoroutines();
            TakeDamage(other.GetComponent<Missile>().damage);
        }
    }
    
    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:flee/3:stay/4:fear/5:jump/d:clear</param>
    public override void AI_Move(int _type)
    {
        if (gameMgr.statGame == GameState.RESULT)
        {
            Stop();
            mAnimator.SetBool(Defines.ANIM_BOOL_FEAR, false);
            mAnimator.SetBool(Defines.ANIM_BOOL_FLEE, false);
            mAnimator.SetBool(Defines.ANIM_BOOL_PATROL, false);
            if (gameMgr.isClear)
            {
                mAnimator.SetInteger(Defines.ANIM_INT_IDLE, 3);
            }
            else
            {
                mAnimator.SetInteger(Defines.ANIM_INT_IDLE, 1);
            }
            return;
        }
        if (gameMgr.statGame == GameState.DIALOG) { return; }

        int _random = Random.Range(0, 2);
        switch (_type)
        {
            case 0: //배회(미 인식)
                headerCanvas.ShowText(0, _random);
                StartCoroutine(PatrolMove());
                break;
            case 1: //맞았을 때
                headerCanvas.ShowEmote(0);
                StartCoroutine(Hit());
                break;
            case 2: //도망갈 때
                StartCoroutine(Run());
                break;
            case 3: //가만히 있을 때, 정지할 때
                StartCoroutine(Idle());
                break;
            case 4: //시야 안에 들어왔을 떄
                StartCoroutine(Fear());
                break;
            case 5: //시야 안에 들어왔을 떄
                StartCoroutine(Jump());
                break;
            default: //깨끗해졌을 때
                StartCoroutine(Clean());
                break;
        }
    }
    #region Animation Settings
    /// <summary>
    /// 애니메이션 변경 시 호출, 현재 State에 따른 애니메이션 적용
    /// Bool Type
    /// </summary>
    protected override void SetAnim()
    {
        mAnimator.SetBool(Defines.ANIM_BOOL_FEAR, false);
        mAnimator.SetBool(Defines.ANIM_BOOL_FLEE, false);
        mAnimator.SetBool(Defines.ANIM_BOOL_PATROL, false);
        mAnimator.SetInteger(Defines.ANIM_INT_IDLE, Random.Range(0, 3));

        switch (base.statAnim)
        {
            case AnimState.IDLE:
                mAnimator.SetInteger(Defines.ANIM_INT_IDLE, Random.Range(0, 3));
                break;
            case AnimState.PATROL:
                mAnimator.SetBool(Defines.ANIM_BOOL_PATROL, true);
                mAnimator.SetTrigger(Defines.ANIM_TRIGGER_WALK);
                break;
            case AnimState.HIT:
                mAnimator.SetTrigger(Defines.ANIM_TRIGGER_HIT);
                break;
            case AnimState.RUN:
                mAnimator.SetBool(Defines.ANIM_BOOL_FLEE, true);
                break;
            case AnimState.FEAR:
                mAnimator.SetTrigger(Defines.ANIM_TRIGGER_HIT);
                mAnimator.SetBool(Defines.ANIM_BOOL_FEAR, true);
                break;
            case AnimState.CLEAN:
                mAnimator.SetTrigger(Defines.ANIM_TRIGGER_CLEAN);
                break;
        }
    }

    /// <summary>
    /// trigger type Anims
    /// </summary>
    /// <param name="_num">0:Jump/1:Stretch/2:Shake/3:Hit</param>
    public override void SetAnim(int _num)
    {
        mAnimator.SetBool(Defines.ANIM_BOOL_FEAR, false);
        mAnimator.SetBool(Defines.ANIM_BOOL_FLEE, false);
        mAnimator.SetBool(Defines.ANIM_BOOL_PATROL, false);

        switch (_num)
        {
            case 0:
                mAnimator.SetTrigger(Defines.ANIM_TRIGGER_CLEAN);
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

    public void Anim_JumpStart()
    {
        isGround = false;
        AI_Move(5);
    }

    public void Anim_JumpEnd()
    {
        isGround = true;
        AI_Move(2);
    }
    #endregion
}
