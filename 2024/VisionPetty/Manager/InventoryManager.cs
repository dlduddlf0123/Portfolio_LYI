using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

namespace AroundEffect
{

   public enum InventoryType
    {
        NONE =-1,
        FOOD = 0,
        TOY,
        FASION,
        BATH,
        MEDICINE,
        CONTEST,
    }


    /// <summary>
    /// 10/23/2024-LYI
    /// 인벤토리 관리 클래스
    /// 각종 아이템 관리
    /// </summary>
    public class InventoryManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        GameManager gameMgr;

        //전체 인벤토리
        public Inventory[] arr_inventory;

        //dic, 아이템 번호로 설명 불러오기
        Dictionary<int, List<object>> dic__itemDescription = new();


        private void Awake()
        {
            InventoryInit();
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
        }

        public void InventoryInit()
        {
            gameMgr = GameManager.Instance;

            RedrawInventory();
        }

        public void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            if (InventoryItem.IsNull(inventoryEvent.EventItem))
            {
                return;
            }

            for (int i = 0; i < arr_inventory.Length; i++)
            {
                int a = i;
                if (inventoryEvent.TargetInventoryName == arr_inventory[a].name)
                {
                    if (inventoryEvent.InventoryEventType == MMInventoryEventType.Click)
                    {
                        //해당 아이템 생성 호출
                        inventoryEvent.Slot.Use();
                        //inventoryEvent.Slot.Drop();
                        //inventoryEvent.EventItem.Drop(arr_inventory[a].PlayerID);
                        RedrawInventory();
                    }
                }

            }
        }

        public void RedrawInventory()
        {
            Debug.Log("RedrawInventory()");
             for (int i = 0; i < arr_inventory.Length; i++)
            {
                MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, arr_inventory[i].name, null, 0, 0, arr_inventory[i].PlayerID);
            }
        }

        public void SortInventory(Inventory inven, bool isGrade)
        {
            //BurbirdEquip[] arr = new BurbirdEquip[inven.Content.Length];

            //Array.Copy(inven.Content, arr, inven.Content.Length);
            ////null값 제거
            //arr = arr.Where(a => a != null).ToArray();

            //if (isGrade)
            //{
            //    arr = arr.OrderByDescending(a => a.grade).ThenBy(b => b.equipID).ToArray();
            //}
            //else
            //{
            //    arr = arr.OrderBy(a => a.equipID).ThenByDescending(b => b.grade).ToArray();
            //}
            ////Array.Sort(arr, (a, b) =>
            ////{
            ////    b.grade.CompareTo(a.grade);
            ////    b.equipID.CompareTo(a.equipID);
            ////});


            ////null값 다시 채우기
            //int nullCount = inven.Content.Length - arr.Length;
            //Array.Resize(ref arr, inven.Content.Length);
            //for (int i = 0; i < nullCount; i++)
            //{
            //    arr[arr.Length - i - 1] = null;
            //}

            //inven.Content = arr;
        }

        //TODO: 인벤 세이브, 로드


        //TODO: 아이템 생성 관련




    }
}