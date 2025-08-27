using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Rabyrinth.ReadOnlys;

public class PlayerKnight: PlayerCharacter
{
    public int Char_Index;

    public MeleeWeaponTrail trail;
    public Transform TrailEnd;
    private Vector3 TrailEndPos;
    private Vector3 TrailEndPos_Wheel;
    private Color TrailColor;
    private GameObject[] WeaponElectEffect;
    private GameObject WeaponFireEffect;

    private float TrailRange;

    private bool useSkill;

    protected override void ChildAwake()
    {
        WeaponElectEffect = new GameObject[4];
        for(int index = 0; index < 4; index++)
            WeaponElectEffect[index] = trail.transform.parent.GetChild(index+2).gameObject;

        WeaponFireEffect = trail.transform.parent.GetChild(6).gameObject;

        TrailRange = 0.0f;
        TrailColor = new Color(0.47f, 1.0f, 1.0f, 1.0f);

        TrailEndPos = new Vector3(
            TrailEnd.transform.localPosition.x,
            TrailEnd.transform.localPosition.y,
            TrailEnd.transform.localPosition.z);

        TrailEndPos_Wheel = new Vector3(
            TrailEnd.transform.localPosition.x,
            TrailEnd.transform.localPosition.y,
            TrailEnd.transform.localPosition.z + 1.0f);
    }

    protected override void Init()
    {

        TrailEnd.transform.localPosition = TrailEndPos;
        useSkill = false;

        isBarrier = false;
        BarrierEffect.SetActive(false);

        Status.Type = GameMgr.PlayData.GameData.lCharData[Char_Index].Type;

        if (GameMgr.isEvent)
        {
            Status.MaxHP = Status.HP = 2500000;

            Status.AttackPoint = 50000;

            Status.Defense = 80000;

        }
        else
        {
            Status.MaxHP = Status.HP =
                GameMgr.PlayData.GameData.lCharData[Char_Index].HP * GameMgr.PlayData.PlayerData.MaxFloor;
            Status.AttackPoint =
                GameMgr.PlayData.GameData.lCharData[Char_Index].Attack * GameMgr.PlayData.PlayerData.MaxFloor;
            Status.Defense =
                GameMgr.PlayData.GameData.lCharData[Char_Index].Defense * GameMgr.PlayData.PlayerData.MaxFloor;
        }

        Status.MoveSpeed =
            GameMgr.PlayData.GameData.lCharData[Char_Index].MoveSpeed;
        Status.AttackSpeed =
            GameMgr.PlayData.GameData.lCharData[Char_Index].AttackSpeed;
        Status.AttackRange =
            GameMgr.PlayData.GameData.lCharData[Char_Index].AttackRange;
        Status.CriticalChance =
            GameMgr.PlayData.GameData.lCharData[Char_Index].Critical;
        Status.CriticalBonus =
            GameMgr.PlayData.GameData.lCharData[Char_Index].Critical_Damage;
        Status.FindRange =
            GameMgr.PlayData.GameData.lCharData[Char_Index].FindRange;

        PlayerState = CharacterState.idle;
        PlayerWeaponState = WeaponState.Default;
        SpInit();

        if (GameMgr.Main_UI.PlayerHpBar.Hp_Bar != null)
            GameMgr.Main_UI.PlayerHpBar.Hp_Bar.value = 1.0f;
	}

    private IEnumerator UseBuff(float _time, System.Action _start, System.Action _end)
    {
        _start();

        yield return new WaitForSeconds(_time);

        _end();
    }

    public override void UseBuffSkill(float _time, System.Action _start, System.Action _end)
    {
        StartCoroutine(UseBuff(_time, _start, _end));
    }

    public override void SetTrail(float range, WeaponState _type)
    {
        TrailRange = range;

        TrailEnd.transform.localPosition = new Vector3(
            TrailEndPos.x,
            TrailEndPos.y,
            TrailEndPos.z + TrailRange);

        PlayerWeaponState = _type;

        switch(_type)
        {
            case WeaponState.Default:
                trail._colors[0] = TrailColor;
                trail.SetOffset(0.4375f);
                WeaponFireEffect.SetActive(false);
                for (int index = 0; index < 4; index++)
                    WeaponElectEffect[index].SetActive(false);
                break;
            case WeaponState.Elect:
                trail._colors[0] = Color.white;
                trail.SetOffset(0.56275f);
                WeaponFireEffect.SetActive(false);
                for (int index = 0; index < 4; index++)
                    WeaponElectEffect[index].SetActive(true);
                break;
            case WeaponState.Fire:
                trail._colors[0] = Color.white;
                trail.SetOffset(0.688f);
                WeaponFireEffect.SetActive(true);
                for (int index = 0; index < 4; index++)
                    WeaponElectEffect[index].SetActive(false);
                break;
        }
    }

    protected override void OnTrail()
    {
        trail.Emit = true;
    }
    protected override void OffTrail()
    {
        trail.Emit = false;
    }

    protected override void UseSkill()
    {
        if (useSkill || PlayerState == CharacterState.die)
            return;

        useSkill = true;
        StartCoroutine(WheelWind(3.0f));
    }


