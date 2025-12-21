using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public Item(string _Type, string _Name, string _Explain, string _Number, bool _isUsing)
    { Type = _Type; Name = _Name; Explain = _Explain; Number = _Number; isUsing = _isUsing; }

    public string Type, Name, Explain, Number;
    public bool isUsing;
}

[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
}

public class InventoryManager : MonoBehaviour
{
    public TextAsset ItemDatabase;
    public List<Item> AllItemList, MyItemList,CurItemList;
    public string curType = "Item";
    public GameObject[] Slot, UsingImage;
    public Image[] ItemImage;
    public Sprite[] ItemSprite;
    public GameObject ExplainPanel;
    public InputField ItemNameInput, ItemNumberInput;
    string filePath;

    public int selectedItemNum;


    // Start is called before the first frame update
    void Start()
    {
        //전체 아이템 리스트 불러오기
        string[] line = ItemDatabase.text.Substring(0, ItemDatabase.text.Length - 1).Split('\n');
        for(int i=0;i<line.Length;i++)
        {
            string[] row = line[i].Split('\t');
            AllItemList.Add(new Item(row[0], row[1], row[2], row[3], row[4] == "True"));
        }
        filePath = Application.persistentDataPath + "/MyItemText.txt";
        print(filePath);
        Load();
        
    }

    public void ResetItemClick()
    {
        //Item BasicItem = AllItemList.Find(x => x.Name == "healthPotion");
        //BasicItem.isUsing = true;
        //MyItemList = new List<Item>() { BasicItem };
        MyItemList = AllItemList;
        Save();
        Load();
    }

    public void SlotClick(int slotNum)
    {
        // 설명창에 이름, 이미지, 개수, 설명 나타내기
        selectedItemNum = slotNum;
        ExplainPanel.GetComponentInChildren<Text>().text = CurItemList[slotNum].Name;
        ExplainPanel.transform.GetChild(2).GetComponent<Image>().sprite = Slot[slotNum].transform.GetChild(1).GetComponent<Image>().sprite;
        ExplainPanel.transform.GetChild(3).GetComponent<Text>().text = CurItemList[slotNum].Number + "개";
        ExplainPanel.transform.GetChild(4).GetComponent<Text>().text = CurItemList[slotNum].Explain;
        ExplainPanel.SetActive(true);

       /* Item CurItem = CurItemList[slotNum];
        Item UsingItem = CurItemList.Find(x => x.isUsing == true);
        if(curType =="Item")
        {
            if (UsingItem != null) UsingItem.isUsing = false;
            CurItem.isUsing = true;
        }
        else
        {

        }
        Save();*/
    }

    public void UseClick()
    {
        Item CurItem = CurItemList[selectedItemNum];
        Item UsingItem = CurItemList.Find(x => x.isUsing == true);
        if (curType == "Item")
        {
            if (UsingItem != null) UsingItem.isUsing = false;
            CurItem.isUsing = true;
        }
        else
        {

        }
        Save();
    }

    public void TabClick(string tabName)
    {
        curType = tabName;
        CurItemList = MyItemList.FindAll(x => x.Type == tabName);

        for(int i=0;i<Slot.Length;i++)
        {
            bool isExist = i < CurItemList.Count;
            Slot[i].GetComponentInChildren<Text>().text = isExist ? CurItemList[i].Name : "";

            if(isExist)
            {
                ItemImage[i].gameObject.SetActive(true);
                ItemImage[i].sprite = ItemSprite[AllItemList.FindIndex(x => x.Name == CurItemList[i].Name)];
                UsingImage[i].SetActive(CurItemList[i].isUsing);
            }
            else
            {
                ItemImage[i].gameObject.SetActive(false);
                UsingImage[i].SetActive(false);
            }
        }
    }

    public void GetItemClick()
    {
        Item curItem = MyItemList.Find(x => x.Name == ItemNameInput.text);
        ItemNumberInput.text = ItemNumberInput.text == "" ? "1" : ItemNumberInput.text;
        if (curItem != null) curItem.Number = (int.Parse(curItem.Number) + int.Parse(ItemNumberInput.text)).ToString();
        else
        {
            // 전체에서 얻을 아이템을 찾아 내 아이템에 추가
            Item curAllItem = AllItemList.Find(x => x.Name == ItemNameInput.text);
            if (curAllItem != null)
            {
                curAllItem.Number = ItemNumberInput.text;
                MyItemList.Add(curAllItem);
            }
        }       
        Save();
    }

    public void RemoveItemClick()
    {
        Item curItem = MyItemList.Find(x => x.Name == ItemNameInput.text);
        if (curItem != null)
        {

            int curNumber = int.Parse(curItem.Number) - int.Parse(ItemNumberInput.text == "" ? "1" : ItemNumberInput.text);

            if (curNumber <= 0) MyItemList.Remove(curItem);
            else curItem.Number = curNumber.ToString();
        }       
        Save();
    }

    void Save()
    {
        string jdata = JsonUtility.ToJson(new Serialization<Item>(MyItemList));

        File.WriteAllText(filePath, jdata);

        TabClick(curType);
    }

    void Load()
    {
        if (!File.Exists(filePath)) { ResetItemClick(); return; }

        string jdata = File.ReadAllText(filePath);
        MyItemList = JsonUtility.FromJson<Serialization<Item>>(jdata).target;

        TabClick(curType);
    }
}
