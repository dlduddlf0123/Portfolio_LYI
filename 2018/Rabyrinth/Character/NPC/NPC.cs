using UnityEngine;
using System.Collections;
using Rabyrinth.ReadOnlys;

public class NPC : Character
{

    protected GameManager GameMgr = null;
    // 원거리 공격이 발사되는 위치
    protected Transform magicPos;


    public Transform target = null;
    //public UnityEngine.AI.NavMeshObstacle nvObst = null;

    protected UnityEngine.AI.NavMeshAgent nvAgent = null;
    private SpawnManager spawnManager = null;
    private Animator animator = null;

    private ParticleSystem[] hitEffect;

    private float SlowTime;
    private float SlowValue;

    protected float InitAttakSpeed;
    protected float InitMoveSpeed;

    bool isSetEmission;
    protected Renderer renderer;
    protected Material mat;
    float emission;

    protected bool isBoss;

    // 공격범위
    public bool isActiveHP = false;
    //public HP_Bar hpBar = null;

    // 적의 State
    public CharacterState EnemyState { get; set; }

    protected override void ChildAwake() { }
    protected override void Init() { }
    protected virtual void Attack() { }
    protected virtual bool FindTarget() { return true; }

    protected override void DoAwake()
    {
        isBoss = false;
        SlowTime = 0.0f;
        SlowValue = 0.0f;

        GameMgr = MonoSingleton<GameManager>.Inst;

        hitEffect = new ParticleSystem[transform.GetChild(transform.childCount - 1).childCount];

        for (int index = 0; index < hitEffect.Length; index++)
            hitEffect[index] = transform.GetChild(transform.childCount - 1).GetChild(index).GetComponent<ParticleSystem>();

        spawnManager = GameMgr.spawnManager.GetComponent<SpawnManager>(); 
        Status = EnemyAttribute.Create();

        renderer = transform.GetChild(1).GetComponent<Renderer>();
        mat = renderer.materials[0];
        isSetEmission = false;

        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        this.gameObject.SetActive(true);

        EnemyState = CharacterState.idle;

        animator = this.gameObject.GetComponent<Animator>();
        nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        //nvObst = this.gameObject.GetComponent<UnityEngine.AI.NavMeshObstacle>();

        // 적의 타겟은 플레이어의 transform
        target = GameMgr.Player.transform;

        foreach (Collider coll in gameObject.GetComponentsInChildren<CapsuleCollider>())
            coll.enabled = true;

        ChildAwake();
    }

    private IEnumerator OffEmision()
    {
        yield return new WaitForSeconds(0.08f);
        mat.SetFloat(Defines.SHADER_PROPERTY_EMISSION, 1.0f);
        DynamicGI.UpdateMaterials(renderer);
        DynamicGI.UpdateEnvironment();
        isSetEmission = false;
    }

    protected void UpdateMat()
    {
        mat.SetFloat(Defines.SHADER_PROPERTY_EMISSION, 1.0f);
        DynamicGI.UpdateMaterials(renderer);
        DynamicGI.UpdateEnvironment();
    }
 
    protected IEnumerator SetPlayer()
    {
        while(GameMgr.Player == null)
            yield return null;

        nvAgent.enabled = true;
        // 코루틴 시작
        StartTrace();
    }
 
    protected void StartTrace()
    {
        StartCoroutine(this.EnemyStateCheck());
        StartCoroutine(this.EnemyAction());
    }

    public void Slow(float _time, float _value)
    {
        Status.AttackSpeed = InitAttakSpeed * _value;
        Status.MoveSpeed = InitMoveSpeed * _value;
        if(SlowTime <= 0)
            StartCoroutine(WaitSlowTime(_time));
    }

