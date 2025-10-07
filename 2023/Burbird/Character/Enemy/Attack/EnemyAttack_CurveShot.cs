using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class EnemyAttack_CurveShot : EnemyRangedAttack
    {
        public int projectileNum = 1;

        protected override void DoAwake()
        {
            base.DoAwake();
        }

        void Start()
        {

        }

        public override IEnumerator Attack()
        {
            yield return StartCoroutine(WaitForAttack());

            if (projectileNum > 1)
            {
                ActiveMultiCurveShot(origin_missile, projectileNum, stageMgr.playerControll.centerTr.position);
            }
            else
            {
                ActiveCurveShot(origin_missile, stageMgr.playerControll.centerTr.position);
            }

            yield return new WaitForSeconds(1f / enemy.Status.ATKSpeed);

            AfterAttack();
        }

        /// <summary>
        /// 3/23/2023-LYI
        /// 목표 지점까지 위쪽 방향으로 포물선 공격
        /// </summary>
        public void ActiveCurveShot(GameObject originGo, Vector3 target)
        {
            EnemyProjectile missile = shooter.CreateMissile(originGo);
            missile.transform.position = enemy.centerTr.position; 
            SetMissileStat(missile);

            missile.CurveShot(target);
        }
        public void ActiveMultiCurveShot(GameObject originGo, int missileNum, Vector3 target)
        {
            StartCoroutine(MultiCurveShot(originGo, missileNum, target, 0.25f));

            //for (int i = 0; i < missileNum; i++)
            //{
            //    EnemyProjectile missile = shooter.CreateMissile(originGo);
            //    missile.transform.position = enemy.centerTr.position;
            //    SetMissileStat(missile);

            //    missile.CurveShot(target);
            //}
        }
        protected IEnumerator MultiCurveShot(GameObject originGo, int missileNum, Vector3 target, float delay = 0.1f)
        {
            for (int i = 0; i < missileNum; i++)
            {
                EnemyProjectile missile = shooter.CreateMissile(originGo);
                missile.transform.position = enemy.centerTr.position;
                SetMissileStat(missile);
                missile.CurveShot(target);
                yield return new WaitForSeconds(delay);
            }
        }


        public override void AfterAttack()
        {
            base.AfterAttack();
            //enemyController.AI_Move(EnemyState.MOVE);
        }
    }
}