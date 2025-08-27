
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class HeroScrollElement : MonoBehaviour
{

    public Button 	buttonComponent;
	public Image 	active_select;
    public Image 	icon_image;
	public Text 	mana_text;
	public Text 	name_text;



    private HeroScrollItem item;
    private HeroScrollList scrollList;

	public delegate void EventCallback(long _uid, string _order); //kdw add
	public EventCallback OnEventCallback;


    // Use this for initialization
    void Start()
    {
        buttonComponent.onClick.AddListener(HandleClick);
    }

	public void Setup(HeroScrollItem currentItem, HeroScrollList currentScrollList, EventCallback _callback)
    {
		item = currentItem;
		scrollList = currentScrollList;

		OnEventCallback = _callback;

		//------------------------------------------------------------
		int _index = (int)item.uid;

		Player player = CGame.Instance.kPlayer;

		HeroCard card = player.CardList_find (_index); 
		TableInfo_charic table = CGameTable.Instance.Get_TableInfo_charic ( card.index );

		name_text.text = table.name;
		mana_text.text = "" + table.mana;

		string imagestr = "image/" + _index;
		icon_image.sprite = Resources.Load<Sprite>(imagestr) as Sprite as Sprite ;

		active_select.gameObject.SetActive (false); //활성여부.
    }

    public void HandleClick()
    {
		//print("click " + item.uid);
        //scrollList.TryTransferItemToOtherShop(item);

		if(OnEventCallback != null)
			OnEventCallback(item.uid, "select");
    }


}