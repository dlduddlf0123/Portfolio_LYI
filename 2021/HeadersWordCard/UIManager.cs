using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ReadOnly; //custum namespace

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

    public int debugStageNum = 0;
    public bool isSkipInEditor = true;


    //Additional Canvas Objects
    public Fade fadeCanvas;
    public Canvas stageTitleCanvas; //When stage starts, Fading title UI display
    Text txt_stageTitle_title;
    Text txt_stageTitle_sub;

    //public RectTransform ui_select;
    //public RectTransform ui_payment;
    public RectTransform ui_warning;

    public UIWindow ui_currentWindow;
    public List<UIWindow> list_lastWindow = new List<UIWindow>();

    public GameObject stageSelect;
    public GameObject shadowPlane;

    //WarningUI
    public Button warning_btn_start { get; set; }


    Coroutine ui_currentCoroutine = null;

    public float stageSize = 1f;
    public int select_episodeNum = 0;

    /// <summary>
    /// UI 오브젝트들의 초기화, 할당 등
    /// </summary>
    private void Awake()
    {
        gameMgr = GameManager.Instance;

        //txt_stageTitle_title = stageTitleCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        //txt_stageTitle_sub = stageTitleCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>();

    }


    /// <summary>
    /// UI 오브젝트들의 기능 부여
    /// </summary>
    // Use this for initialization
    void Start()
    {
        List<Button[]> list_button = new List<Button[]>();

        for (int num = 0; num < transform.childCount; num++)
        {
            list_button.Add(transform.GetChild(num).GetComponentsInChildren<Button>());
        }

        foreach (var btn in list_button)
        {
            for (int num = 0; num < btn.Length; num++)
            {
                Button _btn = btn[num];
                btn[num].onClick.AddListener(() =>
                {
                    _btn.interactable = false;
                    //StartCoroutine(gameMgr.LateFunc(() => _btn.interactable = true,0.5f));
                    //gameMgr.soundMgr.PlaySfx(transform, Defines.SOUND_SFX_SELECT, 1, 0);
                });
            }
        }

    }

    /// <summary>
    /// 뒤로가기 버튼 기능
    /// </summary>
    public void CallLastWindow()
    {
        SetUIActive(list_lastWindow[list_lastWindow.Count - 1], true);
        list_lastWindow.RemoveAt(list_lastWindow.Count - 1);
    }

    //각 장면에 해당하는 UI 활성화 함수
    public void SetUIActive(UIWindow _sceneUI, bool _isFade = true, bool _isBack = false)
    {
        if (_isFade)
        {
            fadeCanvas.StartFade(() => { UIActive(_sceneUI, _isBack); });
        }
        else
        {
            UIActive(_sceneUI, _isBack);
        }
    }

    void UIActive(UIWindow _sceneUI, bool _isBack)
    {

        if (!_isBack)
        {
            list_lastWindow.Add(ui_currentWindow);
            if (list_lastWindow.Count > 10)
            {
                list_lastWindow.RemoveAt(0);
            }
        }

        ui_currentWindow = _sceneUI;
    }

}
