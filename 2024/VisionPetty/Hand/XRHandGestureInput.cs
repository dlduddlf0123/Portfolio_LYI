using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands.Samples.GestureSample;

namespace AroundEffect
{
    public enum HandGestureType
    {
        NONE = -1,
        PALM_UP = 0,
        PALM_DOWN,
        FIST,
        POINT,
        MIDDLE_UP,
        PROMISE,
        THUMB_RIGHT,
        THUMB_LEFT,
        THUMB_UP,
    }


    /// <summary>
    /// 8/30/2024-LYI
    /// Gesture check with XRHands+6
    /// Hold events when gesture trigger
    /// XRHands를 이용한 제스쳐 체크
    /// 제스쳐 작동 시 Event 홀더
    /// </summary>
    public class XRHandGestureInput : MonoBehaviour
    {
        GameManager gameMgr;
        VisionPolySpatialInput polySpatialInput;
        LifeUIManager lifeUIMgr;

        public XRHandTrackingEvents handTrackingEvent;



        [Header("Gesture")]
        public List<HeadersStaticHandGesture> list_gesture = new ();



        [Header("Collider")]
        public EventCollider col_palmBackGround;
        public EventCollider col_palmFrontCharacter;



        [Header("Event")]
        public  UnityEvent onHandGesture;

        [Header("Property")]
        public Transform tr_characterAnchor;
        public Transform tr_handWrist;

        //손에 들고 있는 캐릭터
        public CharacterManager handCharacter;


        public HandGestureType gestureType;

        bool isFirst = true;
        public bool isHolding = false;

        void Start()
        {
            HandInit();
        }


        /// <summary>
        /// 9/3/2024-LYI
        /// 손 관련 입력 등록, 상태 초기화
        /// </summary>
        public void HandInit()
        {
            if (isFirst)
            {
                isFirst = false;

                gameMgr = GameManager.Instance;
                polySpatialInput = gameMgr.MRMgr.polySpatialInput;
                lifeUIMgr = gameMgr.lifeMgr.lifeUIMgr;

                //제스처 행동 등록
                //제스쳐와 enum 순서 맞춰 일괄 할당
                for (int i = 0; i < list_gesture.Count; i++)
                {
                    int a = i; //버그 방지용 데이터 할당
                    list_gesture[a].gesturePerformed.AddListener(() => OnGesturePerform((HandGestureType)a));
                    list_gesture[a].gestureEnded.AddListener(() => OnGestureEnd((HandGestureType)a));
                }

                //list_gesture[0].gesturePerformed.AddListener(() => GesturePerform(HandGestureType.PALM_UP));
                //list_gesture[0].gestureEnded.AddListener(() => OnGestureEnd(HandGestureType.PALM_UP));
                //list_gesture[1].gesturePerformed.AddListener(() => GesturePerform(HandGestureType.PALM_DOWN));
                //list_gesture[1].gestureEnded.AddListener(() => OnGestureEnd(HandGestureType.PALM_DOWN));
                //list_gesture[2].gesturePerformed.AddListener(() => GesturePerform(HandGestureType.FIST));
                //list_gesture[2].gestureEnded.AddListener(() => OnGestureEnd(HandGestureType.FIST));


                //Collider 반응 동작 할당
                col_palmBackGround.handInput = this;
                col_palmFrontCharacter.handInput = this;

                col_palmBackGround.OnEnterEvent.AddListener(Gesture_HandOnCharacterCheck);
            }

            handCharacter = null;
            isHolding = false;

        }



