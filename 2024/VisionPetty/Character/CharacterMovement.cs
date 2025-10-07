using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using Pathfinding;
//using MoreMountains.Feedbacks;

namespace AroundEffect
{
    public enum MoveState
    {
        NONE = 0,
        START,
        WALK,
        RUN,
        STOP,
    }

    /// <summary>
    /// 240805 LYI
    /// AI Movement, Navigation
    /// Move actions
    /// Move stats
    /// </summary>
    public class CharacterMovement : MonoBehaviour
    {
        protected GameManager gameMgr;

        public CharacterManager charMgr;

        public Rigidbody m_rigidbody;

        [Header("MMF_Player")]
        //public MMF_Player[] arr_feedback;

        [Header("Transform Properties")]
        public Transform defaultParent; //?????? parent ??????
        public Transform moveMarker; //?????? ????, ???? ??????
        public Transform tr_tower; //?? ???? ?? ???? ???????? ?????? ????

        //???? ????
        private Vector3 moveTargetPos;

        [Header("Navigation")]
        [SerializeField]
        Seeker m_seeker;

        [SerializeField]
        RichAI m_navAI;

        bool isFirstMove = true;
        float lastMoveTime = 0f;

        public MoveState statMove = MoveState.NONE;



        [Header("GamePlay Properties")]
        public float moveSpeed = 5f;
        public float jumpForce = 5f;
        public float rotationSpeed = 10f;

        public float maxJumpDistance = 0.05f; //???? ???? ????
        public float maxJumpHeight = 0.1f; //???? ???? ????
        public float minJumpHeight = 0.03f; //???? ???? ????

        public float actJumpHeight = 0.1f; //???? ????
        public float actJumpDuration = 0.2f; //???? ????
        public float jumpStartWaitTime = 0.16f;

        public float destinationDistance = 0.03f; //???????? ???????? ???? ??????
        public float shortMoveDistance = 0.06f; //???? ???????? ????

        [Header("Move Status")]
        public bool isGround; //?????? ??????????

        public bool isTowerAble; //?? ?????? ????????

        [Header("Update Properties")]
        public float remainDistance; //???? ???????? ???? ??????
        public float movingVelocity; //???? ???????? ????


        public bool isMove = false; //???? ?????? ????
        public bool isAction = false;    //?????? ?????????? ????
        public bool isDie = false;
        public bool isHit = false;
       
        public bool isJump = false; //???? ???? ???? ????
        public bool isCrushAble = false; //???? ??????
        public bool isGoingSleep = false;

        float sturnTime = 1.2f;
        float crushAnimMoveTime = 0.12f;


        public Coroutine currentCoroutine = null;

        bool isInit = false;



        public virtual void Init()
        {
            if (!isInit)
            {
                gameMgr = GameManager.Instance;
                //  m_character = GetComponent<Tok_Character>();
                m_rigidbody = GetComponent<Rigidbody>();

                defaultParent = transform.parent;


                isInit = true;
            }

            SetFixedMode(false);
            ResetParent();

            m_rigidbody.isKinematic = false;
            m_rigidbody.useGravity = true;

            transform.rotation = Quaternion.identity;
            isDie = false;

            Stop();
        }

        /// <summary>
        /// ?????? ???? ??????, ?????? ??????
        /// </summary>
        public virtual void Stop()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            Debug.Log(gameObject.name + "_Movement: Stop()");

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            // m_AIPath.isStopped = true;
            isAction = false;
            isJump = false;
            isCrushAble = false;

            isMove = false;
            charMgr.Animation.m_animator.SetBool(Constants.Animator.BOOL_MOVE_START, false);
            m_rigidbody.velocity = Vector3.zero;



            charMgr.Animation.SetAnimation(AnimationType.IDLE);


            m_navAI.destination = transform.position;
            m_navAI.enabled = false;
        }


        /// <summary>
        /// 9/3/2024-LYI
        /// call from reset collider
        /// when character falling to ground in real world
        /// </summary>
        public void ResetPosition()
        {
            SetFixedMode(false);
            ResetParent();

            m_rigidbody.velocity = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;


            Stop();
        }

