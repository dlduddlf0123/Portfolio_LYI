using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

namespace AroundEffect
{

    public class Medicine_Syringe : MonoBehaviour
    {

        GameManager gameMgr;

        public XRGrabInteractable grabbable;

        public Rigidbody m_rigidbody;
        public Collider bodyColl;

        public MMF_Player mmf_use; //사용 효과
        public MMF_Player mmf_disappear; //사라짐 효과
        public ParticleSystem p_poof; //펑 효과

        UnityAction onSyringeActive;

        public bool isHolding = false;
        public bool isUsed = false;

        bool isInit = false;

        private void Awake()
        {
            SyringeInit();

        }

        public void SyringeInit()
        {

            if (!isInit)
            {
                gameMgr = GameManager.Instance;

                grabbable.selectEntered.AddListener(OnAttach);
                grabbable.selectExited.AddListener(OnDetach);

                //사용 후 사라지기
                mmf_use.Events.OnComplete.AddListener(mmf_disappear.PlayFeedbacks);
                mmf_use.Events.OnComplete.AddListener(() => onSyringeActive?.Invoke());
                //mmf_disappear.Events.OnPlay.AddListener(p_poof.Play);
                mmf_disappear.Events.OnComplete.AddListener(SyringeDisable);

                isInit = true;
            }

            //property init
            isHolding = false;
            isUsed = false;

            m_rigidbody.isKinematic = false;

            bodyColl.enabled = true;

            //이벤트 초기화
            onSyringeActive = null;
            transform.SetParent(gameMgr.MRMgr.tr_MRAnchor);
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                //들고있을 때 닿으면 작동
                if (isHolding && !isUsed)
                {
                    //이벤트 할당용
                    CharacterManager character = coll.gameObject.GetComponentInParent<CharacterManager>();
                    onSyringeActive = character.AI.OnSyringeInjected;

                    transform.SetParent(character.AI.tr_syringe);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;

                    UseSyringe();
                }
            }
        }


        //아이템 활성화 시
        public void UseSyringe()
        {
            isUsed = true;
            m_rigidbody.isKinematic = true;

            gameObject.SetActive(true);

            mmf_use.PlayFeedbacks();
        }

        public void SyringeDisable()
        {
            gameObject.SetActive(false);

            //gameMgr.lifeMgr.itemSpawner.ini TODO: Item init 같은거 만들것
        }

        /// <summary>
        /// 10/29/2024-LYI
        /// Grab 작동 시 이벤트
        /// 손 비활성화 관련 코드 Interactor쪽으로 이관
        /// </summary>
        /// <param name="args"></param>
        public void OnAttach(SelectEnterEventArgs args)
        {
            if (args.interactorObject != null)
            {
                //밀려나지 않도록 처리, 들고 있을 때 만 작동 처리
                isHolding = true;
                bodyColl.enabled = false;
            }
        }

        public void OnDetach(SelectExitEventArgs args)
        {
            if (args.interactorObject != null)
            {
                isHolding = false;
                bodyColl.enabled = true;
            }
        }



    }
}