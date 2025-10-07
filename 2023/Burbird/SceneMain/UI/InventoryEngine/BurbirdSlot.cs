using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MoreMountains.InventoryEngine;

namespace Burbird
{

    public class BurbirdSlot : InventorySlot
    {
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
        }
        public override void DrawIcon(InventoryItem item, int index)
        {
            if (ParentInventoryDisplay != null)
            {
                if (!InventoryItem.IsNull(item))
                {
                    SetIcon(item.Icon);
                    SetQuantity(item.Quantity);
                    SetBackground((BurbirdEquip)item);
                }
                else
                {
                    DisableIconAndQuantity();
                }
            }
        }

        public virtual void SetBackground(BurbirdEquip equip)
        {
            switch (equip.grade)
            {
                case EquipmentGrade.NONE:
                    image.color = Color.white;
                    break;
                case EquipmentGrade.COMMON:
                    image.color = Color.white;
                    break;
                case EquipmentGrade.UNCOMMON:
                    image.color = Color.green;
                    break;
                case EquipmentGrade.RARE:
                    image.color = Color.blue;
                    break;
                case EquipmentGrade.EPIC:
                    image.color = new Color(1,0,1);
                    break;
                case EquipmentGrade.LEGENDARY:
                    image.color = Color.yellow;
                    break;
                case EquipmentGrade.MYTHIC:
                    image.color = Color.red;
                    break;
                default:
                    image.color = Color.white;
                    break;
            }
        }

    }
}