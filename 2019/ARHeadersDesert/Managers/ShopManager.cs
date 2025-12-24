using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.ReadOnly;

//강화에 관련된 전반적인 기능
//상점 창에 걸어서 작동, UIManager, MissileManager와 연계
public class ShopManager : MonoBehaviour
{
    GameManager gameMgr;
    UIManager uiMgr;
    MissileManager missileMgr;

    /// <summary>
    /// 강화 아이템 창 셋업
    /// 후에 추가로 강화할 것을 늘려도 괜찮도록 구성하기(유연하게)
    /// 
    /// 장비는 1회 구매후 착용 버튼 활성화
    /// 구매한 목록은 데이터로 저장하고 있어야 함
    /// 
    /// </summary>
    /// 

    public List<ShopItem> list_item;
    PopUp popup;

    Text text_coin;

    Image icon;
    Text text_comment;

    Button btn_upgrade;

    RectTransform content;
    public ShopItem currentItem;

    //현재 선택된 정보 가져오는 용 변수
    int idx_lv;
    int idx_cost;

    //각 강화 요소의 강화레벨(저장해야됨)
    int lv_damage;
    int lv_speed;
    int lv_delay;
    //각 요소의 강화 계수(x레벨)
    public const int C_DAMAGE = 1;
    public const float C_SPEED = 0.2f;
    public const float C_DELAY = 0.1f;
    private void Awake()
    {
        gameMgr = GameManager.Instance;
        uiMgr = gameMgr.uiMgr;
        missileMgr = gameMgr.missileMgr;

        lv_damage = PlayerPrefs.GetInt(Defines.SAVE_INT_LEVEL_DAMAGE, 1);
        lv_speed = PlayerPrefs.GetInt(Defines.SAVE_INT_LEVEL_SPEED, 1);
        lv_delay = PlayerPrefs.GetInt(Defines.SAVE_INT_LEVEL_DELAY, 1);

        list_item = new List<ShopItem>();

        text_coin = transform.GetChild(2).GetComponent<Text>();
        content = transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        popup = transform.GetChild(4).GetComponent<PopUp>();
        currentItem = content.GetChild(PlayerPrefs.GetInt("BubbleType")).GetComponent<ShopItem>();
    }
    private void Start()
    {
        SetCoinText();
        ResetItem();
    }
    private void OnEnable()
    {
        SetCoinText();
        ResetItem();
    }

    //코인 텍스트 갱신
    public void SetCoinText()
    {
        text_coin.text = gameMgr.coin.ToString();
        Debug.Log("Coin:" + gameMgr.coin);
    }

    //아이템 목록 갱신
    public void ResetItem()
    {
        if (content == null) { return; }

        list_item.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            list_item.Add(content.GetChild(i).GetComponent<ShopItem>());
            ItemInit(list_item[i], i);
        }
    }


    //리스트에 각 아이템 추가
    //추가 될 때 해당 아이템의 값 결정(초기화)
    public void ItemInit(ShopItem _item, int _num)
    {
        _item.text_name.text = gameMgr.ReadShopData(_num, 0);
        string comment = gameMgr.ReadShopData(_num, 1);
        switch (_num)
        {
            case 0:
                _item.type = BubbleType.NORMAL;
                _item.level = PlayerPrefs.GetInt("NormalLevel", 1);
                _item.isBuy = true;
                _item.isEquip = Convert.ToBoolean(PlayerPrefs.GetInt("NormalEquip", 1));
                _item.SetCommentButton(popup, Defines.SPRITE_ICON_BUBBLE_NORMAL, comment);

                _item.upgradeScale = 5;
                break;
            case 1:
                //_item.level = 0; //PlayerPrefs.GetInt("LvSpread", 0);   //강화 시 사용
                //아이템의 가격 csv 불러오기
                _item.type = BubbleType.SPREAD;
                _item.level = PlayerPrefs.GetInt("SpreadLevel", 1);
                _item.isBuy = Convert.ToBoolean(PlayerPrefs.GetInt("SpreadBuy", 0));
                _item.isEquip = Convert.ToBoolean(PlayerPrefs.GetInt("SpreadEquip", 0));
                _item.SetCommentButton(popup, Defines.SPRITE_ICON_BUBBLE_SPREAD, comment);

                _item.upgradeScale = 2;
                _item.bubbleNum = (int)gameMgr.ReadMissileData(1, 4) + _item.level / 5 * 2;
                break;
            case 2:
                _item.type = BubbleType.SNIPE;
                _item.level = PlayerPrefs.GetInt("SnipeLevel", 1);
                _item.isBuy = Convert.ToBoolean(PlayerPrefs.GetInt("SnipeBuy", 0));
                _item.isEquip = Convert.ToBoolean(PlayerPrefs.GetInt("SnipeEquip", 0));
                _item.SetCommentButton(popup, Defines.SPRITE_ICON_BUBBLE_POWER, comment);

                _item.upgradeScale = 5;
                break;
            case 3:
                _item.type = BubbleType.REPEAT;
                _item.level = PlayerPrefs.GetInt("RepeatLevel", 1);
                _item.isBuy = Convert.ToBoolean(PlayerPrefs.GetInt("RepeatBuy", 0));
                _item.isEquip = Convert.ToBoolean(PlayerPrefs.GetInt("RepeatEquip", 0));
                _item.SetCommentButton(popup, Defines.SPRITE_ICON_BUBBLE_REPEAT, comment);

                _item.upgradeScale = 2;
                _item.bubbleNum = (int)gameMgr.ReadMissileData(3, 4) + _item.level / 5;
                break;
        }

        _item.damage = (int)gameMgr.ReadMissileData(_num, 0) + _item.level * _item.upgradeScale;
        _item.speed = gameMgr.ReadMissileData(_num, 1);
        _item.delay = gameMgr.ReadMissileData(_num, 2);

        _item.buyCost = (int)gameMgr.ReadMissileData(_num, 5);
        _item.upgradeCost = (int)gameMgr.ReadMissileData(_num, 6) * _item.level;
        _item.SetBuyButton();

        if (_item.isEquip)
        {
            EquipItem(_item);
        }
    }

    /// <summary>
    /// 아이템 장비하기
    /// </summary>
    /// <param name="_item"></param>
    public void EquipItem(ShopItem _item)
    {
        //현재 장착한 아이템 외 해제하기
        foreach (ShopItem item in list_item)
        {
            item.UnEquip();
        }
        _item.isEquip = true;
        _item.SetBuyButton();

        currentItem = _item;
        PlayerPrefs.SetInt("BubbleType", (int)_item.type);
        Debug.Log("BubbleType: " + _item.type);

        gameMgr.missileMgr.SetMissileStat(_item);
        gameMgr.missileMgr.RefreshMissile();
    }

    //구매 버튼 동작
    public void BuyItemButton(ShopItem _item)
    {
        if (gameMgr.coin <_item.buyCost)
        {
            Debug.Log("Not Enough Money");
        }
        else
        {
            Debug.Log("Purchased!");

            gameMgr.coin -= _item.buyCost;
            SetCoinText();
            PlayerPrefs.SetInt(Defines.SAVE_INT_COIN, gameMgr.coin);

            gameMgr.missileMgr.SetMissileStat(_item);
            gameMgr.missileMgr.RefreshMissile();
            _item.isBuy = true;

            EquipItem(_item);
        }
    }
}
