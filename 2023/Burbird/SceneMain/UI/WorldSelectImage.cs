using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Burbird
{
    public class WorldSelectImage : MonoBehaviour
    {
        public StageData stageData;

        Image img_world;

        Text txt_name;
        Text txt_maxRoom;

        Button btn_select;


        private void Awake()
        {
            img_world = GetComponent<Image>();

            txt_name = transform.GetChild(0).GetComponent<Text>();
            txt_maxRoom = transform.GetChild(1).GetComponent<Text>();
            btn_select = transform.GetChild(2).GetComponent<Button>();
        }


        public void SetWorldSelectImage(StageData data)
        {
            stageData = data;
            txt_name.text = stageData.stageNum + ". " + stageData.stageName;
            txt_maxRoom.text = "Rooms: " + stageData.maxRoom;

            btn_select.onClick.AddListener(ButtonSelect);
        }

        void ButtonSelect()
        {
            GameManager gameMgr = GameManager.Instance;

            gameMgr.playStageData = stageData;
            ES3.Save("CurrentStageNum", stageData.stageNum);
            gameMgr.uiMgr.ui_world.SetStageUI(stageData);
            gameMgr.uiMgr.SetUIActive(UIWindow.WORLD, false);
        }


    }
}