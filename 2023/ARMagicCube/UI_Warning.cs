using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;
using UnityEngine.UI;

public class UI_Warning : MonoBehaviour
{
    public MMF_Player mmf_open;
    public MMF_Player mmf_close;

    public Button btn_close;
    public Button btn_bg;


    public void Init()
    {
        btn_bg.onClick.AddListener(Close);
        btn_close.onClick.AddListener(Close);
    }

    public void Open()
    {
        mmf_open.PlayFeedbacks();
    }
    public void Close()
    {
        mmf_close.PlayFeedbacks();
    }


}
