using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneShop : MonoBehaviour {

    public Button button_close;

    public ShopScrollList kShopScroll;

    // Use this for initialization
    void Start () {

        button_close.onClick.AddListener(onClick_close);

        kShopScroll.Setup(OnEvent_select, "");

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.anyKey)        {                    }
    }

    void onClick_close()
    {
        CGame.Instance.SceneChange(1);
    }

    //카드 선택 후 처리----------------------------------------------------------------------------
    void OnEvent_select(long _uid, string _order)
    {
        print("OnEvent_select_cast " + _uid + " " + _order);
        switch (_order) //선택모드
        {
        }

        //MyServer.Instance.Rq_CardCast((int)_cast_uid, 10, Callback_Rq_CardCast);
    }
/*
    public void Callback_Rq_CardCast(Hashtable _data)
    {
        string _rt = (string)_data[0]; print("Callback_Rq_CardCast : " + _rt);
        if (_rt == "ok")
        {
            string list_str = (string)_data[1];

            //카드 리스트 스트링.
            string[] source = list_str.Split("\t"[0]); int offset = 0;

            kList_new_card.Clear(); // 연출할것들.

            int count = Convert.ToInt32(source[offset++]);
            for (int i = 0; i < count; i++)
            {
                CardInfo card = new CardInfo(); card.SetString(source[offset++]);
                CGame.Instance.GetPlayer().CardList_insert(card);

                kList_new_card.Add(card); // 연출할것들.
            }

            //캐스트 연출로 넘기자.

            Display_start();


        }
    }
*/
}
