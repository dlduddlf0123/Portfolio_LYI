using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VRTokTok.UI
{
    /// <summary>
    /// 현재 활성화 된 UI 관리
    /// 현재는 필요x
    /// </summary>
    public enum UIWindow
    {
        TITLE,
        READY,
        MAIN,
        GAME,
        SETTING,
        SELECT,
        WARNING,
    }

    /// <summary>
    /// UI 관리 클래스
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        GameManager gameMgr;

        public UI_Game ui_game;
        public UI_Pause ui_pause;

        public int debugStageNum = 0;
        public bool isSkipInEditor = true;


        //public RectTransform ui_select;
        //public RectTransform ui_payment;
        //public RectTransform ui_warning;

        //public UIWindow ui_currentWindow;
        //public List<UIWindow> list_lastWindow = new List<UIWindow>();

        //public GameObject stageSelect;
        //public GameObject shadowPlane;

        //WarningUI
        //public Button warning_btn_start { get; set; }


        Coroutine ui_currentCoroutine = null;

        /// <summary>
        /// UI 오브젝트들의 초기화, 할당 등
        /// </summary>
        private void Awake()
        {
            gameMgr = GameManager.Instance;


        }


        /// <summary>
        /// UI 오브젝트들의 기능 부여
        /// </summary>
        // Use this for initialization
        void Start()
        {

        }

        /// <summary>
        /// 뒤로가기 버튼 기능
        /// </summary>
        //public void CallLastWindow()
        //{
        //    SetUIActive(list_lastWindow[list_lastWindow.Count - 1], true);
        //    list_lastWindow.RemoveAt(list_lastWindow.Count - 1);
        //}

        ////각 장면에 해당하는 UI 활성화 함수
        //public void SetUIActive(UIWindow _sceneUI, bool _isFade = true, bool _isBack = false)
        //{
        //    if (_isFade)
        //    {
        //        gameMgr.fade.StartFadeMiddleEnd(() => { UIActive(_sceneUI, _isBack); });
        //    }
        //    else
        //    {
        //        UIActive(_sceneUI, _isBack);
        //    }
        //}

        //void UIActive(UIWindow sceneUI, bool isBack)
        //{

        //    if (!isBack)
        //    {
        //        list_lastWindow.Add(ui_currentWindow);
        //        if (list_lastWindow.Count > 10)
        //        {
        //            list_lastWindow.RemoveAt(0);
        //        }
        //    }

        //    ui_currentWindow = sceneUI;
        //}

    }
}