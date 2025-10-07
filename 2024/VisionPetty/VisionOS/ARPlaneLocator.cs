using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    /// <summary>
    /// 9/13/2024-LYI
    /// Plane쪽에 달아 입력 감지
    /// 
    /// 입력장치의 입력이 감지되면 현재 충돌한 위치 체크
    /// PlaneGenerator에 포지션 전달
    /// 
    /// </summary>
    public class ARPlaneLocator : MonoBehaviour
    {
        GameManager gameMgr;
        ARPlaneGenerator Generator;

        public Vector3 placedPos;
        public Quaternion placedRot;


        private void Awake()
        {
            gameMgr = GameManager.Instance;
            Generator = gameMgr.MRMgr.AR_PlaneGenerator;

            placedPos = Vector3.zero;
            placedRot = Quaternion.identity;
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND))
            {
                if (Generator.statPlane == ARPlaneMode.NONE)
                {
                    Generator.CreateDummy();
                    placedPos = coll.ClosestPoint(transform.position);

                    Generator.MoveDummy(placedPos);
                }
            }
        }

        private void OnTriggerStay(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND))
            {
                if (Generator.statPlane == ARPlaneMode.MOVE)
                {
                    placedPos = coll.ClosestPoint(transform.position);
                    Generator.MoveDummy(placedPos);
                }
            }

        }


        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND))
            {

                //??
            }
        }


    }
}