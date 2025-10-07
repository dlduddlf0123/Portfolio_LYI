using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AroundEffect
{

    /// <summary>
    /// 12/9/2024-LYI
    /// 미니게임용 캐릭터
    /// 조작, 충돌 관련
    /// </summary>

    public class MiniGameCharacter : MonoBehaviour
    {
        public MiniGameManager minigameMgr;

        public CharacterAnimation Animation;

        Rigidbody m_rigidbody;

        public int currentPos = 0;

        float jumpHeight = 0.1f;
        float jumpTime = 0.2f;

        bool isJump = false;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            Animation.Init();

            m_rigidbody = GetComponent<Rigidbody>();
            currentPos = 1;
        }



        public void LeftJump()
        {
            if (minigameMgr.statMiniGame != MiniGameStatus.GAME) { return; }
            if (isJump)
            {
                return;
            }
            isJump = true;

            if (currentPos > 0)
            {
                Animation.PlayTriggerAnimation(TriggerAnimationType.MINIGAME_LEFT);
                jumpHeight = 0.05f;
                jumpTime = 0.3f;

                currentPos -= 1;
                if (currentPos < 0)
                {
                    currentPos = 0;
                }
                JumpToPosition(minigameMgr.arr_movePos[currentPos]);
                Debug.Log("LeftJump");
            }

        }

        public void RightJump()
        {
            if (minigameMgr.statMiniGame != MiniGameStatus.GAME){ return; }
            if (isJump)
            {
                return;
            }

            isJump = true;

            if (currentPos < 2)
            {
                Animation.PlayTriggerAnimation(TriggerAnimationType.MINIGAME_RIGHT);
                jumpHeight = 0.05f;
                jumpTime = 0.3f;

                currentPos += 1;
                if (currentPos >2)
                {
                    currentPos = 2;
                }
                JumpToPosition(minigameMgr.arr_movePos[currentPos]);
                Debug.Log("RightJump");
            }
        }

        public void UpJump()
        {
            if (minigameMgr.statMiniGame != MiniGameStatus.GAME) { return; }
            if (isJump)
            {
                return;
            }
            isJump = true;

            Animation.PlayTriggerAnimation(TriggerAnimationType.MINIGAME_UP);
            jumpHeight = 0.5f;
            jumpTime = 0.5f;

            JumpToPosition(minigameMgr.arr_movePos[currentPos]);

            Debug.Log("UpJump");
        }


        public void OnGameOver()
        {
            Animation.PlayTriggerAnimation(TriggerAnimationType.HIT);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RightJump();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LeftJump();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UpJump();
            }
        }
#endif

        Coroutine currentCoroutine;

        /// <summary>
        /// 9/3/2024-LYI
        /// Display for jump
        /// </summary>
        /// <param name="target"></param>
        public void JumpToPosition(Transform target, UnityAction action = null)
        {
            if (!gameObject.activeInHierarchy) { return; }

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            Debug.Log(gameObject.name + "-Jump");

            currentCoroutine = StartCoroutine(JumpToPositionByTranslate(target, action));
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
            //float randomRange = 0.2f;
            //float pitch = Random.Range(1 - randomRange, 1 + randomRange);
            //gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_JUMP, pitch);

            Vector3 startJumpPosition = transform.position;
            Vector3 targetJumpPosition = target.position;
            float jumpStartTime = Time.time;

            while (Time.time - jumpStartTime < jumpTime)
            {
                float normalizedTime = (Time.time - jumpStartTime) / jumpTime;

                targetJumpPosition = target.position;
                Vector3 newPosition = Vector3.Lerp(startJumpPosition, targetJumpPosition, normalizedTime);

                // Adjust the following line to make the jump height increase more gradually
                float additionalInterpolation = 1 - Mathf.Pow(1 - normalizedTime, 3);
                newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight* 0.5f * additionalInterpolation;

                transform.position = newPosition;

                yield return null;
            }

            // Ensure the character reaches the final destination
            transform.position = targetJumpPosition;
            m_rigidbody.velocity = Vector3.zero;

            Animation.SetAnimation(AnimationType.IDLE);

            isJump = false;

            action?.Invoke();
        }


    }
}