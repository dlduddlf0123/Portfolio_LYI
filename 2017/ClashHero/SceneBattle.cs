using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickIcon
{
	public int index = 0;
	public GameObject active_go;
	public GameObject select_go;

	public void Init( GameObject go)
	{		
	}

	public void SetActive( bool _v )
	{
		if( _v )	{ active_go.SetActive(_v); }
		else 		{ active_go.SetActive(_v); }
	}

	public void SetSelect( bool _v )
	{
		if( _v )	{ select_go.SetActive(_v); }
		else 		{ select_go.SetActive(_v); }
	}
}


public class SceneBattle : MonoBehaviour 
{
	public bool bActive = false; 
	public CharicManager kCharicManager;
	public SkillManager kSkillManager; 	// 20170420

	//result

	public GameObject kResult_Panel;
	public GameObject kResult_win;
	public GameObject kResult_lose;
	public Text reward_gold;
	public Button result_ok_button;

	public Text timer_text;

	//player
	Player kPlayer;
	Player kEnemy = new Player();

	//king tower
	Charic king_hero;
    Charic king_enemy;

	//battle

	int select_enemy_index = 0;		//ai select
	public GameObject spawn_pos_1;	//spawn pos

	//int next_hero_index = 0;	//next hero index 
	//List<int> hero_pick = new List<int>(); // 4
	//int select_hero_index = 0;	//user select

	//20170511
	public enum eBattleState
	{
		None,
		init, 	//
		play, 	//
		result,	//
		Max
	};
	public eBattleState kBattleState = 0;
	float fBattleState_time;
	string sBattleResult = "";
	bool bQuickBattle = false;


	public GameObject PickIcon_0;
	public GameObject PickIcon_1;
	public GameObject PickIcon_2;
	public GameObject PickIcon_3;

	PickIcon[] kPickIcon = new PickIcon[4];


	// mana1 hero
	public Slider Mana1_slider;
	public Text Mana1_text;
	int Mana1_cur = 0;	//마나.
	int Mana1_old = 0;
	float Mana1_set_time = 0; 
	float Mana1_speed = 2.5f; //create time

	// mana2 enemy
	public Slider Mana2_slider;
	public Text Mana2_text;
	int Mana2_cur = 0;	//마나.
	float Mana2_set_time = 0; 
	float Mana2_speed = 2.5f; //create time


	CTimeUnit kTime = new CTimeUnit();
	//string lasttime = kTime.CreateTime(180);
	//print("" + kTime.GetString_remain_time());

	//----------------------------------------------------------------------------
    void Awake()
	{
		kPlayer = CGame.Instance.kPlayer;


		kCharicManager = gameObject.AddComponent<CharicManager>();

		kSkillManager = new SkillManager (kCharicManager);

		Mana1_cur = 0;
		Mana1_set_time = 0; 
		Mana1_slider.value = 0;

		Mana2_cur = 0;
		Mana2_set_time = 0; 
		Mana2_slider.value = 0;

		kPickIcon [0] = new PickIcon ();
		kPickIcon [1] = new PickIcon ();
		kPickIcon [2] = new PickIcon ();
		kPickIcon [3] = new PickIcon ();

		kTime.CreateTime (0);
		timer_text.text = "00:00";
    }	
    	
