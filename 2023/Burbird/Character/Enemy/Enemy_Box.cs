using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

namespace Burbird
{

    /// <summary>
    /// 적 캐릭터의 스탯 관리, 충돌 체크, 사망 관리
    /// </summary>
    public class Enemy_Box : Enemy
    {
        public override  void EnemyDie()
        {
            if (stageMgr.isEnemyDebug)
                Debug.Log("EnemyDie: " + gameObject.name);

            isDie = true;

            for (int boxTime = 0; boxTime < 3; boxTime++)
            {
                if (isDropable)
                {
                    if (!isClone)
                    {
                        //경험치 축적
                        stageMgr.currentRoom.roomExp += Status.EXP;

                        //아이템 드랍
                        // stageMgr.itemSpawner.SpawnItem(centerTr.position);
                    }

                    //코인 드랍
                    for (int i = 0; i < Random.Range(1, 4); i++)
                    {
                        stageMgr.coinSpawner.SpawnCoin(centerTr.position, Status.Coin);
                    }

                    //하트 드랍
                    stageMgr.heartSpawner.SpawnHeart(centerTr.position, 2);
                }
            }

            for (int i = 0; i < list_hitedFeather.Count; i++)
            {
                list_hitedFeather[i].FeatherFall();
            }

            StartCoroutine(DieAct());
        }

        /// <summary>
        /// 사망 시 처리
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator DieAct()
        {
            //사망 시 이벤트(자폭 등)
            onEnemyDie.Invoke();

            //적 캐릭터 사망 모션
            transform.GetChild(0).gameObject.SetActive(false);

            GetComponent<Rigidbody2D>().simulated = false;
            statCanvas.gameObject.SetActive(false);

            stageMgr.enemySpawner.EnemyListInit(this);
            yield return new WaitForSeconds(0.01f);

            //초기화 처리
            transform.GetChild(0).gameObject.SetActive(true);

            GetComponent<Rigidbody2D>().simulated = true;
            statCanvas.gameObject.SetActive(true);

           // EnemyInit();

            gameObject.SetActive(false);
        }

    }
}