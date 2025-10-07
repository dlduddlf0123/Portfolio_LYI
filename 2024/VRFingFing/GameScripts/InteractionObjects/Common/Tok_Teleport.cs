using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;
using VRTokTok.Character;

namespace VRTokTok.Interaction
{

    /// <summary>
    /// 10/5/2023-LYI
    /// 밟을 경우 반대 텔레포트로 이동하는 발판ㅎ
    /// </summary>
    public class Tok_Teleport : Tok_Interact
    {
        [Header("Tok_Teleport")]
        public Tok_Teleport tel_destination;
        public ParticleSystem efx_portal;
        public ParticleSystem efx_pop;

        public Transform tr_teleport;

        [SerializeField]
        MMF_Player mmf_in;
        [SerializeField]
        MMF_Player mmf_out;

        public bool isReady = true;

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                if (coll.gameObject.GetComponent<Tok_Movement>().isAction)
                {
                    return;
                }
                Teleport(coll.gameObject);
            }
        }
        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                isReady = true;
                SetPortal(true);
                //mmf_out.PlayFeedbacks();
            }
        }

        public void SetPortal(bool isActive)
        {
            if (efx_portal != null)
            {
                if (isActive)
                {
                    efx_portal.Play();
                }
                else
                {
                    efx_portal.Stop();
                }
            }
        }

        public void Teleport(GameObject coll)
        {
            if (!isReady || tel_destination == null)
            {
                return;
            }
            isReady = false;
            tel_destination.isReady = false;
            Debug.Log("Teleport!");

            SetPortal(false);
            //mmf_in.PlayFeedbacks();
            tel_destination.SetPortal(false);
            //tel_destination.mmf_in.PlayFeedbacks();

            tel_destination.PlayPop();
            PlayPop();


            Vector3 destinationPoint = tel_destination.tr_teleport.position;
            coll.transform.position = destinationPoint;
            coll.gameObject.GetComponent<Tok_Movement>().Stop();
        }

        public void PlayPop()
        {
            efx_pop.Play();
            GameManager.Instance.soundMgr.PlaySfxRandomPitch(transform.position, Constants.Sound.SFX_INTERACTION_TELEPORT);
        }


        public override void InteractInit()
        {
            base.InteractInit();
        }

        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}