	void Start () 
	{
		// mission_index
		// map_bg

		//enemy -------------------------------------------------------
		Enemy_spawn_init( 1 );

        //CGame.Instance.kDef.mission_cur
        
		//hero set -> scene deck

		//PickIcon 
		PickIcon_set(PickIcon_0);
		PickIcon_set(PickIcon_1);
		PickIcon_set(PickIcon_2);
		PickIcon_set(PickIcon_3);

		// charic -------------------------------------------------------
		// charic manager

		kCharicManager.Charic_remove_all ();

        Vector3 map_center = new Vector3(-0.25f, 0, 2.25f);

        //킹타워 배치

        king_hero = kCharicManager.Charic_add(1, 1000, Charic.eType.Hero);
		king_hero.Charic_init (kCharicManager,kSkillManager);
        king_hero.kGO.transform.position = map_center + new Vector3(0, 0, -8.0f);
        

        king_enemy = kCharicManager.Charic_add(2, 1000, Charic.eType.Enemy);
		king_enemy.Charic_init (kCharicManager,kSkillManager);
        king_enemy.kGO.transform.position = map_center + new Vector3(0, 0, 8.0f);

        king_enemy.kGO.transform.LookAt(king_hero.kGO.transform);
        
		// battle -------------------------------------------------------

		BattleState_set(eBattleState.init);

		// result -------------------------------------------------------
		result_ok_button.onClick.AddListener(onclick_result_ok);
		kResult_Panel.SetActive(false);

		CGameSnd.Instance.PlayBGM (eSound.bgm_battle);
    }

	void PickIcon_set(GameObject _go)
	{
		int index = 0;
		if (_go.name == "PickIcon_0") {			index = CGame.Instance.kPlayer.kDeckList [0];	kPickIcon [0].index = index;		}
		if (_go.name == "PickIcon_1") {			index = CGame.Instance.kPlayer.kDeckList [1];	kPickIcon [1].index = index;		}
		if (_go.name == "PickIcon_2") {			index = CGame.Instance.kPlayer.kDeckList [2];	kPickIcon [2].index = index;		}
		if (_go.name == "PickIcon_3") {			index = CGame.Instance.kPlayer.kDeckList [3];	kPickIcon [3].index = index;		}
		print("Pick_PickIcon_set   index: "  + index);

		TableInfo_charic table = CGameTable.Instance.Get_TableInfo_charic ( index );

		Button button = _go.transform.GetComponent<Button> ();
		if(_go.name == "PickIcon_0") button.onClick.AddListener (onClick_PickIcon_0);
		if(_go.name == "PickIcon_1") button.onClick.AddListener (onClick_PickIcon_1);
		if(_go.name == "PickIcon_2") button.onClick.AddListener (onClick_PickIcon_2);
		if(_go.name == "PickIcon_3") button.onClick.AddListener (onClick_PickIcon_3);

		if(_go.name == "PickIcon_0") kPickIcon [0].active_go = _go.transform.Find ("Image_active").gameObject;
		if(_go.name == "PickIcon_1") kPickIcon [1].active_go = _go.transform.Find ("Image_active").gameObject;
		if(_go.name == "PickIcon_2") kPickIcon [2].active_go = _go.transform.Find ("Image_active").gameObject;
		if(_go.name == "PickIcon_3") kPickIcon [3].active_go = _go.transform.Find ("Image_active").gameObject;

		if(_go.name == "PickIcon_0") kPickIcon [0].select_go = _go.transform.Find ("Image_select").gameObject;
		if(_go.name == "PickIcon_1") kPickIcon [1].select_go = _go.transform.Find ("Image_select").gameObject;
		if(_go.name == "PickIcon_2") kPickIcon [2].select_go = _go.transform.Find ("Image_select").gameObject;
		if(_go.name == "PickIcon_3") kPickIcon [3].select_go = _go.transform.Find ("Image_select").gameObject;

		Image image_c = _go.transform.Find ("Image").GetComponent<Image> ();
		string imagestr = "image/" + index;
		image_c.sprite = Resources.Load<Sprite>(imagestr) as Sprite as Sprite ;

		Text text_mana = _go.transform.Find ("Text_mana").GetComponent<Text> ();
		text_mana.text = "" + table.mana;
	}

