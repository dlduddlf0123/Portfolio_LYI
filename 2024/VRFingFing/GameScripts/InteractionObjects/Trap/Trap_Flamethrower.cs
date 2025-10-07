using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTokTok.Character;

namespace VRTokTok.Interaction
{

    public class Trap_Flamethrower : Tok_Interact
    {
        GameManager gameMgr;

        [SerializeField]
        ParticleSystem[] arr_flame;
        [SerializeField]
        GameObject coll_flame;

        public float startDelay = 0f;
        public float shotTime = 2f;
        public float waitTime = 2f;

        public float collActiveWaitTime = 0.4f;

        Coroutine flameCoroutine = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        // Start is called before the first frame update
        void Start()
        {
            InteractInit();
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                coll.gameObject.GetComponentInParent<Tok_Movement>().OnFail(GameOverType.FLAME);
            }
        }

        void Stop()
        {
            if (flameCoroutine != null)
            {
                StopCoroutine(flameCoroutine);
            }
        }

        void FlameActive()
        {
            for (int i = 0; i < arr_flame.Length; i++)
            {
                arr_flame[i].Play();
            }
            StartCoroutine(CollActiveWait(true, collActiveWaitTime));

            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_TRAP_FLAMETHROWER);

            // coll_flame.gameObject.SetActive(true);
        }
        void FlameDisable()
        {
            for (int i = 0; i < arr_flame.Length; i++)
            {
                arr_flame[i].Stop();
            }
            StartCoroutine(CollActiveWait(false, collActiveWaitTime));
           // coll_flame.gameObject.SetActive(false);
        }

        IEnumerator CollActiveWait(bool isActive, float time)
        {
            yield return new WaitForSeconds(time);
            coll_flame.gameObject.SetActive(isActive);
        }


        IEnumerator RepeatShoot()
        {
            yield return new WaitForSeconds(startDelay);

            while (gameObject.activeSelf)
            {
                FlameActive();
                yield return new WaitForSeconds(shotTime);
                FlameDisable();
                yield return new WaitForSeconds(waitTime);
            }
        }


        public override void InteractInit()
        {
            base.InteractInit();
            
            Stop();
            
            FlameDisable();
            flameCoroutine = StartCoroutine(RepeatShoot());
        }

        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            Stop();
            flameCoroutine = StartCoroutine(RepeatShoot());
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
            StopAllCoroutines();
                FlameDisable();
        }

    }
}