using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InventoryEngine;

using LitJson;

namespace Burbird
{
    public enum EquipmentGrade
    {
        NONE = 0,
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY,
        MYTHIC,
    }

    /// <summary>
    /// 각 장비 아이템에 들어간다
    /// </summary>
    public class BurbirdEquip : InventoryItem
    {
        GameManager gameMgr;

        [Header("Status")]
        public int equipID; //장비 번호
        public EquipmentGrade grade = EquipmentGrade.NONE; //장비 등급

        public int upgradeLevel = 1; //강화 레벨
        public int mainStat = 0; //메인 스탯(공격,체력) 수치

        public string[] arr_statusDescription = new string[6]; //스탯 설명 0~5

        //장비마다 스탯을 정하고 장비 시 그 스탯을 총 장비 스탯에 더한다
        public Status equipStat = new Status();

        private void Awake()
        {
            gameMgr = GameManager.Instance;

            if (ItemClass == ItemClasses.Weapon)
            {
                equipStat.ATKDamage = mainStat;
            }
            else if (ItemClass == ItemClasses.Armor)
            {
                equipStat.maxHp = mainStat;
            }
        }

        /// <summary>
        /// 장비 코드에 맞는 스탯 불러오기
        /// </summary>
        public void LoadStatus(Status stat)
        {
            equipStat = stat;
            if (ItemClass == ItemClasses.Weapon)
            {
                mainStat = (int)stat.ATKDamage;
               equipStat.ATKDamage = mainStat;
            }
            else if (ItemClass == ItemClasses.Armor)
            {
                mainStat = stat.maxHp;
                equipStat.maxHp = mainStat;
            }

            //arr_statusDescription[0] = "ItemName";
            //arr_statusDescription[1] = "Rare:";
            //arr_statusDescription[2] = "Epic:";
            //arr_statusDescription[3] = "Legendary:";
            //arr_statusDescription[4] = "Mythic:";
        }

        /// <summary>
        /// 5/4/2023-LYI
        /// Setting Descriptions of item
        /// </summary>
        /// <param name="arr_description">0:Name/1:Rare/2:Epic/3:Legendary/4:Mythic</param>
        public void SetDescriptions(string[] arr_description)
        {
            arr_statusDescription = arr_description;
        }


        public override bool Equip(string playerID)
        {
            base.Equip(playerID);
            Debug.Log(ItemName + ": Equip");
            gameMgr.dataMgr.equipStat += equipStat;
            gameMgr.dataMgr.RefreshAllPlayerStatus();
            gameMgr.uiMgr.ui_equip.RefreshStatusText();
            return true;
        }
 
        public override bool UnEquip(string playerID)
        {
            base.UnEquip(playerID);
            Debug.Log(ItemName + ": Unequip");
            gameMgr.dataMgr.equipStat -= equipStat;
            gameMgr.dataMgr.RefreshAllPlayerStatus();
            gameMgr.uiMgr.ui_equip.RefreshStatusText();
            return true;
        }


        public override void Swap(string playerID)
        {
            base.Swap(playerID);
            Debug.Log(ItemName + ": Swap");
            gameMgr.dataMgr.equipStat -= equipStat;
            gameMgr.dataMgr.RefreshAllPlayerStatus();
            gameMgr.uiMgr.ui_equip.RefreshStatusText();

        }

    }
}