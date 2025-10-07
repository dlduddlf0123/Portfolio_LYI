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
        SLEEP,
    };

    public enum TriggerAnimationType
    {
        IDLE = 0,
        JUMP,
        TOK,
        CRASH,
        HIT,
        EAT,
        POOP,
        SYRINGE,
        MIDDLE_UP,
        MINIGAME_LEFT,
        MINIGAME_RIGHT,
        MINIGAME_UP,
    }


    /// <summary>
    /// 10/16/2024-LYI
    /// 눈 모양
    /// </summary>
    public enum EyeShapeType
    {
        BASE = 0,
        BLINK,
        CIRCLE,
        CLOSED,
        DEATH,
        SMILE,
        TORNADO,
        VACANT,
    }

    /// <summary>
    /// 11/4/2024-LYI
    /// 눈꺼풀 모양
    /// </summary>
    public enum EyeLidShape
    {
        OPEN = 0,
        HALF,
        CLOSE,
    }

    #endregion

    /// <summary>
    /// 240805 LYI
    /// Character Status
    /// Animation Controll
    /// Attribute
    /// </summary>
    public class CharacterAnimation : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Animation")]
        public Animator m_animator;
        public AnimationType typeAnim;  //작동중인 애니메이션 상태


        //3/4/2024-LYI
        //눈 활성화 체크용
        [Header("Eye Shape")]
        public GameObject[] arr_eyeShapeL;
        public GameObject[] arr_eyeTransformL;

        public GameObject[] arr_eyeShapeR;
        public GameObject[] arr_eyeTransformR;

        //8/29/2024-LYI
        //눈 로테이션 용
        public GameObject eyeRotateBaseL;
        public GameObject eyeRotateNewL;

        public GameObject eyeRotateBaseR;
        public GameObject eyeRotateNewR;

        public bool isEyeShapeUpdate = true;

        //11/4/2024-LYI
        //눈꺼풀 추가 0:위, 1:아래
        [Header("Eye Lid")]
        public GameObject[] arr_eyeLidL;
        public GameObject[] arr_eyeLidR;


        Coroutine animCoroutine = null;

        public ParticleSystem efx_portal; //등장, 사라질 시 사용될 포탈 파티클


        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        public virtual void Init()
        {
            EyeLidChange(EyeLidShape.OPEN);
            EyeShapeReset();
        }


        /// <summary>
        /// 3/4/2024-LYI
        /// 눈 활성화 체크 루프
        /// </summary>
        private void Update()
        {

            if (isEyeShapeUpdate)
            {
                if (eyeRotateNewL != null)
                {
                    eyeRotateNewL.transform.localRotation = eyeRotateBaseL.transform.localRotation;
                }

                if (eyeRotateNewR != null)
                {
                    eyeRotateNewR.transform.localRotation = eyeRotateBaseR.transform.localRotation;
                }

                UpdateEyeActive(arr_eyeTransformL, arr_eyeShapeL);
                UpdateEyeActive(arr_eyeTransformR, arr_eyeShapeR);
            }
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
                case TriggerAnimationType.EAT:
                    m_animator.SetFloat(Constants.Animator.FLOAT_REACTION, blendNum);
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_EAT);
                    break;
                case TriggerAnimationType.POOP:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_POOP);
                    break;
                case TriggerAnimationType.SYRINGE:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_SYRINGE);
                    break;
                case TriggerAnimationType.HIT:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_HIT);
                    break;
                case TriggerAnimationType.MIDDLE_UP:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_MIDDLE_UP);
                    break;
                case TriggerAnimationType.MINIGAME_LEFT:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_MINIGAME_JUMP_LEFT);
                    break;
                case TriggerAnimationType.MINIGAME_RIGHT:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_MINIGAME_JUMP_RIGHT);
                    break;
                case TriggerAnimationType.MINIGAME_UP:
                    m_animator.SetTrigger(Constants.Animator.TRIGGER_MINIGAME_JUMP_UP);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 12/3/2024-LYI
        /// Set bool anim
        /// </summary>
        /// <param name="type"></param>
        public virtual void SetAnimation(AnimationType type)
        {
            if (!gameObject.activeInHierarchy) { return; }
            typeAnim = type;
            Debug.Log(gameObject.name + "_Animation - State: " + type.ToString());

            m_animator.SetBool(Constants.Animator.BOOL_MOVE, false);
            m_animator.SetBool(Constants.Animator.BOOL_JUMP, false);
            m_animator.SetBool(Constants.Animator.BOOL_SLEEP, false);

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
                case AnimationType.SLEEP:
                    m_animator.SetBool(Constants.Animator.BOOL_SLEEP, true);

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
        //public void OnGameOver(GameOverType type, UnityAction action = null)
        //{
        //    int blendNum = 0;
        //    float t = 0;
        //    switch (type)
        //    {
        //        case GameOverType.WATER:
        //            t = 2.45f;
        //            blendNum = 1;
        //            break;
        //        default:
        //            t = 1.8f;
        //            blendNum = 0;
        //            break;
        //    }
        //    PlayTriggerAnimation(TriggerAnimationType.FAIL, blendNum);

        //    StartCoroutine(AnimationWait(t, action));
        //}
        public IEnumerator AnimationWait(float time, UnityAction action = null)
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



        /// <summary>
        /// 9/5/2024-LYI
        /// Call from touch collider
        /// Play animation when each direction collider colled
        /// </summary>
        /// <param name="direction"></param>
        public virtual void PettingStart(TouchCollider_Direction direction)
        {
            m_animator.SetFloat(Constants.Animator.FLOAT_TOUCH_DIRECTION, (int)direction);
            m_animator.SetBool(Constants.Animator.BOOL_TOUCH, true);
            m_animator.SetTrigger(Constants.Animator.TRIGGER_TOUCH);

            EyeShapeChange(EyeShapeType.SMILE);
        }

        public void PettingEnd()
        {
            m_animator.enabled = true;
            m_animator.SetBool(Constants.Animator.BOOL_TOUCH, false);
            EyeShapeReset();
        }


        /// <summary>
        /// 10/16/2024-LYI
        /// 양눈 모양 변경
        /// </summary>
        /// <param name="eyeShape"></param>
        public void EyeShapeChange(EyeShapeType eyeShape)
        {
            isEyeShapeUpdate = false;

            eyeRotateNewL.transform.localRotation = Quaternion.identity;
            eyeRotateNewR.transform.localRotation = Quaternion.identity;

            for (int i = 0; i < arr_eyeShapeL.Length; i++)
            {
                arr_eyeShapeL[i].gameObject.SetActive(false);
                arr_eyeShapeR[i].gameObject.SetActive(false);
            }

            if (eyeShape != EyeShapeType.BASE)
            {
                arr_eyeShapeL[(int)eyeShape].transform.localScale = Vector3.one;
                arr_eyeShapeR[(int)eyeShape].transform.localScale = Vector3.one;
            }

            arr_eyeShapeL[(int)eyeShape].gameObject.SetActive(true);
            arr_eyeShapeR[(int)eyeShape].gameObject.SetActive(true);

        }

        public void EyeShapeReset()
        {
            isEyeShapeUpdate = true;
        }


        #region Eye Lid Shape

        /// <summary>
        /// 11/4/2024-LYI
        /// 눈꺼풀 모양 변경
        /// </summary>
        public void EyeLidChange(EyeLidShape shape)
        {
            switch (shape)
            {
                case EyeLidShape.OPEN:
                    arr_eyeLidL[0].SetActive(false);
                    arr_eyeLidL[1].SetActive(false);
                    arr_eyeLidR[0].SetActive(false);
                    arr_eyeLidR[1].SetActive(false);
                    break;
                case EyeLidShape.HALF:
                    arr_eyeLidL[0].SetActive(true);
                    arr_eyeLidL[1].SetActive(false);
                    arr_eyeLidR[0].SetActive(true);
                    arr_eyeLidR[1].SetActive(false);
                    break;
                case EyeLidShape.CLOSE:
                    arr_eyeLidL[0].SetActive(true);
                    arr_eyeLidL[1].SetActive(true);
                    arr_eyeLidR[0].SetActive(true);
                    arr_eyeLidR[1].SetActive(true);
                    break;
            }

        }

        #endregion

    }//class
}//namespace
