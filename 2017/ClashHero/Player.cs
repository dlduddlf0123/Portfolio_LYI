using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{	
	public string sVersion = "1.0.0";
	public string user_id = "";
	public string nickname = "";
	public int gold = 0;
	public int cash = 0;
    public int vip_rank = 0;
    public int vip_exp = 0;
    public int level = 0;
    public int exp_cur = 0;
    public int exp_max = 0;

	public int stage = 1; 

    //deck
    public List<int> kDeckList = new List<int>();  //index
    int deck_max = 4;

    //hero
    public List<HeroCard> kHeroList = new List<HeroCard>(); 

    //item
    public List<ItemInfo> ItemList = new List<ItemInfo>();
    
    //for battle
	public int next_hero_index = 0;

    public void Init() //default
	{
		nickname = "myname";
		gold = 1000;
		cash = 100; //

		level = 1;
		exp_cur = 0;
		exp_max = 100;

		// card list init
		CardList_clear ();
		CardList_add (1001);
		CardList_add (1002);
		CardList_add (1003);
		CardList_add (1004);
		CardList_add (1005);

		//deck list init
		kDeckList.Clear ();
		kDeckList.Add (1001);
		kDeckList.Add (1002);
		kDeckList.Add (1003);
		kDeckList.Add (1004);

        //ItemList_clear();
        ItemList_add((int)eItemCode.gold, 100);


    }
	//------------------------------------------------------------
	public void CardList_clear()
	{
		kHeroList.Clear ();
	}
	public HeroCard CardList_find(int _index)
	{
		for (int i = 0; i < kHeroList.Count; i++) 
		{
			HeroCard card = kHeroList [i];
			if (card.index == _index)
				return card;
		}
		return null;
	}
	public void CardList_add(int _index)
	{
		HeroCard card = CardList_find (_index);
		if (card == null) {
			card = new HeroCard ();
			card.index = _index;
			card.count = 1;			
			kHeroList.Add (card);
		} 
		else 
		{
			card.count += 1;
		}
	}

	//------------------------------------------------------------

	public void DeckList_set(int _num, int _index)
	{
		if (_num >= deck_max)
			return;
		kDeckList [_num] = _index;
	}

	public int DeckList_get(int _num)
	{
		if (_num >= deck_max)
			return 0;
		return  kDeckList [_num];
	}

	public bool DeckList_is(int _index)
	{
		if (kDeckList [0] == _index)			return true;
		if (kDeckList [1] == _index)			return true;
		if (kDeckList [2] == _index)			return true;
		if (kDeckList [3] == _index)			return true;
		
		return  false;
	}


    // 아이템 리스트 관리 -------------------------------------------- 20170518
    public void ItemList_clear()
    {
        //foreach (ItemInfo Item in this.ItemList) Item.Clear();
        this.ItemList.Clear();
    }
    public ItemInfo ItemList_find(int _index)
    {
        foreach (ItemInfo Item in this.ItemList)
        {
            if (Item.index == _index) return Item;
        }
        return null;
    }
    // 아이템 리스트 추가
    public bool ItemList_insert(ItemInfo _item)
    {
        ItemInfo item = ItemList_find(_item.index);
        if (item == null) {
            ItemList.Add(_item);
            return true;
        }
        return false;
    }
    public bool ItemList_delete(int _index)
    {
        ItemInfo kItem = ItemList_find(_index);
        if (kItem == null) return false;
        ItemList.Remove(kItem);
        return true;
    }
    //아이템 카운트 변경..
    public bool ItemList_add(int _index, int _count)
    {
        ItemInfo kItem = ItemList_find(_index);
        if (kItem == null)
        {
            kItem = new ItemInfo();
            kItem.index = _index;
            ItemList_insert(kItem);
        }

        kItem.count += _count;
        if (kItem.count < 0) kItem.count = 0;

        return true;
    }
}

//1000	킹타워
//1001	전사
//1002	아처
//1003	법사
//1004	자이언트
//1005	호그라이더
//1006	삼총사

//----------------------------------------------------------------------------------------------------
public enum eHeroCode : int //charic index 
{
    king 		= 1000,
    dealer 		= 1001,      //근딜
    ranger 		= 1002,      //원딜
    magician 	= 1003,    //법사
    giant 		= 1004,       //방어
    tanker 		= 1005,      //공성
    supporter 	= 1006,   //부대
    healer 		= 1007,      //힐러
    buffer 		= 1008,      //
}

//----------------------------------------------------------------------------------------------------
public class HeroCard
{
	public int index;
	public int count;
	public int level;
	//...
}

//----------------------------------------------------------------------------------------------------
public enum eItemCode : int //item index //20170518
{
    cash = 2000,            //캐쉬
    gold = 2001,            //금화        
    vip_rank = 2002,        //VIP
    vip_exp = 2003,         //VIP
    player_exp = 2004,      //경험치
    stamina = 2005,         //행동력
}

public class ItemInfo
{
    public long item_uid = 0;  //0 이면 디스플레이용, 소지품은 서버가 아이디 할당.
    public int index = 0; //카테고리 구분.
    public int count = 0; //스텟, 소지품은 카운트 관리.

    public void Clear()
    {
        item_uid = 0; 
        index = 0;
        count = 0;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("{0}@{1}@{2}",
            item_uid.ToString(),
            index.ToString(),
            count.ToString()
            );
        return sb.ToString();
    }

    public void SetString(string kStr)
    {
        string[] source = kStr.Split("@"[0]); //플레이어정보와 구별되도록 구분자를 '@'로 변경.
        int offset = 0;

        item_uid = Convert.ToInt64(source[offset++]);
        index = Convert.ToInt32(source[offset++]);
        count = Convert.ToInt32(source[offset++]);
    }
}
