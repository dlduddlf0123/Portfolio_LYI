using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 3/22/2023-LYI
    /// 지상, 근접, 점프
    /// 플레이어를 발견하면 접근
    /// 일정 사거리 내에 있으면 점프
    /// 후딜레이 발생
    /// </summary>
    public class GroundJumpMonsterController : EnemyController
    {
        [Header("Jump Option")]
        [SerializeField]
        private int jumpTimes = 1;

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
            yield return new WaitForSeconds(0.5f);

            if (isPlayerCheck)
            {
                AI_Move(EnemyState.CHASE);
            }
            else
            {
                AI_Move(EnemyState.MOVE);
            }
        }

        /// <summary>
        /// 평화로운 상태
        /// 추적중이 아닐 경우 배회
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Movement()
        {
            //랜덤 방향 배회
            direction = (Random.Range(0, 2) == 0) ? -1 : 1;
            ChangeSpriteDirection();

            float t = 0;
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (t < moveTime &&
                !isPlayerCheck)
            {
                t += 0.01f;
                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime);

                GroundCheck();
                FrontCheck();

                yield return wait;
            }

            AI_Move(EnemyState.IDLE);
        }

        /// <summary>
        /// 공격 준비 태세
        /// 유효 거리까지 플레이어에게 접근한 뒤 공격한다
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Chase()
        {
            float t = 0;

            ChangeDirectionToPlayer();

            WaitForSeconds sec = new WaitForSeconds(0.01f);
            while (t < moveTime * 2)
            {
                t += 0.01f;

                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime);
                GroundCheck();
                FrontCheck();

                //공격 범위 내에 플레이어가 있을 경우
                if (isPlayerCheck && isGround)
                {
                    //거리가 범위 내일 경우
                    if (Vector2.Distance(stageMgr.playerControll.transform.position, transform.position) < 5f)
                    {
                        AI_Move(EnemyState.ATTACK);
                        ////같은 높이의 플랫폼인 경우
                        //if (Mathf.Abs(stageMgr.playerControll.transform.position.y - transform.position.y) < 1)
                        //{
                        //}
                        //else
                        //{
                        //    //다른 높이의 플랫폼일 경우
                        //    //점프?
                        //}
                    }
                }



                yield return sec;
            }

            AI_Move(EnemyState.CHASE);
        }

        /// <summary>
        /// 잠시 힘 모으고 앞으로 점프
        /// 4/4/2023-LYI
        /// 여러번 점프할 수 있도록 변경
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Attack()
        {
            isKnockBackable = false; //공격 중에 넉백 막기
            for (int i = 0; i < jumpTimes; i++)
            {
                ChangeDirectionToPlayer();

                yield return StartCoroutine(Charging());

                //점프파티클 + 효과음?

                m_rigidbody2D.AddForce(Vector2.right * direction * 4f + Vector2.up * 8f, ForceMode2D.Impulse);

                while (!isGround)
                {
                    yield return new WaitForSeconds(0.01f);
                }
            }

            isKnockBackable = true;

            AI_Move(EnemyState.IDLE);
        }


        IEnumerator Charging()
        {
            WaitForSeconds sec = new WaitForSeconds(0.01f);

            Vector3 currentScale = transform.GetChild(0).localScale;
            Vector3 smallScale = new Vector3(currentScale.x, currentScale.y * 0.5f, currentScale.z);
            Color32 c;

            float t = 0;
            //충전
            while (t < 0.5f)
            {
                t += 0.02f;

                c = Color32.Lerp(Color.white, Color.red, t);
                //색깔 변경
                enemyStat.ChangeSpritesColor(c);

                transform.GetChild(0).localScale = Vector3.Lerp(currentScale, smallScale, t);

                yield return sec;
            }

            //초기화
            transform.GetChild(0).localScale = currentScale;
            enemyStat.ChangeSpritesColor(Color.white);
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
                isGround = false;
            }
            Debug.DrawRay(frontBound, Vector2.right * direction * 0.3f, frontColor, 0.1f);
        }
        protected override void GroundCheck()
        {
            Vector2 downBound = arr_collider[0].bounds.center + Vector3.right * arr_collider[0].bounds.size.x * 0.5f * direction;

            int characterMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Character")) | (1 << LayerMask.NameToLayer("Ignore Raycast"));

            //?????? ???? ????
            RaycastHit2D ray_ground = Physics2D.Raycast(downBound, Vector2.down, arr_collider[0].bounds.size.y, ~characterMask);


            Color rayColor = Color.blue;

            if (ray_ground)
            {
                if (ray_ground.collider.gameObject.CompareTag("Ground"))
                {
                    rayColor = Color.green;
                    isGround = true;
                }
            }
            else
            {
                rayColor = Color.red;
                isGround = false;

                if (e_state == EnemyState.MOVE)
                {
                    direction *= -1;
                    ChangeSpriteDirection();
                }
            }

            Debug.DrawRay(downBound, Vector2.down * arr_collider[0].bounds.size.y, rayColor, 0.1f);
        }



    }
}