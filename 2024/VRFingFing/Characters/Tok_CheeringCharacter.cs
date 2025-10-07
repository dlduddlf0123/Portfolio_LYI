using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;

namespace VRTokTok.Character
{
    /// <summary>
    /// 8/31/2023-LYI
    /// 응원하는 캐릭터용 애니메이션 컨트롤
    /// </summary>
    public class Tok_CheeringCharacter : Tok_Character
    {
        //public TokSelect tok_select;

        Coroutine cheeringCoroutine = null;
        [SerializeField]
        Transform tr_renderRoot;//우다다 위 테나로 인해 별도 지정
        [SerializeField]
        Renderer[] arr_renderer;
        [SerializeField]
        Material m_enable;
        [SerializeField]
        Material m_disable;

        public GameObject tokSelect;
        public Transform tr_particleRoot;

        public bool isLock = false;

        public int lockStageNum = 200; //활성화 되는 스테이지 번호

        private void Start()
        {
            if (tr_renderRoot == null)
            {
                tr_renderRoot = this.transform;
            }
            arr_renderer = tr_renderRoot.GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            if (isLock)
            {
                m_animator.SetBool("isLock", true);
                return;
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #region On Cheering

        /// <summary>
        /// 8/31/2023-LYI
        /// 응원 멈추기
        /// </summary>
        public void StopCheering()
        {
            if (cheeringCoroutine != null)
            {
                StopCoroutine(cheeringCoroutine);
                cheeringCoroutine = null;
            }

            OnCheeringIdle();
        }

        public void OnCheeringIdle()
        {
            if (isLock) { return; }
            if (cheeringCoroutine != null)
            {
                StopCoroutine(cheeringCoroutine);
                cheeringCoroutine = null;
            }
            cheeringCoroutine = StartCoroutine(CheeringIdle());
        }

        /// <summary>
        /// 8/31/2023-LYI
        /// 응원 애니 호출
        /// </summary>
        /// <param name="num">0:Start/1:Fail/2:Clear</param>
        public void PlayCheeringAnim(int num)
        {
            if (isLock) { return; }
            if (!GameManager.Instance.playMgr.cheeringSeat.isChearingSeatActive) { return; }
            if (!gameObject.activeInHierarchy) { return; }


            if (cheeringCoroutine != null)
            {
                StopCoroutine(cheeringCoroutine);
                cheeringCoroutine = null;
            }

            cheeringCoroutine = StartCoroutine(CheeringAction(num));
        }

        /// <summary>
        /// 8/31/2023-LYI
        /// 랜덤 Idle 동작 재생
        /// Idle 코루틴에서 호출
        /// </summary>
        /// <param name="num">0~2 Idle</param>
        public void PlayIdleAnim(int num)
        {
            if (isLock) { return; }
            if (!gameObject.activeInHierarchy) { return; }

            //Debug.Log(gameObject.name + "_idleNum: " + num);
            m_animator.SetTrigger(VRTokTok.Constants.Animator.TRIGGER_IDLE);
            m_animator.SetFloat(VRTokTok.Constants.Animator.FLOAT_IDLE, num);
        }

        /// <summary>
        /// 8/24/2023-LYI
        /// 평소에 응원 동작 루프
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheeringIdle()
        {
            while (playMgr.statPlay == PlayStatus.PLAY &&
                isLock == false)
            {
                int a = Random.Range(0, 3);
                PlayIdleAnim(a);

                float t = Random.Range(3f, 5f);
                yield return new WaitForSeconds(t);
            }
        }

        /// <summary>
        /// 8/31/2023-LYI
        /// 응원모션 재생 코루틴
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private IEnumerator CheeringAction(int num)
        {
            // Debug.Log(gameObject.name + "_actionNum: " + num);
            m_animator.SetTrigger("t_action");
            m_animator.SetFloat("actionNum", num);

            float t = Random.Range(3f, 5f);
            yield return new WaitForSeconds(t);
            OnCheeringIdle();
        }


        /// <summary>
        /// 7/15/2024-LYI
        /// 응원석 캐릭터 활성화 여부 변경
        /// 캐릭터 머테리얼, 애니, 터치 여부 변경
        /// </summary>
        /// <param name="isActive"></param>
        public void SetLock(bool isActive)
        {
            if (arr_renderer == null)
            {
                return;
            }
            if (isActive)
            {
                //잠금상태로 변경
                isLock = true;

                for (int i = 0; i < arr_renderer.Length; i++)
                {
                    arr_renderer[i].material = m_disable;
                }

                m_animator.SetBool("isLock", true);
                StopCheering();

                tokSelect.SetActive(false);
            }
            else
            {
                //잠금 해제
                isLock = false;

                for (int i = 0; i < arr_renderer.Length; i++)
                {
                    arr_renderer[i].material = m_enable;
                }
                m_animator.SetBool("isLock", false);
                tokSelect.SetActive(true);
            }

        }


        #endregion
    }
}