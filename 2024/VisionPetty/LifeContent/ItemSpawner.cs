using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

namespace AroundEffect
{

    /// <summary>
    /// 10/23/2024-LYI
    /// 아이템 생성, 관리 클래스
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        GameManager gameMgr;

        //활성, 비활성 아이템 관리(오브젝트 풀링)
        public List<GameObject> list_activeItem = new List<GameObject>();
        public List<GameObject> list_disableItem = new List<GameObject>();


        public List<InventoryItem> list_originItem = new List<InventoryItem>();

        public GameObject origin_poop;
        
        public Transform tr_spawn;


        public void SpawnerInit()
        {
            gameMgr = GameManager.Instance;

            FillInventory();
        }

        public void FillInventory()
        {
            gameMgr.invenMgr.arr_inventory[(int)InventoryType.FOOD].AddItem(list_originItem[0], 99);
            gameMgr.invenMgr.arr_inventory[(int)InventoryType.FOOD].AddItem(list_originItem[1], 99);
            gameMgr.invenMgr.arr_inventory[(int)InventoryType.BATH].AddItem(list_originItem[2], 1);
            gameMgr.invenMgr.arr_inventory[(int)InventoryType.MEDICINE].AddItem(list_originItem[3], 99);


        }


        /// <summary>
        /// 10/23/2024-LYI
        /// UI에서 아이템 클릭 시 호출
        /// </summary>
        /// <param name="originItem"></param>
        /// <param name="spawnPos"></param>
        public void ItemSpawn(InventoryItem originItem)
        {
            GameObject go = gameMgr.objPoolingMgr.CreateObject(list_disableItem, originItem.Prefab, tr_spawn.position, this.transform);
            //TODO: 생성 효과

            Debug.Log("Item Spawned: " + go.name);
        }

        public void PoopSpawn(Vector3 pos)
        {
            GameObject go = gameMgr.objPoolingMgr.CreateObject(list_disableItem, origin_poop, pos, this.transform);
            //TODO: 생성 효과
            go.GetComponent<Poop>().OnPoop();
            Debug.Log("Poop Spawned: " + go.name);
        }

    }
}