using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

namespace Burbird
{
    public class InventoryChecker : MonoBehaviour
    {
        GameManager gameMgr;

        public Inventory mainInven;

        /// <summary>
        /// 0:Head,1:Feather,2:Body,3:Beak,4:Wing,5:Foot
        /// </summary>
        public Inventory[] arr_equipInven = new Inventory[6];

        //dic, 아이템 번호로 설명 불러오기
        Dictionary<int, List<object>> dic__equipDescription = new();

        //각 부위의 등급, 레벨에 따른 스탯 정리
        List<List<object>> list__equipStat_Beak = new();
        List<List<object>> list__equipStat_Body = new();
        List<List<object>> list__equipStat_Feather = new();
        List<List<object>> list__equipStat_Foot = new();
        List<List<object>> list__equipStat_Head = new();
        List<List<object>> list__equipStat_Wing = new();

        protected const string SAVE_FOLDER_NAME = "InventoryEngine/";
        protected const string SAVE_FILE_EXTENSION = ".inventory";

        private void Awake()
        {
            InventoryCheckerInit();
        }

        private void Start()
        {
            RefreshEquipStat();
        }

        /// <summary>
        /// 5/3/2023-LYI
        /// 인벤토리 체커에서 인벤토리 할당
        /// CSV파일에서 스탯 읽어오기
        /// </summary>
        public void InventoryCheckerInit()
        {
            gameMgr = GameManager.Instance;

            for (int i = 0; i < arr_equipInven.Length; i++)
            {
                arr_equipInven[i] = transform.GetChild(i + 1).GetComponent<Inventory>();
            }
            
            dic__equipDescription = gameMgr.csvLoader.ReadCSVDataDic("ItemChart");

            list__equipStat_Beak = gameMgr.csvLoader.ReadCSVDatas2("BeakStatus");
            list__equipStat_Body = gameMgr.csvLoader.ReadCSVDatas2("BodyStatus");
            list__equipStat_Feather = gameMgr.csvLoader.ReadCSVDatas2("FeatherStatus");
            list__equipStat_Foot = gameMgr.csvLoader.ReadCSVDatas2("FootStatus");
            list__equipStat_Head = gameMgr.csvLoader.ReadCSVDatas2("HeadStatus");
            list__equipStat_Wing = gameMgr.csvLoader.ReadCSVDatas2("WingStatus");
        }

        /// <summary>
        /// 5/3/2023-LYI
        /// 등급 별 최대 레벨 체크할 때 호출
        /// </summary>
        /// <param name="grade">현재 장비의 등급</param>
        /// <returns></returns>
        int EquipMaxLevel(EquipmentGrade grade)
        {
            switch (grade)
            {
                case EquipmentGrade.NONE:
                case EquipmentGrade.COMMON:
                    return 20;
                case EquipmentGrade.UNCOMMON:
                    return 30;
                case EquipmentGrade.RARE:
                    return 40;
                case EquipmentGrade.EPIC:
                    return 60;
                case EquipmentGrade.LEGENDARY:
                    return 80;
                case EquipmentGrade.MYTHIC:
                    return 100;
                default:
                    return 20;
            }
        }

        /// <summary>
        /// 로컬 데이터 저장하기
        /// 장착 장비, 인벤토리 저장하기
        /// </summary>
        public void SaveInventory()
        {
            //SerializedInventory serializedInventory = new SerializedInventory();
            //FillSerializedInventory(serializedInventory);
            //MMSaveLoadManager.Save(serializedInventory, DetermineSaveName(), _saveFolderName);

            mainInven.SaveInventory();

            for (int i = 0; i < arr_equipInven.Length; i++)
            {
                arr_equipInven[i].SaveInventory();
            }
        }
        /// <summary>
        /// 로컬 데이터 불러오기
        /// 장착 장비, 인벤토리 불러오기
        /// </summary>
        public void LoadInventory()
        {
            LoadSavedInventory(mainInven);

            for (int i = 0; i < arr_equipInven.Length; i++)
            {
                LoadSavedInventory(arr_equipInven[i]);
            }

        }
        public void LoadSavedInventory(Inventory inven)
        {
            SerializedInventory serializedInventory = (SerializedInventory)MMSaveLoadManager.Load(typeof(SerializedInventory), DetermineSaveName(inven), SAVE_FOLDER_NAME);
            ExtractSerializedInventory(inven, serializedInventory);
            MMInventoryEvent.Trigger(MMInventoryEventType.InventoryLoaded, null, this.name, null, 0, 0, inven.PlayerID);
        }
        string DetermineSaveName(Inventory inven)
        {
            return inven.gameObject.name + "_" + inven.PlayerID + SAVE_FILE_EXTENSION;
        }
        void ExtractSerializedInventory(Inventory inven, SerializedInventory serializedInventory)
        {
            if (serializedInventory == null)
            {
                return;
            }

            inven.InventoryType = serializedInventory.InventoryType;
            inven.DrawContentInInspector = serializedInventory.DrawContentInInspector;
            inven.Content = new InventoryItem[serializedInventory.ContentType.Length];
            
            for (int i = 0; i < serializedInventory.ContentType.Length; i++)
            {
                if ((serializedInventory.ContentType[i] != null) && (serializedInventory.ContentType[i] != ""))
                {
                    //220506 이영일
                    //인벤토리 로드 시 Resources.Load로 테스트, Addressable 방식으로 변경할 것

                    InventoryItem _loadedInventoryItem;
                    _loadedInventoryItem = gameMgr.addressMgr.dic_inventoryItem[serializedInventory.ContentType[i]];


                    if (_loadedInventoryItem == null)
                    {
                        Debug.LogError("InventoryEngine : Couldn't find any inventory item to load  " 
                            + " named " + serializedInventory.ContentType[i] + ". Make sure all your items definitions names (the name of the InventoryItem scriptable " +
                            "objects) are exactly the same as their ItemID string in their inspector. " +
                            "Once that's done, also make sure you reset all saved inventories as the mismatched names and IDs may have " +
                            "corrupted them.");
                    }
                    else
                    {
                        inven.Content[i] = _loadedInventoryItem.Copy();
                        inven.Content[i].Quantity = serializedInventory.ContentQuantity[i];
                    }
                }
                else
                {
                    inven.Content[i] = null;
                }
            }
        }

