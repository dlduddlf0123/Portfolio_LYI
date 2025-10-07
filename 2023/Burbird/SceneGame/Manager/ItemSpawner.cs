using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

namespace Burbird
{

    /// <summary>
    /// 드랍 아이템 생성
    /// 아이템 확률 계산
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        StageManager stageMgr;

        public GameObject origin_dropItem;
        public Queue<GameObject> queue_dropItemPool = new Queue<GameObject>();
        public List<BurbirdDropItem> list_spawnItemPool = new List<BurbirdDropItem>();
        public List<InventoryItem> list_originItem = new List<InventoryItem>();

        Transform tr_active;
        Transform tr_disable;

        public int dropChance = 240;
        private void Awake()
        {
            stageMgr = StageManager.Instance;

            tr_active = transform.GetChild(0);
            tr_disable = transform.GetChild(1);
        }

        public void Init(BurbirdDropItem item)
        {
            GameManager.Instance.objPoolingMgr.ObjectInit(queue_dropItemPool, item.gameObject, tr_disable);
            list_spawnItemPool.Remove(item);
        }
        public void AbsorbItem()
        {
            for (int i = 0; i < list_spawnItemPool.Count; i++)
            {
                 list_spawnItemPool[i].StartAbsorbMove();
            }
        }

        void RandomItemSet(BurbirdDropItem item)
        {
            int rand = Random.Range(0, list_originItem.Count);

            item.SetItem(list_originItem[rand]);
        }

        /// <summary>
        /// 드랍 아이템 생성
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="itemIndex"></param>
        public void SpawnItem(Vector3 spawnPos)
        {
            //아이템 드랍 확률 계산 1/240, Int 기준 계산, 분모를 변수로
            if (Random.Range(0,dropChance) > 1)
            {
                return;
            }

            BurbirdDropItem item;
            float forcePitch = 5f;

            item = GameManager.Instance.objPoolingMgr.CreateObject(queue_dropItemPool, origin_dropItem, spawnPos, tr_active).GetComponent<BurbirdDropItem>();
            item.itemPicker.Quantity = 1;

            RandomItemSet(item);
            list_spawnItemPool.Add(item);
            item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Random.Range(-1f * forcePitch, 1f + forcePitch) + Vector2.up * 20f);

        }

    }
}