        public void ResetParent()
        {
            transform.SetParent(defaultParent);
            //m_rigidbody.isKinematic = false;
        }

        public void SetParentMenu(Transform tr)
        {
            transform.SetParent(tr);
            m_rigidbody.isKinematic = true;
        }

        private void OnDisable()
        {
            Stop();
        }


        public void SetMoveMarker(Transform target)
        {
            moveMarker.SetParent(target);
            moveMarker.transform.localPosition = Vector3.zero;
        }
        public void SetMoveMarker(Vector3 targetPos)
        {
            moveMarker.transform.position = targetPos;
        }

        public void ResetMoveMarker()
        {
            moveMarker.SetParent(charMgr.transform);
            moveMarker.transform.localPosition = Vector3.zero;
        }

        public void ResetRotation()
        {
            transform.rotation = Quaternion.identity;
        }

        public void StartMove(UnityAction action = null)
        {
            if (!gameObject.activeInHierarchy || isAction) { return; }

            if (currentCoroutine != null)
            {
                Stop();
                currentCoroutine = null;
            }

            Debug.Log(gameObject.name + "_Movement: Move()");
            currentCoroutine = StartCoroutine(NavigationMoveCoroutine(action));
        }



        private IEnumerator NavigationMoveCoroutine(UnityAction action)
        {
            Vector3 target = moveMarker.position;
            remainDistance = Vector3.Distance(target, transform.position);

            m_navAI.destination = target;
            m_navAI.maxSpeed = moveSpeed;
            m_navAI.enabled = true;

            isMove = true;

            int randomMove = Random.Range(0, 2);
            if (randomMove == 0)
            {
                statMove = MoveState.WALK;
                charMgr.Animation.SetAnimation(AnimationType.WALK);
            }
            else
            {
                statMove = MoveState.RUN;
                charMgr.Animation.SetAnimation(AnimationType.RUN);
            }

            float t = 0;
            bool isStuck = false;
            while (!m_navAI.reachedDestination &&
                !isStuck)
            {
                t += Time.deltaTime;

                if (t > 3f)
                {
                    isStuck = true;
                }

                Vector3 targetDirection = m_navAI.velocity.normalized;

                if (targetDirection != Vector3.zero && m_navAI.velocity.sqrMagnitude > 0.005f)
                {
                    float targetYRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

                m_navAI.destination = target;

                yield return null;
            }


            //after movement
            m_navAI.destination = transform.position;
            m_navAI.enabled = false;

            //statMove = MoveState.STOP;

            isMove = false;
            charMgr.Animation.SetAnimation(AnimationType.IDLE);

            //statMove = MoveState.NONE;

            lastMoveTime = 0;

            action?.Invoke();

        }


        /// <summary>
        /// 9/3/2024-LYI
        /// Display for jump
        /// </summary>
        /// <param name="target"></param>
        public void JumpToPosition(Transform target, UnityAction action = null)
        {
            if (!gameObject.activeInHierarchy || isAction) { return; }

            if (currentCoroutine != null)
            {
                Stop();
                currentCoroutine = null;
            }

            Debug.Log(charMgr.gameObject.name + "-Jump");
            
            currentCoroutine =  StartCoroutine(JumpToPositionByTranslate(target, action));
        }

        /// <summary>
        /// 9/3/2024-LYI
        /// Jump with transform
        /// using translate with move
        /// usually using at move to hand
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private IEnumerator JumpToPositionByTranslate(Transform target, UnityAction action)
        {
            Quaternion originRot = transform.rotation;
            Quaternion targetRot = GetTargetRotation(moveMarker.position);

            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.JUMP);

            yield return new WaitForSeconds(jumpStartWaitTime);


            float randomRange = 0.2f;
            float pitch = Random.Range(1 - randomRange, 1 + randomRange);
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_JUMP, pitch);

            float t = 0;

            Vector3 startJumpPosition = transform.position;
            Vector3 targetJumpPosition = moveMarker.position;
            float jumpStartTime = Time.time;

