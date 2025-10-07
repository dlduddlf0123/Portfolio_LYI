using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class GroundMeleeMonsterController : EnemyController
    {
        void Start()
        {
            AI_Move(EnemyState.IDLE);
        }
        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (Vector2.Distance(coll.transform.position, transform.position) > 4f)
                {
                    isPlayerCheck = true;
                    AI_Move(EnemyState.CHASE);
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

        protected override IEnumerator Idle()
        {
            yield return new WaitForSeconds(2f);

            if (isPlayerCheck)
            {
                AI_Move(EnemyState.CHASE);
            }
            else
            {
                AI_Move(EnemyState.MOVE);
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

            AI_Move(EnemyState.IDLE);
        }

        protected override IEnumerator Chase()
        {
            float t = 0;

            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;

            ChangeSpriteDirection();

            while (t < moveTime * 2)
            {
                t += 0.01f;

                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * 4 * Time.deltaTime);
                FrontCheck();

                yield return new WaitForSeconds(0.01f);
            }

            AI_Move(EnemyState.CHASE);
        }


    }
}