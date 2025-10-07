using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction
{
    public class Trap_Bomb : Tok_Interact
    {
        public GameObject bombModel;
        public ParticleSystem p_explosion;
        public GameObject coll_explosion;

        public bool isExplosion = false;

        public override void InteractInit()
        {
            base.InteractInit();

            BombInit();
        }

        public void BombInit()
        {

            isExplosion = false;

            bombModel.SetActive(true);
            coll_explosion.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }


        public void Explosion()
        {
            isExplosion = true;

            p_explosion.Play();
            bombModel.SetActive(false);
            coll_explosion.gameObject.SetActive(true);

            StartCoroutine(AfterExplosion());
        }

        IEnumerator AfterExplosion()
        {
            yield return new WaitForSeconds(p_explosion.main.duration);

            coll_explosion.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }


        private void OnCollisionEnter(Collision coll)
        {
            if (coll.gameObject.CompareTag("Arrow"))
            {
                ActiveInteraction();
            }

            if (isExplosion)
            {
                if (coll.gameObject.CompareTag("Header"))
                {
                    //Restart
                    coll.gameObject.SetActive(false);
                }
            }
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Arrow"))
            {
                ActiveInteraction();
            }

            if (isExplosion)
            {
                if (coll.gameObject.CompareTag("Header"))
                {
                    //Restart
                    coll.gameObject.SetActive(false);
                }
            }
        }

        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            
            Explosion();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}
