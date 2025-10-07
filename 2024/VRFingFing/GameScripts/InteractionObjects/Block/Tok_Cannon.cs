using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;
using VRTokTok.Character;

namespace VRTokTok.Interaction
{
    /// <summary>
    /// 5/13/2024-LYI
    /// 캐릭터 발사하는 대포
    /// 충돌한 캐릭터를 장전 후 해당 위치로 발사한다
    /// </summary>
    public class Tok_Cannon : Tok_Interact
    {
        [Header("Tok_Cannon")]
        [SerializeField]
        Transform tr_headerAnchor;

        [SerializeField]
        Collider m_cannonColl;

        public Transform tr_cannonDestination;

        Tok_Movement bullet_header;

        Vector3 originScale = Vector3.one;

        public MMF_Player mmf_ready;
        public MMF_Player mmf_fire;
        public MMF_Player mmf_finish;

        public ParticleSystem p_fire;
        public ParticleSystem p_land;

        public float firePower = 1.4f;

        public override void InteractInit()
        {
            base.InteractInit();

            if (!base.isInit)
            {
                base.isInit = true;

                mmf_fire.Events.OnComplete.AddListener(CannonFire);
                //mmf_finish.Events.OnComplete.AddListener(InteractInit);
                mmf_finish.Events.OnComplete.AddListener(FireReady);
            }

            ResetBullet();
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                bullet_header = coll.gameObject.GetComponent<Tok_Movement>();

                if (bullet_header.isDie || bullet_header.isAction) 
                {
                    return;
                }
                originScale = bullet_header.transform.localScale;

                bullet_header.transform.SetParent(tr_headerAnchor);
                bullet_header.transform.localPosition = Vector3.zero;
                bullet_header.transform.localRotation = Quaternion.identity;
                if (bullet_header.m_character.typeHeader == HeaderType.TENA)
                {
                    bullet_header.transform.localScale *= 0.9f;
                }
                else
                {
                    bullet_header.transform.localScale *= 0.5f;
                }
                bullet_header.FixedMode(true);

                ActiveInteraction();
            }
        }

        /// <summary>
        /// 5/13/2024-LYI
        /// 대포 발사
        /// </summary>
        public void CannonFire()
        {
            FireCoolDown();

            if (bullet_header == null)
            {
                return;
            }
            //발사 이펙트
            p_fire.Play();
            //발사 사운드
            GameManager.Instance.soundMgr.PlaySfx(tr_headerAnchor.position, Constants.Sound.SFX_INTERACTION_CANNON_FIRE);

            bullet_header.ResetParent();
            bullet_header.transform.localScale = originScale;

            bullet_header.FixedMode(false);

            Vector3 fireVec = bullet_header.transform.forward.normalized * 
                CalculatedFirePower(tr_headerAnchor.position,tr_cannonDestination.position, 45) * firePower;

            bullet_header.FireHeader(fireVec, ForceMode.Impulse);
            StartCoroutine(CannonBulletRotation());
        }


        /// <summary>
        /// 5/30/2024-LYI
        /// 시작 벡터에서 끝 벡터로 도착할 수 있는 힘 계산
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        float CalculatedFirePower(Vector3 start, Vector3 end, float angle)
        {
            Vector3 delta = end - start;
            float g = Mathf.Abs(Physics.gravity.y);

            float radAngle = Mathf.Deg2Rad * angle;

            float v0 = Mathf.Sqrt((g * delta.magnitude * delta.magnitude) /
                (2 * (delta.magnitude * Mathf.Tan(radAngle) - delta.y)));

            return v0;
        }


        /// <summary>
        /// 5/30/2024-LYI
        /// 발사 직후 재 발사까지 대기
        /// </summary>
        public void FireCoolDown()
        {
            m_cannonColl.enabled = false;
        }

        public void FireReady()
        {
            m_cannonColl.enabled = true;
        }

