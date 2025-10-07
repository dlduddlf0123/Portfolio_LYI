using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    public class FlyingMeleeMonController : EnemyController
    {

        private void Start()
        {
            isKnockBackable = false;
            //AI_Move(EnemyState.IDLE);
        }

        /// <summary>
        /// 4/11/2023-LYI
        /// 단순 충돌 시 반대 방향대로 움직이도록 변경
        /// Reflect 대신 충돌 체크 시 방향 반대로 바꾸도록 변경
        /// </summary>
        /// <param name="coll"></param>
        private void OnTriggerEnter2D(Collider2D coll)
        {
            //플랫폼이 아닌 땅과 닿을 때
            if (coll.gameObject.CompareTag("Ground") && coll.gameObject.layer != 10)
            {
                //현재 속도
                Vector2 inputDirection = m_rigidbody2D.velocity;
                //충돌 위치 벡터
                Vector2 surfaceNormal = coll.ClosestPoint(transform.position) - (Vector2)transform.position;

                float x, y;
                //SurfaceNormal값의 절대값이 높은 곳이 충돌 부위
                //x가 높으면 가로 y가 높으면 세로
                if (Mathf.Abs(surfaceNormal.x) > Mathf.Abs(surfaceNormal.y))
                {
                    //SurfaceNormal 값이 양수일 경우 우측 벽에 튕김
                    x = (surfaceNormal.x >= 0) ? -1 : 1;
                    //y는 진행방향대로
                    y = (inputDirection.y >= 0) ? 1 : -1;
                }
                else
                {
                    //x는 진행방향대로
                    x = (inputDirection.x >= 0) ? 1 : -1;
                    //SurfaceNormal 값이 양수일 경우 위벽에 튕김
                    y = (surfaceNormal.y >= 0) ? -1 : 1;
                }

                //최종 방향과 속도 적용 벡터
                Vector2 speed = new Vector2(x, y) * moveSpeed * speedMultiplier * 12f;

                //속도 적용 전 초기화
                m_rigidbody2D.velocity = Vector2.zero;

                //속도 적용
                m_rigidbody2D.AddForce(speed);
            }
        }

        Vector3 RandomDirection()
        {
            int rand = Random.Range(0, 4);

            switch (rand)
            {
                case 0:
                    return Vector3.right + Vector3.up;
                case 1:
                    return Vector3.right + Vector3.down;
                case 2:
                    return Vector3.left + Vector3.up;
                case 3:
                    return Vector3.left + Vector3.down;
                default:
                    return Vector3.right + Vector3.down;
            }
        }

        /// <summary>
        /// 방향 결정, 위아래 랜덤
        /// </summary>
        /// <param name="isLeft"></param>
        /// <returns></returns>
        Vector3 SideDirection(bool isLeft)
        {
            int rand = Random.Range(0, 2);

            if (isLeft)
            {
                switch (rand)
                {
                    case 0:
                        return Vector3.left + Vector3.up;
                    case 1:
                        return Vector3.left + Vector3.down;
                    default:
                        return Vector3.left + Vector3.down;
                }
            }
            else
            {
                switch (rand)
                {
                    case 0:
                        return Vector3.right + Vector3.up;
                    case 1:
                        return Vector3.right + Vector3.down;
                    default:
                        return Vector3.right + Vector3.down;
                }
            }
        }

        /// <summary>
        /// 4/6/2023-LYI
        /// 공중 움직임
        /// 분신일 경우 Direction 따라가기
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Movement()
        {
            ChangeSpriteDirection();

            Vector3 dir;

            //분신이면 현재 디렉션 따라 이동
            //아니면 랜덤
            if (enemyStat.isClone)
            {
                dir = SideDirection((direction == -1));
            }
            else
            {
                dir = RandomDirection();
            }

            //Reflect 활용을 위한 물리 이동
            m_rigidbody2D.AddForce(dir * moveSpeed * speedMultiplier * 12f);

            yield return new WaitForSeconds(moveTime);
        }
    }
}