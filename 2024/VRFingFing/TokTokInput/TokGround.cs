using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Interaction;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using Pathfinding;

using VRTokTok.Character;
using VRTokTok.Manager;

namespace VRTokTok
{
    /// <summary>
    /// 7/4/2023-LYI
    /// 바닥 터치 관련 데이터 가지고 있기
    /// 전체 바닥 타일 정보 가지고 있기
    /// 플레이어 위치 타일 정보 가지고 있기
    /// </summary>
    public class TokGround : MonoBehaviour
    {
        GameManager gameMgr;
        TokTokManager tokMgr;

        public InteractableUnityEventWrapper Event { get; set; } //바닥 이벤트 체크, 스테이지마다 변경됨


        public UnityAction OnGroundTok;
        public UnityAction OnGroundStay;
        public UnityAction OnGroundRelease;


        bool isFirst = true;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            tokMgr = gameMgr.playMgr.tokMgr;

            Event = GetComponent<InteractableUnityEventWrapper>();
        }


        public void Init()
        {
            OnGroundTok = OnTok;

            if (isFirst)
            {
                Event.WhenSelect.AddListener(OnGroundTok);
                Event.WhenSelectingInteractorViewAdded.AddListener(OnGroundStay);
                Event.WhenUnselect.AddListener(OnGroundRelease);
                isFirst = false;
            }
        }


        /// <summary>
        /// 바닥 터치 시 동작
        /// 터치 입력 위치 보내기
        /// </summary>
        public void OnTok()
        {
            Vector3 pos = Vector3.zero;

            if (tokMgr.arr_pokeHand[0].State == InteractorState.Select ||
                tokMgr.arr_pokeHand[0].State == InteractorState.Hover)
            {
                pos = tokMgr.arr_pokeHand[0].TouchPoint;
            }
            if (tokMgr.arr_pokeHand[1].State == InteractorState.Select ||
                tokMgr.arr_pokeHand[1].State == InteractorState.Hover)
            {
                pos = tokMgr.arr_pokeHand[1].TouchPoint;
            }
            if (tokMgr.arr_pokeController[0].State == InteractorState.Select||
                tokMgr.arr_pokeController[0].State == InteractorState.Hover)
            {
                pos = tokMgr.arr_pokeController[0].TouchPoint;
            }
            if (tokMgr.arr_pokeController[1].State == InteractorState.Select ||
                tokMgr.arr_pokeController[1].State == InteractorState.Hover)
            {
                pos = tokMgr.arr_pokeController[1].TouchPoint;
            }

            pos = new Vector3(pos.x, transform.position.y, pos.z);

            //if (OVRPlugin.GetHandTrackingEnabled())
            //{
            //    if (tokMgr.arr_pokeHand[0].State == InteractorState.Select)
            //    {
            //        pos = tokMgr.arr_pokeHand[0].TouchPoint;
            //    }
            //    pos = tokMgr.arr_pokeHand[1].TouchPoint;
            //}
            //else
            //{
            //    pos = tokMgr.arr_pokeController[0].TouchPoint;
            //    pos = tokMgr.arr_pokeController[1].TouchPoint;
            //}

            ////왼손 오른손 체크
            //Vector3 pos0 = tokMgr.arr_hand[0].GetComponent<OVRSkeleton>().Bones[20].Transform.position;
            //Vector3 pos1 = tokMgr.arr_hand[1].GetComponent<OVRSkeleton>().Bones[20].Transform.position;

            //if (tokMgr.isLeftHanded)
            //{
            //    pos = new Vector3(pos0.x, transform.position.y, pos0.z);
            //}
            //else
            //{
            //    pos = new Vector3(pos1.x, transform.position.y, pos1.z);
            //}
            //if (tokMgr.arr_poke[0].State != InteractorState.Normal)
            //{
            //    pos = pos0;
            //}
            //else if (tokMgr.arr_poke[1].State != InteractorState.Normal)
            //{
            //    pos = pos1;
            //}

            //float dist0 = Vector3.Distance(pos0, transform.position);
            //float dist1 = Vector3.Distance(pos1, transform.position);

            //pos  =dist0 < dist1  ? pos0 : pos1;

            //Debug.Log("TokObj: " + transform.parent.gameObject.name);
            tokMgr.Tok(pos, transform.parent.gameObject,this);
        }




        //private void OnCollisionEnter(Collision coll)
        //{
        //    if (coll.gameObject.CompareTag("Player"))
        //    {
        //        if (coll.gameObject.GetComponentInParent<OVRHand>().gameObject.name ==
        //            gameMgr.currentStage.tokMgr.arr_hand[0].gameObject.name)
        //        {

        //        }
        //        gameMgr.currentStage.tokMgr.MarkerActive(coll.contacts[0].point);
        //    }
        //}
        //private void OnCollisionExit(Collision coll)
        //{
        //    if (coll.gameObject.CompareTag("Player"))
        //    {

        //    }
        //}

    }
}