using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using MoreMountains.Feedbacks;

/// <summary>
/// 3/14/2024-LYI
/// 각종 팝업 메시지 출력하는 창
/// </summary>
public class UI_Popup : MonoBehaviour
{
    GameManager gameMgr;

    [Header("UI")]
    [SerializeField]
    Image popup_bg;

    [SerializeField]
    TextMeshProUGUI txt_title;
    [SerializeField]
    TextMeshProUGUI txt_message;
    [SerializeField]
    TextMeshProUGUI txt_info;

    [SerializeField]
    Button btn_blackBG;
    [SerializeField]
    Button btn_confirm;
    [SerializeField]
    Button btn_cancle;

    [Header("Effect")]
    [SerializeField]
    MMF_Player mmf_open;
    [SerializeField]
    MMF_Player mmf_close;


    [Header("Internet Warning")]
    [SerializeField]
    GameObject popup_warning;
    [SerializeField]
    Button btn_warningConfirm;
    [SerializeField]
    MMF_Player mmf_openWarning;
    [SerializeField]
    MMF_Player mmf_closeWarning;


    public bool isPopupActive = false;
    public bool isWarningActive = false;


    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    public void Init()
    {
        btn_blackBG.onClick.AddListener(ButtonCancle);
        btn_cancle.onClick.AddListener(ButtonCancle);
        btn_warningConfirm.onClick.AddListener(ButtonWarningConfirm);

        mmf_close.Events.OnComplete.AddListener(() => {
            btn_blackBG.gameObject.SetActive(false);
            popup_bg.gameObject.SetActive(false);
        });
    }


    /// <summary>
    /// 3/14/2024-LYI
    /// 팝업 활성화
    /// </summary>
    /// <param name="downloadByte"></param>
    /// <param name="actionConfirm"></param>
    public void PopupMessage(string title, int downloadByte, UnityAction actionConfirm = null, UnityAction actionCancle = null)
    {
        if (isPopupActive)
        {
            return;
        }
        isPopupActive = true;

        Debug.Log("UI_Popup: Active" + isPopupActive);

        popup_warning.SetActive(false);
        txt_title.text = title;
        btn_blackBG.gameObject.SetActive(true);
        popup_bg.gameObject.SetActive(true);

        if (downloadByte == 0)
        {
            txt_info.gameObject.SetActive(false);
        }
        else
        {
            txt_info.gameObject.SetActive(true);
            txt_info.text = "다운로드 용량: " + downloadByte + "MB";
        }

        //버튼 기능들 리셋
        btn_confirm.onClick.RemoveAllListeners();
        btn_confirm.onClick.AddListener(ButtonCancle);

        if (actionConfirm != null)
        {
            btn_confirm.onClick.AddListener(actionConfirm);
        }

        btn_blackBG.onClick.RemoveAllListeners();
        btn_cancle.onClick.RemoveAllListeners();

        btn_blackBG.onClick.AddListener(ButtonCancle);
        btn_cancle.onClick.AddListener(ButtonCancle);

        if (actionCancle != null)
        {
            btn_blackBG.onClick.AddListener(actionCancle);
            btn_cancle.onClick.AddListener(actionCancle);
        }

        mmf_open.PlayFeedbacks();
    }


    public void ButtonCancle()
    {
        if (!isPopupActive)
        {
            return;
        }
        isPopupActive = false;

        Debug.Log("UI_Popup: Cancle" + isPopupActive);

        gameMgr.soundMgr.PlaySfx(Vector3.zero, gameMgr.ui_librarySelect.sfx_click);
        mmf_close.PlayFeedbacks();
    }

    public void OpenWarningPopup()
    {
        if (isWarningActive)
        {
            return;
        }
        isWarningActive = true;

        popup_bg.gameObject.SetActive(false);
        popup_warning.SetActive(true);
        mmf_openWarning.PlayFeedbacks();

    }

    public void ButtonWarningConfirm()
    {
        isWarningActive = false;
        mmf_closeWarning.PlayFeedbacks();

        if (!gameMgr.addressableMgr.isLoadComplete)
        {
            Application.Quit();
        }
    }

    public void CheckUIPopupActive()
    {
        isPopupActive = btn_blackBG.gameObject.activeSelf;
    }
}
