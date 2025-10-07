using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect {
    public class CharacterPetting : MonoBehaviour
    {
        public CharacterManager charMgr;

        //직접 위치 할당
        public Transform[] arr_boneTr;

        //배열들
        public Transform[] arr_contactTr;
        Vector3[] arr_boneOriginPos;

        // 쓰다듬기 효과 활성화 여부
        public bool[] arr_isPetting;

        Coroutine[] arr_touchCoroutine;

        public float pettingDistance = 0.01f; // 쓰다듬기 효과의 강도
        public float pettingLerpPercent = 0.1f; // 메시가 따라오는 속도

        //초기화
        public void Init()
        {
            arr_contactTr = new Transform[arr_boneTr.Length];

            //포지션 저장
            arr_boneOriginPos = new Vector3[arr_boneTr.Length];
            for (int i = 0; i < arr_boneOriginPos.Length; i++)
            {
                arr_boneOriginPos[i] = arr_boneTr[i].localPosition;
            }

            arr_isPetting = new bool[arr_boneTr.Length];
            arr_touchCoroutine = new Coroutine[arr_boneTr.Length];
        }


        void TouchStop(Coroutine c)
        {
            if (c != null)
            {
                StopCoroutine(c);
                c = null;
            }
        }


        /// <summary>
        /// 10/10/2024-LYI
        /// Display func
        /// Call when hand touching character
        /// Start petting effect
        /// </summary>
        /// <param name="direction">where hand touched</param>
        /// <param name="touchedObject">which finger touched</param>
        public void PettingStart(TouchCollider_Direction direction, TouchCollider_HandType handType, GameObject touchedObject)
        {
            int index = (int)direction;

            if (arr_isPetting[index] == true) { return; } 
            arr_isPetting[index] = true;

            for (int i = 0; i < arr_isPetting.Length; i++)
            {
                if (arr_isPetting[i] == true && i != index)
                {
                    PettingEnd((TouchCollider_Direction)i, handType);
                }
            }
            

            Debug.Log(gameObject.name + "-Animation: PettingStart(), Direction: " + direction.ToString());


            charMgr.Animation.PettingStart(direction);

            arr_contactTr[index] = touchedObject.transform;

            TouchStop(arr_touchCoroutine[index]);
            arr_touchCoroutine[index] = StartCoroutine(PettingEffect(direction, handType));

        }

        /// <summary>
        /// 10/10/2024-LYI
        /// Display func
        /// Call when hand dettached
        /// Stop act
        /// </summary>
        /// <param name="direction"></param>
        public void PettingEnd(TouchCollider_Direction direction, TouchCollider_HandType handType)
        {
            int index = (int)direction;

            if (arr_isPetting[index] == false ) { return; }
            arr_isPetting[index] = false;

            Debug.Log(gameObject.name + "-Animation: PettingEnd(), Direction: " + direction.ToString());

            charMgr.Animation.PettingEnd();

            TouchStop(arr_touchCoroutine[index]);
            arr_touchCoroutine[index] = StartCoroutine(ResetMesh(direction));

        }

        IEnumerator PettingEffect(TouchCollider_Direction direction, TouchCollider_HandType handType)
        {
            int index = (int)direction;

            //애니메이션 자세 바꿀때 기다리기
            //yield return new WaitForSeconds(1f);

            if (direction == TouchCollider_Direction.BACK)
            {
                charMgr.Animation.m_animator.enabled = false;
            }


            Transform parentTransform = arr_boneTr[index].parent;

            Vector3 startPos, handPos;
            Vector3 normalVec;
            float distance = 0;

            while (arr_isPetting[index] &&
                charMgr.Collider.arr_touchCollider[index].isColled)
            {
                startPos = parentTransform.TransformPoint(arr_boneOriginPos[index]);
                handPos = FlattenY(arr_contactTr[index].position, startPos.y);

                normalVec = (handPos - startPos).normalized;
                distance = Vector3.Distance(handPos, startPos);

                if (distance > pettingDistance)
                {
                    distance = pettingDistance;
                }

                Vector3 result = startPos + (normalVec * distance);
                arr_boneTr[index].position = Vector3.Lerp(startPos, result, pettingLerpPercent);

                //Debug.Log("Distance: " + distance);

                yield return null;
            }

            charMgr.AI.OnTouchEnd(direction, handType);
        }

        Vector3 FlattenY(Vector3 pos, float y)
        {
            return new Vector3(pos.x, y, pos.z);
        }

        IEnumerator ResetMesh(TouchCollider_Direction direction)
        {
            int index = (int)direction;

            float distance = Vector3.Distance( arr_boneTr[index].localPosition, arr_boneOriginPos[index]);
            while (distance > 0.0003f)
            {
                arr_boneTr[index].localPosition = Vector3.Lerp(arr_boneTr[index].localPosition, arr_boneOriginPos[index], pettingLerpPercent);
                distance = Vector3.Distance( arr_boneTr[index].localPosition, arr_boneOriginPos[index]);
                yield return null;
            }

            arr_boneTr[index].localPosition = arr_boneOriginPos[index];
        }
    }
}