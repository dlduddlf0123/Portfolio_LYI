using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rabyrinth.ReadOnlys;

public class PlayerCharacter : Character
{
    protected GameManager GameMgr = null;

    // 플레이어의 State(CharacterAttribute.cs)
    public CharacterState PlayerState { get; protected set; }

    public WeaponState PlayerWeaponState { get; protected set; }

    // 스폰된 적들의 리스트
    public List<NPC> listTarget { get; private set; }
    // 적 타켓의 위치
    public Transform targetTr;
    // 타겟의 정보 ( HP,AttackRange 등등)
    public NPC MainTarget;

    // 크리티컬 확률을 계산하기위한 랜덤변수
    public int CriticalNum;
    public int animationHitTime;

    // 네비게이션과 애니메이션
    public UnityEngine.AI.NavMeshAgent nvAgent;
    protected Animator animator;
    // 타겟과 플레이어 사이의 거리
    
    protected float sqrAttackRange;
    public Transform PlayerHead;

    public int SP {  get; set; }

    private float SP_SpawnSpeed;
    private float BuffTime;

    private ParticleSystem[] hitEffect;
    protected GameObject BarrierEffect;

    protected bool isBarrier;

    protected override void Init() { }
    protected override void ChildAwake() { }
    protected virtual void Attack() { }
    protected virtual void OnTrail() { }
    protected virtual void OffTrail() { }
    protected virtual void UseSkill() { }
    public virtual void UseBuffSkill(float _time, System.Action _start, System.Action _end) { }
    public virtual void SetTrail(float range, WeaponState _tyoe) { }

    protected override void DoAwake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;

        hitEffect = new ParticleSystem[transform.GetChild(transform.childCount - 1).childCount];

        for (int index = 0; index < hitEffect.Length; index++ )
            hitEffect[index] = transform.GetChild(transform.childCount - 1).GetChild(index).GetComponent<ParticleSystem>();

        BarrierEffect = transform.GetChild(transform.childCount - 2).gameObject;

        listTarget = new List<NPC>();

        GameMgr.playerCamera.Target = this;
        GameMgr.playerCamera.Init();

        Status = PlayerAttribute.Create();

        PlayerState = CharacterState.idle;

        isBarrier = false;

        animator = gameObject.GetComponent<Animator>();

        nvAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        GameMgr.Main_Cam.GetComponent<PlayerCamera>().Init();

        SpInit();

