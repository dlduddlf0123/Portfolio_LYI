using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class BossGroundMelee : EnemyController
    {
        public int chargeCount = 0;
        public int jumpCount = 0;

        int maxChargeCount = 3;
        int maxJumpCount = 3;

       public  bool isWall = false;

        // Start is called before the first frame update
        void Start()
        {
            maxChargeCount = Random.Range(1, 4);
            maxJumpCount = Random.Range(2, 5);
            AI_Move(EnemyState.IDLE);
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
                    currentCoroutine = StartCoroutine(Movement());
                    break;
                case EnemyState.CHASE: //CHASE
                    currentCoroutine = StartCoroutine(Chase());
                    break;
                case EnemyState.ATTACK: //ATTACK
                    currentCoroutine = StartCoroutine(enemyAttack.Attack());
                    break;
                default:
                    currentCoroutine = StartCoroutine(Idle());
                    break;
            }
        }

        protected override IEnumerator Idle()
        {
            isWall = false;
            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;
            ChangeSpriteDirection();

            yield return new WaitForSeconds(1f);

             if (stageMgr.playerControll.centerTr.position.y - enemyStat.centerTr.position.y > 1f)
            {
                AI_Move(EnemyState.CHASE);
            }
            else
            {
                bool isRandom = (Random.Range(0, 2) == 0) ? true : false;
                if (isRandom)
                {
                    AI_Move(EnemyState.MOVE);
                }
                else
                {
                    AI_Move(EnemyState.CHASE);
                }
            }
        }


        /// <summary>
        /// Charge to front for 2~3 times
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Movement()
        {
            chargeCount++;

            float t = 0;

            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;
            ChangeSpriteDirection();

            isWall = false;

            while (t < moveTime * 2 && !isWall)
            {
                t += 0.01f;

               m_rigidbody2D.velocity = new Vector3(direction * moveSpeed * speedMultiplier* 5f, m_rigidbody2D.velocity.y);
                FrontCheck();

                yield return new WaitForSeconds(0.01f);
            }


            if (chargeCount < maxChargeCount)
            {
                AI_Move(EnemyState.MOVE);
            }
            else
            {
                chargeCount = 0;
                maxChargeCount = Random.Range(1, 4);
                AI_Move(EnemyState.IDLE);
            }
        }

        /// <summary>
        /// Jump to player 1~2 times then, charge
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Chase()
        {
            jumpCount++;
            isWall = false;

            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;

            ChangeSpriteDirection();

            m_rigidbody2D.AddForce(Vector2.right * direction * 4f + Vector2.up * 7f, ForceMode2D.Impulse);

            yield return new WaitForSeconds(2f);

            if (jumpCount < maxJumpCount)
            {
                AI_Move(EnemyState.CHASE);
            }
            else
            {
                jumpCount = 0;
                maxJumpCount = Random.Range(2, 5);
                AI_Move(EnemyState.MOVE);
            }
        }
        protected override void FrontCheck()
        {
            Vector2 frontBound = arr_collider[0].bounds.center + Vector3.right * arr_collider[0].bounds.size.x * 0.5f * direction;

            int characterMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Ignore Raycast"));

            RaycastHit2D ray_front = Physics2D.Raycast(frontBound, Vector2.right * direction, 0.3f, ~characterMask);

            Color frontColor = Color.blue;

            if (ray_front)
            {
                frontColor = Color.red;
                FrontCheckAction();
                isWall = true;
            }
            else
            {
                isWall = false;

            }
            Debug.DrawRay(frontBound, Vector2.right * direction * 0.3f, frontColor, 0.1f);
        }

    }
}