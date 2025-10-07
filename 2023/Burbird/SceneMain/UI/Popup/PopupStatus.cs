using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Burbird
{

    public class PopupStatus : MonoBehaviour
    {
        GameManager gameMgr;

        Text[] arr_txt_status;
        Button btn_close;
        private void Awake()
        {
            gameMgr = GameManager.Instance;

            arr_txt_status = transform.GetChild(1).GetComponentsInChildren<Text>();
            btn_close = transform.GetChild(2).GetComponent<Button>();
        }
        void Start()
        {
            btn_close.onClick.AddListener(gameMgr.uiMgr.ui_equip.ClosePopup);
            RefreshStatusPopupText();
        }

        public void RefreshStatusPopupText()
        {
            arr_txt_status[0].text = "ATK: " + gameMgr.dataMgr.playerStat.ATKDamage;
            arr_txt_status[1].text = "SPD: " + gameMgr.dataMgr.playerStat.ATKSpeed;
            arr_txt_status[2].text = "HP: " + gameMgr.dataMgr.playerStat.maxHp;
            arr_txt_status[3].text = "AvoidChance: " + gameMgr.dataMgr.playerStat.avoidChance;
            arr_txt_status[4].text = "CritChance: " + gameMgr.dataMgr.playerStat.critChance;
            arr_txt_status[5].text = "CritDamage: " + gameMgr.dataMgr.playerStat.critDamage;
        }
    }
}