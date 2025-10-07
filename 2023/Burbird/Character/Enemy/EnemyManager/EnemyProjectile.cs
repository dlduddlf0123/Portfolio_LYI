using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    public class EnemyProjectile : MonoBehaviour
    {
        public EnemyProjectileManager shooter;

        Rigidbody2D m_rigidbody2d;
        Collider2D m_collider;
        Animator m_anim;

        public int missileDamage = 10;

        public float speed = 5f;
        public float weight = 1f;
        public float lifeTime = 10f;

        public ProjectileType missileType;

        bool isGetable = false;
        bool isHited = false;

        public Transform currentTarget;

        Coroutine currentCoroutine = null;
        private void Awake()
        {
            m_rigidbody2d = GetComponent<Rigidbody2D>();
        }


        private void OnEnable()
        {
            Stop();
            currentCoroutine = StartCoroutine(LifeTimer());
        }

        public void Stop()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        public virtual void Init()
        {
            isHited = false;
           
            Stop();
            OnMissileDisable();

            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;

            m_rigidbody2d.velocity = Vector2.zero;
            shooter.MissileInit(this);
        }

        /// <summary>
        /// 미사일이 사라질 때 효과
        /// 애니메이션, 파티클 혹은 분열하는 미사일 등의 효과 적용
        /// </summary>
        public virtual void OnMissileDisable()
        {
            switch (missileType)
            {
                case ProjectileType.NORMAL:
                    break;
                case ProjectileType.PIERCE:
                    break;
                case ProjectileType.BOMB:
                    EnemyDeath_Boom death = GetComponent<EnemyDeath_Boom>();
                    death.ActiveMissileDeathEffect(this.transform, missileDamage);
                    break;
                case ProjectileType.SUMMON:
                    EnemyDeath_Summon summon = GetComponent<EnemyDeath_Summon>();
                    summon.ActiveMissileDeathEffect();
                    break;
                default:
                    break;
            }

        }


        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                coll.GetComponentInParent<Player>().GetDamage(missileDamage,transform.position);
                Init();
            }
            if (coll.gameObject.CompareTag("Ground"))
            {
                if (missileType == ProjectileType.PIERCE)
                {
                    Stop();
                    currentCoroutine = StartCoroutine(InitTime());
                }
                else
                {
                    Init();
                }
            }
        }

        //public void TargetShot(Transform player)
        //{
        //    currentTarget = player;
        //    m_rigidbody.AddForce((currentTarget.transform.position - transform.position).normalized * speed, ForceMode2D.Impulse);
        //}

        public void TargetShot(Transform target)
        {
            currentTarget = target;
            m_rigidbody2d.velocity = Vector2.zero;
            m_rigidbody2d.AddForce((currentTarget.transform.position - transform.position).normalized * speed, ForceMode2D.Impulse);
        }
        public void TargetShot(Vector3 target)
        {
            m_rigidbody2d.velocity = Vector2.zero;
            m_rigidbody2d.AddForce(target.normalized * speed, ForceMode2D.Impulse);
        }

        public void UpShot()
        {
            m_rigidbody2d.velocity = Vector2.zero;
            m_rigidbody2d.AddForce(transform.up * speed, ForceMode2D.Impulse);
        }

        /// <summary>
        /// 4/18/2023-LYI
        /// 물리기반 커브샷 계산
        /// </summary>
        /// <param name="target"></param>
        public void CurveShot(Vector2 target)
        {
            Vector2 playerPos = target;
            Vector2 enemyPos = transform.position;
            float heightDifference = playerPos.y - enemyPos.y;
            float distance = Mathf.Abs(playerPos.x - enemyPos.x);

            // Check if the target position is below the enemy position
            if (heightDifference < 0)
            {
                heightDifference = Mathf.Abs(heightDifference);
            }

            // Calculate the firing angle based on the height difference and the horizontal distance
            float angleRadians = Mathf.Atan((4 * heightDifference) / distance);
            float angleDegrees = angleRadians * Mathf.Rad2Deg;

            // Adjust the firing angle based on the desired angle
            angleDegrees = Mathf.Clamp(angleDegrees, 30, 90f);

            int dir = (transform.position.x < target.x) ? 1 : -1;

            m_rigidbody2d.velocity = CalculateLaunchVelocity(angleDegrees, distance, dir);
        }

        private Vector2 CalculateLaunchVelocity(float angleDegrees, float distance, int dir)
        {
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            float velocity = Mathf.Sqrt(distance * Physics2D.gravity.magnitude / Mathf.Sin(2 * angleRadians));

            // Use the firing angle and horizontal distance to calculate the launch velocity
            float launchSpeedX = velocity * Mathf.Cos(angleRadians);
            float launchSpeedY = velocity * Mathf.Sin(angleRadians);


            // Return the launch velocity as a Vector2
            return new Vector2(launchSpeedX * dir, launchSpeedY);
        }

        IEnumerator InitTime()
        {
            yield return new WaitForSeconds(5f);
            Init();
        }

        /// <summary>
        /// 투사체를 발사 했을 때 자연소멸할 시간
        /// </summary>
        /// <returns></returns>
        IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(lifeTime);
            Init();
        }


    }
}