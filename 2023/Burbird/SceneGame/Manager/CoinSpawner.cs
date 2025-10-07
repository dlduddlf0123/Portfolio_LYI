using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class CoinSpawner : MonoBehaviour
    {
        StageManager stageMgr;

        public GameObject origin_coin;
        Transform tr_active;
        Transform tr_disable;

        public Queue<GameObject> queue_coin = new Queue<GameObject>();
        public List<BurbirdItemCoin> list_activeCoin = new List<BurbirdItemCoin>();

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            tr_active = transform.GetChild(0);
            tr_disable = transform.GetChild(1);
        }
        public void Init(BurbirdItemCoin coin)
        {
            GameManager.Instance.objPoolingMgr.ObjectInit(queue_coin, coin.gameObject, tr_disable);
            list_activeCoin.Remove(coin);
        }

        /// <summary>
        /// 코인 흡수동작 실행
        /// </summary>
        /// <param name="isLate">2초 기다린 후 흡수</param>
        public void AbsorbCoin(bool isLate = false)
        {
            for (int i = 0; i < list_activeCoin.Count; i++)
            {
               //() => stageMgr.GetCoin(list_activeCoin[i].itemQuantity)
   
                   list_activeCoin[i].StartAbsorbMove(null, isLate);
            }
        }

        /// <summary>
        /// 한개 코인 소환하기
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="value"></param>
        public void SpawnCoin(Vector3 spawnPos, int value)
        {
            BurbirdItemCoin coin;
            float forcePitch = 5f;

            coin = GameManager.Instance.objPoolingMgr.CreateObject(queue_coin, origin_coin, spawnPos, tr_active).GetComponent<BurbirdItemCoin>();
            coin.itemPicker.Quantity = value;
            list_activeCoin.Add(coin);

            coin.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Random.Range(-1f * forcePitch, 1f + forcePitch) + Vector2.up * 20f);
        }
        /// <summary>
        /// 정해진 값에서 여러개의 코인 뿌리기
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="value"></param>
        public void SpawnCoins(Vector3 spawnPos, int value)
        {
            int SpawnNum = Random.Range(5, 10);
            int coin = 0;
            for (int i = 0; i <SpawnNum; i++)
            {
                if (i == 0)
                {
                    coin = value % SpawnNum;
                }
                else
                {
                    coin = 0;
                }
                SpawnCoin(spawnPos,value / SpawnNum + coin);
            }
        }

    }
}