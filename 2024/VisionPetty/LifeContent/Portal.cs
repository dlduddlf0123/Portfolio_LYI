using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{
    public class Portal : MonoBehaviour
    {
        public ParticleSystem p_portal;

        public Transform tr_land;

        // Start is called before the first frame update
        void Start()
        {

        }


        public void SpawnCharacter(CharacterManager character)
        {
            StartCoroutine(ActivePortal(character));
        }

        IEnumerator ActivePortal(CharacterManager character)
        {
            p_portal.Play();

            yield return new WaitForSeconds(1f);
            p_portal.Stop();

            character.CharacterInit();
            character.Stop();

            character.AI.isEvent = true;

            character.transform.position = this.transform.position;
            character.gameObject.SetActive(true);
            character.Movement.SetMoveMarker(tr_land);
            character.Movement.JumpToPosition(tr_land, 
                ()=>
                {
                    character.AI.isEvent = false;
                    character.AI.AIMove(AIState.IDLE); 
                });

        }

    }
}