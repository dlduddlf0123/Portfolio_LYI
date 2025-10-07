using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    public class BossAttack_Shot : EnemyRangedAttack
    {
        public int projectileNum = 1;
        public int patternNum = 0;

        protected override void DoAwake()
        {
            base.DoAwake();
        }


        public override IEnumerator Attack()
        {
            yield return StartCoroutine(WaitForAttack());

            if (patternNum == 0)
            {
                StartCoroutine(ShotGun());
                patternNum = 1;
            }
            else
            {
                ActiveMultiStraightMissile(origin_missile,projectileNum);
                patternNum = 0;
            }
            //if (projectileNum > 1)
            //{
            //    StartCoroutine(ShotGun());
            //}
            //else
            //{
            //    ActiveMissile(origin_missile);
            //}

            yield return new WaitForSeconds(0.1f * projectileNum + 1f / enemy.Status.ATKSpeed);

            AfterAttack();
        }


        /// <summary>
        /// 전방으로 수많은 투사체 발사
        /// 타겟 기준 Y좌표 랜덤값 +-
        /// </summary>
        /// <returns></returns>
        IEnumerator ShotGun()
        {
            WaitForSeconds wait = new WaitForSeconds(0.1f);
            int p = 0;

            Vector3 targetVec = stageMgr.playerControll.centerTr.position - transform.position;
            while (p < projectileNum)
            {
                p++;

                EnemyProjectile missile = shooter.CreateMissile(origin_missile);

                SetMissileStatusTransform(missile, stageMgr.playerControll.centerTr.position);
                missile.TargetShot(targetVec + Vector3.up * Random.Range(-3f, 3f));

                yield return wait;
            }
        }


        public override void AfterAttack()
        {
            //base.AfterAttack();
            enemyController.AI_Move(EnemyState.MOVE);
        }
    }
}