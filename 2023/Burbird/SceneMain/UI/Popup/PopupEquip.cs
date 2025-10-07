using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using TMPro;

namespace Burbird
{ /// <summary>
  /// 장비 선택 시 열리는 창, 클릭한 장비의 정보를 읽어온다
  /// 장비버튼으로 맞는 인벤토리에 장비
  /// </summary>
	public class PopupEquip : MonoBehaviour, MMEventListener<MMInventoryEvent>
	{
		GameManager gameMgr;

		[Header("Top UI")]
		[SerializeField]
		TextMeshProUGUI txt_name;
		[SerializeField]
		TextMeshProUGUI txt_category;
		[SerializeField]
		Button btn_close;

		[Header("Center UI")]
		[SerializeField]
		Image img_icon;
		[SerializeField]
		TextMeshProUGUI txt_level;

		[SerializeField]
		TextMeshProUGUI[] txt_status;

		[Header("Bottom UI")]
		[SerializeField]
		Button btn_equip;
		[SerializeField]
		Button btn_unequip;
		[SerializeField]
		Button btn_upgrade;


		Text txt_upgradeCoin; //강화 레벨에 따른 계산

		private void Awake()
		{
			gameMgr = GameManager.Instance;


			txt_status = new TextMeshProUGUI[transform.GetChild(5).childCount];
			for (int i = 0; i < transform.GetChild(5).childCount; i++)
			{
				txt_status[i] = transform.GetChild(5).GetChild(i).GetComponent<TextMeshProUGUI>();
			}

			txt_upgradeCoin = transform.GetChild(8).GetChild(1).GetComponent<Text>();

		}

		private void Start()
		{
			btn_close.onClick.AddListener(gameMgr.uiMgr.ui_equip.ClosePopup);
			btn_equip.onClick.AddListener(()=> { 
				gameMgr.uiMgr.ui_equip.equipInput.Equip();
				gameMgr.invenChecker.SaveInventory();
				gameMgr.uiMgr.ui_equip.ClosePopup();
			});
			btn_unequip.onClick.AddListener(() => {
				gameMgr.uiMgr.ui_equip.equipInput.UnEquip();
				gameMgr.invenChecker.SaveInventory();
				gameMgr.uiMgr.ui_equip.ClosePopup();
			});

			btn_upgrade.onClick.AddListener(() => gameMgr.uiMgr.ui_equip.TogglePopupEquip());
		}

		/// <summary>
		/// 버튼 Equip, UnEquip 교체
		/// </summary>
		/// <param name="isEquip"></param>
		void ChangeButton(bool isEquip)
        {
            if (isEquip)
			{
				btn_equip.gameObject.SetActive(false);
				btn_unequip.gameObject.SetActive(true);
			}
            else
			{
				btn_equip.gameObject.SetActive(true);
				btn_unequip.gameObject.SetActive(false);
			}

        }

		/// <summary>
		/// Starts the display coroutine or the panel's fade depending on whether or not the current slot is empty
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void DisplayDetails(InventoryItem item)
		{
			if (InventoryItem.IsNull(item))
			{
				StartCoroutine(FillDetailFieldsWithDefaults(0));
			}
			else
			{
				FillDetailFields(item);
			}
		}

		/// <summary>
		/// Fills the various detail fields with the item's metadata
		/// </summary>
		/// <returns>The detail fields.</returns>
		/// <param name="item">Item.</param>
		/// <param name="initialDelay">Initial delay.</param>
		protected void FillDetailFields(InventoryItem item)
		{
			//yield return new WaitForSeconds(initialDelay);

			BurbirdEquip bEquip = (BurbirdEquip)item;

			if (txt_name != null) { txt_name.text = bEquip.arr_statusDescription[0]; }
			if (txt_category != null) { txt_category.text = bEquip.grade.ToString(); }
			//if (txt_description != null) { txt_description.text = item.Description; }
			if (txt_level != null) { txt_level.text ="LV " + bEquip.equipStat.level +"/"+bEquip.equipStat.maxLevel; }
			if (img_icon != null) { img_icon.sprite = item.Icon; }

            if (txt_status != null)
			{
				//공격력 표시 + 강화시 상승량 표시(녹색)
				txt_status[0].text = item.ShortDescription; //짧은 설명문

				//무기일 경우 공격력, 방어구일 경우 체력
				if (bEquip.ItemClass == ItemClasses.Weapon)
				{
					txt_status[1].text = "ATK +" + bEquip.equipStat.ATKDamage + " (+ " + bEquip.equipStat.ATKDamage + ")";
				}
				else if (bEquip.ItemClass == ItemClasses.Armor)
				{
					txt_status[1].text = "HP +" + bEquip.equipStat.maxHp + " (+ " + bEquip.equipStat.maxHp + ")";
				}
				
				//2~5 장비 등급 별 효과 표시(해당 등급 도달 시)
				txt_status[2].text = "Rare: " + bEquip.arr_statusDescription[1];
				txt_status[3].text = "Epic: " + bEquip.arr_statusDescription[2];
				txt_status[4].text = "Legendary: " + bEquip.arr_statusDescription[3];
				txt_status[5].text = "Mythic: " + bEquip.arr_statusDescription[4]; 

			}
		}

		/// <summary>
		/// Fills the detail fields with default values.
		/// </summary>
		/// <returns>The detail fields with defaults.</returns>
		/// <param name="initialDelay">Initial delay.</param>
		protected virtual IEnumerator FillDetailFieldsWithDefaults(float initialDelay)
		{
			yield return new WaitForSeconds(initialDelay);
			//if (txt_name != null) { txt_name.text = DefaultTitle; }
			//if (txt_category != null) { txt_category.text = DefaultShortDescription; }
			//if (txt_description != null) { txt_description.text = DefaultDescription; }
			//if (txt_level != null) { txt_level.text = DefaultQuantity; }
			//if (img_icon != null) { img_icon.sprite = DefaultIcon; }
		}

		/// <summary>
		/// Catches MMInventoryEvents and displays details if needed
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			if (InventoryItem.IsNull(inventoryEvent.EventItem))
			{
				return;
			}
			if (!inventoryEvent.EventItem.IsEquippable)
			{
				return;
			}
			switch (inventoryEvent.InventoryEventType)
			{
				case MMInventoryEventType.Click:
					DisplayDetails(inventoryEvent.EventItem);
					break;
				case MMInventoryEventType.UseRequest:
					DisplayDetails(inventoryEvent.EventItem);
					break;
				case MMInventoryEventType.InventoryOpens:
					DisplayDetails(inventoryEvent.EventItem);
					break;
				case MMInventoryEventType.Drop:
					DisplayDetails(null);
					break;
				case MMInventoryEventType.EquipRequest:
					DisplayDetails(null);
					break;
			}

			if (inventoryEvent.EventItem.TargetEquipmentInventoryName == inventoryEvent.TargetInventoryName)
			{
				ChangeButton(true);
			}
			else
			{
				ChangeButton(false);
			}
		}


		/// <summary>
		/// On Enable, we start listening for MMInventoryEvents
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMInventoryEvent>();
		}

		/// <summary>
		/// On Disable, we stop listening for MMInventoryEvents
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMInventoryEvent>();
		}
	}
}