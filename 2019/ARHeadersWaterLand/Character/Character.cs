using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Project.ReadOnly;

public enum CharacterState
{
    PATROL = 0,
    STAY,
    HIT,
    FLEE,
    FEAR,
    CLEAN,
};

//대가리 번호
public enum Headers
{
    NONE = 0,
    GIRRAFE = 1,
    ZEBRA = 2,
    RHINO = 3,
    ELEPHANT = 4,
    PIG = 5,
    ENEMY = 9,
}

public class MonsterData
{
    public AI_Data[] AI_Data;
}

public class AI_Data
{
    public int Type; // 현재 수행중인 동작
    public int True; // 동작 수행 후 조건이 true 일때 호출되는 다음 동작
    public int False; // 동작 수행 후 조건이 false 일때 호출
    public int Condition; // 해당 동작과 연결되어 있는 동작의 Index
}

public class CharacterAttribute
{
    // 대가리 캐릭터
    public Headers header { get; set; }
    // 이동속도
    public float moveSpeed { get; set; }
    // 타겟 검색범위
    public float findRange { get; set; }
    // 최대 HP
    public int maxHp { get; set; }
    // HP
    public int hp { get; set; }
}

public class Character : MonoBehaviour
{
    protected GameManager gameMgr;

    //외부 클래스
    public CharacterAttribute Status { get; set; }
    public Animator m_Animator { get; set; }
    public Rigidbody m_Rigidbody { get; set; }
    public NavMeshAgent nvAgent { get; set; }
    public CharacterState characterState { get; set; }

    public BezierCurve bezierCurve { get; set; } //캐릭터가 따라갈 객체

    public bool isDie { get; set; }
    public bool isHit { get; set; }
    
    public Vector3 spawnPoint { get; set; }

    public SkinnedMeshRenderer mRenderer; //피부
    public Material mSkin;    //오염 상태 머테리얼
    public Texture[] mSkinTex;      //텍스쳐 변경용 텍스쳐

    public GameObject[] colls; //앞,중간,뒤 컬리더
    public ParticleSystem hitEffect;
    public int lastHit; //마지막으로 맞은 지점 0=앞 1=중간 2=뒤

    public float accel = 1.0f;

    public AudioClip sfx_move { get; set; }
    public AudioClip sfx_fast { get; set; }
    public AudioClip sfx_hit { get; set; }
    public AudioClip sfx_clean { get; set; }

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        Status = new CharacterAttribute();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        bezierCurve = this.transform.GetChild(0).GetComponent<BezierCurve>();

