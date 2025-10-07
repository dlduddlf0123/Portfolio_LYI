using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

namespace VRTokTok.Interaction
{

    /// <summary>
    /// 10/5/2023-LYI
    /// 가시함정
    /// 반응형 함정
    /// 플레이어가 밟으면 일정 시간 뒤 튀어나온다
    /// 일정 시간마다 튀어나왔다 들어갔다를 반복한다
    /// 가시에 닿으면 다시시작하게된다
    /// 높이조절은 MMF Player 활용
    /// </summary>
    public class Trap_Spike : Tok_Interact
    {
        Material mat_spike;
        [SerializeField]
        Collider m_collider;

        [SerializeField]
        Color startColor;
        [SerializeField]
        Color redColor;

        [Header("MMF_Players")]
        [SerializeField]
        MMF_Player mmf_up;
        [SerializeField]
        MMF_Player mmf_down;

        [SerializeField]
        Animator m_animator;

        public float upDelay = 1f; //밟은 뒤 튀어나올 때 까지의 시간
        public float downDelay = 1f; //튀어나온 뒤 들어갈 때 까지의 시간

        public bool isUp = false;


        public override void InteractInit()
        {
            base.InteractInit();
            mat_spike = m_renderer.material;
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                ActiveSpike();
            }
        }


        public void ActiveSpike()
        {
            if (isUp) { return; } //중복 작동 방지

            StartCoroutine(SpikeUp());
        }

        /// <summary>
        /// 10/5/2023-LYI
        /// 가시 올라오는 동작
        /// </summary>
        /// <returns></returns>
        IEnumerator SpikeUp()
        {
            isUp = true;
            mat_spike.color = startColor;

            //서서히 색깔 변화 추가
            float upTime = 0;
            while (upTime < upDelay)
            {
                upTime += Time.deltaTime;
                mat_spike.color = Color.LerpUnclamped(startColor, redColor, upTime);
                yield return null;
            }

            mat_spike.color = redColor;

            m_animator.SetTrigger("tActive");
            //mmf_up.PlayFeedbacks();
            GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_TRAP_SPIKE_OUT);
            m_collider.enabled = true;

            //서서히 색깔 변화 추가
            float downTime = 0;
            while (downTime < downDelay)
            {
                downTime += Time.deltaTime;
                mat_spike.color = Color.LerpUnclamped(redColor, startColor,  downTime);
                yield return null;
            }

            m_collider.enabled = false;
            mat_spike.color = startColor;
            //mmf_down.PlayFeedbacks();
            isUp = false;
        }




        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            ActiveSpike();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }


    }
}