        /// <summary>
        /// 9/3/2024-LYI
        /// AddListner when gesture perform in StaticHandGesture
        /// </summary>
        /// <param name="type"></param>
        public void OnGesturePerform(HandGestureType type)
        {
            Debug.Log(gameObject.name + "- GesturePerform: " + type.ToString());
            gestureType = type;


            if (gameMgr.statGame == GameStatus.MINIGAME)
            {
                switch (type)
                {
                    case HandGestureType.THUMB_RIGHT:
                        Gesture_ThumbRight();
                        break;
                    case HandGestureType.THUMB_LEFT:
                        Gesture_ThumbLeft();
                        break;
                    case HandGestureType.THUMB_UP:
                        Gesture_ThumbUp();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case HandGestureType.NONE:
                        break;
                    case HandGestureType.PALM_UP:
                        Gesture_PalmUp_Perform();
                        Gesture_HandOnCharacterCheck();
                        break;
                    case HandGestureType.PALM_DOWN:
                        Gesture_HandOffCharacterCheck();
                        break;
                    case HandGestureType.FIST:
                        Gesture_HandOffCharacterCheck();
                        polySpatialInput.FistColliderUpdate(handTrackingEvent.handedness == Handedness.Left, true);

                        break;
                    case HandGestureType.POINT:
                        break;
                    case HandGestureType.MIDDLE_UP:
                        Gesture_MiddleUp();
                        break;
                    case HandGestureType.PROMISE:
                        Gesture_Promise();
                        break;

                    default:
                        break;
                }
            }

        }


        public void OnGestureEnd(HandGestureType type)
        {
            Debug.Log(gameObject.name + "- GestureEnd: " + type.ToString());

            switch (type)
            {
                case HandGestureType.NONE:
                    break;
                case HandGestureType.PALM_UP:
                    Gesture_PalmUp_End();
                    break;
                case HandGestureType.PALM_DOWN:
                    break;
                case HandGestureType.FIST:
                    polySpatialInput.FistColliderUpdate(handTrackingEvent.handedness == Handedness.Left, false);
                    break;
                case HandGestureType.POINT:
                    break;
                case HandGestureType.MIDDLE_UP:
                    Gesture_MiddleCancle();
                    break;
                case HandGestureType.PROMISE:
                    Gesture_PromiseCancle();
                    break;
                default:
                    break;
            }
        }


        public void Gesture_PalmUp_Perform()
        {
            if (handTrackingEvent.handedness == Handedness.Left)
            { 
                lifeUIMgr.ui_handLeft.MenuEnable();
                polySpatialInput.SetCapsuleActive(true, false);

            }
            if (handTrackingEvent.handedness == Handedness.Right)
            {
                lifeUIMgr.ui_handRight.MenuEnable();
                polySpatialInput.SetCapsuleActive(false, false);
            }
        }
        public void Gesture_PalmUp_End()
        {
            if (handTrackingEvent.handedness == Handedness.Left)
            {
                lifeUIMgr.ui_handLeft.MenuDisable();
                polySpatialInput.SetCapsuleActive(true, true);

            }
            if (handTrackingEvent.handedness == Handedness.Right)
            {
                lifeUIMgr.ui_handRight.MenuDisable();
                polySpatialInput.SetCapsuleActive(false, true);
            }
        }



        /// <summary>
        /// 9/3/2024-LYI
        /// Check gesture that character going to player's hand
        /// if true, call characters ai to hand on
        /// </summary>
        public void Gesture_HandOnCharacterCheck()
        {
            //손바닥, 지형과 충돌 중인 경우
            if (gestureType == HandGestureType.PALM_UP &&
                col_palmBackGround.isColled)
            {

                //손이 빈 경우
                if (handCharacter == null)
                {
                    if (gameMgr.lifeMgr.currentCharacter == null)
                    {
                        return;
                    }
                    handCharacter = gameMgr.lifeMgr.currentCharacter;
                    gameMgr.lifeMgr.currentCharacter = null;

                    isHolding = true;

                    //call character's gesture checker
                    handCharacter.Gesture.Gesture_HandOn(this);
                }
                else
                {
                    handCharacter.Gesture.HandLand(this);
                    SetHandUIActive(false);

                    handCharacter = null;
                    isHolding = false;

                }
            }
        }


        /// <summary>
        /// 10/7/2024-LYI
        /// 캐릭터가 손 위에 도착했을 때 호출
        /// </summary>
        public void OnCharacterArrivedHand()
        {
            SetHandUIActive(true);

        }


