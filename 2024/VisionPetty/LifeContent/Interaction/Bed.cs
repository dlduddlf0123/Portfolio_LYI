using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{
    public class Bed : MonoBehaviour
    {
        public Transform[] arr_bedTr;
        public int sleepCount = 0;


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                CharacterManager character =  coll.gameObject.GetComponentInParent<CharacterManager>();

                if (character.Movement.isGoingSleep)
                {
                    SetCharacterPosition(character);
                }
            }
        }


        public void SetCharacterPosition(CharacterManager character)
        {
            character.AI.OnSleeping();

            int num = 0;
            for (int i = 0; i < arr_bedTr.Length; i++)
            {
                if (arr_bedTr[i].childCount < 1)
                {
                    num = i;
                    break;
                }
            }

            character.transform.SetParent(arr_bedTr[num]);
            character.transform.localPosition = Vector3.zero;
            character.transform.localRotation = Quaternion.identity;

            sleepCount++;
        }


    }
}