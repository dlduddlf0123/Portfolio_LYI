using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 3/22/2023-LYI
    /// 저격 샷
    /// 조준: 플레이어에게 라인 추적
    /// 발사: 추적 중단, 잠시 대기 후 발사
    /// </summary>
    public class EnemyAttack_SnipeShot : EnemyRangedAttack
    {

        //3/22/2023-LYI
        //발사 궤도 표시하는 라인, 저격수 느낌
        [SerializeField]
        private LineRenderer snipeLine;
        public int projectileNum = 1;

        Vector3 shootVec; //최종 발사할 벡터

        protected override void DoAwake()
        {
            base.DoAwake();
        }

        void Start()
        {
            snipeLine.gameObject.SetActive(false);
        }

        /// <summary>
        /// 3/22/2023-LYI
        /// 조준 중
        /// </summary>
        /// <returns></returns>
        public override IEnumerator WaitForAttack()
        {
            Vector3[] arr_linePos = new Vector3[2];
            arr_linePos[0] = shooter.transform.position; //슈터에서
            arr_linePos[1] = (stageMgr.playerControll.centerTr.position - shooter.transform.position).normalized; //플레이어 방향까지

            snipeLine.gameObject.SetActive(true);
            snipeLine.transform.localPosition = Vector3.zero - transform.position;
            snipeLine.startColor = Color.red;
            snipeLine.endColor = Color.red;

            snipeLine.SetPosition(0, arr_linePos[0]);
            WaitForSeconds wait = new WaitForSeconds(0.01f);


            int characterMask =
                (1 << LayerMask.NameToLayer("Player")) |
                (1 << LayerMask.NameToLayer("Character")) |
                (1 << LayerMask.NameToLayer("Ignore Raycast")) |
                (1 << LayerMask.NameToLayer("Platform"));

            //대기 시간 중 동작
            float t = 0;
            while (t < 1.5f)
            {
                t += 0.01f;

                arr_linePos[0] = shooter.transform.position;
                arr_linePos[1] = (stageMgr.playerControll.centerTr.position - shooter.transform.position).normalized;
                RaycastHit2D lineEnd = Physics2D.Raycast(arr_linePos[0], arr_linePos[1], Mathf.Infinity, ~characterMask);

                if (lineEnd)
                {
                    snipeLine.SetPosition(1, lineEnd.point + (Vector2)arr_linePos[1] * 1f);
                }
                else
                {
                    snipeLine.SetPosition(1, arr_linePos[1] * 100f);
                }

                yield return wait;
            }

            snipeLine.startColor = Color.yellow;
            snipeLine.endColor = Color.yellow;

            shootVec = stageMgr.playerControll.centerTr.position - enemy.centerTr.position; //최종 발사할 위치

            yield return new WaitForSeconds(0.2f);
            snipeLine.gameObject.SetActive(false);
        }

        /// <summary>
        /// 3/22/2023-LYI
        /// 발사
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Attack()
        {
            yield return StartCoroutine(WaitForAttack());

            if (projectileNum > 1)
            {
                ActiveMultiStraightMissile(origin_missile, projectileNum,shootVec);
            }
            else
            {
                ActiveMissile(origin_missile, shootVec);
            }

            //발사 후 딜레이
            yield return new WaitForSeconds(1f / enemy.Status.ATKSpeed);

            AfterAttack();
        }

        public override void AfterAttack()
        {
            base.AfterAttack();
            //enemyController.AI_Move(EnemyState.MOVE);
        }
    }
}