        /// <summary>
        /// 9/4/2024-LYI
        /// Check gesture that character land or fall
        /// </summary>
        public void Gesture_HandOffCharacterCheck()
        {
            if (handCharacter == null || !isHolding)
            {
                return;
            }

            handCharacter.Gesture.HandFall(this);
            SetHandUIActive(false);

            handCharacter = null;
            isHolding = false;

        }


        /// <summary>
        /// 10/7/2024-LYI
        /// Set Hand UI activate
        /// </summary>
        /// <param name="isActive">true: active</param>
        void SetHandUIActive(bool isActive)
        {
            if (handTrackingEvent.handedness == Handedness.Left)
            {
                if (isActive)
                {
                    lifeUIMgr.ui_handLeft.CheckStatusUIActive();
                }
                else
                {
                    lifeUIMgr.ui_handLeft.DisableStatusUI();
                }

            }
            if (handTrackingEvent.handedness == Handedness.Right)
            {
                if (isActive)
                {
                    lifeUIMgr.ui_handRight.CheckStatusUIActive();
                }
                else
                {
                    lifeUIMgr.ui_handRight.DisableStatusUI();
                }
            }
        }

        Coroutine coroutineMiddle;
        float middleTime = 0f;
        float middleTimeMax = 1f;
        /// <summary>
        /// 12/2/2024-LYI
        /// 뻐큐 효과
        /// 시간 재고 지나면 모두 호감도 감소, 놀라서 도망가기
        /// </summary>
        public void Gesture_MiddleUp()
        {
            if (coroutineMiddle != null)
            {
                middleTime = 0;
            }
            else
            {
                coroutineMiddle = StartCoroutine(MiddleTimer());
            }
        }
        public void Gesture_MiddleCancle()
        {
            middleTime = 0;
            if (coroutineMiddle != null)
            {
                StopCoroutine(coroutineMiddle);
                coroutineMiddle = null;
            }
        }

        IEnumerator MiddleTimer()
        {
            while (middleTime < middleTimeMax)
            {
                middleTime += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            if (middleTime >= middleTimeMax)
            {
                for (int i = 0; i < gameMgr.lifeMgr.arr_character.Length; i++)
                {
                    gameMgr.lifeMgr.arr_character[i].AI.OnMiddleUp();
                }
                coroutineMiddle = null;

            }
        }



        Coroutine coroutinePromise;
        float promiseTime = 0f;
        float promiseTimeMax = 1f;
        /// <summary>
        /// 12/3/2024-LYI
        /// 캐릭터 호감도 최대일 때 약속 효과
        /// </summary>
        public void Gesture_Promise()
        {
            if (gameMgr.lifeMgr.currentCharacter == null) { return; }

            if (gameMgr.lifeMgr.currentCharacter.Status.level_like != StatusLevel.VERY_GOOD ||
                gameMgr.lifeMgr.currentCharacter.Status.isPromised) { return; }

            if (coroutinePromise != null)
            {
                promiseTime = 0;
            }
            else
            {
                coroutinePromise = StartCoroutine(PromiseTimer());
            }

        }
        IEnumerator PromiseTimer()
        {
            while (promiseTime < promiseTimeMax)
            {
                promiseTime += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            if (promiseTime >= promiseTimeMax)
            {
                gameMgr.lifeMgr.currentCharacter.AI.OnPromise();
                coroutinePromise = null;

            }
        }
        public void Gesture_PromiseCancle()
        {
            promiseTime = 0;
            if (coroutinePromise != null)
            {
                StopCoroutine(coroutinePromise);
                coroutinePromise = null;
            }
        }

        public void Gesture_ThumbRight()
        {
            if (gameMgr.statGame == GameStatus.MINIGAME)
            {
                gameMgr.lifeMgr.miniGameMgr.mini_character.RightJump();
            }

        }
        public void Gesture_ThumbLeft()
        {
            if (gameMgr.statGame == GameStatus.MINIGAME)
            {
                gameMgr.lifeMgr.miniGameMgr.mini_character.LeftJump();
            }

        }
        public void Gesture_ThumbUp()
        {
            if (gameMgr.statGame == GameStatus.MINIGAME)
            {
                gameMgr.lifeMgr.miniGameMgr.mini_character.UpJump();
            }

        }


    }//class
}//namespace