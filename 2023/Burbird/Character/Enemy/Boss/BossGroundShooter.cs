using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class BossGroundShooter : EnemyController
    {
        EnemyRangedAttack[] arr_attack;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (Vector2.Distance(coll.transform.position, transform.position) > 4f)
                {
                    isPlayerCheck = true;
                    AI_Move(EnemyState.ATTACK);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (Vector2.Distance(coll.transform.position, transform.position) > 4f)
                {
                    isPlayerCheck = false;
                }
            }
        }

        public override void AI_Move(EnemyState AIstate)
        {
            //같은 동작일 경우 실행 안함, 중복 발사 방지용
            if (e_state == AIstate) { return; }

            Stop();

            if (enemyStat.isDie) { return; }

            e_state = AIstate;

            if (stageMgr.isEnemyDebug)
                Debug.Log(gameObject.name + "AI Move:" + AIstate);
            switch (AIstate)
            {
                case EnemyState.IDLE: //IDLE
                    currentCoroutine = StartCoroutine(Idle());
                    break;
                case EnemyState.MOVE: //WALK
                    currentCoroutine = StartCoroutine(Movement());
                    break;
                case EnemyState.CHASE: //CHASE
                    currentCoroutine = StartCoroutine(Chase());
                    break;
                case EnemyState.ATTACK: //ATTACK
                    currentCoroutine = StartCoroutine(Attack());
                    break;
                default:
                    currentCoroutine = StartCoroutine(Idle());
                    break;
            }
        }



        protected override IEnumerator Movement()
        {
            direction = (Random.Range(0, 2) == 0) ? -1 : 1;
            ChangeSpriteDirection();

            float t = 0;
            while (t < moveTime)
            {
                t += 0.01f;
                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime);

                GroundCheck();
                FrontCheck();

                yield return new WaitForSeconds(0.01f);
            }

            CheckNextMove(EnemyState.IDLE);
        }

        //protected override IEnumerator Attack()
        //{
        //    //원거리 공격이면 원거리 공격 진행
        //    if (enemyAttack != null)
        //    {
        //        currentCoroutine = StartCoroutine(arr_attack[0].Attack());
        //    }
        //    else
        //    {

        //    }
        //    yield return null;

        //}
    }
}