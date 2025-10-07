using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

namespace Burbird
{
    public class UIEquip : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        GameManager gameMgr;

        [Header("Equip")]
        public EquipmentInput equipInput;
        [SerializeField]
        Button btn_sort;
        [SerializeField]
        TextMeshProUGUI txt_sort;

        bool isSortGrade = true; //등급별 정렬인지?

        [Header("Popup")]
        public PopupEquip popup_equip;
        public RectTransform popup_character;
        public PopupStatus popup_status;

        public Button popup_fade;

        Text txt_ATK;
        Text txt_HP;

        Button[] btn_characterSelect = new Button[2];
        Button[] btn_status = new Button[2];

        private void Awake()
        {
            gameMgr = GameManager.Instance;

            equipInput = GetComponent<EquipmentInput>();

            txt_ATK = transform.GetChild(2).GetChild(0).GetComponent<Text>();
            txt_HP = transform.GetChild(2).GetChild(1).GetComponent<Text>();

            btn_characterSelect[0] = transform.transform.GetChild(2).GetChild(2).GetComponent<Button>();
            btn_characterSelect[1] = transform.transform.GetChild(2).GetChild(4).GetComponent<Button>();

            btn_status[0] = transform.transform.GetChild(2).GetChild(3).GetComponent<Button>();
            btn_status[1] = transform.transform.GetChild(2).GetChild(5).GetComponent<Button>();
        }

        private void Start()
        {
            popup_fade.onClick.AddListener(ClosePopup);

            btn_sort.onClick.AddListener(ButtonSort);
            LoadSortType();

            isSortGrade = ES3.Load("IsSortGrade", true);
            txt_sort.text = (isSortGrade) ? "Grade" : "Type";

            btn_characterSelect[0].onClick.AddListener(TogglePopupCharacter);
            btn_characterSelect[1].onClick.AddListener(TogglePopupCharacter);
            btn_status[0].onClick.AddListener(TogglePopupStatus);
            btn_status[1].onClick.AddListener(TogglePopupStatus);

            RefreshStatusText();
        }

        public void ClosePopup()
        {
            popup_equip.gameObject.SetActive(false);
            popup_character.gameObject.SetActive(false);
            popup_status.gameObject.SetActive(false);
            popup_fade.gameObject.SetActive(false);
        }

        void LoadSortType()
        {
            isSortGrade = ES3.Load("IsSortGrade", true);
            txt_sort.text = (isSortGrade) ? "Grade" : "Type";
        }
        public void ButtonSort()
        {
            isSortGrade = !isSortGrade;
            //인벤토리 정렬 진행
            gameMgr.invenChecker.SortInventory(isSortGrade);
            //인벤토리 다시 그리기
            gameMgr.invenChecker.RedrawInventory();

           //save sortType
            ES3.Save("IsSortGrade", isSortGrade);
            txt_sort.text = (isSortGrade) ? "Grade" : "Type";
        }

        public void ToggleGameObject(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }
        public void TogglePopupEquip()
        {
            ToggleGameObject(popup_equip.gameObject);
            ToggleGameObject(popup_fade.gameObject);
        }
        public void TogglePopupCharacter()
        {
            ToggleGameObject(popup_character.gameObject);
            ToggleGameObject(popup_fade.gameObject);
        }
        public void TogglePopupStatus()
        {
            ToggleGameObject(popup_status.gameObject);
            ToggleGameObject(popup_fade.gameObject);

            gameMgr.dataMgr.RefreshAllPlayerStatus();
            popup_status.RefreshStatusPopupText();
            RefreshStatusText();
        }

        public void RefreshStatusText()
        {
            txt_ATK.text = "ATK " + gameMgr.dataMgr.playerStat.ATKDamage;
            txt_HP.text = "HP" + gameMgr.dataMgr.playerStat.maxHp;
        }

        #region Inventory

        public void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            if (InventoryItem.IsNull(inventoryEvent.EventItem))
            {
                return;
            }
            if (!inventoryEvent.EventItem.IsEquippable)
            {
                return;
            }

            if (inventoryEvent.InventoryEventType == MMInventoryEventType.Click)
            {
                TogglePopupEquip();
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
            RefreshStatusText();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
        }
        #endregion
    }
}