using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace AroundEffect
{

    #region Enum States
    public enum AnimationType
    {
        NONE = 0,
        IDLE,
        WALK,
        RUN,
    };

    public enum TriggerAnimationType
    {
        IDLE = 0,
        JUMP,
        TOK,
        CRASH,
        FAIL,
        CLEAR,
    }

    /// <summary>
    /// 11/14/2023-LYI
    /// 게임오버 원인 체크
    /// </summary>
    public enum GameOverType
    {
        NONE = 0,
        WATER = 1,
        SPIKE,
        FLAME,
        MISSILE,
        PRESS,
    }

    #endregion


    /// <summary>
    /// 8/26/2024-LYI
    /// 캐릭터 애니 등 동작 제어
    /// </summary>
    public class Character : MonoBehaviour
    {
        GameManager gameMgr;

        public Animator m_animator;
        public AnimationType typeAnim;  //작동중인 애니메이션 상태

        public ParticleSystem efx_portal; //등장, 사라질 시 사용될 포탈 파티클


        //3/4/2024-LYI
        //눈 활성화 체크용
        public GameObject[] arr_eyeShapeL;
        public GameObject[] arr_eyeTransformL;

        public GameObject[] arr_eyeShapeR;
        public GameObject[] arr_eyeTransformR;


        Coroutine animCoroutine = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;

            if (m_animator == null)
            {
                m_animator = GetComponent<Animator>();
            }
        }

        public virtual void Init()
        {

        }


        /// <summary>
        /// 3/4/2024-LYI
        /// 눈 활성화 체크 루프
        /// </summary>
        private void Update()
        {
            UpdateEyeActive(arr_eyeTransformL, arr_eyeShapeL);
            UpdateEyeActive(arr_eyeTransformR, arr_eyeShapeR);
        }

        void UpdateEyeActive(GameObject[] arr_eyeTransform, GameObject[] arr_eyeShape)
        {
            if (arr_eyeTransform.Length > 0)
            {
                for (int i = 1; i < arr_eyeTransform.Length; i++)
                {
                    if (arr_eyeTransform[i].transform.localScale.x < 0.001f)
                    {
                        arr_eyeShape[i].SetActive(false);
                    }
                    else
                    {
                        arr_eyeShape[i].SetActive(true);

                        if (i > 0)
                            arr_eyeShape[i].transform.localScale = arr_eyeTransform[i].transform.localScale;
                    }
                }
                arr_eyeShape[0].SetActive((arr_eyeTransform[0].transform.localScale.y < 1) ? false : true);
            }
        }


        public virtual void PlayTriggerAnimation(TriggerAnimationType type, int blendNum = 0)
        {
            if (!gameObject.activeInHierarchy) { return; }
            Debug.Log(gameObject.name + "_Animation - Tragger: " + type.ToString());

            switch (type)
            {
                case TriggerAnimationType.IDLE:
                    m_animator.SetFloat(Constants.Animator.FLOAT_IDLE, blendNum);
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_IDLE);
                    break;
                case TriggerAnimationType.JUMP:
                    m_animator.SetBool(Constants.Animator.BOOL_JUMP, true);
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_JUMP);
                    break;
                case TriggerAnimationType.TOK:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_TOK);
                    break;
                case TriggerAnimationType.CRASH:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_CRASH);
                    break;
                case TriggerAnimationType.FAIL:
                    m_animator.SetFloat(Constants.Animator.FLOAT_FAIL, blendNum);
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_FAIL);
                    break;
                case TriggerAnimationType.CLEAR:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_CLEAR);
                    break;
                default:
                    break;
            }
        }


        public virtual void SetAnimation(AnimationType type)
        {
            if (!gameObject.activeInHierarchy) { return; }
            typeAnim = type;
            Debug.Log(gameObject.name + "_Animation - State: " + type.ToString());

            m_animator.SetBool(Constants.Animator.BOOL_MOVE, false);
            m_animator.SetBool(Constants.Animator.BOOL_JUMP, false);

            switch (type)
            {
                case AnimationType.IDLE:
                    m_animator.SetBool(Constants.Animator.BOOL_MOVE, false);

                    if (animCoroutine != null)
                    {
                        StopCoroutine(animCoroutine);
                        animCoroutine = null;
                    }
                    animCoroutine = StartCoroutine(IdleLoop());
                    break;
                case AnimationType.WALK:
                    m_animator.SetBool(Constants.Animator.BOOL_MOVE, true);
                    m_animator.SetInteger(Constants.Animator.INT_MOVE, 0);
                    break;
                case AnimationType.RUN:
                    m_animator.SetBool(Constants.Animator.BOOL_MOVE, true);
                    m_animator.SetInteger(Constants.Animator.INT_MOVE, Random.Range(1, 3));
                    break;
                case AnimationType.NONE:
                default:
                    break;
            }
        }


        public float GetCurrentAnimationTime()
        {
            return m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }

        /// <summary>
        /// 8/24/2023-LYI
        /// 평소에 응원 동작 루프
        /// </summary>
        /// <returns></returns>
        private IEnumerator IdleLoop()
        {
            while (gameObject.activeInHierarchy &&
                typeAnim == AnimationType.IDLE)
            {
                float t = Random.Range(3f, 5f);
                yield return new WaitForSeconds(t);
                int randomBlend = Random.Range(0, 3);
                PlayTriggerAnimation(TriggerAnimationType.IDLE, randomBlend);
            }
        }


        /// <summary>
        /// 11/14/2023-LYI
        /// 각 게임오버 상황 시 재생될 애니메이션
        /// </summary>
        /// <param name="type"></param>
        public void OnGameOver(GameOverType type, UnityAction action = null)
        {
            int blendNum = 0;
            float t = 0;
            switch (type)
            {
                case GameOverType.WATER:
                    t = 2.45f;
                    blendNum = 1;
                    break;
                default:
                    t = 1.8f;
                    blendNum = 0;
                    break;
            }
            PlayTriggerAnimation(TriggerAnimationType.FAIL, blendNum);

            StartCoroutine(GameOverWait(t, action));
        }
        IEnumerator GameOverWait(float time, UnityAction action = null)
        {
            //애니 재생 중이면 재생중인 클립 길이만큼
            if (m_animator.GetCurrentAnimatorClipInfoCount(0) > 0)
            {
                //AnimationClip currentClip = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip;
                yield return new WaitForSeconds(time);
            }
            else
            {
                yield return null;
            }

            if (action != null)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// 9/19/2024-LYI
        /// 카드 등장 모션
        /// </summary>
        public void CharacterAppear(string cardName)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            StartCoroutine(AppearCoroutine(cardName));
        }
        public void CharacterDisappear(UnityAction action = null)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            if (animCoroutine != null)
            {
                StopCoroutine(animCoroutine);
                animCoroutine = null;
            }
            animCoroutine = StartCoroutine(DisappearCoroutine(action));
        }

        IEnumerator AppearCoroutine(string cardNum)
        {
            //efx_portal.Play();
            //gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_STAGE_PORTAL);
            //m_animator.gameObject.SetActive(false);
            //yield return new WaitForSeconds(1f);

            //efx_portal.Stop();
            
            
            m_animator.gameObject.SetActive(true);
            PlayTriggerAnimation(TriggerAnimationType.TOK);
            
            
            yield return new WaitForSeconds(0.2f);

            SetAnimation(AnimationType.IDLE);
        }


        IEnumerator DisappearCoroutine(UnityAction action = null)
        {
            efx_portal.Play();
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_STAGE_PORTAL);
            m_animator.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            efx_portal.Stop();
            m_animator.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            if (action != null)
            {
                action.Invoke();
            }

            m_animator.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }


        #region Card Animation

        /// <summary>
        /// 9/20/2024-LYI
        /// ARCard에서 호출
        /// 각 카드에서 등장 할 때의 애니메이션 재생
        /// 카드 번호마다 다를 수 있음
        /// </summary>
        public void OnCardAppear(NumberType type)
        {
            PlayTriggerAnimation(TriggerAnimationType.CLEAR);
            //switch (type)
            //{
            //    case NumberType.Card_00:
            //        PlayTriggerAnimation(TriggerAnimationType.CLEAR);
            //        break;
            //    case NumberType.Card_01:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_02:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_03:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_04:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_05:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_06:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_07:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_08:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_09:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_Plus:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_Minus:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    case NumberType.Card_Equals:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //    default:
            //        PlayTriggerAnimation(TriggerAnimationType.TOK);
            //        break;
            //}
        }
        public void OnCardLink()
        {

        }
        public void OnCardResult(NumberType type = NumberType.Card_00)
        {
            switch (type)
            {
                case NumberType.Card_Equals:
                    PlayTriggerAnimation(TriggerAnimationType.TOK);
                    break;
                default:
                    PlayTriggerAnimation(TriggerAnimationType.TOK);
                    break;
            }
        }
        #endregion
    }
}