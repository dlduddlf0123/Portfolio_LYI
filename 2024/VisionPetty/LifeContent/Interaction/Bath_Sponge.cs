using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace AroundEffect
{

    /// <summary>
    /// 10/28/2024-LYI
    /// 캐릭터 몸을 닦는 스펀지
    /// 비누거품 파티클 + 뽀득뽀득 소리
    /// </summary>
    public class Bath_Sponge : MonoBehaviour
    {
        GameManager gameMgr;

        public ParticleSystem p_bubble;
        public XRGrabInteractable grabbable;

        public Collider bodyColl;

        public bool isHolding = false;
        public bool isWashing = false;

        private void Awake()
        {
            SpongeInit();

        }

        public void SpongeInit()
        {
            gameMgr = GameManager.Instance;
            isWashing = false;
            isHolding = false;

            bodyColl.enabled = true;

            grabbable.selectEntered.AddListener(OnAttach);
            grabbable.selectExited.AddListener(OnDetach);
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                if (isHolding)
                {
                    isWashing = true;
                    SpongeEnable();
                }
            }
            
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                if (isHolding)
                {
                    isWashing = false;
                    SpongeDisable();
                }
            }

        }


        public void SpongeEnable()
        {
            p_bubble.Play();
        }

        public void SpongeDisable()
        {
            p_bubble.Stop();
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
                isHolding = true;
                bodyColl.enabled = false;

                //bool isLeft = args.interactorObject.handedness == UnityEngine.XR.Interaction.Toolkit.Interactors.InteractorHandedness.Left;
                //gameMgr.MRMgr.polySpatialInput.SetCapsuleActive(isLeft, false);
            }
        }

        public void OnDetach(SelectExitEventArgs args)
        {
            if (args.interactorObject != null)
            {
                isHolding = false;
                bodyColl.enabled = true;
                //bool isLeft = args.interactorObject.handedness == UnityEngine.XR.Interaction.Toolkit.Interactors.InteractorHandedness.Left;
                //gameMgr.MRMgr.polySpatialInput.SetCapsuleActive(isLeft, true);
            }
        }

    }
}