	void onClick_PickIcon_0()	{	onClick_PickIcon(0);	}
	void onClick_PickIcon_1()	{	onClick_PickIcon(1);	}
	void onClick_PickIcon_2()	{	onClick_PickIcon(2);	}
	void onClick_PickIcon_3()	{	onClick_PickIcon(3);	}
	void onClick_PickIcon(int _num)
	{
		int hero_index = kPickIcon [_num].index;
		//int hero_index = kPlayer.DeckList_get (_num);
		print("onClick_PickIcon " + hero_index);

		CGameSnd.Instance.PlaySound (eSound.ui_button);

		PickIcon_select( _num );

		kPlayer.next_hero_index = hero_index;

		//Spawn_hero (kPlayer.next_hero_index);
	}

	// Update is called once per frame
	void Update () 
	{
		// charic
        kCharicManager.Charic_update(); //캐릭터 업데이트

		// battle
		BattleState_update(); //20170511

		// enemy spawn
		Enemy_spawn_update();

		// pick
		PickIcon_update();

		// hero spawn
		if (Input.GetMouseButtonDown (0) ) 		
		{
			Spawn_hero();
        }


		//test
        if (Input.GetKeyDown(KeyCode.T))
        {
			//Mana1_set(3);

            //Spawn_enemy();

            // charic add test
            //Charic charic; //test
            //charic = kCharicManager.Charic_add(0, 1001, Charic.eType.Hero); // charic add
            //charic.Charic_init(kCharicManager);


            // move
            //charic.move_target = king_enemy.kGO.transform;
            //charic.Act_set (Charic.eAct.walk);          

            // target search
            //ArrayList arrary = kCharicManager.FindTarget(charic);
            //foreach (Charic ch in arrary)
            //{
            //    print("" + ch.ID);            
            //}

            //print("pick");
            //GameObject go = CGame.Instance.GetRaycastObject();
            //print ("" + go.name);

            //print("spawn1");
            //GameObject go = CGame.Instance.GameObject_from_prefab ("prefabs/charic_01", null);
            //go.transform.position = Vector3.zero;

            //print("spawn hero");
            //GameObject go = CGame.Instance.GameObject_from_prefab("prefabs/charic_01", null);
            //Vector3 pos = CGame.Instance.GetRaycastObjectPoint();
            //go.transform.position = pos;
        }

    }


	void Spawn_hero()
	{
		print("Spawn_hero " + kPlayer.next_hero_index );

		Vector3 pos = CGame.Instance.GetRaycastObjectPoint(); //마우스 클릭한 곳.
		if (pos.z < -8.0f || pos.z > 12.0f)
			return;
		
		if(kPlayer.next_hero_index == 0 ) return;
				           
		TableInfo_charic tbl = CGameTable.Instance.Get_TableInfo_charic ( kPlayer.next_hero_index );

		if(Mana1_cur < tbl.mana ) return;

		//spawn hero
		Spawn_hero (kPlayer.next_hero_index, pos); 

		int mana = Mana1_cur - tbl.mana; 
		Mana1_set( mana );

		PickIcon_select(-1);

		kPlayer.next_hero_index = 0;		
	}

    //20170413
	void Spawn_hero(int _index, Vector3 pos)
    {			
		Charic charic = kCharicManager.Charic_add(0, _index, Charic.eType.Hero); // charic add
		charic.kGO.transform.position  = pos;
		charic.Charic_init(kCharicManager,kSkillManager);

		CGameSnd.Instance.PlaySound (eSound.ui_beep);
    }

    //20170413
	void Spawn_enemy(int _index)
    {
		Charic charic = kCharicManager.Charic_add(0, _index, Charic.eType.Enemy); // charic add
		charic.kGO.transform.position = spawn_pos_1.transform.localPosition;
		charic.Charic_init(kCharicManager,kSkillManager);    

		CGameSnd.Instance.PlaySound (eSound.ui_alarm);
    }

