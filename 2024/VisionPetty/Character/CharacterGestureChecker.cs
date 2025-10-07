using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.XR.Hands;

namespace AroundEffect
{
    public class CharacterGestureChecker : MonoBehaviour
    {
        GameManager gameMgr;

        public CharacterManager charMgr;


        public Coroutine currentCoroutine = null;

        public bool isOnHand = false;
        public bool isLeftHand = false;

        public virtual void Init()
        {
            gameMgr = GameManager.Instance;

            isOnHand = false;
            Stop();

        }

        public virtual void Stop()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            Debug.Log(charMgr.gameObject.name + "_Gesture: Stop()");
        }



        /// <summary>
        /// 9/3/2024-LYI
        /// Move when gesture perform
        /// get hand's information
        /// </summary>
        /// <param name="tr"></param>
        //public virtual void GestureMove(XRHandGestureInput handInput)
        //{
        //    Stop();

        //    Debug.Log(charMgr.gameObject.name + "_AI: " + handInput.gestureType.ToString());

        //    switch (handInput.gestureType)
        //    {
        //        case HandGestureType.NONE:
        //            break;
        //        case HandGestureType.PALM_UP:
        //            if (handInput.isHolding)
        //            {
        //                HandLand(handInput);
        //            }
        //            else
        //            {
        //                float distance = Vector3.Distance(charMgr.Movement.transform.position,
        //                    handInput.tr_characterAnchor.position);
        //                if (distance < 1f)
        //                {
        //                    HandRide(handInput);
        //                }
        //            }
        //            break;
        //        case HandGestureType.PALM_DOWN:
        //            break;
        //        case HandGestureType.FIST:
        //            break;
        //        case HandGestureType.POINT:
        //            break;
        //        case HandGestureType.MIDDLE_UP:
        //            break;
        //        case HandGestureType.PROMISE:
        //            break;
        //        default:
        //            break;
        //    }

        //}

        public virtual void Gesture_HandOn(XRHandGestureInput handInput)
        {
            //Debug.Log(charMgr.gameObject.name + "- Gesture: " + handInput.gestureType.ToString());

            HandRide(handInput);
        }
        //public virtual void GestureHandOff(XRHandGestureInput handInput)
        //{
        //    Stop();

        //    Debug.Log(charMgr.gameObject.name + "- Gesture: " + handInput.gestureType.ToString());

        //    switch (handInput.gestureType)
        //    {
        //        case HandGestureType.NONE:
        //            break;
        //        case HandGestureType.FIST:
        //            HandFall(handInput);
        //            break;
        //        case HandGestureType.PALM_UP:
        //            HandLand(handInput);
        //            break;
        //        case HandGestureType.PALM_DOWN:
        //            HandFall(handInput);
        //            break;
        //        default:
        //            break;
        //    }

        //}

        #region GestureMove

        /// <summary>
        /// 9/3/2024-LYI
        /// 캐릭터 부르기
        /// 손 쪽으로 온 뒤 손 위로 점프
        /// 손 Transform 가져오기
        /// AI Move 말고 GestureMove?
        /// </summary>
        /// <returns></returns>
        protected virtual void HandRide(XRHandGestureInput handInput)
        {
            Debug.Log(charMgr.gameObject.name + "- HandRide()");

            isOnHand = true;
            isLeftHand = handInput.handTrackingEvent.handedness == Handedness.Left;

            charMgr.Stop();
            charMgr.Movement.SetMoveMarker(handInput.tr_characterAnchor.transform);

            Vector3 direction = handInput.tr_characterAnchor.position - charMgr.Movement.transform.position;
            Vector3 targetPos = charMgr.Movement.transform.position + direction * 0.8f;
            charMgr.Movement.SetMoveMarker(targetPos);

            charMgr.Movement.StartMove(() => HandCheck(handInput));
        }

        /// <summary>
        /// 9/3/2024-LYI
        /// 여전히 거리가 안쪽이면 점프
        /// 아니면 갸우뚱? 후 Idle
        /// </summary>
        /// <param name="handInput"></param>
        void HandCheck(XRHandGestureInput handInput)
        {
            charMgr.Stop();

            float distance = Vector3.Distance(charMgr.Movement.transform.position, handInput.tr_characterAnchor.position);
            if (distance < 1f)
            {
                Debug.Log(charMgr.gameObject.name + "- Hand check true");

                charMgr.Movement.SetMoveMarker(handInput.tr_characterAnchor.transform);
                //거리 내 점프
                charMgr.Movement.JumpToPosition(handInput.tr_characterAnchor,
                    () =>
                    {
                        charMgr.Stop();
                        charMgr.Movement.SetFixedMode(true);
                        charMgr.Movement.transform.SetParent(handInput.tr_characterAnchor);

                        //도착완료 시 함수 호출
                        handInput.OnCharacterArrivedHand();
                    });
            }
            else
            {
                Debug.Log(charMgr.gameObject.name + "- Hand check false, Distance: " + distance.ToString());
                //거리 외 동작
                charMgr.AI.AIMove(AIState.IDLE);
            }
        }

        /// <summary>
        /// 9/3/2024-LYI
        /// 손 내렸을 때 착지
        /// </summary>
        /// <param name="handInput"></param>
        public virtual void HandLand(XRHandGestureInput handInput)
        {
            Debug.Log(charMgr.gameObject.name + "- HandLand()");

            charMgr.Stop();
            isOnHand = false;

            //내리기 방식?
            //현재 위치에서 땅 중심 방향으로 이동(z축만?)

            charMgr.Movement.SetFixedMode(false);

            charMgr.Movement.SetMoveMarker(handInput.tr_characterAnchor.transform);

            Vector3 direction = gameMgr.lifeMgr.astarPath.gameObject.transform.position - handInput.tr_characterAnchor.position;
            Vector3 targetPos = handInput.tr_characterAnchor.position + direction * 0.5f;

            charMgr.Movement.SetMoveMarker(targetPos);
            charMgr.Movement.ResetParent();

            //charMgr.Movement.SetMoveMarker();
            charMgr.Movement.JumpToPosition(handInput.tr_characterAnchor, () => charMgr.AI.AIMove(AIState.IDLE));
        }


        /// <summary>
        /// 9/4/2024-LYI
        /// 손에서 바닥으로 떨어지기
        /// </summary>
        /// <param name="handInput"></param>
        public virtual void HandFall(XRHandGestureInput handInput)
        {
            Debug.Log(charMgr.gameObject.name + "- HandFall()");

            charMgr.Stop();
            isOnHand = false;

            charMgr.Movement.OnHandFall();

            handInput.HandInit();
        }



        #endregion



    }
}