using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 3/21/2023-LYI
    /// 지상, 근접, 돌진
    /// 플레이어를 발견하면 접근
    /// 일정 사거리 내에 있으면 차징
    /// 차징 이후 빠른 돌진, 후딜레이 발생
    /// </summary>
    public class GroundChargeMonsterController : EnemyController
    {
        [Header("Charge Option")]
        [SerializeField]
        private int chargeTimes = 1;

        Coroutine attackCoroutine;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                //컬리더 구분을 위한 거리 체크
                //직접 부딪힌 경우를 제외
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

        public override void Stop()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            StopAttack();
        }

        void StopAttack()
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
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
        /// 3/21/2023-LYI
        /// 평화로운 상태
        /// 추적중이 아닐 경우 배회
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Movement()
        {

            direction = (Random.Range(0, 2) == 0) ? -1 : 1;
            ChangeSpriteDirection();


            float t = 0;
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (t < moveTime &&
                !isPlayerCheck)
            {
                t += 0.01f;
                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime);
               // m_rigidbody2D.velocity = new Vector3(direction * moveSpeed * speedMultiplier, m_rigidbody2D.velocity.y);

                GroundCheck();
                FrontCheck();

                yield return wait;
            }

            AI_Move(EnemyState.IDLE);
        }

        /// <summary>
        /// 3/21/2023-LYI
        /// 공격 준비 태세
        /// 유효 거리까지 플레이어에게 접근한 뒤 공격한다
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Chase()
        {
            float t = 0;

            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;

            ChangeSpriteDirection();

            WaitForSeconds sec = new WaitForSeconds(0.01f);
            while (t < moveTime * 2)
            {
                t += 0.01f;

                //m_rigidbody2D.velocity = new Vector3(direction * moveSpeed * speedMultiplier, m_rigidbody2D.velocity.y);
                transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * Time.deltaTime);
                FrontCheck();

                /*
                 * 플레이어 발견 시 준비 차지 = 색깔 서서히 변경, 애니메이션 
                 * 돌진 = Translate + 가속 + 이펙트
                 * 플레이어 거리 체크, 일정 거리 안으로 들어오면 돌진
                 * Y값 차이가 얼마 안나서 같은 단수인게 체크되면 돌진
                 * Y값 차이가 많이나면 플랫폼일 경우 바꾸기?
                */
                //공격 범위 내에 플레이어가 있을 경우
                if (isPlayerCheck)
                {
                    //거리가 범위 내일 경우
                    if (Vector2.Distance(stageMgr.playerControll.transform.position, transform.position) < 5f)
                    {
                        //같은 높이의 플랫폼인 경우
                        if (Mathf.Abs(stageMgr.playerControll.transform.position.y - transform.position.y) < 1)
                        {
                            AI_Move(EnemyState.ATTACK);
                        }
                        else
                        {
                            //다른 높이의 플랫폼일 경우
                            //점프?
                         //   AI_Move(EnemyState.MOVE);
                        }
                    }
                }
                yield return sec;
            }

            AI_Move(EnemyState.IDLE);
        }

        /// <summary>
        /// 잠시 힘 모으고 앞으로 돌진
        /// 4/4/2023-LYI
        /// 여러번 돌진 할 수 있도록 변경
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Attack()
        {
            isKnockBackable = false; //공격 중에 넉백 막기
            for (int i = 0; i < chargeTimes; i++)
            {
                StopAttack();
               
                yield return attackCoroutine = StartCoroutine(AttackAct());
                yield return new WaitForSeconds(0.5f);
            }

            isKnockBackable = true;
            AI_Move(EnemyState.IDLE);
        }

        /// <summary>
        /// 4/4/2023-LYI
        /// 돌진 부분 동작
        /// </summary>
        /// <returns></returns>
        IEnumerator AttackAct()
        {
            float t = 0;
            bool isLeft = (stageMgr.playerControll.transform.position.x < transform.position.x);
            direction = isLeft ? -1 : 1;

            ChangeSpriteDirection();


            WaitForSeconds sec = new WaitForSeconds(0.01f);

            Vector3 currentScale = transform.GetChild(0).localScale;
            Vector3 smallScale = new Vector3(currentScale.x, currentScale.y * 0.5f, currentScale.z);
            Color32 c;
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
            //돌진 파티클 + 효과음?

            sec = new WaitForSeconds(0.001f);
            t = 0;
            Vector2 start = transform.position;
            while (Vector2.Distance(transform.position, start) < 5f)
            {
                // t += 0.01f;

                m_rigidbody2D.velocity = new Vector3(direction * moveSpeed * speedMultiplier * 20, m_rigidbody2D.velocity.y);
                //transform.Translate(Vector3.right * direction * moveSpeed * speedMultiplier * 20 * Time.deltaTime);
                FrontCheck();

                yield return sec;
            }
            m_rigidbody2D.velocity = Vector2.up * m_rigidbody2D.velocity.y;
        }


    }
}