using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class UI_ModeChange : MonoBehaviour
{
    GameManager gameMgr;

    public RectTransform toggleUI;
    public Button btn_select;
    public Button btn_transform;
    public Button btn_detail;
    public Button btn_animation;


    public Button btn_toggleUI;
    bool isToggleActive = true;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    private void Start()
    {
        btn_select.onClick.AddListener(() => gameMgr.ChangeGameMode(GameMode.CHARACTER_SELECT));
        btn_transform.onClick.AddListener(() => gameMgr.ChangeGameMode(GameMode.CHARACTER_TRANSFORM));
        btn_detail.onClick.AddListener(() => gameMgr.ChangeGameMode(GameMode.DETAIL_TRANSFORM));
        btn_animation.onClick.AddListener(() => gameMgr.ChangeGameMode(GameMode.ANIMATION));

        btn_toggleUI.onClick.AddListener(ToggleUI);
    }

    void ToggleUI()
    {
        if (isToggleActive == true)
        {
            isToggleActive = false;
            toggleUI.gameObject.SetActive(isToggleActive);
        }
        else
        {
            isToggleActive = true;
            toggleUI.gameObject.SetActive(isToggleActive);
        }
    }

   public  void ButtonChangeMode(GameMode mode)
    {
        btn_select.image.sprite = gameMgr.uiMgr.sprite_blue;
        btn_transform.image.sprite = gameMgr.uiMgr.sprite_blue;
        btn_detail.image.sprite = gameMgr.uiMgr.sprite_blue;
        btn_animation.image.sprite = gameMgr.uiMgr.sprite_blue;

        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CHARACTER_SELECT:
                btn_select.image.sprite = gameMgr.uiMgr.sprite_yellow;
                break;
            case GameMode.CHARACTER_TRANSFORM:
                btn_transform.image.sprite = gameMgr.uiMgr.sprite_yellow;
                break;
            case GameMode.DETAIL_TRANSFORM:
                btn_detail.image.sprite = gameMgr.uiMgr.sprite_yellow;
                break;
            case GameMode.ANIMATION:
                btn_animation.image.sprite = gameMgr.uiMgr.sprite_yellow;
                break;
            default:
                break;
        }
    }
}
