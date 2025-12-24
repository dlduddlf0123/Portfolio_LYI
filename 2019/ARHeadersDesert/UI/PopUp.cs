using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.ReadOnly;


public class PopUp : MonoBehaviour
{
    GameManager gameMgr;

    Button btn_popup;
    Image img_icon;
    Text txt_level;
    Text txt_comment;

    Button btn_upgrade;
    Text txt_upgrade;
    Text txt_upgradeCost;
    GameObject buttonLock;

    Text txt_missile;

    Button btn_close;

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        btn_popup = GetComponent<Button>();
        img_icon = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        txt_level = transform.GetChild(1).GetChild(1).GetComponent<Text>();
        txt_comment = transform.GetChild(2).GetChild(0).GetComponent<Text>();
        btn_upgrade = transform.GetChild(3).GetComponent<Button>();
        txt_upgrade = btn_upgrade.transform.GetChild(0).GetComponent<Text>();
        txt_upgradeCost = btn_upgrade.transform.GetChild(1).GetComponent<Text>();

        txt_missile = transform.GetChild(4).GetChild(0).GetComponent<Text>();
        buttonLock = transform.GetChild(5).gameObject;
        btn_close = transform.GetChild(6).GetComponent<Button>();
    }
    private void Start()
    {
        btn_close.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    public void PopUpInit(ShopItem _item)
    {
        txt_level.text = "LV " + _item.level.ToString();
        txt_upgradeCost.text = _item.upgradeCost.ToString();
        txt_missile.text = gameMgr.ReadShopData(5, 1) + ": " + _item.damage + "/ "
            + gameMgr.ReadShopData(5, 2) +  " +" + _item.upgradeScale + "\n" 
            + gameMgr.ReadShopData(5, 3) + ": " + _item.bubbleNum;
    }

    public void OpenPopUp(ShopItem _item,string _path, string _text)
    {
        img_icon.sprite = gameMgr.b_sprites.LoadAsset<Sprite>(_path);

        txt_comment.text = _text;
        gameMgr.SplitText(txt_comment);
        txt_upgrade.text = gameMgr.ReadShopData(5, 0); //강화 글자

        PopUpInit(_item);

        if (_item.isBuy)
        {
            buttonLock.gameObject.SetActive(false);
            btn_upgrade.onClick.RemoveAllListeners();
            btn_upgrade.onClick.AddListener(() => { BuyUpgrade(_item); });
        }
        else
        {
            buttonLock.gameObject.SetActive(true);
        }
    }

    //업그레이드 버튼 작동
    public void BuyUpgrade(ShopItem _item)
    {
        if (gameMgr.coin < _item.upgradeCost)
        {
            Debug.Log("You need more coin");
            return;
        }
        gameMgr.coin -= _item.upgradeCost;
        PlayerPrefs.SetInt("Coin", gameMgr.coin);
        gameMgr.shopMgr.SetCoinText();

        _item.level++;
        //강화 저장
        _item.SaveLevelData();

        //스텟 계산
        gameMgr.shopMgr.ItemInit(_item, (int)_item.type);
        //UI 재설정
        PopUpInit(_item);
    }
}
