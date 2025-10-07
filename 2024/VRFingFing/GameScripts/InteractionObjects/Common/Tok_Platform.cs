using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

using VRTokTok.Character;

namespace VRTokTok.Interaction
{

    /// <summary>
    /// 10/11/2023-LYI
    /// 스위치 등에 대한 반응으로 이동하는 발판
    /// 공중에서 움직이기도 한다
    /// </summary>
    public class Tok_Platform : Tok_Interact
    {
        [Header("Tok_Platform")]
        public Rigidbody m_rigidbody;

        public GameObject movePlatform;
        public Transform tr_end; //활성화 시 위치
        public Transform tr_start; //비활성화 시 위치

        public TokGround tokGround; //터치 활성화

        [Header("Active on start")]
        public bool isActiveOnStart = false;
        [MMFCondition("isActiveOnStart",true)]
        public float startDelay = 0.5f;

        [Header("Auto move")]
        public bool isAutoMove = true;
        [MMFCondition("isAutoMove", true)]
        public float autoWaitTime = 2f;

        [Header("Move property")]
        public bool isMoving = false;
        public bool isMoveToEnd = true; //끝쪽으로 가는가

        public float moveSpeed;



        /// <summary>
        /// 11/2/2023-LYI
        ///  Pulley에서 사용됨
        ///  현재 플랫폼 위에 올라간 오브젝트 무게 체크
        /// </summary>
        public int weight = 0;


        public override void InteractInit()
        {
            base.InteractInit();

            movePlatform.transform.position = tr_start.position;
            isMoveToEnd = true;

            PlatformStop();
            if (isActiveOnStart)
            {
                StartCoroutine(ActiveOnStart());
            }

        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header") ||
                coll.gameObject.CompareTag("Item"))
            {
                coll.gameObject.transform.SetParent(movePlatform.transform);
                weight++;
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header") ||
                coll.gameObject.CompareTag("Item"))
            {
                Tok_Movement header;

                if (coll.gameObject.TryGetComponent<Tok_Movement>(out header))
                {  //7/19/2024-LYI
                   //현재 플랫폼에 소속된 경우만 해제
                    if (header.transform.parent == movePlatform.transform)
                    {
                        header.transform.SetParent(header.defaultParent);
                    }
                }
                else
                {
                    coll.gameObject.transform.parent = null;
                }
                weight--;
            }
        }

        IEnumerator ActiveOnStart()
        {
            yield return new WaitForSeconds(startDelay);
            PlatformMove();
        }

        void PlatformMove()
        {
            isMoving = true;
            StartCoroutine(MoveCoroutine());
        }


        void PlatformStop()
        {
            isMoving = false;

            StopAllCoroutines();
        }


        IEnumerator MoveCoroutine()
        {
            Transform destination = isMoveToEnd ? tr_end : tr_start;
            //Vector3 moveVec = Vector3.zero;
            //moveVec = (destination.position - movePlatform.transform.position).normalized;

            while (isMoving &&
                movePlatform.transform.position != destination.position)
            {
                movePlatform.transform.position =
                     Vector3.MoveTowards(movePlatform.transform.position, destination.position, moveSpeed * Time.deltaTime);


                //moveVec = (destination.position - movePlatform.transform.position) *
                //    moveSpeed * Time.deltaTime;
                // movePlatform.transform.Translate(moveVec * moveSpeed * Time.deltaTime);

                //foreach (Transform tr in list_affectedObject)
                //{
                //    tr.Translate(moveVec);
                //}

                yield return null;
            }
            movePlatform.transform.position = destination.position;
            isMoving = false;

            //방향 전환
            isMoveToEnd = !isMoveToEnd;

            if (isAutoMove)
            {
                yield return new WaitForSeconds(autoWaitTime);

                PlatformMove();
            }
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 플랫폼 작동
        /// </summary>
        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            PlatformMove();
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 플랫폼 멈춤
        /// </summary>
        public override void DisableInteraction()
        {
            base.DisableInteraction();
            PlatformStop();
        }

    }
}