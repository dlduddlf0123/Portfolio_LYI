using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandCalibration : MonoBehaviour
{
    public Button calibration_btn_close;
    public Slider calibration_slider_handDepth;
    public Slider calibration_slider_right;
    public Slider calibration_slider_front;
    public Image calibration_img_center;

    // Start is called before the first frame update
    void Start()
    {
        //calibration_btn_close = ui_handCalibration.GetChild(0).GetComponent<Button>();
        //calibration_slider_handDepth = ui_handCalibration.GetChild(1).GetComponent<Slider>();
        //calibration_slider_right = ui_handCalibration.GetChild(2).GetComponent<Slider>();
        //calibration_slider_front = ui_handCalibration.GetChild(3).GetComponent<Slider>();
        //calibration_img_center = ui_handCalibration.GetChild(4).GetComponent<Image>();
    }
}