            while (Time.time - jumpStartTime < actJumpDuration)
            {
                t += 0.01f * rotationSpeed;

                transform.rotation = Quaternion.Lerp(originRot, targetRot, t);

                float normalizedTime = (Time.time - jumpStartTime) / actJumpDuration;

                targetJumpPosition = moveMarker.position;
                Vector3 newPosition = Vector3.Lerp(startJumpPosition, targetJumpPosition, normalizedTime);

                // Adjust the following line to make the jump height increase more gradually
                float additionalInterpolation = 1 - Mathf.Pow(1 - normalizedTime, 3);
                newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * actJumpHeight * 0.5f * additionalInterpolation;

                transform.position = newPosition;

                yield return null;
            }

            // Ensure the character reaches the final destination
            transform.position = targetJumpPosition;
            m_rigidbody.velocity = Vector3.zero;

            charMgr.Animation.SetAnimation(AnimationType.IDLE);

            isJump = false;

            action?.Invoke();
        }

        public Quaternion GetTargetRotation(Vector3 target)
        {
            Vector3 ta = new Vector3(target.x, transform.position.y, target.z);
            Vector3 pos = ta - transform.position;
            return Quaternion.LookRotation(pos);
        }

        public void SetFixedMode(bool isFix)
        {
            m_rigidbody.isKinematic = isFix;

            //charMgr.AI.isEvent = isFix;
        }


        /// <summary>
        /// 11/13/2024-LYI
        /// 캐릭터 방향 바라보기 필요해서 제작
        /// 그 외에도 여러가지 바라볼 때 사용 가능
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action"></param>
        public void LookTarget(Transform target, UnityAction action = null)
        {
            if (!gameObject.activeInHierarchy || isAction) { return; }

            if (currentCoroutine != null)
            {
                Stop();
                currentCoroutine = null;
            }

            Debug.Log(charMgr.gameObject.name + "-LookTarget");

            currentCoroutine =  StartCoroutine(LookJumpTarget(target, action));
        }

        IEnumerator LookJumpTarget(Transform target, UnityAction action = null)
        {
            Quaternion originRot = transform.rotation;
            Quaternion targetRot = GetTargetRotation(target.position);

            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.TOK);

            float t = 0;
            while (t < 1)
            {
                t += 0.01f * rotationSpeed;

                transform.rotation = Quaternion.Lerp(originRot, targetRot, t);

                yield return null;
            }

            charMgr.Animation.SetAnimation(AnimationType.IDLE);

            action?.Invoke();
        }


        /// <summary>
        /// 9/10/2024-LYI
        /// Call from gesture checker
        /// active when character falling from hand
        /// </summary>
        public void OnHandFall()
        {
            SetFixedMode(false);
            Stop();
            ResetParent();
            ResetRotation();
        }

        /// <summary>
        /// 12/2/2024-LYI
        /// 가운데 손가락 올렸을 때 뒷걸음치기
        /// </summary>
        public void OnMiddleUp()
        {
            if (!gameObject.activeInHierarchy || isAction) { return; }

            if (currentCoroutine != null)
            {
                Stop();
                currentCoroutine = null;
            }

            Debug.Log(charMgr.gameObject.name + "-MiddleUp()");

            currentCoroutine = StartCoroutine(MiddleUpMove());

        }


        /// <summary>
        /// 12/2/2024-LYI
        /// 플레이어 바라본 뒤 뒤로 이동
        /// </summary>
        /// <returns></returns>
        IEnumerator MiddleUpMove()
        {

            yield return StartCoroutine(LookJumpTarget(GameManager.Instance.MRMgr.XR_Origin.Camera.transform));

            Transform target = gameMgr.MRMgr.XR_Origin.transform;

            charMgr.Animation.PlayTriggerAnimation(TriggerAnimationType.MIDDLE_UP);

            yield return new WaitForSeconds(2f);

            //Vector3 startPos = transform.position;
            //Vector3 endPos = transform.position + (target.transform.position - transform.position).normalized * 0.5f;
            //float t = 0;
            //while (t < 2)
            //{
            //    t += Time.deltaTime;

            //    transform.position = Vector3.Lerp(startPos, endPos, t * 0.5f);
            //    yield return null;
            //}

            charMgr.AI.AIMove(AIState.IDLE);
        }


    }
}
    

    