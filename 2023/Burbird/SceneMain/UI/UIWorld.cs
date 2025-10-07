using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Burbird
{
    /// <summary>
    /// 월드 UI, 게임을 켰을 때 가장 먼저 보이는 화면
    /// 스테이지 선택, 게임 시작 기능
    /// 플레이어 정보, 퀘스트, 우편, 공지 확인 기능 포함
    /// BM 관련 팝업 표시
    /// </summary>
    public class UIWorld : MonoBehaviour
    { 
        GameManager gameMgr;

        Button btn_player; //계정 버튼, 계정 정보 팝업, 닉네임 등 표시,;
        Text txt_playerLevel;

        Button btn_worldSelect; //월드 선택 버튼
        Text txt_worldName;
        Text txt_worldRoom;

        Button btn_gamePlay; //현재 월드 게임 시작 버튼
        Text txt_playStemina;
        private void Awake()
        {
            gameMgr = GameManager.Instance;

            btn_player = transform.GetChild(0).GetComponent<Button>();
            txt_playerLevel = btn_player.transform.GetChild(0).GetComponent<Text>();

            btn_worldSelect = transform.GetChild(1).GetComponent<Button>();
            txt_worldName = btn_worldSelect.transform.GetChild(0).GetComponent<Text>();
            txt_worldRoom = btn_worldSelect.transform.GetChild(1).GetComponent<Text>();

            btn_gamePlay = transform.GetChild(3).GetComponent<Button>();
            txt_playStemina = btn_gamePlay.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        }
        void Start()
        {
            btn_worldSelect.onClick.AddListener(()=>gameMgr.uiMgr.SetUIActive(UIWindow.WORLDSELECT, false));
            btn_gamePlay.onClick.AddListener(ButtonGameStart);

            RefreshWorldUI();
        }

        public void RefreshWorldUI()
        {
            SetStageUI(gameMgr.playStageData);
            TextChangePlayerLevel(gameMgr.dataMgr.PlayerLevel.ToString()) ;
        }


        public void TextChangePlayerLevel(string s)
        {
            txt_playerLevel.text = s; 
        }

        public void SetStageUI(StageData stage)
        {
            if (gameMgr.addressMgr.isLoadComplete == false)
            {
                return;
            }
            txt_worldName.text = stage.stageNum + ". " + stage.stageName;
            txt_worldRoom.text = "Top room cleared: " + stage.clearedRoom +" / "+ stage.maxRoom;

            txt_playStemina.text = stage.steminaForPlay.ToString();
         }


        /// <summary>
        /// !!!!게임 시작 버튼!!!!
        /// </summary>
        public void ButtonGameStart()
        {
            gameMgr.GameStart();
        }
    }
}