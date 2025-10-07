using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace Burbird
{
    public class RouletteItem : MonoBehaviour
    {
        public ItemType itemType = ItemType.NONE;

        public Image img;
        public TextMeshProUGUI text;
        public Perk perk;

        public int value = 0;

        /// <summary>
        /// æ∆¿Ã≈€ ¡§∫∏ ºº∆√
        /// </summary>
        /// <param name="type">∞ÒµÂ, ¥Ÿ¿Ãæ∆, ∆‹ ≈∏¿‘ º≥¡§</param>
        /// <param name="num">»πµÊ«“ º˝¿⁄</param>
        /// <param name="p">∆‹¿Œ ∞ÊøÏ ∆‹ ¡§∫∏</param>
        public void SetItem(ItemType type, int num,Sprite sprite, Perk p = null)
        {
            itemType = type;
            switch (itemType)
            {
                case ItemType.GOLD:
                    img.sprite = sprite;
                    value = num;
                    text.gameObject.SetActive(true);
                    break;
                case ItemType.DIAMOND:
                    img.sprite = sprite;
                    value = num;
                    text.gameObject.SetActive(true);
                    break;
                case ItemType.PERK:
                    perk = p;
                    p.PerkInit();
                    img.sprite = p.perk_img_icon.sprite;
                    text.gameObject.SetActive(false);
                    break;
                default:
                    value = num;
                    text.gameObject.SetActive(false);
                    break;
            }

            text.text = "X"+value.ToString();
        }

    }
}