using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class FlyingRangeMonsterController : EnemyController
    {

        void Start()
        {
            isKnockBackable = false;
          //  AI_Move(EnemyState.MOVE);
        }


        public override void AI_Move(EnemyState AIstate)
        {
            base.AI_Move(AIstate);
        }

        Vector3 RandomDirection()
        {
            int rand = Random.Range(0, 4);
            if (direction == 1)
            {
                rand = Random.Range(0, 2);
            }
            else
            {
                rand = Random.Range(2, 4);
            }
            switch (rand)
            {
                case 0:
                    return Vector3.right ;
                case 1:
                    return Vector3.left;
                case 2:
                    return  Vector3.up;
                case 3:
                    return  Vector3.down;
                default:
                    return Vector3.left;
            }
        }

        /// <summary>
        /// ???? ?? ????
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Movement()
        {
            ChangeSpriteDirection();

            Vector3 dir = RandomDirection();

            float t = 0;
            while (t < enemyStat.Status.ATKSpeed * 0.5f)
            {
                t += 0.01f;

                transform.Translate(dir * moveSpeed * speedMultiplier * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            AI_Move(EnemyState.ATTACK);
        }
    }
}