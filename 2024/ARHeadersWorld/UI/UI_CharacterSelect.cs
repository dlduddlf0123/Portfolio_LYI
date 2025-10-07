using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class UI_CharacterSelect : MonoBehaviour
{
    GameManager gameMgr;

    public Button[] arr_btnSelect;

    public Button btn_confirm;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }


    void Start()
    {
        if (arr_btnSelect != null &&
            arr_btnSelect.Length > 0)
        {
            for (int i = 0; i < arr_btnSelect.Length; i++)
            {
                int a;
                a = i;
                arr_btnSelect[a].onClick.AddListener(() => ButtonSelect(a));
            }
        }

        btn_confirm.onClick.AddListener(ButtonConfirm);
    }

    private void OnEnable()
    {
        SelectUIInit();
    }

    public void SelectUIInit()
    {
        if (gameMgr.selectARCharacter == null)
        {
            ChangeButtonSprite(99);
        }
        else
        {
            ChangeButtonSprite((int)gameMgr.selectARCharacter.typeHeader - 1);
        }
    }


    void ButtonConfirm()
    {
        gameMgr.ChangeGameMode(GameMode.CHARACTER_TRANSFORM);
    }

    void ButtonSelect(int num)
    {
        gameMgr.SetARObject(num);
        ChangeButtonSprite(num);
    }

    void ChangeButtonSprite(int selectNum)
    {
        for (int i = 0; i < arr_btnSelect.Length; i++)
        {
            arr_btnSelect[i].image.sprite = gameMgr.uiMgr.sprite_yellow;
        }

        if (selectNum <= arr_btnSelect.Length)
        {
            arr_btnSelect[selectNum].image.sprite = gameMgr.uiMgr.sprite_blue;
        }
    }

}
