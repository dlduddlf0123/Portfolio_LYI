using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using VRTokTok.Interaction;

using MoreMountains.Feedbacks;

namespace VRTokTok.Interaction
{
    /// <summary>
    /// 4/1/2024-LYI
    /// Block에서 떨어지는 발판
    /// </summary>
    public class Block_Falling : Tok_Interact
    {
        [Header("Private")]
        Rigidbody m_rigidbody;
        Vector3 respawnVector; //다시 생성될 위치

        Material m_material;
        [SerializeField]
        Color startColor;
        [SerializeField]
        Color redColor;

        [SerializeField]
        Collider[] arr_collider;

        [Header("Public")]
        public MMPositionShaker mmf_shake; //떨리는 효과
        public MMF_Player mmf_respawn; //리스폰 효과
        public MMF_Player mmf_disappear; //사라지는 효과


        public float timeUntilFall = 3f; //떨어지기 까지의 시간
        public float respawnTime = 3f; //땅에 떨어진 뒤 리스폰 되는 시간
        public float fallSpeed = 1f; //떨어지는 속도

        public float standingTime = 0f; //서있는 시간 체크

        public bool isCharacterStanding = false; //캐릭터가 서 있는지 여부
        public bool isFalling = false; //떨어지고 있는지?

        bool isFirst = true;

        Coroutine currentCoroutine = null;

        public override void InteractInit()
        {
            base.InteractInit();

            if (isFirst)
            {
                isFirst = false;
                m_rigidbody = GetComponent<Rigidbody>();
                m_material = m_renderer.material;

                //리스폰 위치 저장
                respawnVector = transform.localPosition;
            }

            if (isFalling)
            {
                RespawnBlock();
            }

            Stop();
            transform.localPosition = respawnVector;

            standingTime = 0;
            isCharacterStanding = false;
            isFalling = false;
            m_rigidbody.isKinematic = false;

            m_material.color = startColor;

            SetColldersActive(true);
        }


        /// <summary>
        /// 4/17/2024-LYI
        /// 코루틴 정지
        /// </summary>
        private void Stop()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (isFalling)
            {
                //떨어지는 도중에 다른 블록을 만난경우
                if (coll.gameObject.CompareTag("Fall"))
                {
                    Stop(); 
                    BlockDisappear();
                }

                return;
            }
            if (coll.gameObject.CompareTag("Header"))
            {
                isCharacterStanding = true;
            }
        }

        private void OnTriggerStay(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                isCharacterStanding = true;
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (isFalling) { return; }
            if (coll.gameObject.CompareTag("Header"))
            {
                isCharacterStanding = false;
                standingTime = 0;
                m_material.color = startColor;
                mmf_shake.Stop();
            }
        }

        //Update로 올라간 시간 체크
        private void Update()
        {
            if (m_material == null)
            {
                return;
            }

            //캐릭터가 서있고 떨어지는 중이 아닌 경우
            if (isCharacterStanding && !isFalling)
            {
                standingTime += Time.deltaTime;
                if (!mmf_shake.Shaking)
                {
                    mmf_shake.Play();
                    GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_BLOCK_FALL_SHAKE);
                }
                m_material.color = Color.LerpUnclamped(startColor, redColor, standingTime * 2f);

                if (standingTime > timeUntilFall)
                {
                    mmf_shake.Stop();
                    ActiveInteraction();
                }
            }
        }


        /// <summary>
        /// 4/1/2024-LYI
        /// 발판 떨어지기
        /// </summary>
        /// <returns></returns>
        IEnumerator BlockFalling()
        {
            if (isFalling)
            {
                yield break;
            }

            isFalling = true;
            m_rigidbody.isKinematic = true;
            m_material.color = redColor;
            GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_BLOCK_FALL_FALLING);


            Transform bottomTarget = GameManager.Instance.tableMgr.playTable.tr_stageDownPlane;

            while (isFalling && 
                transform.position.y > bottomTarget.position.y &&
                !RayCheckGround())
            {
                transform.localPosition -= Vector3.up * Time.deltaTime * fallSpeed;
                yield return null;
            }

            BlockDisappear();
        }

        float groundRayLength = 0.03f;
        private bool RayCheckGround()
        {
            Vector3 start = transform.position - Vector3.up * arr_collider[2].bounds.size.y * 0.25f;

            int frontRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                    (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE)) |
                       (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));

            //플랫폼 레이 체크
            RaycastHit groundRay;

            Color rayColor = Color.blue;

            if (Physics.Raycast(start, Vector3.down, out groundRay, groundRayLength, ~frontRayMask))
            {
                if (groundRay.collider.gameObject.CompareTag("Ground"))
                {
                    rayColor = Color.green;
                    Debug.DrawRay(start, Vector3.down * groundRayLength, rayColor, 0.1f);
                    return true;
                }
            }
            else
            {
                Debug.DrawRay(start, Vector3.down * groundRayLength, rayColor, 0.1f);
                return false;
            }

            return false;
        }


        /// <summary>
        /// 4/17/2024-LYI
        /// 블록 이동 정지 및 작아지며 사라지는 효과
        /// </summary>
        /// <returns></returns>
        void BlockDisappear()
        {
            m_rigidbody.isKinematic = true;
            mmf_disappear.PlayFeedbacks();
            GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_BLOCK_FALL_DISAPPEAR);

            SetColldersActive(false);

             currentCoroutine = StartCoroutine(WaitforRespawn());
        }


        /// <summary>
        /// 4/17/2024-LYI
        /// 컬리더 활성화 상태 변경
        /// </summary>
        /// <param name="isActive"></param>
        void SetColldersActive(bool isActive)
        {
            for (int i = 0; i < arr_collider.Length; i++)
            {
                arr_collider[i].enabled = isActive;
            }
        }

        /// <summary>
        /// 4/17/2024-LYI
        /// 떨어지면 일정 시간 뒤 리셋
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitforRespawn()
        {
            yield return new WaitForSeconds(respawnTime);

            RespawnBlock();
        }



        /// <summary>
        /// 4/1/2024-LYI
        /// 떨어진 뒤 호출
        /// 블록 재 생성
        /// </summary>
        public void RespawnBlock()
        {
            transform.localPosition = respawnVector;

            //재생성 효과 재생
            mmf_respawn.PlayFeedbacks();
            GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_BLOCK_FALL_RESPAWN);

            isCharacterStanding = false;
            isFalling = false;
            standingTime = 0;
            m_rigidbody.isKinematic = false;

            m_material.color = startColor;

            SetColldersActive(true);
        }




        public override void ActiveInteraction()
        {
            base.ActiveInteraction();

            currentCoroutine = StartCoroutine(BlockFalling());
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}