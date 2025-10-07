using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Character
{
    public class Tok_FootSound : MonoBehaviour
    {
        Tok_Movement tok_character;
        public bool isColliding = false;

        float resetTime = 0.1f;
        float delayTime = 0f;

        Coroutine currentCoroutine = null;

        private void Awake()
        {
            tok_character = GetComponentInParent<Tok_Movement>();
        }

        private void Update()
        {
            if (isColliding)
            {
                if (delayTime < resetTime)
                {
                    delayTime += Time.deltaTime;
                }
                else
                {
                    isColliding = false;
                }
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Ground"))
            {
                if (isColliding ||
                    tok_character.isDie)
                {
                    return;
                }
                isColliding = true;


                float pitchRange = 0.2f;
                float randomPitch = Random.Range(1 - pitchRange, 1 + pitchRange);
                GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_HEADER_FOOT, randomPitch);
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Ground"))
            {
                delayTime = 0f;
            }
        }


    }
}