    IEnumerator WheelWind(float _time)
    {
        while(PlayerState != CharacterState.attack)
            yield return null;

        if (PlayerState == CharacterState.die)
            yield break;

        StartCoroutine(GameMgr.playerCamera.ShakeCamera(3.0f, 0.12f, ShakeType.hor));

        //TrailEnd.Translate(Vector3.forward * 1.5f);
        SP = 0;
        //trail._colors[0] = trailColor;
        animator.speed = 1.0f;
        animator.SetBool(Defines.ANI_PARAM_USE_SKILL, true);
        animator.SetTrigger("isSkill");
        nvAgent.speed = Status.MoveSpeed * 1.2f;
        nvAgent.Resume();

        float time = 0.0f;
        while(time < _time)
        {
            TrailEnd.transform.localPosition = TrailEndPos_Wheel;
            GameMgr.Main_UI.UpdateNPC_HpBarValue();
            PlayerState = CharacterState.skill;
            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        nvAgent.speed = Status.MoveSpeed;
        TrailEnd.transform.localPosition = new Vector3(
            TrailEndPos.x,
            TrailEndPos.y,
            TrailEndPos.z + TrailRange);

        PlayerState = CharacterState.idle;
        OffTrail();
        animator.SetBool(Defines.ANI_PARAM_USE_SKILL, false);
        //trail._colors[0] = Color.red;
        useSkill = false;
    }

    public void RoundRangeAttack()
    {
        Time.timeScale = 0.66f;
        float dist = 0.0f;

        for (int i = 0; i < listTarget.Count; i++)
        {
            // i번째 몬스터가 die상태가 아니라면
            if (listTarget[i].EnemyState != CharacterState.die)
            {
                // Vector3.Distance(a,b) : a와 b사이의 거리
                Vector3 range = listTarget[i].transform.position - transform.position;
                dist = range.sqrMagnitude;

                float _dist = 4.0f + TrailRange;
                if (dist <= _dist * _dist)
                    listTarget[i].TakeDamage(
                        (int)((float)GameMgr.DamageReduse(
                        Status.AttackPoint, listTarget[i].Status.Defense) * Status.CriticalBonus),
                        HitEffect.Default, true);
            }
        }
    }

    // 범위공격
    public void RangeAttack()
    {
        if (targetTr == null)
            return;

        float dist = 0.0f;
        Time.timeScale = 0.75f;
        // 플레이어와 메인타겟과의 벡터

        Vector3 vecMainTarget = targetTr.transform.position - transform.position;

        for (int i = 0; i < listTarget.Count; i++)
        {
            if (listTarget[i].EnemyState != CharacterState.die)
            {
                // dist = 플레이어와 타겟몬스터와의 거리
                Vector3 range = listTarget[i].transform.position - transform.position;
                dist = range.sqrMagnitude;

                float degree = PlayerWeaponState != WeaponState.Default ? -0.75f : -0.5f;
                // 범위 안에 들어온 모든 i번째 몬스터에게 AttackRangeEnemy
                if (dist <= Status.AttackRange * Status.AttackRange &&
                    Vector3.Dot(vecMainTarget, range) > degree)
                    AttackRangeEnemy(listTarget[i]);
            }
        }
    }

    public void AttackRangeEnemy(NPC _enemy)
    {
        bool isCritical = Random.Range(0, 101) <= Status.CriticalChance ? true : false;

        if (MainTarget.Status.HP > 0)
        {
            if(isCritical)
            {
                Time.timeScale = 0.5f;

                if (!GameMgr.playerCamera.isShake)
                    StartCoroutine(GameMgr.playerCamera.ShakeCamera(0.1f, 0.1f, ShakeType.ver));

                if (_enemy == MainTarget)
                    _enemy.TakeDamage((int)(GameMgr.DamageReduse(Status.AttackPoint, _enemy.Status.Defense) *
                        (isCritical ? Status.CriticalBonus : 1.0f)), HitEffect.Default, isCritical);
                else
                    _enemy.TakeDamage((int)(GameMgr.DamageReduse(
                        Status.AttackPoint, _enemy.Status.Defense) *
                        0.33f * (isCritical ? Status.CriticalBonus : 1.0f)),
                        HitEffect.Default, isCritical);
            }
            else
            {
                if (_enemy == MainTarget)
                    _enemy.TakeDamage((int)(GameMgr.DamageReduse(Status.AttackPoint, _enemy.Status.Defense)), HitEffect.Default, isCritical);
                else
                    _enemy.TakeDamage((int)(GameMgr.DamageReduse(Status.AttackPoint, _enemy.Status.Defense) * 0.33f),
                        HitEffect.Default, isCritical);
            }
        }
    }

    public void SetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    //public void ActiveSkill_1()
    //{
    //    Debug.Log("active skill_1");
    //    Instantiate(Skill1, this.transform.position, this.transform.rotation);
    //}
    //public void ActiveSkill_2()
    //{
    //    Debug.Log("active skill_2");
    //    this.transform.position = Vector3.Lerp(this.transform.position, targetEnemy.transform.position, Time.deltaTime*100.0f);
    //    targetEnemy.TakeDamage(500);
    //}

    //public void ActiveSkill_3()
    //{
    //    Debug.Log("active skill_3");
    //    Instantiate(Skill3, this.transform.position, this.transform.rotation);
    //}

}
