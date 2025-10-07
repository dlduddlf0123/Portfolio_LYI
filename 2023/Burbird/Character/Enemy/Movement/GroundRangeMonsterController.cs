using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class GroundRangeMonsterController : EnemyController
    {
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

        protected override void FrontCheckAction()
        {
            switch (e_state)
            {
                case EnemyState.MOVE:
                case EnemyState.CHASE:
                    direction *= -1;
                    ChangeSpriteDirection();
                    break;
                case EnemyState.ATTACK:
                    break;
                default:
                    break;
            }
        }
    }
}