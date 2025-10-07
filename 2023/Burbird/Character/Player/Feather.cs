using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Burbird
{
    public class Feather : MonoBehaviour
    {
        StageManager stageMgr;

        public PlayerShooter shooter;

        public List<Debuff> list_debuff = new List<Debuff>();

        Rigidbody2D m_rigidbody2D;
        Collider2D m_collider;
        Animator m_anim;

        GameObject hitParticle;

        public Enemy hitEnemy;

        public int featherDamage = 1;
        public int weekMultiflier = 2;

        public float featherSpeed = 5f;
        public float featherKnockBackPower = 1f;

        public float lifeTime = 4f;

        bool isGetable = false;
        public bool isHited = false;

        //퍽 효과 관련
        public bool isPierce;
        public bool isBounce;
        public bool isChain;
        public bool isDeath;

        int bounceCount = 0;
        public int chainCount = 0;

        float bounceReduceDMG = 0.3f;
       public float chainReduceDMG = 0.3f;
        float pierceReduceDMG = 0.3f;


        Coroutine currentCoroutine = null;
        private void Awake()
        {
            stageMgr = StageManager.Instance;
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_collider = GetComponent<Collider2D>();
            m_anim = GetComponent<Animator>();

            hitParticle = transform.GetChild(1).gameObject;

        }
        //private void Update()
        //{
        //    if (!m_rigidbody.isKinematic)
        //    {
        //        float angle = Mathf.Atan2(m_rigidbody.velocity.y, m_rigidbody.velocity.x) * Mathf.Rad2Deg;
        //        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //    }
        //}

        private void OnDisable()
        {
            Init();
            StopAllCoroutines();
        }
        public void Init()
        {
            m_rigidbody2D.velocity = Vector2.zero;
            m_rigidbody2D.simulated = true;
            isGetable = false;
            isHited = false;

            hitEnemy = null;

            bounceCount = 0;
            chainCount = 0;
        }

        public void Stop()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Ground") && 
                coll.gameObject.layer != LayerMask.NameToLayer("Platform"))
            {
                if (isHited)
                {
                    return;
                }
                // Debug.Log("Feather Ground!");

                if (!isBounce ||
                    bounceCount > 1)
                {
                    shooter.FeatherListInit(this);
                    gameObject.SetActive(false);
                }
                else
                {
                    //Raycast를 통한 Reflect 구현
                    //현재 진행 벡터
                    Vector2 inputDirection = m_rigidbody2D.velocity;
                    //충돌지점 벡터
                    Vector2 surfaceNormal = coll.ClosestPoint(transform.position) - (Vector2)transform.position;
                    //벽만 체크하는 마스크
                    int characterMask =
                        (1 << LayerMask.NameToLayer("Player")) | 
                        (1 << LayerMask.NameToLayer("Character")) |
                        (1 << LayerMask.NameToLayer("Ignore Raycast")) |
                        (1 << LayerMask.NameToLayer("Platform"));

                    //레이캐스트 진행
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, inputDirection.normalized, 1f, ~characterMask);
                    if (hit.collider != null)
                    {
                        surfaceNormal = hit.normal;
                    }

                    //반사 벡터 받기
                    Vector2 reflectDirection = Vector2.Reflect(inputDirection, surfaceNormal);
           
                    //속도 적용
                    m_rigidbody2D.velocity = reflectDirection;
                    //깃털 방향 전환
                    transform.GetChild(0).rotation = SetFeatherRotation(Vector3.Normalize(m_rigidbody2D.velocity));

                    //깃털 데미지 감소
                    featherDamage = (int)(featherDamage * (1 - bounceReduceDMG));

                    //바운스 횟수 체크
                    bounceCount++;
                }
            }

            if (coll.gameObject.CompareTag("Enemy"))
            {
                if (isHited)
                {
                    return;
                }
                if (hitEnemy != null)
                {
                    if (coll.gameObject.GetComponentInParent<Enemy>().transform.position == hitEnemy.transform.position)
                    {
                        return;
                    }
                }

                isHited = true;

                // Debug.Log("Feather Enemy!");
                hitEnemy = coll.transform.GetComponentInParent<Enemy>();
                //if (lastHitTarget != null)
                //{
                //    if (lastHitTarget.transform == hitEnemy.transform)
                //    {
                //        return;
                //    }
                //}
                //lastHitTarget = hitEnemy.gameObject;

                //타격 이펙트

                //즉사 확률 계산
                //확률 전달?
                if (isDeath)
                {
                    if (!hitEnemy.IsBoss())
                    {
                        //즉사!
                        featherDamage = 999999;
                    }
                }

                //데미지
                hitEnemy.GetDamage(featherDamage, Color.red);
                shooter.particleHolder.PlayParticle_FeatherHit(transform.position);

                if (hitEnemy.enemyController != null)
                {
                    //히트한 적 넉백 효과
                    if (hitEnemy.enemyController.isKnockBackable)
                    {
                        int knockBackDirection = (transform.position.x < hitEnemy.centerTr.position.x) ? 1 : -1;
                        hitEnemy.enemyController.m_rigidbody2D.AddForce(
                            Vector3.right * knockBackDirection * 1f * featherKnockBackPower
                            + Vector3.up * 1f * featherKnockBackPower, ForceMode2D.Impulse);
                    }
                }

                //히트한 적에게 디버프 효과 추가
                for (int i = 0; i < list_debuff.Count; i++)
                {
                    hitEnemy.GetEffect(list_debuff[i]);
                }

                //체인샷일 경우
                if (isChain)
                {
                    //체인 횟수 이내일 때
                    if (chainCount < 2)
                    {
                        chainCount++; //체인 횟수 증가

                        Enemy nextTarget = null; //다음 타겟 체크
                        float currentDist = 0.0f; //거리 체크
                        float targetDist = 10.0f; //튕기는 최소 거리 제한

                        //적 캐릭터 목록 체크
                        for (int i = 0; i < stageMgr.enemySpawner.list_activeEnemy.Count; i++)
                        {
                            //현재 히트한 적은 제외
                            if (hitEnemy == stageMgr.enemySpawner.list_activeEnemy[i])
                            {
                                continue;
                            }

                            //임시 거리 측정
                            currentDist = Vector3.Distance(transform.position,
                                stageMgr.enemySpawner.list_activeEnemy[i].transform.position);

                            //다음 타겟이 있을경우
                            if (nextTarget != null)
                            {
                                //다음 타겟과의 거리 측정
                                targetDist = Vector3.Distance(transform.position, nextTarget.centerTr.position);
                            }

                            //임시 거리가 최소 거리 이하일 경우
                            if (currentDist < targetDist)
                            {
                                //타겟을 임시 타겟으로 지정
                                nextTarget = stageMgr.enemySpawner.list_activeEnemy[i];
                            }
                        }

                        //모든 타겟이 최소거리보다 적었을 경우 다음 타겟 없음, 발사 안함
                        if (nextTarget != null)
                        {
                            //다음 타겟 방향 지정
                            Vector3 fireVec = Vector3.Normalize(nextTarget.centerTr.position - transform.position);

                            //다음 타겟으로 발사
                            shooter.ChainShot(hitEnemy, transform.position,chainCount, fireVec, shooter.minFirePower);
                        }
                    }
                }
                if (isPierce)
                {
                    //관통 시 효과
                    isHited = false;
                    return;
                }

                isHited = true;
                m_rigidbody2D.simulated = false;
                m_rigidbody2D.velocity = Vector2.zero;

                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                    return;
                }
                Stop();
                currentCoroutine = StartCoroutine(LateInit());
            }
        }
        public void FeatherFall()
        {
            isHited = false;
            m_rigidbody2D.simulated = true;
        }


        public void FireFeather(Vector3 moveVec, float power)
        {
            Stop();
            currentCoroutine = StartCoroutine(FeatherMove(moveVec, power));
        }
        public IEnumerator FeatherMove(Vector3 moveVec, float power)
        {
            m_rigidbody2D.velocity = Vector2.zero;
            m_rigidbody2D.AddForce(moveVec * power * 100f * featherSpeed);

            WaitForSeconds waitTime = new WaitForSeconds(0.01f);

            transform.GetChild(0).rotation = Quaternion.identity;
            transform.GetChild(0).rotation = SetFeatherRotation(moveVec);

            float t = 0;

            //4/10/2023-LYI
            //미사일이 사라지지 않는 버그때문에 추가
            while (!isHited &&
                t < lifeTime)
            {
                t += 0.01f;
                if (m_rigidbody2D.velocity == Vector2.zero && 
                    t> 0.3f)
                {
                    break;
                }

                yield return waitTime;
            }

            if (!isHited)
            {
                shooter.FeatherListInit(this);
                gameObject.SetActive(false);
            }
        }

        public Quaternion SetFeatherRotation(Vector3 moveVec)
        {
            float angle = Mathf.Atan2(moveVec.y, moveVec.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }


        IEnumerator LateInit()
        {
            yield return new WaitForSeconds(0.1f);
            shooter.FeatherListInit(this);
            gameObject.SetActive(false);
        }
    }
}