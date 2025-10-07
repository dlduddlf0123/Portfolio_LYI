using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    /// <summary>
    /// 함정 관련 함수
    /// </summary>
    public class Trap : MonoBehaviour
    {
        public int trapDamage;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                trapDamage = (int)(coll.GetComponentInParent<Player>().playerStatus.maxHp * 0.3f);
                coll.GetComponentInParent<Player>().GetDamage(trapDamage, transform.position);
            }
        }


    }
}