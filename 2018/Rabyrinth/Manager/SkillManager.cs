using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rabyrinth.ReadOnlys;

public class SkillManager : MonoBehaviour {

    private GameManager GameMgr;
    private Transform MagicPoolTr;

    private Queue<Meteor> qMeteors;

    private ParticleSystem obj_IceField;
    private ParticleSystem obj_DimensionField;
    private ParticleSystem obj_RecoveryField;
    private ParticleSystem obj_SP_RecoveryField;

    private ParticleSystem obj_IceField_BG;
    private ParticleSystem obj_RecoveryField_BG;
    private ParticleSystem obj_SP_RecoveryField_BG;

    private Vector3 diagonal;

    private void Awake()
    {
        GameMgr = MonoSingleton<GameManager>.Inst;
        MagicPoolTr = GameMgr.MagicPoolTr;

        diagonal = new Vector3(-1.0f, 0, -1.0f);

        qMeteors = new Queue<Meteor>();
        for (int index = 0; index < MagicPoolTr.GetChild(0).GetChild(0).childCount; index++)
        {
            Meteor _meteor = new Meteor
            {
                meteor = MagicPoolTr.GetChild(0).GetChild(0).GetChild(index),
                explosion = MagicPoolTr.GetChild(0).GetChild(1).GetChild(index),
            };
            qMeteors.Enqueue(_meteor);
        }

        obj_IceField = MagicPoolTr.GetChild(1).GetComponent<ParticleSystem>();
        obj_DimensionField = MagicPoolTr.GetChild(2).GetComponent<ParticleSystem>();
        obj_RecoveryField = MagicPoolTr.GetChild(3).GetComponent<ParticleSystem>();
        obj_SP_RecoveryField = MagicPoolTr.GetChild(4).GetComponent<ParticleSystem>();

        obj_IceField_BG = obj_IceField.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        obj_RecoveryField_BG = obj_RecoveryField.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        obj_SP_RecoveryField_BG = obj_SP_RecoveryField.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 스킬 사용
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_type"></param>
    /// <param name="_range"></param>
    public void UseSkill(Vector3 _pos, Skill _skill)
    {
        switch (_skill.type)
        {
            case SkillType.Meteor:
                StartCoroutine(MeteorSwarm(_pos, _skill));
                break;
            case SkillType.IceField:
                SetFieldEffectSize(_skill.type, _skill.range);
                StartCoroutine(IceField(_pos, _skill));
                break;
            case SkillType.DimensionField:
                SetFieldEffectSize(_skill.type, _skill.range);
                StartCoroutine(DimensionField(_pos, _skill));
                break;
            case SkillType.RecoveryField:
                SetFieldEffectSize(_skill.type, _skill.range);
                StartCoroutine(RecoveryField(_pos, _skill));
                break;
            case SkillType.SP_RecoveryField:
                SetFieldEffectSize(_skill.type, _skill.range);
                StartCoroutine(RecoveryField(_pos, _skill));
                break;
            case SkillType.Barrier:
                GameMgr.Player.OnBarrier(_skill.time);
                break;
            default:
                if(_skill.type == SkillType.PlassmaSword ||
                    _skill.type == SkillType.FlameSword)
                {
                    float speed = GameMgr.Player.Status.AttackSpeed;
                    float move = GameMgr.Player.Status.MoveSpeed;
                    float range = GameMgr.Player.Status.AttackRange;
                    int attack = GameMgr.Player.Status.AttackPoint;
                    GameMgr.Player.UseBuffSkill(_skill.time,
                        () =>
                        {
                            GameMgr.Player.Status.AttackSpeed *= _skill.range;
                            GameMgr.Player.Status.MoveSpeed *= _skill.range;
                            GameMgr.Player.Status.AttackRange *= _skill.range;
                            GameMgr.Player.Status.AttackPoint =
                                (int)((float)GameMgr.Player.Status.AttackPoint *
                                (float)_skill.damage);
                            if (_skill.type == SkillType.PlassmaSword)
                                GameMgr.Player.SetTrail(
                                    (_skill.range - 1.0f) * GameMgr.Player.Status.AttackRange,
                                    WeaponState.Elect);
                            else
                                GameMgr.Player.SetTrail(
                                    (_skill.range - 1.0f) * GameMgr.Player.Status.AttackRange,
                                    WeaponState.Fire);
                        },
                        () =>
                        {
                            GameMgr.Player.Status.AttackSpeed = speed;
                            GameMgr.Player.Status.MoveSpeed = move;
                            GameMgr.Player.Status.AttackRange = range;
                            GameMgr.Player.Status.AttackPoint = attack;
                            GameMgr.Player.SetTrail(0.0f, WeaponState.Default);
                        });
                }
                break;
        }
    }
    /// <summary>
    /// 메테오
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_range"></param>
    /// <returns></returns>
    private IEnumerator MeteorSwarm(Vector3 _pos, Skill _skill)
    {
        float time = 0.0f;
        while (time <= _skill.time)
        {
            if (qMeteors.Count > 0)
            {
                Vector3 randPos = Random.insideUnitCircle * _skill.range;
                Vector3 meteorSpawnPos = new Vector3(_pos.x + randPos.x + 10.0f, _pos.y + 10.0f, _pos.z + randPos.z);
                Vector3 explostionPos = new Vector3(_pos.x + randPos.x, 0.0f, _pos.z + randPos.z);

                Meteor _meteor = qMeteors.Dequeue();

                _meteor.meteor.position = meteorSpawnPos;
                _meteor.meteor.gameObject.SetActive(true);

                StartCoroutine(Throw(_meteor, explostionPos, _pos, _skill));
            }
            yield return new WaitForSeconds(0.3f);
            time += 0.3f;
        }
    }

    private IEnumerator Throw(Meteor _meteor, Vector3 _spawnPos, Vector3 _usePos, Skill _skill)
    {
        while (_meteor.meteor.position.y > 0.5f)
        {
            _meteor.meteor.Translate((_spawnPos - _meteor.meteor.transform.position).normalized * Time.deltaTime * 20.0f);
            yield return null;
        }
        _meteor.meteor.gameObject.SetActive(false);

        _meteor.explosion.position = _spawnPos;
        _meteor.explosion.gameObject.SetActive(true);

        float dist = 0.0f;

        for (int i = 0; i < GameMgr.Player.listTarget.Count; i++)
        {
            // i번째 몬스터가 die상태가 아니라면
            if (GameMgr.Player.listTarget[i].EnemyState != CharacterState.die)
            {
                // Vector3.Distance(a,b) : a와 b사이의 거리
                Vector3 range = GameMgr.Player.listTarget[i].transform.position - _usePos;
                dist = range.sqrMagnitude;

                if (dist <= _skill.range * _skill.range)
                    GameMgr.Player.listTarget[i].TakeDamage(
                        (int)(GameMgr.DamageReduse(
                        GameMgr.Player.Status.AttackPoint,
                        GameMgr.Player.listTarget[i].Status.Defense) *
                        _skill.damage), HitEffect.Fire);
            }
        }

        StartCoroutine(EnqeueMeteor(_meteor));
    }

    private IEnumerator EnqeueMeteor(Meteor _meteor)
    {
        yield return new WaitForSeconds(1.0f);

        _meteor.explosion.gameObject.SetActive(false);

        qMeteors.Enqueue(_meteor);
    }

    /// <summary>
    /// 아이스 필드
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_range"></param>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator IceField(Vector3 _pos, Skill _skill)
    {
        obj_IceField.transform.position = _pos;
        obj_IceField.gameObject.SetActive(true);

        float time = 0.0f;

        while (time <= _skill.time)
        {
            float dist = 0.0f;

            for (int i = 0; i < GameMgr.Player.listTarget.Count; i++)
            {
                if (GameMgr.Player.listTarget[i].EnemyState != CharacterState.die)
                {
                    Vector3 range = GameMgr.Player.listTarget[i].transform.position - _pos;
                    dist = range.sqrMagnitude;

                    if (dist <= _skill.range * _skill.range)
                    {
                        GameMgr.Player.listTarget[i].TakeDamage(
                        (int)(GameMgr.DamageReduse(
                        GameMgr.Player.Status.AttackPoint,
                        GameMgr.Player.listTarget[i].Status.Defense) *
                        _skill.damage),
                        HitEffect.Freez);

                        GameMgr.Player.listTarget[i].Slow(1.0f, 0.8f);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
            time += 0.5f;
        }

        obj_IceField.gameObject.SetActive(false);
    }

    /// <summary>
    /// 디멘션 필드
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_range"></param>
    /// <returns></returns>
    private IEnumerator DimensionField(Vector3 _pos, Skill _skill)
    {
        obj_DimensionField.transform.position = _pos;
        obj_DimensionField.Play();

        List<Transform> listTr = new List<Transform>();
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < GameMgr.Player.listTarget.Count; i++)
        {
            if (GameMgr.Player.listTarget[i].EnemyState != CharacterState.die)
            {
                Vector3 range = GameMgr.Player.listTarget[i].transform.position - _pos;
                
                float dist = range.sqrMagnitude;

                if (dist <= _skill.range * _skill.range)
                {
                    GameMgr.Player.listTarget[i].TakeDamage(
                        (int)(GameMgr.DamageReduse(
                        GameMgr.Player.Status.AttackPoint,
                        GameMgr.Player.listTarget[i].Status.Defense) *
                        _skill.damage),
                        HitEffect.Dizziness);

                    listTr.Add(GameMgr.Player.listTarget[i].transform);

                    //RaycastHit rangeRay;
                    //range.y += 0.1f;
                    //Debug.DrawRay(transform.position, range * _range, Color.red);

                    //int layerMask = 1 << 8;
                    //layerMask = ~layerMask;

                    //if (Physics.Raycast(transform.position, range, out rangeRay, _range, layerMask))
                    //{
                    //    if (rangeRay.collider.CompareTag("NPC"))
                    //    {
                    //        GameMgr.Player.listTarget[i].TakeDamage(
                    //            (int)(GameMgr.Player.Status.AttackPoint), HitEffect.Dizziness);

                    //        GameMgr.Player.listTarget[i].transform.position = _pos;
                    //    }
                    //}
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
        for (int index = 0; index < listTr.Count; index++)
            listTr[index].position = _pos;
    }

    /// <summary>
    /// 리커버리 필드
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_range"></param>
    /// <param name="_time"></param>
    /// <param name="_type"></param>
    /// <returns></returns>
    private IEnumerator RecoveryField(Vector3 _pos, Skill _skill)
    {
        GameObject obj = _skill.type == SkillType.RecoveryField ?
            obj_RecoveryField.gameObject : obj_SP_RecoveryField.gameObject;

        obj.transform.position = _pos;
        obj.SetActive(true);

        float time = 0.0f;

        while (time <= _skill.time)
        {
            float dist = 0.0f;

            if (GameMgr.Player.PlayerState != CharacterState.die)
            {
                Vector3 range = GameMgr.Player.transform.position - _pos;
                dist = range.sqrMagnitude;

                if (dist <= _skill.range * _skill.range)
                    GameMgr.Player.TakeRecovery(_skill.type, _skill.damage);
            }

            yield return new WaitForSeconds(0.4f);
            time += 0.4f;
        }

        obj.SetActive(false);
    }

    private void SetFieldEffectSize(SkillType _type, float range)
    {
        obj_IceField = MagicPoolTr.GetChild(1).GetComponent<ParticleSystem>();
        obj_DimensionField = MagicPoolTr.GetChild(2).GetComponent<ParticleSystem>();
        obj_RecoveryField = MagicPoolTr.GetChild(3).GetComponent<ParticleSystem>();
        obj_SP_RecoveryField = MagicPoolTr.GetChild(4).GetComponent<ParticleSystem>();
        switch (_type)
        {
            case SkillType.IceField:
                obj_IceField.startSize = range * 3;
                obj_IceField_BG.startSize = range * 3;
                break;
            case SkillType.DimensionField:
                obj_DimensionField.startSize = range * 5;
                break;
            case SkillType.RecoveryField:
                obj_RecoveryField.startSize = range * 3;
                obj_RecoveryField_BG.startSize = range * 3;
                break;
            case SkillType.SP_RecoveryField:
                obj_SP_RecoveryField.startSize = range * 3;
                obj_SP_RecoveryField_BG.startSize = range * 3;
                break;
        }
    }
}

public class Meteor
{
    public Transform meteor;
    public Transform explosion;
}

public class Skill
{
    public SkillType type;
    public int level;
    public float damage;
    public float range;
    public float time;
    public float cooltime;
}
