using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class GroundStayMonsterController : EnemyController
    {
        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                isPlayerCheck = true;
                AI_Move(EnemyState.ATTACK);
            }
        }
        private void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                isPlayerCheck = false;
                AI_Move(EnemyState.IDLE);
            }
        }


        public override void AI_Move(EnemyState AIstate)
        {
            if (enemyStat.isDie)
                return;

            Stop();

            e_state = AIstate;

            if (stageMgr.isEnemyDebug)
                Debug.Log(gameObject.name + "AI Move:" + AIstate);
            switch (AIstate)
            {
                case EnemyState.IDLE: //IDLE
                    currentCoroutine = StartCoroutine(Idle());
                    break;
                case EnemyState.MOVE: //WALK
                    currentCoroutine = StartCoroutine(Idle());
                    break;
                case EnemyState.CHASE: //CHASE
                    currentCoroutine = StartCoroutine(enemyAttack.Attack());
                    break;
                case EnemyState.ATTACK: //ATTACK
                    currentCoroutine = StartCoroutine(enemyAttack.Attack());
                    break;
                default:
                    currentCoroutine = StartCoroutine(Idle());
                    break;
            }
        }


    }
}