        /// <summary>
        /// 장착 중인 아이템 능력치 계산
        /// </summary>
        public void RefreshEquipStat()
        {
            Debug.Log("Equipment stat refresh");

            gameMgr.dataMgr.equipStat = new Status();

            for (int i = 0; i < arr_equipInven.Length; i++)
            {
                if (!InventoryItem.IsNull(arr_equipInven[i].Content[0]))
                {
                    BurbirdEquip bEquip = (BurbirdEquip)arr_equipInven[i].Content[0];

                    gameMgr.dataMgr.equipStat += bEquip.equipStat;
                }
            }

            gameMgr.dataMgr.RefreshAllPlayerStatus();
        }

        /// <summary>
        /// InventoryDisplay 갱신
        /// OnEnable()에서 적용안됨
        /// </summary>
        public void RedrawInventory()
        {
            MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, mainInven.name, null, 0, 0, mainInven.PlayerID);
            for (int i = 0; i < arr_equipInven.Length; i++)
            {
                MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, arr_equipInven[i].name, null, 0, 0, arr_equipInven[i].PlayerID);
            }
        }

        /// <summary>
        /// 인벤토리 정렬
        /// 장비 아이템의 번호에 따라 정렬
        /// 등급별 / 장비별 정렬 기능 제공
        /// </summary>
        /// <param name="isGrade">등급별 정렬인가</param>
        public void SortInventory(bool isGrade)
        {
            BurbirdEquip[] arr = new BurbirdEquip[mainInven.Content.Length];

            Array.Copy(mainInven.Content, arr, mainInven.Content.Length);
            //null값 제거
            arr = arr.Where(a => a != null).ToArray();

            if (isGrade)
            {
                arr = arr.OrderByDescending(a => a.grade).ThenBy(b => b.equipID).ToArray();
            }
            else
            {
                arr = arr.OrderBy(a => a.equipID).ThenByDescending(b => b.grade).ToArray();
            }
            //Array.Sort(arr, (a, b) =>
            //{
            //    b.grade.CompareTo(a.grade);
            //    b.equipID.CompareTo(a.equipID);
            //});


            //null값 다시 채우기
            int nullCount = mainInven.Content.Length - arr.Length;
            Array.Resize(ref arr, mainInven.Content.Length);
            for (int i = 0; i < nullCount; i++)
            {
                arr[arr.Length - i - 1] = null;
            }

            mainInven.Content = arr;

            //RefreshDisplay?
        }

        #region Item Data Management

        /// <summary>
        /// 데이터 목록에서 아이템 불러오기
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public InventoryItem LoadItem(string itemName)
        {
            try
            {
                return gameMgr.addressMgr.dic_inventoryItem[itemName];
            }
            catch (System.NullReferenceException)
            {
                StaticManager.UI.MessageUI.PopupMessage("!Null Reference Exception!: Check the Item Name: " + itemName);
                return null;
            }

            //if (InventoryItem.IsNull(gameMgr.addressMgr.dic_inventoryItem[itemName]))
            //{
            //    Debug.Log("!Null Reference Exception!: Check the Item Name: " + itemName);
            //    return null;
            //}
            //else
            //{
            //    return gameMgr.addressMgr.dic_inventoryItem[itemName];
            //}
        }


        #endregion

    }

    /// <summary>
    /// 4/27/2023-LYI
    /// 기존 Inventory 저장 시스템을 Easy Save 방식으로 저장하기 위해 제작
    /// 이 클래스 자체를 Easy Save에 저장하는 식
    /// </summary>
    class BurbirdInventory
    {
        //기존 인벤토리에 있던 옵션들, 연동용
        public int NumberOfRows;
        public int NumberOfColumns;
        public string InventoryName = "Inventory";
        public MoreMountains.InventoryEngine.Inventory.InventoryTypes InventoryType;
        public bool DrawContentInInspector = false;
        public string[] ContentType;
        public int[] ContentQuantity;

        //4/27/2023-LYI
        //BurbirdEquip에 필요한 아이템 구별 요소 추가
        //아이템 강화 레벨, 등급, ID
        //각 아이템저장?
        public BurbirdEquip[] Contents;
        public int ItemGrade;
        public int ItemLevel;
        public int ItemID;
    }
}