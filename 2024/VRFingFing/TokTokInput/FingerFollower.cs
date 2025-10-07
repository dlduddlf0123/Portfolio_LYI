using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok
{
    /// <summary>
    /// 6/24/2024-LYI
    /// Call from TokTokManager
    /// 손가락 할당해 주면 따라감
    /// </summary>
    public class FingerFollower : MonoBehaviour
    {
        public ParticleSystem efx_finger;
        public TrailRenderer efx_fingerTrail;
        public ParticleSystem efx_pop;

        public Transform tr_followTarget;

        public bool isGroundClick = false;

        private void OnEnable()
        {
            PlayParticle();
        }
        private void OnDisable()
        {
            StopParticle();
        }


        void Update()
        {
            if (tr_followTarget != null)
            {
                transform.position = tr_followTarget.position;
                GroundCheck();
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Tok"))
            {
                if (coll.gameObject.GetComponent<HandMarker>())
                {
                    coll.gameObject.GetComponent<HandMarker>().ActiveInteraction();
                }

            }
        }



        public void GroundCheck()
        {
            Vector3 start = transform.position;

            int frontRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
                           (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
                           (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                    (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                                 (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER));
           //7/1/2024-LYI 점프대 터치 이슈로 레이어 제외
           //(1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT)

            //플랫폼 레이 체크
            RaycastHit groundRay;

            Color rayColor = Color.blue;

            float groundRayLength = 0.02f;

            if (Physics.Raycast(start, Vector3.down, out groundRay, groundRayLength, ~frontRayMask))
            {
                if (groundRay.collider.gameObject.CompareTag("Ground"))
                {
                    isGroundClick = true;
                    rayColor = Color.green;
                }
            }
            else
            {
                isGroundClick = false;
                if (GameManager.Instance.playMgr.tokMgr.lastTokGround != null)
                {
                    GameManager.Instance.playMgr.tokMgr.lastTokGround.gameObject.SetActive(false);
                    GameManager.Instance.playMgr.tokMgr.lastTokGround.gameObject.SetActive(true);
                    GameManager.Instance.playMgr.tokMgr.lastTokGround = null;
                }
            }
            Debug.DrawRay(start, Vector3.down * groundRayLength, rayColor, 0.1f);
        }



        /// <summary>
        /// 6/24/2024-LYI
        /// 손가락 효과 재생
        /// </summary>
        public void PlayParticle()
        {
            efx_finger.Play();
            efx_fingerTrail.gameObject.SetActive(true);
        }

        public void StopParticle()
        {
            efx_finger.Stop();
            efx_fingerTrail.gameObject.SetActive(false);
        }
    }
}