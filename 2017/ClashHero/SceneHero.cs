using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHero : MonoBehaviour {

	public Button button_close;

	public GameObject Deck_0;
	public GameObject Deck_1;
	public GameObject Deck_2;
	public GameObject Deck_3;

	public GameObject Deck_select;
	public Button Deck_select_none;
	public Button Deck_select_0;
	public Button Deck_select_1;
	public Button Deck_select_2;
	public Button Deck_select_3;

	public Text 	Notice_text; 

	public HeroScrollList kHeroScroll;

	int iSelected_hero_index = 0;

	Player kPlayer;

	// Use this for initialization
	void Start () 
	{
		kPlayer = CGame.Instance.kPlayer;

		button_close.onClick.AddListener(onClick_close);

		//hero list
		kHeroScroll.Setup(OnEvent_select_hero, "");

		//deck list
		Deck_display();

		Deck_select_none.onClick.AddListener(onClick_deck_select_none);
		Deck_select_0.onClick.AddListener(onClick_deck_select_0);
		Deck_select_1.onClick.AddListener(onClick_deck_select_1);
		Deck_select_2.onClick.AddListener(onClick_deck_select_2);
		Deck_select_3.onClick.AddListener(onClick_deck_select_3);

		Deck_select.SetActive (false);

		Notice_text.text = "";

	}

	void Deck_display()
	{
		HeroCard card0 = kPlayer.CardList_find (kPlayer.DeckList_get (0));
		HeroCard card1 = kPlayer.CardList_find (kPlayer.DeckList_get (1));
		HeroCard card2 = kPlayer.CardList_find (kPlayer.DeckList_get (2));
		HeroCard card3 = kPlayer.CardList_find (kPlayer.DeckList_get (3));

		TableInfo_charic table0 = CGameTable.Instance.Get_TableInfo_charic ( card0.index );
		TableInfo_charic table1 = CGameTable.Instance.Get_TableInfo_charic ( card1.index );
		TableInfo_charic table2 = CGameTable.Instance.Get_TableInfo_charic ( card2.index );
		TableInfo_charic table3 = CGameTable.Instance.Get_TableInfo_charic ( card3.index );

		HeroScrollItem item_0 = new HeroScrollItem(); item_0.uid = card0.index;		HeroScrollElement deck_0 = Deck_0.GetComponent<HeroScrollElement>(); 		deck_0.Setup(item_0, null, OnEvent_select_deck_0); // 초기화.
		HeroScrollItem item_1 = new HeroScrollItem(); item_1.uid = card1.index;		HeroScrollElement deck_1 = Deck_1.GetComponent<HeroScrollElement>(); 		deck_1.Setup(item_1, null, OnEvent_select_deck_1); // 초기화.
		HeroScrollItem item_2 = new HeroScrollItem(); item_2.uid = card2.index;		HeroScrollElement deck_2 = Deck_2.GetComponent<HeroScrollElement>(); 		deck_2.Setup(item_2, null, OnEvent_select_deck_2); // 초기화.
		HeroScrollItem item_3 = new HeroScrollItem(); item_3.uid = card3.index;		HeroScrollElement deck_3 = Deck_3.GetComponent<HeroScrollElement>(); 		deck_3.Setup(item_3, null, OnEvent_select_deck_3); // 초기화.


	}

	// Update is called once per frame
	void Update () {
		
	}




	void onClick_close()
	{
		CGameSnd.Instance.PlaySound(eSound.ui_button);
		CGame.Instance.SceneChange(1);
	}


	// -------------------------------------------------------------------------------------------
	void OnEvent_select_deck_0(long _uid, string _order) { print("OnEvent_select_deck_0 " + _uid + " " + _order); }
	void OnEvent_select_deck_1(long _uid, string _order) { print("OnEvent_select_deck_1 " + _uid + " " + _order); }
	void OnEvent_select_deck_2(long _uid, string _order) { print("OnEvent_select_deck_2 " + _uid + " " + _order); }
	void OnEvent_select_deck_3(long _uid, string _order) { print("OnEvent_select_deck_3 " + _uid + " " + _order); }

	//카드 선택 후 처리----------------------------------------------------------------------------
	void OnEvent_select_hero(long _uid, string _order)
	{		
		print("OnEvent_select_hero " + _uid + " " + _order);

		iSelected_hero_index = (int)_uid;

		bool rt = kPlayer.DeckList_is(iSelected_hero_index);
		if (!rt) 
		{  
			//미 장착이면.
			Deck_select.SetActive (true); //교체할 덱 선택
		} 
		else 
		{
			CGame.Instance.Window_notice("이미 장착중입니다", rtt => {		
					print("");

			});
		}
	} 

	//덱 선택 ------------------------------------------------------------
	void onClick_deck_select_none()	{ Deck_select.SetActive (false);	}
	void onClick_deck_select_0()	{ Deck_select_ok(0);	}
	void onClick_deck_select_1()	{ Deck_select_ok(1);	}
	void onClick_deck_select_2()	{ Deck_select_ok(2);	}
	void onClick_deck_select_3()	{ Deck_select_ok(3);	}

	void Deck_select_ok(int _num)
	{
		Deck_select.SetActive (false);

		kPlayer.DeckList_set (_num, iSelected_hero_index);

		Deck_display ();

		kHeroScroll.RefreshDisplay ();

	}
}
