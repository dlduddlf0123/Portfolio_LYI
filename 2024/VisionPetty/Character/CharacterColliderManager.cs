using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace AroundEffect
{
    public enum TouchCollider_Direction
    {
        NONE = -1,
        FRONT = 0,
        BACK,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
    public enum TouchCollider_HandType
    {
        NORMAL = 0,
        POINT,
        FIST,
        BATH,
    }

    public class CharacterColliderManager : MonoBehaviour
    {
        public CharacterManager charMgr;

        public EventCollider bodyColl;

        public CharacterCollider_Touch[] arr_touchCollider;
        public List<GameObject> list_touchedFinger;

        public bool isDelay = false;
        public int touchFingerCount = 0; //만지고있는 손가락 갯수 세기

        Coroutine currentCoroutine = null;

        public void Init()
        {
            SetBodyReaction();
            SetTouchReaction();
        }

        #region Body Collider
        public virtual void SetBodyReaction()
        {
            bodyColl.OnEnterCollider += (coll)=>OnBodyEnter(coll);
        }

        void OnBodyEnter(Collider coll)
        {
            if (charMgr.AI.isEvent)
            {
                return;
            }
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_FOOD))
            {
                Food food = coll.gameObject.GetComponentInParent<Food>();

                if (food != null)
                {
                    //스탯 변화 호출
                    charMgr.AI.EatFood(food);
                    food.AteFood(); //음식 제거 처리?
                }

            }
        }


        #endregion



        #region Touch Collider

        /// <summary>
        /// 9/5/2024-LYI
        /// 터치 반응 동작 설정
        /// </summary>
        public virtual void SetTouchReaction()
        {
            for (int i = 0; i < arr_touchCollider.Length; i++)
            {
                TouchCollider_Direction direction = (TouchCollider_Direction)i;

                arr_touchCollider[i].OnEnterEvent.AddListener(()=>TouchStart(direction));
                arr_touchCollider[i].OnExitEvent.AddListener(() => TouchEnd(direction));
            }
        }

        /// <summary>
        /// 10/10/2024-LYI
        /// 터치 조건 추가
        /// </summary>
        /// <param name="direction"></param>
        public void TouchStart(TouchCollider_Direction direction)
        {
            if (arr_touchCollider[(int)direction].colledGameObject != null)
            {
                list_touchedFinger.Add(arr_touchCollider[(int)direction].colledGameObject);
            }
            charMgr.AI.OnTouchStart(direction, arr_touchCollider[(int)direction].touchType);
        }

        /// <summary>
        /// 10/10/2024-LYI
        /// 터치해제 조건 추가
        /// </summary>
        /// <param name="direction"></param>
        public void TouchEnd(TouchCollider_Direction direction)
        {
            if (arr_touchCollider[(int)direction].colledGameObject != null)
            {
                if (list_touchedFinger.Contains(arr_touchCollider[(int)direction].colledGameObject))
                {
                    list_touchedFinger.Remove(arr_touchCollider[(int)direction].colledGameObject);
                }
            }

            if (list_touchedFinger.Count <= 0)
            {
                list_touchedFinger.Clear();
                charMgr.AI.OnTouchEnd(direction, arr_touchCollider[(int)direction].touchType);
            }
        }



        /// <summary>
        /// 9/5/2024-LYI
        /// call when touched
        /// </summary>
        /// <param name="time"></param>
        public void StartTouchDelay(float time = 0.3f)
        {
            if (isDelay)
            {
                return;
            }

            isDelay = true;

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            currentCoroutine = StartCoroutine(TouchColliderActiveDelay(time));
        }

        IEnumerator TouchColliderActiveDelay(float time)
        {
            for (int i = 0; i < arr_touchCollider.Length; i++)
            {
                arr_touchCollider[i].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(time);

            yield return new WaitForSeconds(charMgr.Animation.GetCurrentAnimationTime());

            for (int i = 0; i < arr_touchCollider.Length; i++)
            {
                arr_touchCollider[i].gameObject.SetActive(true);
            }
            isDelay = false;
        }
         
        #endregion


    }
}