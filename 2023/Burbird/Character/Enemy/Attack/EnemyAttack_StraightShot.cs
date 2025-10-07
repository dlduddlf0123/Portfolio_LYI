using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    public class EnemyAttack_StraightShot : EnemyRangedAttack
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
                ActiveMultiStraightMissile(origin_missile,projectileNum);
            }
            else
            {
                ActiveMissile(origin_missile);
            }

            yield return new WaitForSeconds(0.1f * projectileNum + 1f / enemy.Status.ATKSpeed);

            AfterAttack();
        }



        public override void AfterAttack()
        {
            //base.AfterAttack();
            enemyController.AI_Move(EnemyState.MOVE);
        }
    }
}