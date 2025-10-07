using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

namespace Burbird
{
    public enum ShopItemType
    {
        NONE = 0,
        COIN,
        GEM,
        MONEY,
        ROOTBOX,
    }

    /// <summary>
    /// 구매 메뉴를 구성하는 각 아이템
    /// 아이템 정보와 가격, 구매버튼이 있다
    /// 구매 정보, 팝업이 뜰 때 전달
    /// </summary>
    public class ShopItem : MonoBehaviour
    {
        GameManager gameMgr;
        public UIShop ui_shop;

        //UI Elements
        Text txt_itemQuantity;
        Image img_item;

        public Button btn_purchase { get; set; }
        Image img_priceType;
        Text txt_price;

        //구매할 아이템 타입
        public ShopItemType purchaseType = ShopItemType.NONE;
        //구매할 아이템 량
        public int purchaseQuantity = 0;

        //지불할 재화 종류
        public ShopItemType priceType = ShopItemType.NONE;
        //결제할 재화 량(가격)
        public int itemPrice = 0;



        private void Awake()
        {
            gameMgr = GameManager.Instance;

            txt_itemQuantity = transform.GetChild(0).GetComponent<Text>();
            img_item = transform.GetChild(1).GetComponent<Image>();

            btn_purchase = transform.GetChild(2).GetComponent<Button>();
            txt_price = btn_purchase.transform.GetChild(0).GetComponent<Text>();
            img_priceType = btn_purchase.transform.GetChild(1).GetComponent<Image>();

        }

        void Start()
        {
            btn_purchase.onClick.AddListener(PurchaseButton);
        }

        public void SetPurchase(ShopItemType type, int quantity)
        {
            purchaseType = type;
            purchaseQuantity = quantity;
        }
        public void SetPrice(ShopItemType type, int price)
        {
            priceType = type;
            itemPrice = price;

            switch (type)
            {
                case ShopItemType.NONE:
                    break;
                case ShopItemType.COIN:
                   //img_priceType.sprite = gameMgr.addressMgr.dic_sprite["Coin"];
                    break;
                case ShopItemType.GEM:
                 //   img_priceType.sprite = gameMgr.addressMgr.dic_sprite["Gem"];
                    break;
                case ShopItemType.MONEY:
                    btn_purchase.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            txt_price.text = price.ToString();
        }

        public void PurchaseButton()
        {
            ui_shop.ActivatePopUp(this);
        }

    }
}