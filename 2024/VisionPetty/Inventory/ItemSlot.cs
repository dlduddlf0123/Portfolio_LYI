using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MoreMountains.InventoryEngine;

namespace AroundEffect
{

    public class ItemSlot : InventorySlot
    {

        protected override void Awake()
        {
            base.Awake();
            InitSlotObjects();
        }
        protected override void Start()
        {
            base.Start();
            
        }

        public void InitSlotObjects()
        {
            IconImage = transform.GetChild(0).GetComponent<Image>();
            QuantityText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        }


        public override void Drop()
        {
            base.Drop();
        }

        public override void DrawIcon(InventoryItem item, int index)
        {
            if (ParentInventoryDisplay != null)
            {
                if (!InventoryItem.IsNull(item))
                {
                    SetIcon(item.Icon);

                    //아이템 정보가 장비가 아니면 텍스트를 숫자로 출력
                    if (item.ItemClass != ItemClasses.Armor &&
                        item.ItemClass != ItemClasses.Weapon)
                    {
                        SetQuantity(item.Quantity);
                    }
                    else
                    {
                        //BurbirdEquip burbirdEquip = (BurbirdEquip)item;
                        //image.color = burbirdEquip.SetGradeColor(burbirdEquip.grade);

                        //SetLevel(burbirdEquip.upgradeLevel);
                    }

                }
                else
                {
                    DisableIconAndQuantity();
                }
            }
        }
    

        public override void SetQuantity(int quantity)
        {
            if (quantity > 0)
            {
                QuantityText.gameObject.SetActive(true);
                QuantityText.alignment = TextAnchor.LowerRight;

                QuantityText.text = "x " + quantity.ToString();
            }
            else
            {
                QuantityText.gameObject.SetActive(false);
            }
        }
        public void CopySlot(ItemSlot originSlot)
        {
            image.sprite = originSlot.image.sprite;
            image.color = originSlot.image.color;
            IconImage.sprite = originSlot.IconImage.sprite;
            QuantityText.text = originSlot.QuantityText.text;
            QuantityText.gameObject.SetActive(originSlot.QuantityText.gameObject.activeSelf);
            QuantityText.alignment = originSlot.QuantityText.alignment;
        }

    }
}