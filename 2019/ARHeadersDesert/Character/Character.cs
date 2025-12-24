using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Project.ReadOnly;

public enum AnimState
{
    IDLE = 0,
    PATROL,
    HIT,
    RUN,
    FEAR,
    CLEAN,
};

//대가리 번호
public enum Headers
{
    NONE = 0,
    GIRRAFE = 1,
    ZEBRA,
    PIG,
    ELEPHANT,
    RHINO,
    MOUSE,
    ENEMY = 9,
}

//public class MonsterData
//{
//    public AI_Data[] AI_Data;
//}

//public class AI_Data
//{
//    public int Type; // 현재 수행중인 동작
//    public int True; // 동작 수행 후 조건이 true 일때 호출되는 다음 동작
//    public int False; // 동작 수행 후 조건이 false 일때 호출
//    public int Condition; // 해당 동작과 연결되어 있는 동작의 Index
//}

/// <summary>
/// 캐릭터 능력치
/// </summary>
public class CharacterAttribute
{
    // 대가리 캐릭터
    public Headers header { get; set; }
    // 이동속도
    public float moveSpeed { get; set; }
    // 타겟 검색범위(카메라)
    public float findRange { get; set; }
    // 최대 HP
    public int maxHp { get; set; }
    // HP
    public int hp { get; set; }
}

public class Character : MonoBehaviour
{
    //외부 클래스
    public GameManager gameMgr { get; set; }
    public WatchCamera watchCamera { get; set; }
    public HeaderCanvas headerCanvas { get; set; }

    //내부 클래스
    public CharacterAttribute Status { get; set; }
    public Animator mAnimator { get; set; }
    public Rigidbody mRigidbody { get; set; }
    public NavMeshAgent mNavAgent { get; set; }

    //Enum
    public AnimState statAnim { get; set; }


    //MovePoint
    public Vector3 spawnPoint { get; set; }
    public Transform currentTarget;

    //Color
    public Material[] mMaterial;
    public Color bodyColor { get; set; } //몸 색깔
    
    //Sounds
    public AudioClip sfx_move { get; set; }
    public AudioClip sfx_fast { get; set; }
    public AudioClip sfx_hit { get; set; }
    public AudioClip sfx_clean { get; set; }

    //내부 변수
    public bool isClean { get; set; }
    public bool isHit { get; set; }
    public bool isGround = true;
    //대사
    public List<List<object>> list__dialog_kor { get; set; }
    public List<List<object>> list__dialog_eng { get; set; }

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        watchCamera = transform.GetChild(0).GetComponent<WatchCamera>();
        headerCanvas = transform.GetChild(1).GetComponent<HeaderCanvas>();

        Status = new CharacterAttribute();
        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mNavAgent = GetComponent<NavMeshAgent>();

        list__dialog_kor = new List<List<object>>();
        list__dialog_eng = new List<List<object>>();

