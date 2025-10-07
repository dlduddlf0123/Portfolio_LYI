using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Oculus.Interaction;


namespace VRTokTok.UI {


    /// <summary>
    /// 2/28/2024-LYI
    /// Menu UI
    /// 각 스테이지 선택에 사용되는 버튼
    /// </summary>
    public class MenuStageNumberButton : MonoBehaviour
    {
        public enum ButtonState
        {
            NORMAL = 0,
            CLEAR,
            LOCK,
        }

        GameManager gameMgr;
        PokeInteractable interactable;

        [Tooltip("0: normal / 1: clear")]
        public Sprite[] arr_btnSprite;

        public Button btn_select;
        public TextMeshProUGUI txt_number;
        public GameObject objLock;

        public Image  img_selected;
        public TextMeshProUGUI txt_selectNumber;

        public ButtonState statButton = ButtonState.LOCK;
        public bool isClear = false;
        public int stageNum = 0;

        public bool isSelect = false;

        bool isFirst = true;

        /// <summary>
        /// 2/28/2024-LYI
        /// 초기화 함수
        /// 버튼 생성 시 호출
        /// </summary>
        public void Init()
        {
            if (isFirst)
            {
                gameMgr = GameManager.Instance;
                interactable = GetComponent<PokeInteractable>();
            }

            //동적 생성된 오브젝트 Pointable Elelment 할당
            interactable.InjectOptionalPointableElement(
                gameMgr.tableMgr.ui_menu.menuPointableCanvas.GetComponent<IPointableElement>());
        }


        /// <summary>
        /// 2/28/2024-LYI
        /// 버튼 상태 변경
        /// 0: normal 1: clear 2: lock
        /// </summary>
        /// <param name="state"></param>
        public void SetButtonState(ButtonState state)
        {
            statButton = state;


            objLock.SetActive(false);
            btn_select.gameObject.SetActive(false);

            switch (state)
            {
                case ButtonState.NORMAL:
                    btn_select.image.sprite = arr_btnSprite[0];
                    img_selected.sprite = arr_btnSprite[0];
                    btn_select.gameObject.SetActive(true);
                    break;
                case ButtonState.CLEAR:
                    btn_select.image.sprite = arr_btnSprite[1];
                    img_selected.sprite = arr_btnSprite[1];
                    btn_select.gameObject.SetActive(true);
                    break;
                case ButtonState.LOCK:
                    btn_select.image.sprite = arr_btnSprite[0];
                    img_selected.sprite = arr_btnSprite[0];
                    objLock.SetActive(true);
                    break;
                default:
                    btn_select.image.sprite = arr_btnSprite[0];
                    img_selected.sprite = arr_btnSprite[0];
                    btn_select.gameObject.SetActive(true);
                    break;
            }
        }

        public void CheckClear()
        {
            isClear = ES3.Load<bool>(stageNum.ToString(), false);
        }

        public void ButtonSelect()
        {
            if (isSelect)
            {
                return;
            }
            isSelect = true;

            objLock.SetActive(false);
            btn_select.gameObject.SetActive(false);

            img_selected.gameObject.SetActive(true);
        }

        public void ButtonDeselect()
        {
            if (!isSelect)
            {
                return;
            }
            isSelect = false;

            img_selected.gameObject.SetActive(false);
            SetButtonState(statButton);
        }



    }
}