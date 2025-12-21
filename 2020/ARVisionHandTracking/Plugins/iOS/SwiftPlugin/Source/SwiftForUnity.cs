
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwiftForUnity : MonoBehaviour
{
    enum DetectionStatus
    {
        None = 0,
        Detecting = 1,
        Detected = 2
    }

    [DllImport("__Internal")]
    private static extern bool SwiftUnity_StartDetect(IntPtr buffer);


    private DetectionStatus m_DetectionStatus = DetectionStatus.None;

    public Text text;

    /// <summary>
    /// 손이 인식 됐을 때의 콜백
    /// </summary>
    public event EventHandler<Vector2> OnHandDetected;


    public void StartDetect(IntPtr buffer)
    {
        if (buffer == IntPtr.Zero)
        {
            Debug.LogError("buffer is NULL!");
            return;
        }

        bool success = SwiftUnity_StartDetect(buffer);

        if (success)
        {
            m_DetectionStatus = DetectionStatus.Detecting;
        }
    }

    private void OnHandDetecedFromNative(string data)
    {
        m_DetectionStatus = DetectionStatus.Detected;
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        string[] temp = data.Split(',');

        if (temp.Length != 2)
        {
            return;
        }

        float x, y = 0.0f;

        if (!Single.TryParse(temp[0], out x) || !Single.TryParse(temp[1], out y))
        {
            return;
        }

        Vector2 vPortSpacePos = new Vector2(x, y);
        text.text = "HandPosition: " + x + ":" + y;
        if (OnHandDetected != null)
        {
            OnHandDetected(this, vPortSpacePos);
        }
    }

    public bool IsIdle
    {
        get => m_DetectionStatus != DetectionStatus.Detecting;
    }

    ///// <summary>
    ///// 손 인식이 완료되었을 때 작동
    ///// </summary>
    ///// <param name="data"></param>
    //private void OnHandRecognitionComplete(string data)
    //{
    //    // Remove classification from the ongoing requests indicator
    //    _requestsInProgress &= ~VisionRequest.HandDetection;


    //    // Handle errors
    //    if (string.IsNullOrEmpty(data))
    //    {
    //        return;
    //    }

    //    string[] temp = data.Split(',');

    //    if (temp.Length != 3)
    //    {
    //        return;
    //    }


    //    float x = 0.0f;
    //    float y = 0.0f;
    //    string name = null;

    //    if (!Single.TryParse(temp[0], out x) || !Single.TryParse(temp[1], out y))
    //    {
    //        return;
    //    }

    //    name = temp[2];


    //    // GameManager.Instance.handCtrl.OnHandTracking(new Vector2(x, y), name);


    //    //If anyone is interested in the results
    //    if (OnHandRecognized != null)
    //    {
    //        //var length = _vision_acquireHandBuffer(_classificationBuffer, _maxObservations);
    //        // if (length < 1) return;

    //        // Notify listeners
    //        OnHandRecognized(this, new HandRecognizedArgs(new Vector2(x, y)));

    //    }
    //}

}
