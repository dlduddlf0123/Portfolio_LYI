using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding;

namespace VRTokTok.Character
{

    /// <summary>
    /// 4/2/2024-LYI
    /// 네비게이션으로 움직이기
    /// </summary>
    public class Tok_NavMovement : Tok_Movement
    {
        [Header("Navigation")]
        [SerializeField]
        Seeker m_seeker;

        [SerializeField]
        AIBase m_pathAI;

        bool isFirstMove = true;
        float lastMoveTime = 0f;

        public override void Init()
        {
            base.Init();
            isFirstMove = true;
        }

        private void Update()
        {
            if (lastMoveTime < 4f)
            {
                lastMoveTime += Time.deltaTime;
            }
           base.RayCheckGround();
        }

        protected override void AI_Move()
        {
            if (!gameObject.activeSelf || isAction) { return; }

            if (currentAI != null)
            {
                Stop();
                currentAI = null;
            }

            Debug.Log(gameObject.name + ": Move()");
            currentAI = StartCoroutine(NavMeshMoveCoroutine());
        }


        public override void Stop()
        {
            base.Stop();

            m_pathAI.destination = transform.position;
            m_pathAI.enabled = false;
        }


        IEnumerator NavMeshMoveCoroutine()
        {
            Vector3 target = moveMarker.position;
                //gameMgr.playMgr.tokMgr.tokMarker.transform.position;
            remainDistance = Vector3.Distance(target, transform.position);


            //6/14/2024-LYI
            //점프 동작 스테이지 시작 시 처음만 나오도록 변경
            if (Mathf.Abs(remainDistance - destinationDistance) < shortMoveDistance)
            {
                statMove = MoveState.WALK;
                m_character.SetAnimation(AnimationType.WALK);
            }
            else
            {
                statMove = MoveState.RUN;
                m_character.SetAnimation(AnimationType.RUN);

                if (isFirstMove)
                {
                    isFirstMove = false;
                    m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, true);

                    Debug.Log(gameObject.name + "StartStoped()");
                    isAction = true;
                    yield return new WaitForSeconds(1f);
                    isAction = false;

                    m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, false);
                }
            }


            //if (statMove == MoveState.NONE &&
            //    lastMoveTime > 0.5f) 
            //{
            //    m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, true);

            //    if (Mathf.Abs(remainDistance - destinationDistance) < shortMoveDistance)
            //    {
            //        statMove = MoveState.WALK;
            //        m_character.SetAnimation(AnimationType.WALK);
            //    }
            //    else
            //    {
            //        statMove = MoveState.RUN;
            //        m_character.SetAnimation(AnimationType.RUN);

            //        Debug.Log(gameObject.name + "StartStoped()");
            //        isAction = true;
            //        yield return new WaitForSeconds(1f);
            //        isAction = false;
            //    }
            //    m_character.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, false);
            //}
            //else
            //{
            //    statMove = MoveState.RUN;
            //}
        
          
            if (statMove == MoveState.WALK)
            {
                m_character.SetAnimation(AnimationType.WALK);
            }
            if (statMove == MoveState.RUN)
            {
                m_character.SetAnimation(AnimationType.RUN);
            }


            m_pathAI.maxSpeed = moveSpeed;

            m_pathAI.enabled = true;

            isMove = true;

            //빌드 시 캐릭터가 안움직이는 원인
            //Path path = m_seeker.StartPath(transform.position, target);

            //yield return StartCoroutine(path.WaitForPath());

            float t = 0;
            bool isStuck = false;
            while (!m_pathAI.reachedDestination &&
                !isStuck)
            {
                t += Time.deltaTime;

                //  Debug.Log(m_pathAI.velocity.sqrMagnitude);
                if (t > 1f && m_pathAI.velocity.sqrMagnitude < 0.005f)
                {
                    isStuck = true;
                }

                // 현재 속도를 향하는 방향으로 회전
                Vector3 targetDirection = m_pathAI.velocity.normalized;

                //if (targetDirection != Vector3.zero &&
                //    m_pathAI.velocity.sqrMagnitude > 0.005f)
                //{
                //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                //}

                if (targetDirection != Vector3.zero && m_pathAI.velocity.sqrMagnitude > 0.005f)
                {
                    // y축만 고려한 회전 방향
                    float targetYRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

                m_pathAI.destination = target;

                yield return null;
            }

            m_pathAI.destination = transform.position;
            m_pathAI.enabled = false;

            statMove = MoveState.STOP;

            isMove = false;

            m_character.SetAnimation(AnimationType.IDLE);

            statMove = MoveState.NONE;

            lastMoveTime = 0;
        }

    }
}