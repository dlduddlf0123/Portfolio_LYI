using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Character;
using VRTokTok.Manager;

namespace VRTokTok.Interaction
{
    public class DeathZone : MonoBehaviour
    {
        public GameOverType typeGameOver = GameOverType.NONE;
        public Transform tr_death;
        public ParticleSystem efx_death;


        private void OnTriggerEnter(Collider coll)
        {
            if (GameManager.Instance.playMgr.statPlay != PlayStatus.PLAY)
            {
                return;
            }
            if (coll.gameObject.CompareTag("Header"))
            {
                Debug.Log("DeathZone:" + gameObject.name +
                    "\n - Character: " + coll.gameObject.name +
                    "\n - Type: " + typeGameOver.ToString());
                coll.gameObject.GetComponent<Tok_Movement>().OnFail(typeGameOver);

                if (tr_death != null)
                {
                    if (typeGameOver == GameOverType.WATER)
                    {
                        coll.gameObject.transform.position = new Vector3(coll.gameObject.transform.position.x,
                            tr_death.position.y,
                             coll.gameObject.transform.position.z);

                        if (coll.gameObject.GetComponent<Tok_Movement>().m_character.typeHeader == HeaderType.TENA)
                        {
                            coll.gameObject.transform.position += Vector3.up * 0.02f;
                        }
                        if (efx_death != null)
                        {
                            efx_death.transform.position = new Vector3(coll.gameObject.transform.position.x,
                           efx_death.transform.position.y,
                             coll.gameObject.transform.position.z);
                            efx_death.Play();
                            GameManager.Instance.soundMgr.PlaySfxRandomPitch(coll.gameObject.transform.position,
                                Constants.Sound.SFX_WATER_SPLASH);
                        }

                    }
                }


               // GameManager.Instance.playMgr.currentStage.RestartStage(true);
            }
        }
    }
}