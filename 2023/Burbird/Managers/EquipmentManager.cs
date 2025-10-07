using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

namespace Burbird
{
    /// <summary>
    /// InventoryEngine과 상호작용, 장비 스탯 관리 후 데이터 매니저에 전달
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        GameManager gameMgr;

        public string playerID = "Player1";

        public Inventory mainInventory; //장착하지 않은 아이템 인벤
        public Inventory equipInventory; //장착중인 아이템 인벤

        //현재 장비 상태 번호
        int currentFeather;
        int currentBody;
        int currentBeak;
        int currentHead;
        int currentAccessory1;
        int currentAccessory2;
        private void Awake()
        {

        }


        public void RefreshEquipStatus()
        {

            gameMgr.dataMgr.equipStat = new Status();
        }


        /// <summary>
        /// 장비에 따른 캐릭터 외형 변경
        /// </summary>
        public void SetPlayerSprite()
        {

        }

        /// <summary>
        /// 장비 장착
        /// </summary>
        public void SetEquipments()
        {

        }

    }
}