	//------------------------------------------------------------------------------------
	void PickIcon_update()
	{
		if( Mana1_cur == Mana1_old ) return;
		Mana1_old = Mana1_cur;
		//print("PickIcon_update " + Mana1_cur );

		TableInfo_charic c0 = CGameTable.Instance.Get_TableInfo_charic ( kPickIcon[0].index );
		if( c0.mana >= Mana1_cur ) kPickIcon[0].SetActive(true); else kPickIcon[0].SetActive(false);
		TableInfo_charic c1 = CGameTable.Instance.Get_TableInfo_charic ( kPickIcon[1].index );
		if( c1.mana >= Mana1_cur) kPickIcon[1].SetActive(true); else kPickIcon[1].SetActive(false);
		TableInfo_charic c2 = CGameTable.Instance.Get_TableInfo_charic ( kPickIcon[2].index );
		if( c2.mana >= Mana1_cur) kPickIcon[2].SetActive(true); else kPickIcon[2].SetActive(false);
		TableInfo_charic c3 = CGameTable.Instance.Get_TableInfo_charic ( kPickIcon[3].index );
		if( c3.mana >= Mana1_cur) kPickIcon[3].SetActive(true); else kPickIcon[3].SetActive(false);
	}

	void PickIcon_select(int _num)
	{
		kPickIcon[0].SetSelect(false);
		kPickIcon[1].SetSelect(false);
		kPickIcon[2].SetSelect(false);
		kPickIcon[3].SetSelect(false);

		if( _num<0 || _num> 3 ) return;

		kPickIcon[_num].SetSelect(true);		
	}

	//-------------------------------------------------------------------------------------
	void Enemy_spawn_init(int _map)
	{
		kEnemy.kDeckList.Clear();
		kEnemy.kDeckList.Add(1001);
		kEnemy.kDeckList.Add(1002);
		kEnemy.kDeckList.Add(1003);
		kEnemy.kDeckList.Add(1004);			

		if(_map == 1 ) //todo
		{
			kEnemy.DeckList_set (0, 1001);
			kEnemy.DeckList_set (1, 1002);
			kEnemy.DeckList_set (2, 1003);
			kEnemy.DeckList_set (3, 1004);
		}

	}

	// 몹 출현 ---------------------------------------------------------------------------

	int enemy_spawn_index = 0;

	void Enemy_spawn_update()
	{
		if (kBattleState != eBattleState.play)
			return;

		if(enemy_spawn_index != 0 )
		{
			TableInfo_charic tbl = CGameTable.Instance.Get_TableInfo_charic ( enemy_spawn_index );
			if(Mana2_cur >= tbl.mana )
			{
				Spawn_enemy(enemy_spawn_index);

				int mana = Mana2_cur - tbl.mana; 
				Mana2_set( mana );

				enemy_spawn_index = 0; // reselect
			}
		}

		//select
		if(enemy_spawn_index == 0 )
		{
			//임의 몹 //todo
			int unit = UnityEngine.Random.Range(0, 3);			
			enemy_spawn_index = kEnemy.kDeckList[unit];
		}

	}

	//20170511

	//-------------------------------------------------------------------------------------
	void BattleState_set(eBattleState _s)
	{
		kBattleState = _s;
		fBattleState_time = Time.time;
		switch (kBattleState)
		{
		case eBattleState.init: BattleState_init_set(); break;
		case eBattleState.play: BattleState_play_set(); break;
		case eBattleState.result: BattleState_result_set(); break;
		}
	}
	//-------------------------------------------------------------------------------------
	void BattleState_update()
	{
		switch (kBattleState)
		{
		case eBattleState.init: BattleState_init_update(); break;
		case eBattleState.play: BattleState_play_update(); break;
		case eBattleState.result: BattleState_result_update(); break;
		}
	}

	//----------------------------------------------
	void BattleState_init_set()
	{
		print("BattleState_init_set");

		sBattleResult = "lose";

		bQuickBattle = false;

		//mana set
		Mana1_set(0); Mana1_speed = 2.5f;
		Mana2_set(0); Mana2_speed = 2.5f;

		//hero unit order set
		kPlayer.next_hero_index = 0;

		//enemy unit order set
		kEnemy.next_hero_index = 0;

		kTime.CreateTime(180);

		CGameSnd.Instance.PlaySound (eSound.battle_notice);
	}
	void BattleState_init_update()
	{
		BattleState_set(eBattleState.play);
	}