    public override void TakeDamage(int attackdamage, HitEffect _type, bool isCritical = false)
    {
        if (EnemyState == CharacterState.idle)
            EnemyState = CharacterState.trace;

        isSetEmission = true;
        mat.SetFloat(Defines.SHADER_PROPERTY_EMISSION, 4.0f);
        DynamicGI.UpdateMaterials(renderer);
        DynamicGI.UpdateEnvironment();
        StartCoroutine(this.OffEmision());


        if (isCritical && ! isBoss)
        {
            EnemyState = CharacterState.hit;
        }
        else if(isCritical == false)
        {
            isCritical = Random.Range(0, 101) <= Status.CriticalChance ? true : false;
            attackdamage = (int)((float)attackdamage * (isCritical ? Status.CriticalBonus : 1.0f));
        }

        switch(_type)
        {
            case HitEffect.Elect:
                hitEffect[(int)HitEffect.Elect].Play();
                break;
            case HitEffect.Fire:
                hitEffect[(int)HitEffect.Fire].Play();
                break;
            case HitEffect.Freez:
                hitEffect[(int)HitEffect.Freez].Play();
                break;
            case HitEffect.Dizziness:
                hitEffect[(int)HitEffect.Dizziness].Play();
                break;
            default:
                switch (GameMgr.Player.PlayerWeaponState)
                {
                    case WeaponState.Elect:
                        hitEffect[(int)HitEffect.Elect].Play();
                        break;
                    case WeaponState.Fire:
                        hitEffect[(int)HitEffect.Fire].Play();
                        break;
                    default:
                        hitEffect[(int)HitEffect.Default].Play();
                        break;
                }
                break;
        }

        Status.HP -= attackdamage;
        GameMgr.Main_UI.SetDamageText(DamageText_Type.NPC, transform.localPosition, attackdamage, isCritical);
        GameMgr.Main_UI.UpdateNPC_HpBarValue();

        if (Status.HP <= 0)
            EnemyDeath();

        //Debug.Log(name + "   HP : " + Status.HP);
    }
    // State를 체크
    IEnumerator EnemyStateCheck()
    {
        float sqrAttackRange = 0.0f;
        float _attackRange = Status.AttackRange;
        Vector3 range;
       
        while (EnemyState != CharacterState.die)
        {
            yield return new WaitForSeconds(0.2f);

            if (target != null)
            {
                range = target.transform.position - transform.position;
                sqrAttackRange = range.sqrMagnitude;

                // 공격범위에 플레이어가 들어왔다면
                if (sqrAttackRange <= _attackRange * _attackRange)
                {
                    if (FindTarget())
                    {
                        EnemyState = CharacterState.attack;
                    }
                    else
                    {
                        _attackRange -= 0.5f;
                        EnemyState = CharacterState.trace;
                    }

                }
                else if (sqrAttackRange < Status.FindRange * Status.FindRange)
                {
                    _attackRange = Status.AttackRange;
                    EnemyState = CharacterState.trace;
                }
            }
            else
            {
                EnemyState = CharacterState.idle;
                target = GameMgr.Player.transform;
            }
        }
    }
    // State에 따른 EnemyCharacter의 Action
    IEnumerator EnemyAction()
    {
        while (EnemyState != CharacterState.die)
        {
            switch (EnemyState)
            {
                case CharacterState.idle:
                        nvAgent.Stop();
                    //if (nvAgent.enabled == true)
                    //{
                    //    nvAgent.Stop();
                    //    nvAgent.enabled = false;
                    //}

                    //if (nvObst.enabled == false)
                    //    nvObst.enabled = true;

                    animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
                    animator.SetBool(Defines.ANI_PARAM_TRACE, false);
                    break;

                case CharacterState.trace:
                    //if (nvObst.enabled == true)
                    //{
                    //    nvObst.enabled = false;
                    //    yield return null;
                    //}

                    nvAgent.speed = Status.MoveSpeed;
                    nvAgent.destination = target.transform.position;
                    nvAgent.Resume();
                    
                    animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
                    animator.SetBool(Defines.ANI_PARAM_TRACE, true);

                    //if (nvAgent.enabled == false)
                    //    nvAgent.enabled = true;

                    //if (nvObst.enabled == false && nvAgent.enabled == true)
                    //{
                    //    nvAgent.speed = Status.MoveSpeed;
                    //    nvAgent.destination = target.transform.position;
                    //    nvAgent.Resume();

                    //    animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
                    //    animator.SetBool(Defines.ANI_PARAM_TRACE, true);
                    //}
                    break;

                case CharacterState.attack:

                    if (target != null)
                    {
                        animator.SetBool(Defines.ANI_PARAM_ATTACK, true);
                        animator.SetBool(Defines.ANI_PARAM_TRACE, false);
                        transform.LookAt(target);
                        animator.speed = Status.AttackSpeed;
                    }
                    EnemyState = CharacterState.idle;

                    break;

                case CharacterState.hit:
                    nvAgent.Stop();
                    //if (nvAgent.enabled == true)
                    //    nvAgent.Stop();

                    animator.SetTrigger(Defines.ANI_PARAM_Hit);
                    
                    break;
            }
            yield return null;
        }
    }

    //죽었을 때
    public void EnemyDeath(bool _isBoss = false)
    {
        if (EnemyState == CharacterState.die)
            return;

        Status.HP = 0;

        StopAllCoroutines();

        mat.SetFloat(Defines.SHADER_PROPERTY_EMISSION, 0.0f);
        DynamicGI.UpdateMaterials(renderer);
        DynamicGI.UpdateEnvironment();

        EnemyState = CharacterState.die;

        //nvObst.enabled = false;

        if (nvAgent.enabled)
            nvAgent.Stop();

        nvAgent.enabled = false;

        animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
        animator.SetBool(Defines.ANI_PARAM_TRACE, false);
        animator.SetBool(Defines.ANI_PARAM_BOOL_DIE, true);
        animator.SetTrigger(Defines.ANI_PARAM_DIE);

        if (!_isBoss)
        {
            if (!GameMgr.isEvent)
            {
                int gold = 10 * GameMgr.PlayData.PlayerData.CurrentFloor;
                GameMgr.PlayData.PlayerData.Gold += gold;
                GameMgr.Main_UI.SetDamageText(DamageText_Type.Reword, transform.localPosition, gold);
            }
            GameMgr.spawnManager.listActives.Remove(this);
            GameMgr.nCurrentDeadNPC++;
        }
    }

    public void EndDie()
    {
        if (Status.Type == 1)
        {
            transform.parent = GameMgr.spawnManager.trNPC_Pool.GetChild(1).GetChild(0);
            GameMgr.spawnManager.meleeNPC_Pool.Add(this);
        }
        else
        {
            transform.parent = GameMgr.spawnManager.trNPC_Pool.GetChild(1).GetChild(1);
            GameMgr.spawnManager.rangeNPC_Pool.Add(this);
        }

        //StartCoroutine(DieAction());
        this.transform.gameObject.SetActive(false);
    }

    protected void PlayerDie()
    {
        nvAgent.Stop();
        //if (nvAgent.enabled == true)
        //{
        //    nvAgent.Stop();
        //    nvAgent.enabled = false;
        //}

        //if (nvObst.enabled == false)
        //    nvObst.enabled = true;

        animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
        animator.SetBool(Defines.ANI_PARAM_TRACE, false);

        EnemyState = CharacterState.idle;

        StopAllCoroutines();
    }

    private IEnumerator WaitSlowTime(float _time)
    {
        SlowTime = _time;
        while (SlowTime > 0)
        {
            SlowTime -= Time.deltaTime;
            yield return null;
        }
        Status.AttackSpeed = InitAttakSpeed;
        Status.MoveSpeed = InitMoveSpeed;
    }
}
