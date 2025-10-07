using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    GameManager gameMgr;

    public Button menu_btn_close;

    //MenuTitleUI
    GameObject menu_titleUI;
    Button title_btn_setting;
    Button title_btn_exit;

    GameObject menu_settingUI;


    private void Awake()
    {
        gameMgr = GameManager.Instance;

        menu_titleUI = transform.GetChild(0).gameObject;
        title_btn_setting = menu_titleUI.transform.GetChild(1).GetComponent<Button>();
        title_btn_exit = menu_titleUI.transform.GetChild(2).GetComponent<Button>();

        menu_settingUI = transform.GetChild(1).gameObject;
    }

    private void OnEnable()
    {
        Vector3 pos = gameMgr.mainCam.transform.position + gameMgr.mainCam.transform.forward;
        transform.position = new Vector3(pos.x, 1, pos.z);
        transform.rotation = Quaternion.LookRotation(transform.position - gameMgr.mainCam.transform.position);
    }

    private void OnDisable()
    {

    }

}