	//----------------------------------------------
	void BattleState_play_set()
	{
		print("BattleState_play_set");
		//play start
	}
	void BattleState_play_update()
	{
		Mana1_updata();
		Mana2_updata();

		timer_text.text = kTime.GetString_remain_time ();

		//check battle end
		bool rt = Battle_end_check();
		if(rt == true)
			BattleState_set(eBattleState.result);					

	}
	bool Battle_end_check()
	{
		if (king_enemy.hp_cur <= 0) {
			sBattleResult = "win";
			return true;
		}
		if (king_hero.hp_cur <= 0) {
			sBattleResult = "lose";
			return true;
		}

		if (!bQuickBattle && kTime.GetRemainTime () <= 60) 
		{
			bQuickBattle = true;
			Mana1_speed = 1.5f;
			Mana2_speed = 1.5f;

			CGameSnd.Instance.PlaySound (eSound.battle_notice);
		}

		if (kTime.GetRemainTime () <= 0) {
			sBattleResult = "lose";
			return true;			
		}

		return false;

	}

	//----------------------------------------------
	void BattleState_result_set()
	{
		print("BattleState_result_set");

		//result display
		kResult_Panel.SetActive(true);

		CGameSnd.Instance.StopBGM ();

		if (sBattleResult == "win") 
		{
			CGameSnd.Instance.PlaySound(eSound.battle_win);

			kResult_win.SetActive(true);
			kResult_lose.SetActive(false);
		}
		else
		{
			CGameSnd.Instance.PlaySound(eSound.battle_lose);

			kResult_win.SetActive(false);
			kResult_lose.SetActive(true);
		}

		//reward
		reward_gold.text = "" + 100;

	}

	void BattleState_result_update()
	{
		
	}

	void onclick_result_ok()
	{
		CGame.Instance.SceneChange(1);
		
	}

	// mana set ------------------------------------------------
	void Mana1_set(int _mana)
	{
		Mana1_cur = _mana;
		Mana1_set_time = 0;
		Mana1_slider.value = (float)Mana1_cur;
		Mana1_text.text = "" + Mana1_cur;
	}
	void Mana1_updata()
	{
		if( Mana1_cur < 10 )
		{
			Mana1_set_time += Time.deltaTime;
			if( Mana1_set_time >= Mana1_speed ) //
			{
				Mana1_cur++; 
				Mana1_set_time = 0;
			}
		}
		
		if( Mana1_slider.value < (float)Mana1_cur)
		{
			Mana1_slider.value += Time.deltaTime * 2.0f; 
			if( Mana1_slider.value >= (float)Mana1_cur) 
			{
				Mana1_slider.value = (float)Mana1_cur;
				Mana1_text.text = "" + Mana1_cur;
			}
		}
	}
	void Mana2_set(int _mana)
	{
		Mana2_cur = _mana;
		Mana2_set_time = 0;
		Mana2_slider.value = (float)Mana2_cur;
		Mana2_text.text = "" + Mana2_cur;
	}
	void Mana2_updata() //enemy
	{
		if( Mana2_cur < 10 )
		{
			Mana2_set_time += Time.deltaTime;
			if( Mana2_set_time >= Mana2_speed ) //
			{
				Mana2_cur++; 
				Mana2_set_time = 0;
			}
		}

		if( Mana2_slider.value < (float)Mana2_cur)
		{
			Mana2_slider.value += Time.deltaTime * 2.0f; 
			if( Mana2_slider.value >= (float)Mana2_cur) 
			{
				Mana2_slider.value = (float)Mana2_cur;
				Mana2_text.text = "" + Mana2_cur;
			}
		}
	}
}
