using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    public Button pause_btn_resume;
    public Button pause_btn_restart;
    public Button pause_btn_exit;

    // Start is called before the first frame update
    void Start()
    {

        ////PauseUI
        //pause_btn_resume = ui_pause.GetChild(1).GetComponent<Button>();
        //pause_btn_restart = ui_pause.GetChild(2).GetComponent<Button>();
        //pause_btn_exit = ui_pause.GetChild(3).GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
