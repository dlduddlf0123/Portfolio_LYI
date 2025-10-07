using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MoreMountains.Feedbacks;

using VRTokTok.UI;

namespace VRTokTok.Manager
{

    /// <summary>
    /// 10/25/2023-LYI
    /// 테이블 내의 동작 관리자
    /// 테이블 돌아가는 로직
    /// 스테이지 셀렉트
    /// 옵션 관련 버튼 관리
    /// </summary>
    public class TableManager : MonoBehaviour
    {
        GameManager gameMgr;

        public UI_Table ui_table;
        public UI_Select ui_select;
        public UI_Menu ui_menu;
        public GameObject ui_loading;

        public PlayDockingTable playTable;

        public GrabConstraints tableGrabHandle;

        [Header("Passthrough")]
        public GameObject passThroughObjects;
        public Material mat_skybox;

        [Header("Table Interactables")]
        public GameObject tableInteractable;

        //[Header("Properties")]
        //public StageType lastStageType = StageType.NONE;
        //public int lastStageNum = 0;

        //public StageType currentStageType = StageType.NONE;
        //public int currentStageNum = 0;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        /// <summary>
        /// 6/18/2024-LYI
        /// Addressable 로드 이후 호출
        /// Table 매니저 초기화 및 하위 클래스 관련 초기화
        /// </summary>
        public void TableElementsInit()
        {
            LoadTableTransform();

            ui_table.TableUIInit();
            ui_menu.MenuUIInit();

            playTable.PlayTableInit();
        }


        public void SaveTableTransform()
        {
            ES3.Save(Constants.ES3.TABLE_POSITION, transform.localPosition);
            ES3.Save(Constants.ES3.TABLE_SCALE, transform.localScale);
        }
        public void LoadTableTransform()
        {
            transform.localPosition = ES3.Load(Constants.ES3.TABLE_POSITION, Vector3.zero);
            transform.localScale = ES3.Load(Constants.ES3.TABLE_SCALE, Vector3.one);
            tableInteractable.transform.position = transform.localPosition;
            tableInteractable.transform.localScale = transform.localScale;

        }

        /// <summary>
        /// 10/25/2023-LYI
        /// 현재 게임 상태에 따라 테이블 활성화 변경
        /// </summary>
        public void ChangeTableUIStatus()
        {
            switch (gameMgr.statGame)
            {
                case GameStatus.MENU:
                    //플레이 버튼 활성화, 스테이지 선택 활성화
                    ui_table.SetLeftTableMenuButton(false);

                    ui_menu.gameObject.SetActive(true);
                    ui_menu.MenuUIInit();
                    ui_menu.MenuFade(true);
                   // ui_select.gameObject.SetActive(true);
                    //ui_select.SelectInit();
                    
                    ui_loading.gameObject.SetActive(false);

                    //6/26/2024-LYI
                    //마지막 스테이지 클리어 하고 메뉴 호출 시 크레딧 보여주기
                    if (gameMgr.playMgr.IsGameAllCleared())
                    {
                        ui_menu.OpenCredit();
                    }
                    break;
                case GameStatus.LOADING:
                    ui_table.SetLeftTableMenuButton(false);

                    if (ui_menu.gameObject.activeSelf)
                    {
                        ui_menu.MenuFade(false);
                    }

                    // ui_select.gameObject.SetActive(false);
                    ui_loading.gameObject.SetActive(true);
                    break;
                case GameStatus.GAME:
                default:
                    ui_table.SetLeftTableMenuButton(true);

                    if (ui_menu.gameObject.activeSelf)
                    {
                        ui_menu.MenuFade(false);
                    }
                   // ui_menu.gameObject.SetActive(false);
                    //  ui_select.gameObject.SetActive(false);
                    ui_loading.gameObject.SetActive(false);
                    break;
            }

        }


        /// <summary>
        /// 10/25/2023-LYI
        /// Call in SelectChecker
        /// 현재 선택된 스테이지가 변경될 때 마다 호출
        /// </summary>
        /// <param name="num"></param>
        public void ChangeSelectStage(int num)
        {
            if (num < 1000)
            {
                return;
            }

            //currentStageNum = num % 1000;
            //currentStageType = (StageType)((int)(num / 1000));

           // ui_table.ChangeStageText(currentStageType.ToString() + " " + currentStageNum.ToString());
        }


        /// <summary>
        /// 10/25/2023-LYI
        /// Call in gamestart button
        /// 게임 시작 시 처리할 것
        /// </summary>
        public void OnTablePlay()
        {
            //선택이 안됐으면 돌리기
            //if (currentStageNum == 0 ||
            //    currentStageType == StageType.NONE)
            //{
            //    Debug.Log("Theres no selected stage");
            //}
            //else
            //{
            //    ES3.Save(Constants.ES3.LAST_STAGE_TYPE, currentStageType);
            //    ES3.Save(Constants.ES3.LAST_STAGE_NUM, currentStageNum);

            //    gameMgr.ChangeGameStat(GameStatus.LOADING);

            //    //게임 시작
            //    int stage = (int)currentStageType * 1000 + currentStageNum;
            //    gameMgr.GameStart(stage);
            //}


        }


        /// <summary>
        /// 10/26/2023-LYI
        /// 게임 종료 후 메뉴로 돌아올 시 처리할 것
        /// </summary>
        public void OnTableMenu()
        {
            playTable.SetTableMenu();
        }

        private void OnApplicationQuit()
        {
            SaveTableTransform();
        }
    }
}