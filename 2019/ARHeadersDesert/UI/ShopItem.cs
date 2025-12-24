using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 190923 1차 완료
/// 상점에서 구매, 장착 기능 구현됨
/// 구매내역 저장 및 코인 저장
/// 미사일 장착시 능력 적용 구현할 것
/// </summary>
public class ShopItem : MonoBehaviour
{
    GameManager gameMgr;
    Button bt_icon;
    public Text text_name;

    Text text_cost;

    Button btn_buyItem;
    Sprite img_equip;
    public Sprite img_equipped;
    Text txt_confirm;
    public Color confirmColor;

    public BubbleType type = BubbleType.NORMAL;

    public int level = 1;
    public int damage = 15;
    public float speed = 2;
    public float delay = 0.3f;
    public int bubbleNum = 1;
    public int buyCost = 1000;
    public int upgradeCost = 100;
    public int upgradeScale = 5;
    
    public bool isEquip = false;
    public bool isBuy = false;

    void Awake()
    {
        gameMgr = GameManager.Instance;

        btn_buyItem = transform.GetChild(1).GetComponent<Button>();
        text_name = transform.GetChild(2).GetComponent<Text>();
        bt_icon = transform.GetChild(3).GetComponent<Button>();

        txt_confirm = btn_buyItem.transform.GetChild(0).GetComponent<Text>();
        text_cost = btn_buyItem.transform.GetChild(1).GetComponent<Text>();
        img_equip = btn_buyItem.image.sprite;
    }

    private void Start()
    {
        if (isEquip == true)
        {
            gameMgr.missileMgr.SetMissileStat(this);
        }
    }


    /// <summary>
    /// 아이콘 클릭 시 설명창 활성화 및 이미지, 텍스트 변경
    /// </summary>
    /// <param name="go">popup</param>
    /// <param name="_path">변경할 스프라이트 경로</param>
    /// <param name="_text">변경할 텍스트(설명)</param>
    public void SetCommentButton(PopUp go, string _path, string _text)
    {
        bt_icon.transform.GetChild(0).GetComponent<Image>().sprite = gameMgr.b_sprites.LoadAsset<Sprite>(_path);
        bt_icon.onClick.AddListener(() =>
        {
            go.gameObject.SetActive(true);
            go.OpenPopUp(this, _path, _text);
        });
    }

    //구매, 장착 버튼 기능 변경
    public void SetBuyButton()
    {
        btn_buyItem.onClick.RemoveAllListeners();

        SaveShopData();  //저장 데이터 갱신
        if (isBuy == false)
        {
            text_cost.gameObject.SetActive(true);
            txt_confirm.gameObject.SetActive(false);
            text_cost.text = buyCost.ToString();

            btn_buyItem.image.sprite = img_equip;
            btn_buyItem.onClick.AddListener(() =>
            { gameMgr.shopMgr.BuyItemButton(this); });
        }
        else if (isEquip == false)
        {
            text_cost.gameObject.SetActive(false);
            txt_confirm.gameObject.SetActive(true);
            txt_confirm.text = gameMgr.ReadShopData(4, 0);
            txt_confirm.color = confirmColor;

            btn_buyItem.image.sprite = img_equip;
            btn_buyItem.onClick.AddListener(() => gameMgr.shopMgr.EquipItem(this));
        }
        else if (isEquip == true)
        {
            text_cost.gameObject.SetActive(false);
            txt_confirm.gameObject.SetActive(true);
            txt_confirm.text = gameMgr.ReadShopData(4, 1);
            txt_confirm.color = Color.white;
            btn_buyItem.image.sprite = img_equipped;
        }
    }

    public void UnEquip()
    {
        isEquip = false;
        SetBuyButton();
    }

    public void SaveLevelData()
    {
        switch (type)
        {
            case BubbleType.NORMAL:
                PlayerPrefs.SetInt("NormalLevel", level);
                break;
            case BubbleType.SPREAD:
                PlayerPrefs.SetInt("SpreadLevel", level);
                break;
            case BubbleType.SNIPE:
                PlayerPrefs.SetInt("SnipeLevel", level);
                break;
            case BubbleType.REPEAT:
                PlayerPrefs.SetInt("RepeatLevel", level);
                break;
        }

    }


    /// <summary>
    /// 데이터 저장
    /// </summary>
    void SaveShopData()
    {
        switch (type)
        {
            case BubbleType.NORMAL:
                PlayerPrefs.SetInt("NormalEquip", Convert.ToInt32(isEquip));
                break;
            case BubbleType.SPREAD:
                PlayerPrefs.SetInt("SpreadBuy", Convert.ToInt32(isBuy));
                PlayerPrefs.SetInt("SpreadEquip", Convert.ToInt32(isEquip));
                break;
            case BubbleType.SNIPE:
                PlayerPrefs.SetInt("SnipeBuy", Convert.ToInt32(isBuy));
                PlayerPrefs.SetInt("SnipeEquip", Convert.ToInt32(isEquip));
                break;
            case BubbleType.REPEAT:
                PlayerPrefs.SetInt("RepeatBuy", Convert.ToInt32(isBuy));
                PlayerPrefs.SetInt("RepeatEquip", Convert.ToInt32(isEquip));
                break;
        }
    }


}
