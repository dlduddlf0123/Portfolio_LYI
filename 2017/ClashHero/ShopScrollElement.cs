using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopScrollElement : MonoBehaviour
{

    public Button buttonComponent;

	public Image 	iconImage;
    public Text 	nameLabel;    
	public Image 	priceImage;
    public Text 	priceText;


    private ShopScrollItem item;
    private ShopScrollList scrollList;

    // Use this for initialization
    void Start()
    {
        buttonComponent.onClick.AddListener(HandleClick);
    }

    public void Setup(ShopScrollItem currentItem, ShopScrollList currentScrollList)
    {
        item = currentItem;

        nameLabel.text = item.itemName;
        iconImage.sprite = item.icon;
        priceText.text = item.price.ToString();

        scrollList = currentScrollList;

    }

    public void HandleClick()
    {
        print("click" + nameLabel.text);
        //scrollList.TryTransferItemToOtherShop(item);
    }
}