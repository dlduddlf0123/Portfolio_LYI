using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class EnemyAttack_UpShot : EnemyRangedAttack
    {
        public int projectileNum = 1;

        // Start is called before the first frame update
        void Start()
        {

        }

        public override IEnumerator WaitForAttack()
        {
            float t = 0;
            while (t < 1)
            {
                t += 0.01f * enemy.Status.ATKSpeed;
                //부풀어 오르기
                // attackMissiles.transform.localScale += Vector3.one * t;
                yield return new WaitForSeconds(0.01f);
            }
        }

        public override IEnumerator Attack()
        {
            yield return StartCoroutine(WaitForAttack());

            //폭발!
            ActiveMultiUpMissile(origin_missile, projectileNum);

            yield return new WaitForSeconds(1f / enemy.Status.ATKSpeed);

            AfterAttack();
        }

        public void ActiveMultiUpMissile(GameObject originGo, int missileNum)
        {
            for (int i = 0; i < missileNum; i++)
            {
                EnemyProjectile missile = shooter.CreateMissile(originGo);
                missile.transform.position = enemy.centerTr.position;
                SetMissileStat(missile);

                missile.transform.localRotation = Quaternion.Euler(Vector3.forward * (60 - 40 * i));
                missile.UpShot();
            }
        }


        public override void AfterAttack()
        {
            enemyController.AI_Move(EnemyState.ATTACK);
        }
    }


}