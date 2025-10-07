using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    public class EnemyAttack_TripleShot : EnemyRangedAttack
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

            ActiveTripleShot(origin_missile, projectileNum);

            yield return new WaitForSeconds(1f / enemy.Status.ATKSpeed);

            AfterAttack();
        }

        public void ActiveTripleShot(GameObject originGo, int missileNum)
        {
            Vector2 forward = enemy.centerTr.right;
            Vector2 upTarget = (enemy.centerTr.up + enemy.centerTr.right).normalized;
            Vector2 downTarget = (-enemy.centerTr.up + enemy.centerTr.right).normalized;

            for (int i = 0; i < missileNum; i++)
            {
                EnemyProjectile[] arr_missile = new EnemyProjectile[3];

                for (int missileIndex = 0; missileIndex < 3; missileIndex++)
                {
                    arr_missile[missileIndex] = shooter.CreateMissile(originGo);
                }

                SetMissileStatusTransform(arr_missile[0], upTarget);
                SetMissileStatusTransform(arr_missile[1], forward);
                SetMissileStatusTransform(arr_missile[2], downTarget);
                arr_missile[0].TargetShot(upTarget);
                arr_missile[1].TargetShot(forward);
                arr_missile[2].TargetShot(downTarget);

            }

            // stageMgr.soundMgr.PlaySfx(transform.position, sfx_featherShot, Random.Range(0.7f, 1.4f));
        }


        public override void AfterAttack()
        {
            enemyController.AI_Move(EnemyState.MOVE);
        }
    }
}