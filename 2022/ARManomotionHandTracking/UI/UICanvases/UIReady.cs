using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReady : MonoBehaviour
{
    public Button ready_btn_start;
    public Slider ready_slider_scale;
    public Button ready_btn_handCalibration;

    // Start is called before the first frame update
    void Start()
    {

        //ready_btn_back = ui_ready.GetChild(0).GetComponent<Button>();
        //ready_btn_start = ui_ready.GetChild(1).GetComponent<Button>();
        //ready_slider_scale = ui_ready.GetChild(3).GetComponent<Slider>();
        //ready_btn_handCalibration = ui_ready.GetChild(4).GetComponent<Button>();

        //ready_btn_start.onClick.RemoveAllListeners();
        //ready_btn_back.onClick.AddListener(() => { ReadyButtonExit(); });
        //ready_btn_start.onClick.AddListener(() => { ReadyButtonStart(); });
        //ready_slider_scale.onValueChanged.AddListener(delegate
        //{
        //    stageSize = ready_slider_scale.value;
        //    ready_slider_scale.transform.GetChild(3).GetComponent<Text>().text = "StageSize: " + stageSize;
        //});

    }
}
