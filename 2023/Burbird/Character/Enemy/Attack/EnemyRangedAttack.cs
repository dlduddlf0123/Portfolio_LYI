using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{

    /// <summary>
    /// 4/13/2023-LYI
    /// 적 캐릭터 공격 관련 스크립트
    /// </summary>
    public class EnemyRangedAttack : MonoBehaviour
    {
        protected StageManager stageMgr;

        protected Enemy enemy;
        protected EnemyController enemyController;
        protected EnemyProjectileManager shooter;

        //미사일
        public GameObject origin_missile;

        [Header("TypeOption")]
        public Enemy summonEnemy;


        [Header("Status")]
        public int missileDamage = 10;

        //public float fireTime = 0.3f;
        //public float fireTic = 0f;
        public float missileSpeed = 5f;
        public float weight = 1f;

        protected Coroutine currentCoroutine;

        void Awake()
        {
            stageMgr = StageManager.Instance;
            enemy = GetComponent<Enemy>();
            enemyController = GetComponent<EnemyController>();
            shooter = stageMgr.enemySpawner.projectileMgr;

            if (shooter == null)
            {
                shooter = GetComponentInChildren<EnemyProjectileManager>();
            }

            DoAwake();        
        }

        protected virtual void DoAwake() { }

        private void FixedUpdate()
        {
            //조준
            Vector3 fireVec = (stageMgr.playerControll.centerTr.position - enemy.centerTr.position).normalized;
            float angle = Mathf.Atan2(fireVec.y, fireVec.x) * Mathf.Rad2Deg;
            enemy.centerTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public virtual void Stop()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        #region Missile Status
        public void SetShooterStat(Enemy enemy)
        {
            missileDamage = enemy.Status.ATKDamage;
            missileSpeed = enemy.Status.ShotSpeed * 5;
        }

        /// <summary>
        /// Setting missile's status
        /// </summary>
        /// <param name="missile"></param>
        protected void SetMissileStat(EnemyProjectile missile)
        {
            missile.speed = missileSpeed * stageMgr.perkChecker.perk_enemyShotSpeedMultiplier;
            missile.missileDamage = missileDamage;

            CheckMissileType(missile);
        }
        /// <summary>
        /// Checking missile's type
        /// add something need for type
        /// </summary>
        /// <param name="missile"></param>
        void CheckMissileType(EnemyProjectile missile)
        {
            switch (missile.missileType)
            {
                case ProjectileType.NORMAL:
                    break;
                case ProjectileType.PIERCE:
                    break;
                case ProjectileType.BOMB:
                    break;
                case ProjectileType.SUMMON:
                    if (missile.GetComponent<EnemyDeath_Summon>())
                    {
                        missile.GetComponent<EnemyDeath_Summon>().summonEnemy = summonEnemy;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Missile Shoot Methods

        /// <summary>
        /// 미사일 스탯, 포지션, 로테이션 적용
        /// </summary>
        /// <param name="missile"></param>
        /// <param name="target"></param>
        protected void SetMissileStatusTransform(EnemyProjectile missile, Vector3 target)
        {
            SetMissileStat(missile);

            missile.transform.position = enemy.centerTr.position;

            Vector3 fireVec = (target - enemy.centerTr.position).normalized;
            float angle = Mathf.Atan2(fireVec.y, fireVec.x) * Mathf.Rad2Deg;
            missile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected void ActiveMissile(GameObject originGo)
        {
            EnemyProjectile missile = shooter.CreateMissile(originGo);

            SetMissileStatusTransform(missile, stageMgr.playerControll.centerTr.position);

            missile.TargetShot(stageMgr.playerControll.centerTr);
        }
        protected void ActiveMissile(GameObject originGo, Vector3 target)
        {
            EnemyProjectile missile = shooter.CreateMissile(originGo);

            SetMissileStatusTransform(missile, stageMgr.playerControll.centerTr.position);

            missile.TargetShot(target);
        }

        protected void ActiveMultiStraightMissile(GameObject originGo, int missileNum)
        {
            StartCoroutine(MultiShot(originGo, missileNum));
        }
        protected void ActiveMultiStraightMissile(GameObject originGo, int missileNum, Vector3 target)
        {
            StartCoroutine(MultiShot(originGo, missileNum, target));
        }

        protected IEnumerator MultiShot(GameObject originGo, int missileNum, float delay = 0.25f)
        {
            for (int i = 0; i < missileNum; i++)
            {
                EnemyProjectile missile = shooter.CreateMissile(originGo);

                SetMissileStatusTransform(missile, stageMgr.playerControll.centerTr.position);
                missile.TargetShot(stageMgr.playerControll.centerTr);
                yield return new WaitForSeconds(delay);
            }
        }
        protected IEnumerator MultiShot(GameObject originGo, int missileNum, Vector3 target, float delay = 0.25f)
        {
            for (int i = 0; i < missileNum; i++)
            {
                EnemyProjectile missile = shooter.CreateMissile(originGo);

                SetMissileStatusTransform(missile, stageMgr.playerControll.centerTr.position);
                missile.TargetShot(target);
                yield return new WaitForSeconds(delay);
            }
        }
        #endregion

        /// <summary>
        /// 공격 전 딜레이, 공격 준비 효과 표시
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator WaitForAttack()
        {
            float t = 0;
            while (t < 0.5f)
            {
                t += 0.01f * enemy.Status.ATKSpeed;


                yield return new WaitForSeconds(0.01f);
            }
        }

        /// <summary>
        /// 공격
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Attack()
        {
            yield return StartCoroutine(WaitForAttack());

            ActiveMissile(origin_missile);

            //공격 후 딜레이
            yield return new WaitForSeconds(2f / enemy.Status.ATKSpeed);

            //공격 후 동작
            AfterAttack();
        }


        /// <summary>
        /// 공격 이후 동작 설정
        /// </summary>
        public virtual void AfterAttack()
        {
            enemyController.CheckNextMove(); 
        }

        //구조 개편으로 Shooter에서 미사일을 생성하므로 어차피 사망시 사라지지 않음
        //public IEnumerator MissileCheck()
        //{
        //    if (shooter !=null)
        //    {
        //        //if (shooter.list_activeMissile.Count > 0)
        //        //    shooter.AllMissileInit();
        //        yield return shooter.WaitForMissile();
        //    }

        //    //yield return new WaitForEndOfFrame();
        //}

    }
}