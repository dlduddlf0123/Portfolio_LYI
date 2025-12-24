using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.ReadOnly;

public class Enemy_Cloud : Character
{
    BlackRain blackRain;

    //Call after Character.Awake()
    protected override void DoAwake()
    {
        blackRain = transform.GetChild(2).GetChild(2).GetComponent<BlackRain>();
        mAnimator = this.transform.GetChild(1).GetComponent<Animator>();

        StatusInit();
        statAnim = AnimState.IDLE;
        currentTarget = null;

        //Navigations
        mNavAgent.speed = Status.moveSpeed;
        mNavAgent.enabled = true;
        mNavAgent.isStopped = false;
    }

    protected override void StatusInit()
    {
        Status.header = Headers.ENEMY;
        Status.moveSpeed = 0.2f;
        Status.findRange = 0.0f;
        Status.maxHp = 20;
        Status.hp = Status.maxHp;

        blackRain.heal = Status.maxHp / 10;

        int rand = Random.Range(0, gameMgr.list_SpawnPoints.Count);
        spawnPoint = gameMgr.list_SpawnPoints[rand].localPosition;
        this.transform.localPosition = spawnPoint;

        isClean = false;
        isHit = false;
    }

    private void Start()
    {
        StatusInit();
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        StatusInit();
    }

    /// <summary>
    /// 총알과 충돌 시 호출
    /// </summary>
    /// <param name="_damage"> 총알에 저장된 데미지</param>
    public override void TakeDamage(int _damage)
    {
        //완전히 깨끗해지면 데미지 받지 않음
        if (isClean == true || Status.hp <= 0) { return; }

        Status.hp -= _damage;
        Debug.Log(this.gameObject.name + " HP: " + Status.hp);
        headerCanvas.SetHP(Status.maxHp, Status.hp);

        StopAllCoroutines();

        if (Status.hp <= 0)
        {
            isClean = true;
            AI_Move(9); //사망 ??
        }
        else
        {
            AI_Move(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ball"))
        {
            StopAllCoroutines();
            TakeDamage(other.GetComponent<Missile>().damage);
        }
    }

    public override void Stop()
    {
        isHit = false;
        StopAllCoroutines();
    }

    //----------------------Coroutines-------------------------
    //기본상태
    //순찰
    //AI: 0
    protected override IEnumerator PatrolMove()
    {
        statAnim = AnimState.PATROL;
        SetAnim();

        mNavAgent.isStopped = false;

        int randPoint = Random.Range(0, gameMgr.limit_headers + 1); //향할 대가리 포인트

        Transform target = gameMgr.list_Headers[randPoint].transform;
        mNavAgent.destination = target.transform.position;

        float moveTime = 0.0f;
        target = gameMgr.list_Headers[randPoint].transform;
        mNavAgent.destination = target.transform.position;

        while (isHit == false
               && isClean == false
               && mNavAgent.remainingDistance > 0f
               && moveTime < 3.0f)
        {
            moveTime += 0.02f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }
        AI_Move(3);
    }

    protected override IEnumerator Clean()
    {
        //return base.Clean();
        statAnim = AnimState.CLEAN;
        mNavAgent.isStopped = true;

        gameMgr.current_enemies--;
        gameMgr.CheckGameClear();
        gameObject.SetActive(false);
        Stop();
        yield break;
    }


    //--------------------------Type별 동작--------------------------
    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:flee/3:stay/4:fear/d:clear</param>
    public override void AI_Move(int _type)
    {
        if (gameMgr.statGame == GameState.RESULT)
        {
            StopAllCoroutines();
            return;
        }
        switch (_type)
        {
            case 0: //배회(미 인식)
                StartCoroutine(PatrolMove());
                break;
            case 1: //맞았을 때
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
            default: //깨끗해졌을 때
                StartCoroutine(Clean());
                break;
        }
    }

    ///// <summary>
    ///// 애니메이션 변경 시 호출, 현재 State에 따른 애니메이션 적용
    ///// </summary>
    //protected override void SetAnim()
    //{
    //    m_Animator.SetBool(Defines.ANIM_TRIGGER_HIT, false);
    //    m_Animator.SetBool(Defines.ANIM_BOOL_FEAR, false);
    //    m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, false);
    //    switch (base.characterState)
    //    {
    //        case CharacterState.IDLE:
    //            break;
    //        case CharacterState.PATROL:
    //            m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, true);
    //            break;
    //        case CharacterState.HIT:
    //            m_Animator.SetBool(Defines.ANIM_TRIGGER_HIT, true);
    //            break;
    //        case CharacterState.FLEE:
    //            m_Animator.SetBool(Defines.ANIM_BOOL_FLEE, true);
    //            break;
    //        case CharacterState.FEAR:
    //            m_Animator.SetBool(Defines.ANIM_BOOL_FEAR, true);
    //            break;
    //        case CharacterState.CLEAN:
    //            break;
    //    }
    //}
}