        DoAwake();
    }

    private void Start()
    {
        sfx_move = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_BUBBLE) as AudioClip;
        sfx_fast = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_FAST) as AudioClip;
        sfx_hit = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_HIT) as AudioClip;
        sfx_clean = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_CLEAN) as AudioClip;
       
    }
    protected virtual void DoAwake() { /* do nothing */ }
    protected virtual void StatusInit() { /* do nothing */ }

    /// <summary>
    /// 각 Type에 따른 코루틴을 실행시킨다
    /// </summary>
    /// <param name="_type">0:patrol/1:hit/2:flee/3:stay/4:fear/5:jump/d:clear</param>
    public virtual void AI_Move(int _type) { /*do nothing*/}

    /// <summary>
    /// 애니메이션 변경 시 호출, 현재 State에 따른 애니메이션 적용
    /// </summary>
    protected virtual void SetAnim() { }            // bool type Anims
    public virtual void SetAnim(int _num) { }   // trigger type Anims


    protected virtual void SpecialMove() { /*do nothing*/ }

    public virtual void Stop()
    {
        StopAllCoroutines();
        mNavAgent.speed = 0;
        mNavAgent.isStopped = true;
    }

    /// <summary>
    /// 씻겨지기(데미지)
    /// </summary>
    /// <param name="_damage">받을 피해량</param>
    public virtual void TakeDamage(int _damage)
    {
        //완전히 깨끗해지면 데미지 받지 않음
        if (isClean == true || Status.hp <= 0) 
            return;

        Stop();

        //초과 데미지로 인한 오류 방지
        if ((Status.hp - _damage) < 0)
        {
            _damage = Status.hp;
        }

        Status.hp -= _damage;

        Debug.Log(this.gameObject.name + " HP: " + Status.hp);

        //색 변경
        SetBodyColor( 1 - ((float)Status.hp / (float)Status.maxHp));

        //클리어 게이지 상승
        gameMgr.uiMgr.GaugePlus(_damage);
        headerCanvas.SetHP(Status.maxHp, Status.hp);

        if (Status.hp <= 0)
        {
            Debug.Log(this.gameObject.name + "듀금");
            ////눈색깔 변형
            //mRenderer[1].material = mMaterial[0];
            //mRenderer[2].material = mMaterial[0];
            isClean = true;
            AI_Move(9); //사망 ??
        }
        else
        {
            isHit = true;
            AI_Move(1);
        }
    }

    /// <summary>
    /// 더러워짐(힐링)
    /// </summary>
    /// <param name="_heal">회복량</param>
    public virtual void TakeHeal(int _heal)
    {
        //최대체력일 때에는 체력을 채우지 않음
        if (Status.hp >= Status.maxHp) { return; }

        if ((Status.hp+_heal) > Status.maxHp)
        {
            _heal = Status.maxHp - Status.hp;
        }
        Status.hp += _heal;

        //색 변경
        SetBodyColor(1 - ((float)Status.hp / (float)Status.maxHp));
        headerCanvas.SetHP(Status.maxHp, Status.hp);
        Debug.Log(this.gameObject.name + " HP: " + Status.hp);
        isClean = false;

        //클리어 게이지 하락
        gameMgr.uiMgr.GaugeMinus(_heal);
    }

    public void SetBodyColor(float _dirty)
    {
        if (mMaterial == null)
        {
            return;
        }
        //색 변경
        bodyColor = new Color(_dirty, _dirty, _dirty);
        foreach (Material _m in mMaterial)
        {
            _m.color = bodyColor;
        }
    }

    #region Normal AI Moves
    //----------------------Coroutines-------------------------
    /// <summary>
    /// AI: 00
    /// 걸으면서 돌아다니기
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator PatrolMove()
    {
        statAnim = AnimState.PATROL;
        SetAnim();

        int randPoint = Random.Range(0, gameMgr.list_FleePoints.Count); //향할 맵 포인트
        float randDist = Random.Range(0.05f, 0.5f); //갈 거리

        Transform target = gameMgr.list_FleePoints[randPoint];
        if (target == currentTarget)
        {
            randPoint = Random.Range(0, gameMgr.list_FleePoints.Count);
            target = gameMgr.list_FleePoints[randPoint];
        }
        currentTarget = target;

        mNavAgent.destination = currentTarget.transform.position;
        mNavAgent.speed = Status.moveSpeed* gameMgr.stage.transform.localScale.x;
        mNavAgent.isStopped = false;

        while (isHit == false &&
            gameMgr.statGame == GameState.PLAYING &&
               mNavAgent.remainingDistance > randDist)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        AI_Move(3);
    }

    /// <summary>
    /// AI: 01
    /// 맞았을 시 잠시 정지(히트애니메이션)
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Hit()
    {
        if (isClean == false)
        {
            statAnim = AnimState.HIT;
            SetAnim();

            yield return new WaitForSeconds(0.2f);
            AI_Move(2);
        }
    }

    /// <summary>
    /// AI: 02
    /// 달려서 도망가기
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Run()
    {
        statAnim = AnimState.RUN;
        SetAnim();

        int randPoint = Random.Range(0, gameMgr.list_FleePoints.Count);

        Transform target = gameMgr.list_FleePoints[randPoint];
        if (target == currentTarget)
        {
            randPoint = Random.Range(0, gameMgr.list_FleePoints.Count);
            target = gameMgr.list_FleePoints[randPoint];
        }

        currentTarget = target;
        mNavAgent.destination = currentTarget.position;
        mNavAgent.isStopped = false;
        mNavAgent.speed = Status.moveSpeed * gameMgr.stage.transform.localScale.x * 5f;

        yield return new WaitForSeconds(0.1f);

        while (gameMgr.statGame == GameState.PLAYING && 
            mNavAgent.remainingDistance > 0.05f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), Status.moveSpeed * 4 * Time.deltaTime);
            yield return new WaitForSeconds(0.0167f);
        }

        isHit = false;
        mNavAgent.speed = Status.moveSpeed * gameMgr.stage.transform.localScale.x;
        AI_Move(3);
    }

    /// <summary>
    /// AI: 03
    /// 대기 동작
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Idle()
    {
        statAnim = AnimState.IDLE;
        SetAnim();

        mNavAgent.isStopped = true;
        yield return new WaitForSeconds(1 / Status.moveSpeed + 1);
        AI_Move(0);
    }

    /// <summary>
    /// AI: 04
    /// 카메라 발견시 놀람
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Fear()
    {
        statAnim = AnimState.FEAR;
        SetAnim();
        yield return new WaitForSeconds(1f);
        AI_Move(1);
    }


    /// <summary>
    /// AI:05
    /// 히트 후 타겟 방향으로 돌면서 점프하기
    /// 점프 모션 + 이동
    /// </summary>
    /// <returns></returns>
    public IEnumerator Jump()
    {
        mNavAgent.isStopped = false;
        int randPoint = Random.Range(0, 4);
        float dist = 4.0f;

        switch (randPoint)
        {
            case 0:
                mNavAgent.destination = transform.localPosition + Vector3.forward * dist;
                break;
            case 1:
                mNavAgent.destination = transform.localPosition + Vector3.right * dist;
                break;
            case 2:
                mNavAgent.destination = transform.localPosition + Vector3.left * dist;
                break;
            case 3:
                mNavAgent.destination = transform.localPosition + Vector3.back * dist;
                break;
        }
        //yield return new WaitForSeconds(0.1f);
        while (isGround == false)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mNavAgent.destination - transform.localPosition), Status.moveSpeed * 20 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }

    /// <summary>
    /// AI: Last(Default)
    /// 사망 시
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Clean()
    {
        isClean = true;
        gameMgr.PlayEffect(this.transform.position, gameMgr.particles[0]);
        statAnim = AnimState.CLEAN;
        SetAnim();

        mNavAgent.isStopped = true;
        gameMgr.CheckGameClear();
        yield return new WaitForSeconds(2.0f);
        AI_Move(0);
    }

    #endregion
}