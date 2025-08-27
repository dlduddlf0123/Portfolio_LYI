using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLobby : MonoBehaviour {
	
	public Text level_name;
	public Text level_text;         //레벨
	public Text exp_text;           //경험치
	public Slider exp_slider;       //경험치
	public Text gold_text;          //게임머니
	public Text cash_text;          //캐쉬탬   

	public Button hero_button;      //영웅관리
	public Button shop_button;      //상점
	public Button rank_button;		//랭킹

	public Button map_button;		//맵선택
	public Button start_button;     //전투시작


	Player kPlayer;


	// Use this for initialization
	void Start ()
	{
		kPlayer = CGame.Instance.kPlayer;
		CGame.Instance.Root_ui = GameObject.Find("Canvas_window");

		level_text.text = "Lv." + kPlayer.level;
		exp_text.text = kPlayer.exp_cur + "/" + kPlayer.exp_max;
		exp_slider.value = (float)kPlayer.exp_cur / (float)kPlayer.exp_max;

		gold_text.text = "" + kPlayer.gold;
		cash_text.text = "" + kPlayer.cash;



		start_button.onClick.AddListener( delegate { onClick_start(100); } );
		map_button.onClick.AddListener(onClick_map);

		shop_button.onClick.AddListener(onClick_shop);        
		hero_button.onClick.AddListener(onClick_hero);
		rank_button.onClick.AddListener(onClick_rank);

		CGameSnd.Instance.PlayBGM (eSound.bgm_main);
	}

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            //CGame.Instance.Window_notice("notice message", rt => {
            //    if (rt == "0") print("notice ok");
            //});

            CGame.Instance.Window_ok("message ?", rt =>
            {
                print("ok");
            });

            //CGame.Instance.Window_yesno("title", "message ?", rt => {
            //    if (rt == "0") print("yes");
            //    if (rt == "1") print("no");
            //});
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            GameObject go = GameObject.Find("Button_map");
            CGame.Instance.GameObject_set_image(go, "texture/title");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TableInfo_text text_table = CGameTable.Instance.Get_TableInfo_text(10000);

            print("" + text_table.Korean);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            CGameFx.Instance.PlayFx( "fx/fx_hit", Vector3.zero);
            //CGameFx.Instance.PlayFx((int)eFx."fx_hit", Vector3.zero, 2.0f);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CGameSnd.Instance.PlaySound(eSound.ui_button);
            //CGameSnd.Instance.PlayBGM(eSound.bgm_main);
            //CGameSnd.Instance.StopBGM();
        }
    }


	// 게임 시작 버튼
	void onClick_start(int _param)
	{
		print("onClick_start " + _param);
		CGameSnd.Instance.PlaySound(eSound.ui_button);
		CGame.Instance.SceneChange(2);
	}

	//맵 선택버튼
	void onClick_map()
	{
		print("onClick_map");
		CGameSnd.Instance.PlaySound(eSound.ui_button);
		//todo 
	}


	void onClick_shop()
	{
		print("onClick_shop");
		CGameSnd.Instance.PlaySound(eSound.ui_button);
		//todo 
		CGame.Instance.SceneChange(4);
	}    
	void onClick_hero()
	{
		print("onClick_hero");
		CGameSnd.Instance.PlaySound(eSound.ui_button);

		CGame.Instance.SceneChange(3);
	}
	void onClick_rank()
	{
		print("onClick_rank");
		//todo 
	}
}
