using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AroundEffect
{
    public class CharacterCollider_Touch : EventCollider
    {

        public TouchCollider_HandType touchType;


        protected override void OnEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND))
            {

               VisionPolySpatialInput input = GameManager.Instance.MRMgr.polySpatialInput;
                HandGestureType type;
                bool isLeft = false;

                if (input != null)
                {
                    for (int i = 0; i < input.arr_handCollL.Length; i++)
                    {
                        if (coll == input.arr_handCollL[i])
                        {
                            isLeft = true;
                        }
                    }
                    type = isLeft ? input.handInputL.gestureType : input.handInputR.gestureType;

                }
                else
                {
                    type = input.handInputR.gestureType;
                }

                switch (type)
                {
                    case HandGestureType.FIST:
                        touchType = TouchCollider_HandType.FIST;
                        break;
                    case HandGestureType.POINT:
                        touchType = TouchCollider_HandType.POINT;
                        break;
                    case HandGestureType.PROMISE:
                    case HandGestureType.NONE:
                    case HandGestureType.PALM_UP:
                    case HandGestureType.PALM_DOWN:
                    case HandGestureType.MIDDLE_UP:
                    default:
                        touchType = TouchCollider_HandType.NORMAL;
                        break;
                }

                base.isColled = true;
                colledGameObject = coll.gameObject;
                OnEnterEvent?.Invoke();
            }
            if ( coll.gameObject.CompareTag(Constants.TAG.TAG_SPONGE))
            {
                Bath_Sponge sponge = coll.gameObject.GetComponentInParent<Bath_Sponge>();

                touchType = TouchCollider_HandType.BATH;

                if (sponge.isHolding)
                {
                    base.isColled = true;
                    colledGameObject = coll.gameObject;
                    OnEnterEvent?.Invoke();
                }
            }
        }
        protected override void OnStay(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND) ||
                coll.gameObject.CompareTag(Constants.TAG.TAG_SPONGE))
            {
                base.isColled = true;
                colledGameObject = coll.gameObject;
            }
            else
            {
                base.isColled = false;
                colledGameObject = null;
            }
        }

        protected override void OnExit(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND))
            {
                touchType = TouchCollider_HandType.NORMAL;

                base.isColled = false;
                colledGameObject = null;

                OnExitEvent?.Invoke();
            }
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_SPONGE))
            {
                Bath_Sponge sponge = coll.gameObject.GetComponentInParent<Bath_Sponge>();
                touchType = TouchCollider_HandType.BATH;

                if (sponge.isHolding)
                {
                    base.isColled = false;
                    colledGameObject = null;
                    OnExitEvent?.Invoke();
                }
            }
        }

    }
}