        /// <summary>
        /// 5/30/2024-LYI
        /// 대포로 비행 중 로테이션 변경
        /// </summary>
        /// <returns></returns>
        IEnumerator CannonBulletRotation()
        {
            Transform tr_child = bullet_header.transform.GetChild(0);
            float childRot = 0f;

            Vector3 direction = Vector3.zero;
            Vector3 lastVelocity = Vector3.zero;

            while (bullet_header != null &&
                bullet_header.isGround == false &&
                bullet_header.isAction == false)
            {
                direction = bullet_header.m_rigidbody.velocity.normalized;

                if (bullet_header.m_rigidbody.velocity != Vector3.zero)
                {
                    bullet_header.transform.rotation = Quaternion.LookRotation(direction);
                    lastVelocity = bullet_header.m_rigidbody.velocity;
                }
                yield return null;
            }

            if (bullet_header != null && bullet_header.isGround)
            {
                float targetYRotation = Mathf.Atan2(lastVelocity.x, lastVelocity.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
                bullet_header.transform.rotation = targetRotation;
            }

            bullet_header.transform.localScale = originScale;
            //tr_child.localRotation = Quaternion.identity;

           Vector3 direction2 = NomalizedY(lastVelocity).normalized;
            bullet_header.transform.rotation = Quaternion.LookRotation(direction2);
            bullet_header.m_rigidbody.velocity = Vector3.zero;
            bullet_header.isCrushAble = false;

            //p_land.transform.position = bullet_header.transform.position - Vector3.up * 0.025f;
            //p_land.Play();

            ResetBullet();
        }

        /// <summary>
        /// 5/13/2024-LYI
        /// 해당 지점으로 포물선 운동 진행
        /// </summary>
        /// <returns></returns>
        IEnumerator CannonMoveCoroutine()
        {
            Vector3 start = tr_headerAnchor.position;
            Vector3 end = tr_cannonDestination.position;

            float jumpTime = 0;
            float actJumpDuration = 1f;

            Vector3 lastPos = start;
            Transform tr_child = bullet_header.transform.GetChild(0);
            float childRot = 0f;

            while (jumpTime < actJumpDuration &&
                 bullet_header != null)
            {
                jumpTime += Time.deltaTime;
                childRot += Time.deltaTime * 10f;

                float normalizedTime =  jumpTime / actJumpDuration;

                Vector3 newPosition = Vector3.Lerp(start, end, normalizedTime);

                 float additionalInterpolation = 1 - Mathf.Pow(1 - normalizedTime, 3);
                newPosition.y += Mathf.Sin(normalizedTime * Mathf.PI) * 0.5f * 0.1f * additionalInterpolation;

                bullet_header.transform.position = newPosition;
                Vector3 direction = (bullet_header.transform.position - lastPos).normalized;
                bullet_header.transform.rotation = Quaternion.LookRotation(direction);

                tr_child.Rotate(new Vector3(0, 0, childRot));

                lastPos = bullet_header.transform.position;

                
                yield return null;
            }

            GameManager.Instance.soundMgr.PlaySfx(tr_headerAnchor.position, Constants.Sound.SFX_HEADER_LANDED);

            bullet_header.transform.position = end;
            bullet_header.transform.localScale = originScale;
            tr_child.localRotation = Quaternion.identity;

            Vector3 direction2 = (NomalizedY(end) - NomalizedY(start)).normalized;
            bullet_header.transform.rotation = Quaternion.LookRotation(direction2);

            ResetBullet();
        }
        Vector3 NomalizedY(Vector3 origin)
        {
            Vector3 result = new Vector3(origin.x, 0, origin.z);
            return result;
        }

        void ResetBullet()
        {
            if (bullet_header != null)
            {
                if (bullet_header.transform.parent == tr_headerAnchor)
                {
                    bullet_header.ResetParent();
                }
                bullet_header.transform.localScale = originScale;
                bullet_header.FixedMode(false);
            }
            bullet_header = null;
        }


        public override void ActiveInteraction()
        {
            base.ActiveInteraction();

            mmf_ready.PlayFeedbacks();
            GameManager.Instance.soundMgr.PlaySfx(tr_headerAnchor.position, Constants.Sound.SFX_INTERACTION_CANNON_READY);

        }


        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }
    }
}