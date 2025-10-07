using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTokTok.Interaction;

namespace VRTokTok
{
    public class HandMarker : Tok_Interact
    {
        public ParticleSystem efx_marker;
        public ParticleSystem efx_pop;

        public bool isActive = true;

        private void OnEnable()
        {
            isActive = true;
            efx_marker.Play();
        }


        public override void InteractInit()
        {
            base.InteractInit();

            isActive = true;
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                ActiveInteraction();
            }
        }

        IEnumerator WaitParticle(ParticleSystem p)
        {
            yield return new WaitForSeconds(p.main.duration + 0.5f);
            gameObject.SetActive(false);
        }


        public override void ActiveInteraction()
        {
            if (!isActive)
            {
                return;
            }
            if (onActive != null)
            {
                onActive.Invoke();
            }

            isActive = false;

            efx_marker.Stop();
            efx_pop.Play();
            GameManager.Instance.soundMgr.PlaySfx(transform.position,
                Constants.Sound.SFX_INTERACTION_HAND_MARKER);

            StartCoroutine(WaitParticle(efx_pop));
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }


    }
}