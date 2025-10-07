using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class HeartSpawner : MonoBehaviour
    {
        StageManager stageMgr;

        public GameObject origin_heart;
        Transform tr_active;
        Transform tr_disable;

        public Queue<GameObject> queue_heart = new Queue<GameObject>();
        public List<BurbirdItemHeart> list_activeHeart = new List<BurbirdItemHeart>();

        public int dropChance = 20;

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            tr_active = transform.GetChild(0);
            tr_disable = transform.GetChild(1);
        }
        public void Init(BurbirdItemHeart heart)
        {
            GameManager.Instance.objPoolingMgr.ObjectInit(queue_heart, heart.gameObject, tr_disable);
            list_activeHeart.Remove(heart);
        }

        public void AbsorbHeart()
        {
            for (int i = 0; i < list_activeHeart.Count; i++)
            {
                //() => stageMgr.GetCoin(list_activeCoin[i].itemQuantity)

                list_activeHeart[i].StartAbsorbMove();
            }
        }
        public void SpawnHeart(Vector3 spawnPos, int chance = 0)
        {
            if (chance == 0)
            {
                chance = dropChance;
            }

            //µå¶ø È®·ü
            if (Random.Range(0, chance) > 1)
            {
                return;
            }

            BurbirdItemHeart heart;
            float forcePitch = 5f;

            heart = GameManager.Instance.objPoolingMgr.CreateObject(queue_heart, origin_heart, spawnPos, tr_active).GetComponent<BurbirdItemHeart>();
            list_activeHeart.Add(heart);

            heart.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Random.Range(-1f * forcePitch, 1f + forcePitch) + Vector2.up * 20f);

            heart.StartAbsorbMove();
        }

    }
}