using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Burbird
{
    public class EnemySimple : Enemy
    {

        public float moveSpeed = 0.1f;

        private void OnEnable()
        {
            StartCoroutine(SimpleMove());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator SimpleMove()
        {
            while (true)
            {
                transform.Translate((stageMgr.playerControll.centerTr.position - transform.position).normalized * moveSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}