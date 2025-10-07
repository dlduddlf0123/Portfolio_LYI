using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTokTok;
using VRTokTok.Manager;


namespace VRTokTok.Character
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

    //캐릭터 번호
    public enum HeaderType
    {
        NONE = 0,
        KANTO = 1,
        ZINO = 2,
        OODADA = 3,
        COCO = 4,
        DOINK = 5,
        TENA = 6,
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
    /// 8/24/2023-LYI
    /// 이동과 분리된 캐릭터 스크립트
    /// 캐릭터 애니메이션 관련 관리
    /// </summary>
    public class Tok_Character : MonoBehaviour
    {
        GameManager gameMgr;
        //public Tok_Movement tok_move;
        protected PlaySceneManager playMgr;

        public HeaderType typeHeader;

        public Animator m_animator;
        public AnimationType typeAnim;  //작동중인 애니메이션 상태

        public ParticleSystem efx_portal; //등장, 사라질 시 사용될 포탈 파티클


        //3/4/2024-LYI
        //눈 활성화 체크용
        public GameObject[] arr_eyeShapeL;
        public GameObject[] arr_eyeShapeR;

        public GameObject[] arr_eyeTransformL;
        public GameObject[] arr_eyeTransformR;


        Coroutine animCoroutine = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            playMgr = gameMgr.playMgr;
        }


        /// <summary>
        /// 3/4/2024-LYI
        /// 눈 활성화 체크 루프
        /// </summary>
        private void Update()
        {
            if (arr_eyeTransformL.Length > 0)
            {
                for (int i = 0; i < arr_eyeTransformL.Length; i++)
                {
                    if (arr_eyeTransformL[i].transform.localScale.x < 0.001f)
                    {
                        arr_eyeShapeL[i].SetActive(false);
                    }
                    else
                    {
                        arr_eyeShapeL[i].SetActive(true);
                    }
                }
            }

            if (arr_eyeTransformR.Length > 0)
            {
                for (int i = 0; i < arr_eyeTransformR.Length; i++)
                {
                    if (arr_eyeTransformR[i].transform.localScale.x < 0.001f)
                    {
                        arr_eyeShapeR[i].SetActive(false);
                    }
                    else
                    {
                        arr_eyeShapeR[i].SetActive(true);
                    }
                }
            }
        }


        public virtual void PlayTriggerAnimation(TriggerAnimationType type, int blendNum = 0)
        {
            if (!gameObject.activeInHierarchy) { return; }
            Debug.Log(gameObject.name + " - TraggerAnim: " + type.ToString());

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

            StartCoroutine(GameOverWait(t,action));
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
        /// 11/13/2023-LYI
        /// 중단되지 않도록 변경
        /// </summary>
        /// <param name="action"></param>
        public void CharacterAppear(UnityAction action = null)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            StartCoroutine(AppearCoroutine(action));
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

        IEnumerator AppearCoroutine(UnityAction action = null)
        {
            efx_portal.Play();
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_STAGE_PORTAL);
            m_animator.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            efx_portal.Stop();
            m_animator.gameObject.SetActive(true);
            PlayTriggerAnimation(TriggerAnimationType.TOK);
            yield return new WaitForSeconds(0.2f);

            SetAnimation(AnimationType.IDLE);
            if (action != null)
            {
                action.Invoke();
            }
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

    }
}