using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    public class EnemyAttack_EightWayShot : EnemyRangedAttack
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


            StartCoroutine(ActiveEightWayShot(origin_missile, projectileNum));


            yield return new WaitForSeconds(1f / enemy.Status.ATKSpeed);

            // attackMissiles.SetActive(false);

            AfterAttack();
        }


        public IEnumerator ActiveEightWayShot(GameObject originGo, int missileNum)
        {
            float angle = 0;
            for (int repeatTime = 0; repeatTime < missileNum; repeatTime++)
            {
                EnemyProjectile[] arr_missile = new EnemyProjectile[8];

                for (int missileIndex = 0; missileIndex < arr_missile.Length; missileIndex++)
                {
                    arr_missile[missileIndex] = shooter.CreateMissile(originGo);

                    arr_missile[missileIndex].transform.position = enemy.centerTr.position;
                    arr_missile[missileIndex].transform.localPosition = Vector3.zero;
                    arr_missile[missileIndex].transform.rotation = Quaternion.Euler(0, 0, angle + 45 * missileIndex);

                    Vector2 target = (arr_missile[missileIndex].transform.position + arr_missile[missileIndex].transform.right) -
                        arr_missile[missileIndex].transform.position;
                    SetMissileStatusTransform(arr_missile[missileIndex], target);

                    arr_missile[missileIndex].TargetShot(target);
                }

                angle += 45 * 0.5f;
                yield return new WaitForSeconds(0.3f);
            }
        }

        public override void AfterAttack()
        {
            enemyController.AI_Move(EnemyState.MOVE);
        }
    }
}