using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace Burbird
{

    /// <summary>
    ///  UI 결제 관련 관리
    ///  버튼 눌러 결제할 때 팝업 이용
    ///  샵 아이템들 관리
    /// </summary>
    public class UIShop : MonoBehaviour
    {
        GameManager gameMgr;

        public ShopPopup popup_shop;

        List<ShopItem> list_shopItem = new List<ShopItem>(); //샵 내의 아이템들

        ShopItem[] arr_rootBox;
        int[] rootBoxPrice = new int[2] { 60, 300 };

        ShopItem[] arr_gem;
        int[] gemPrice = new int[6] { 1000, 6000, 12000, 24000, 59000, 120000 };
        int[] gemQuantity = new int[6] { 80, 500, 1200, 2500, 6500, 14000 };

        ShopItem[] arr_coin;
        int[] coinPrice = new int[3] { 30, 80, 200 };
        int[] coinQuantity = new int[3] { 1440, 4320, 12960 };

        public ShopItem currentItem = null;

        bool isError = false;
        private void Awake()
        {
            gameMgr = GameManager.Instance;

            Transform content = transform.GetChild(0).GetChild(0).GetChild(0);

            arr_rootBox = content.GetChild(0).GetComponentsInChildren<ShopItem>();
            arr_gem = content.GetChild(1).GetComponentsInChildren<ShopItem>();
            arr_coin = content.GetChild(2).GetComponentsInChildren<ShopItem>();
        }
        void Start()
        {
            for (int i = 0; i < arr_rootBox.Length; i++)
            {
                arr_rootBox[i].ui_shop = this;
                arr_rootBox[i].SetPurchase(ShopItemType.ROOTBOX, 1);
                arr_rootBox[i].SetPrice(ShopItemType.GEM, rootBoxPrice[i]);

                list_shopItem.Add(arr_rootBox[i]);
            }
            for (int i = 0; i < arr_gem.Length; i++)
            {
                arr_gem[i].ui_shop = this;
                arr_gem[i].SetPurchase(ShopItemType.GEM, gemQuantity[i]);
                arr_gem[i].SetPrice(ShopItemType.MONEY, gemPrice[i]);

                list_shopItem.Add(arr_gem[i]);
            }
            for (int i = 0; i < arr_coin.Length; i++)
            {
                arr_coin[i].ui_shop = this;
                arr_coin[i].SetPurchase(ShopItemType.COIN, coinQuantity[i]);
                arr_coin[i].SetPrice(ShopItemType.GEM, coinPrice[i]);

                list_shopItem.Add(arr_coin[i]);
            }

            for (int i = 0; i < list_shopItem.Count; i++)
            {
                // list_shopItem[i].btn_purchase.onClick.AddListener(()=>gameMgr.soundMgr.PlaySfx(list_shopItem[i].transform, ReadOnly.Defines.SOUND_SFX_SELECT));
            }
        }


        public void ActivatePopUp(ShopItem item)
        {
            currentItem = item;

            switch (item.purchaseType)
            {
                case ShopItemType.NONE:
                    break;
                case ShopItemType.COIN:
                    popup_shop.evt_yes.AddListener(() => PurchaseCoin());
                    break;
                case ShopItemType.GEM:
                    popup_shop.evt_yes.AddListener(() => PurchaseGem());
                    break;
                case ShopItemType.ROOTBOX:
                    popup_shop.evt_yes.AddListener(() => PurchaseRootBox());
                    break;
                default:
                    break;
            }

            popup_shop.ui_shop = this;
            popup_shop.gameObject.SetActive(true);
        }

        public void PayPrice()
        {
            switch (currentItem.priceType)
            {
                case ShopItemType.NONE:
                    break;
                case ShopItemType.COIN:
                    if (gameMgr.dataMgr.Coin < currentItem.itemPrice)
                    {
                        //거래 실패
                        isError = true;
                    }
                    gameMgr.dataMgr.GetCoin(-currentItem.itemPrice);
                    break;
                case ShopItemType.GEM:
                    if (gameMgr.dataMgr.Diamond < currentItem.itemPrice)
                    {
                        //거래 실패
                        isError = true;
                    }
                    gameMgr.dataMgr.GetDiamond(-currentItem.itemPrice);
                    break;

                case ShopItemType.MONEY:
                    //IAP 구매 팝업 연결
                    
                    break;
                default:
                    isError = true; 
                    break;
            }
        }

        /// <summary>
        /// 코인 구매
        /// </summary>
        public void PurchaseCoin()
        {
            //재화 소비
            PayPrice();

            if (!isError)
            {
                //구매 성공 메시지 팝업

                //재화 획득
                gameMgr.dataMgr.GetCoin(currentItem.purchaseQuantity);
            }
            else
            {
                //오류 메시지 팝업
            }

        }

        public void SetItem(ShopItem item)
        {
            currentItem = item;
        }

        /// <summary>
        /// 다이아 구매
        /// </summary>
        public void PurchaseGem()
        {
            //재화 소비
            PayPrice();

            if (!isError)
            {
                //재화 획득
                gameMgr.dataMgr.GetDiamond(currentItem.purchaseQuantity);
            }
            else
            {

            }
        }

        public void PurchaseRootBox()
        {
            //재화 소비
            PayPrice();
            //상자깡 화면으로 이동, 상자깡 시작
            //RootBoxManager
            if (!isError)
            {
            }
            else
            {

            }
        }

    }
}