        sfx_move = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_BUBBLE) as AudioClip;
        sfx_fast = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_FAST) as AudioClip;
        sfx_hit = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_HIT) as AudioClip;
        sfx_clean = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_CLEAN) as AudioClip;

        //colls = new GameObject[3];
        //for (int i = 0; i < 3; i++)
        //{
        //    colls[i] = this.transform.GetChild(1).GetChild(i).gameObject;
        //}

        //hitEffect = this.transform.GetChild(2).GetComponent<ParticleSystem>();

        DoAwake();
    }

    /// <summary>
    /// 메인 텍스쳐 변환 함수
    /// </summary>
    /// <param name="_idx">텍스쳐 번호</param>
    public virtual void SetSkinTex(int _idx)
    {
        if (mSkin != null)
        {
            mSkin.mainTexture = mSkinTex[_idx];
        }
    }


    //--------------------------------Virtual 함수--------------------------------\\
    protected virtual void DoAwake() { /* do nothing */ }
    public virtual void Init() { /* do nothing */ }
    
    protected virtual void SpecialMove() { /*do nothing*/ }
    public virtual void AI_Move(int _type) { /*do nothing*/}
    protected virtual void SetAnim() { }
    public virtual void Stop() { /*do nothing*/}

    /// <summary>
    /// 총알과 충돌 시 호출
    /// </summary>
    /// <param name="_damage"> 총알에 저장된 데미지</param>
    public virtual void TakeDamage(int _damage)
    {
        StopAllCoroutines();
        bezierCurve.StopAllCoroutines();
        bezierCurve.isMoving = false;

        //완전히 깨끗해지면 데미지 받지 않음
        if (isDie == true && Status.hp <= 0) { return; }

        Status.hp -= _damage;
        Debug.Log(this.gameObject.name + " HP: " + Status.hp);
        //클리어 게이지 상승
        gameMgr.uiMgr.GaugePlus(_damage);

        if (Status.hp <= 0)
        {
            Debug.Log(this.gameObject.name + "듀금");
            StopAllCoroutines();
            AI_Move(2);
            AI_Move(9); //사망 ??
        }
        else
        {
            characterState = CharacterState.HIT;
            SetAnim();

            AI_Move(2);
        }
    }


    //----------------------Coroutines-------------------------
    //기본상태
    //순찰
    //AI: 0
    public virtual IEnumerator PatrolMove()
    {
        if (bezierCurve.isMoving == false)
        {
            StartCoroutine(bezierCurve.CurveMove());
        }
        //Debug.Log(this.gameObject.name + " AI: Patrol");

        characterState = CharacterState.PATROL;
        SetAnim();

        while (isHit == false
            && bezierCurve.isMoving == true)
        {
            transform.position = Vector3.Lerp(transform.position, bezierCurve.transform.position, Status.moveSpeed * accel * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(bezierCurve.transform.position - transform.position), Status.moveSpeed * 4 * accel * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            yield return new WaitForSeconds(0.0167f);
        }
        //Debug.Log(this.gameObject.name + " AI: Patrol End");
        yield return new WaitForSeconds(0.1f);
        bezierCurve.StopAllCoroutines();
        AI_Move(0);
    }

    //히트 당한 후
    //주변의 랜덤한 포인트로 이동
    //도망가기&숨기
    //AI: 2
    public virtual IEnumerator Flee()
    {
        Debug.Log(this.gameObject.name + " AI: Flee");
        characterState = CharacterState.FLEE;
        SetAnim();
        bezierCurve.isFlee = true;
        if (bezierCurve.isMoving == false)
        { StartCoroutine(bezierCurve.CurveMove()); }

        while (isHit == true
            && isDie == false
            && bezierCurve.isMoving == true)
        {
            transform.position = Vector3.Lerp(transform.position, bezierCurve.transform.position, Status.moveSpeed * accel * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(bezierCurve.transform.position - transform.position), Status.moveSpeed * accel * 4 * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            yield return new WaitForSeconds(0.0167f);
        }
        bezierCurve.isFlee = false;
        isHit = false;
        Debug.Log(this.gameObject.name + " AI: Flee End");
        yield return new WaitForSeconds(0.1f);
        bezierCurve.StopAllCoroutines();
        AI_Move(0);
    }

    //죽을 때 동작
    //사망 애니메이션 재생 후 비활성화(?)
    public virtual IEnumerator Clean()
    {
        characterState = CharacterState.CLEAN;
        SetAnim();
        gameMgr.soundMgr.PlaySfx(transform.position, sfx_clean);
        isDie = true;
        Debug.Log(this.gameObject.name + "AI: Clean!");
        yield return new WaitForSeconds(1.0f);
        //this.gameObject.SetActive(false);
        gameMgr.CheckGameClear();
    }

    /// <summary>
    /// 맞았을 때
    /// </summary>
    /// <param name="other"></param>
    public virtual void Hit(Collider other)
    {
        if (gameMgr.gameState != GameState.PLAYING) { return; }
        isHit = true;
        TakeDamage(1);
        hitEffect.Play();
        gameMgr.soundMgr.PlaySfx(transform.position, sfx_hit);
        Debug.Log("Missile Hited!!");
    }


    //---------------------------Animation Events---------------------------\\
    /// <summary>
    /// 03.Models/Fish_Animation/Swim_Fast/Animation Event Function
    /// </summary>
    public virtual void SpeedFast()
    {
        accel = 2f;

        //물장구치는 소리
        gameMgr.soundMgr.PlaySfx(this.transform.position, sfx_fast);
    }
    public virtual void SpeedSlow()
    {
        accel = 1f;
    }

}