        ChildAwake();
    }

    protected void SpInit()
    {
        SP = 0;
        SP_SpawnSpeed = 1.0f;
        BuffTime = 0.0f;
        GameMgr.Main_UI.SpBarSet(0);
    }

    IEnumerator SP_Spawn()
    {
        while (PlayerState != CharacterState.die)
        {
            yield return new WaitForSeconds(1.0f / SP_SpawnSpeed);

            if (SP < 10)
                SP++;
            else
                UseSkill();

            GameMgr.Main_UI.SpBarSet(SP);
        }
    }

    public void SetListNPC(List<NPC> _npc)
    {
        if (listTarget == null)
            listTarget = new List<NPC>();

        listTarget.Clear();
        for(int index = 0; index < _npc.Count; index++)
            listTarget.Add(_npc[index]);
    }

    public void StartTrace()
    {
        gameObject.SetActive(true);
        nvAgent.enabled = true;

        StartCoroutine(this.SP_Spawn());
        StartCoroutine(this.PlayerStateCheck());
        StartCoroutine(this.PlayerAction());
    }
    public void TakeRecovery(SkillType _type, float _value)
    {
        switch (_type)
        {
            case SkillType.RecoveryField:
                if (Status.HP < Status.MaxHP)
                    Status.HP += (int)((float)Status.MaxHP * (float)_value);
                hitEffect[(int)HitEffect.RecoveryHP].Play();
                break;
            case SkillType.SP_RecoveryField:
                hitEffect[(int)HitEffect.RecoverySP].Play();
                SP_SpawnSpeed = _value;
                if (BuffTime <= 0.0f)
                    StartCoroutine(SpRecoverySpeedUp());

                BuffTime = 0.4f;
                break;
        }
    }

    private IEnumerator SpRecoverySpeedUp()
    {
        while (BuffTime > 0)
        {
            BuffTime -= Time.deltaTime;
            yield return null;
        }
        SP_SpawnSpeed = 1.0f;
    }

    private IEnumerator Barrier(float _time)
    {
        isBarrier = true;
        BarrierEffect.SetActive(true);

        yield return new WaitForSeconds(_time);

        isBarrier = false;
        BarrierEffect.SetActive(false);
    }
    public void OnBarrier(float _time)
    {
        StartCoroutine(Barrier(_time));
    }

    public override void TakeDamage(int attackdamage, HitEffect _type, bool isCritical = false)
    {
        if (PlayerState == CharacterState.die)
            return;

        attackdamage = GameMgr.DamageReduse(attackdamage, Status.Defense);

        //if (isBarrier)
        //    attackdamage = (int)((float)attackdamage * GameMgr.PlayData.GameData.lSkillData[(int)SkillType.Barrier].damage);
        if (isBarrier)
            attackdamage = 0;
        //크리티컬이면
        //PlayerState = CharacterState.hit;
        switch (_type)
        {
            case HitEffect.Elect:
                hitEffect[(int)HitEffect.Elect].Play();
                break;
            case HitEffect.Fire:
                hitEffect[(int)HitEffect.Fire].Play();
                break;
            case HitEffect.Default:
                hitEffect[(int)HitEffect.Default].Play();
                break;
            case HitEffect.Freez:
                hitEffect[(int)HitEffect.Freez].Play();
                break;
            case HitEffect.Dizziness:
                hitEffect[(int)HitEffect.Dizziness].Play();
                break;
        }

        Status.HP -= attackdamage;
        GameMgr.Main_UI.SetDamageText(DamageText_Type.Player, transform.localPosition, attackdamage, isCritical);
        GameMgr.Main_UI.PlayerHpBar.DamageSet((float)Status.HP, (float)Status.MaxHP);

        //if ((attackdamage - Status.Defense) > 0)
        //{
        //    //PlayerKing.Status.HP -= EnemyCharacter.Status.AttackDamage - PlayerKing.Status.Defense
        //    Status.HP -= attackdamage;
        //    GameMgr.Main_UI.SetDamageText(DamageText_Type.Player, transform.localPosition, attackdamage, isCritical); ;
        //    HPBar.DamageSet(attackdamage, GameMgr.Player.Status.HP, GameMgr.Player.Status.MaxHP);
        //}
        //else
        //{
        //    Status.HP -= 1;
        //    GameMgr.Main_UI.SetDamageText(DamageText_Type.Player, transform.localPosition, 1, isCritical);
        //    HPBar.DamageSet(attackdamage, GameMgr.Player.Status.HP, GameMgr.Player.Status.MaxHP);
        //}
    
        // HP가 0 이하거나 State가 die가 아닐경우
        if (Status.HP <= 0)
            Death();

        //HP
        //Debug.Log("Player HP : " + Status.HP);
        //gameManager.MainUi.SetDamagePos(transform.localPosition, damageString.ToString());
        //float dam = Status.HP / Status.MaxHP;

        //gameManager.MainUi.SetHPPos(transform.localPosition, dam);
    }
    // PlyaerCharacter의 State를 체크하는 코루틴

    protected IEnumerator PlayerStateCheck()
    {
        // die가 아닐경우
        while (PlayerState != CharacterState.die)
        {
            yield return new WaitForSeconds(0.2f);

            // 타겟지정  
            targetTr = SetTarget();

            if (PlayerState == CharacterState.skill)
                continue;

            if (targetTr != null)
            {
                GameMgr.Main_UI.UpdateNPC_HpBarValue();
                GameMgr.Main_UI.TargetHpBar.gameObject.SetActive(true);
                // sqrMagnitude = Magnitude(벡터의 길이(크기)를 반환)보다 CPU에 부담을 덜주는 연산, 루트연산을 하지 않고 값을 반환한다, 실질적인 벡터의 길이
                // sqrAttackRange = 타겟과 플레이어의 거리차이
                Vector3 range = targetTr.transform.position - transform.position;
                sqrAttackRange = range.sqrMagnitude;

                // 플레이어의 공격범위보다 타겟과의 거리가 작다면( 타겟이 공격범위 안으로 들어왔을 경우 )
                if (sqrAttackRange <= Status.AttackRange * Status.AttackRange)
                    PlayerState = CharacterState.attack;
                else
                    PlayerState = CharacterState.trace;
            }
            else {
                GameMgr.Main_UI.TargetHpBar.gameObject.SetActive(false);
                PlayerState = CharacterState.idle;
            }
        }
    }
    // State에 따른 Player의 Action
    IEnumerator PlayerAction()
    {
        GameMgr.Main_UI.PlayerHpBar.gameObject.SetActive(true);
        // die가 아니라면
        while (PlayerState != CharacterState.die)
        {
            GameMgr.Main_UI.UpdatePlayerHpBar();
            GameMgr.Main_UI.UpdateNPC_HpBarPosition();

            switch (PlayerState)
            {
                // 아무것도 아닌상태, Attack이나 Trace를 하지 않는다
                case CharacterState.idle:

                    OffTrail();
                    animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
                    animator.SetBool(Defines.ANI_PARAM_TRACE, false);
                    break;
                // 공격범위 안에 타겟이 없을경우 trace
                case CharacterState.trace:

                    animator.speed = Status.MoveSpeed * 0.2f;
                    nvAgent.speed = Status.MoveSpeed;
                    // 플레이어의 목표는 타겟의 포지션
                    nvAgent.destination = targetTr.transform.position;
                    nvAgent.Resume();

                    OffTrail();

                    animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
                    animator.SetBool(Defines.ANI_PARAM_TRACE, true);
                    break;
                // 공격 범위 안에 타겟이 있을경우 attack
                case CharacterState.attack:

                    if (targetTr != null)
                    {
                        animator.SetBool(Defines.ANI_PARAM_ATTACK, true);
                        animator.SetBool(Defines.ANI_PARAM_TRACE, false);
                        transform.LookAt(targetTr);
                        animator.speed = Status.AttackSpeed;
                    }
                    break;
                case CharacterState.skill:
                    OnTrail();
                    if (targetTr != null)
                        nvAgent.destination = targetTr.transform.position;
                    break;
                case CharacterState.hit:
                    nvAgent.Stop();
                    animator.SetTrigger(Defines.ANI_PARAM_Hit);
                    break;
            }
            yield return null;
        }
    }
    // 타겟
    public Transform SetTarget()
    {
        //if (PlayerState == CharacterState.attack && MainTarget != null && MainTarget.Status.HP > 0)
        //    return MainTarget.transform;

        // 타겟으로 정할 몬스터
        NPC _targetNPC = null;

        // 거리를 비교할 변수
        float dist = 0.0f;
        float tempDist = float.MaxValue;

        // objectPool 에 담긴 최대 몬스터 갯수만큼 반복
        for (int i = 0; i < listTarget.Count; i++)
        {
            // i번째 몬스터가 die상태가 아니라면
            if (listTarget[i].EnemyState != CharacterState.die)
            {
                // Vector3.Distance(a,b) : a와 b사이의 거리
                Vector3 range = listTarget[i].transform.position - transform.position;
                dist = range.sqrMagnitude;

                // 타겟몬스터는 = (타겟과 플레이어 사이가) tempDist(MaxValue) 보다 작다면 ? i번째 몬스터, 아니면 타겟몬스터(재추적)
                _targetNPC = dist <= tempDist ? listTarget[i] : _targetNPC;
                // tempDist는 두 사이의 거리가 MaxValue보다 작다면 dist, 아니면 tempdist
                tempDist = dist < tempDist ? dist : tempDist;
            }
        }

        // 타겟몬스터가 null이 아니라면
        if (_targetNPC != null)
        {
            // targetEnemy(적에대한 정보(EnemyCharacter.init)) = 타겟몬스터
            MainTarget = _targetNPC;

            // 타겟몬스터의 위치를 반환
            return _targetNPC.transform;
        }
        else
            return null;
    }
 
    public void InitStat()
    {
        Init();
    }

    // HP가 0 이하인경우
    public void Death()
    {
        if (PlayerState == CharacterState.die)
            return;

        GameMgr.isEvent = false;

        GameMgr.Main_UI.PlayerHpBar.gameObject.SetActive(false);

        StopAllCoroutines();

        PlayerState = CharacterState.die;

        // 애니메이터 처리
        animator.SetTrigger(Defines.ANI_PARAM_DIE);
        animator.SetBool(Defines.ANI_PARAM_ATTACK, false);
        animator.SetBool(Defines.ANI_PARAM_TRACE, false);
        animator.SetBool(Defines.ANI_PARAM_USE_SKILL, false);

        Debug.Log("DEAD");
        // NavMesh를 Stop하여 움직일 수 없음
        nvAgent.Stop();
        //Application.LoadLevel(Application.loadedLevel);

        StartCoroutine(GameMgr.spawnManager.SpawnEnd());
    }
}
