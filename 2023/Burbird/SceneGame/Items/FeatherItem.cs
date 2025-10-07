using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class FeatherItem : MonoBehaviour
    {
        public int getFeatherCount = 10;

        private void OnEnable()
        {
            getFeatherCount = Random.Range(5, 10);
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                coll.gameObject.GetComponent<PlayerController2D>().currentFeatherCount += getFeatherCount;
                coll.gameObject.GetComponent<PlayerController2D>().ChangeFeatherState();
                //획득 이펙트, 사운드

                gameObject.SetActive(false);
            }
        }
    }
}