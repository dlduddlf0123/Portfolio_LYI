using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ManoHandMove : MonoBehaviour
{
    public ContourGizmo contourGizmo;

    public GameObject palmCenter;
    public GameObject finger_index;
    public GameObject finger_thumb;

    /// <summary>
    /// 0:Palm,1:Index,2:Thumb
    /// </summary>
    public HandFollower[] arr_handFollwer;

    public Text textGesture;
    public Text textSide;
    public Text textWarning;

    public Slider depth;
    public Text txt_depth;

    Camera mainCam;

    public Vector3 handPos = Vector3.zero;

   public  HandSide handSide;
    public ManoGestureContinuous handGestureContinuous;
    public ManoGestureTrigger handGestureTrigger;

    public float handDepth = 0f;
    public bool isPinch = false;
    public bool isTracking = false;

    public bool isContour = true;

    private void Start()
    {
        //ManomotionManager.OnManoMotionFrameProcessed += HandCollMove;
        mainCam = GameManager.Instance.arMainCamera;
        depth.value = handDepth;
        txt_depth.text = "HandDepth: " + handDepth;
        depth.onValueChanged.AddListener((float _value) =>
        {
            handDepth = _value;
            txt_depth.text = "HandDepth: " + handDepth;
        });

       // ManomotionManager.Instance.ShouldRunContour(true);
    }


    void Update()
    {
        TrackingInfo trackingInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
        GestureInfo gestureInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
        Warning warning = ManomotionManager.Instance.Hand_infos[0].hand_info.warning;


        Vector3 _palmCenter = trackingInfo.palm_center;

        float _depth = trackingInfo.depth_estimation;
        float _depthEstimation = Mathf.Clamp(trackingInfo.depth_estimation, 0.4f, 1);

#if !UNITY_EDITOR
        handGestureContinuous = gestureInfo.mano_gesture_continuous;
        handGestureTrigger = gestureInfo.mano_gesture_trigger;
        handSide = gestureInfo.hand_side;

        palmCenter.transform.position = ManoUtils.Instance.CalculateNewPositionDepth(_palmCenter + handPos, _depth + handDepth);
        
#endif
        //if (isContour)
        //{
        //    contourGizmo.ShowContour();
        //}

        if (trackingInfo.skeleton.confidence > 0)
        {
            Vector3 _indexFingerPos = trackingInfo.skeleton.joints[8];
            Vector3 _thumbFingerPos = trackingInfo.skeleton.joints[4];
            finger_index.transform.position = ManoUtils.Instance.CalculateNewPositionSkeletonJointDepth(_indexFingerPos, _depth + handDepth);
            finger_thumb.transform.position = ManoUtils.Instance.CalculateNewPositionSkeletonJointDepth(_thumbFingerPos, _depth + handDepth);
        }

        if (warning != Warning.WARNING_HAND_NOT_FOUND)
        {
            if (!arr_handFollwer[0].gameObject.activeSelf)
            {
                for (int i = 0; i < arr_handFollwer.Length; i++)
                {
                    arr_handFollwer[i].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (arr_handFollwer[0].gameObject.activeSelf)
            {

                for (int i = 0; i < arr_handFollwer.Length; i++)
                {
                    arr_handFollwer[i].gameObject.SetActive(false);
                }
            }
        }


        if (textGesture != null)
        {
            textGesture.text = "Gesture:" + gestureInfo.mano_gesture_continuous.ToString() + "\n Pinch:" + isPinch;
        }

        if (textSide != null)
        {
            textSide.text = "Hand Side:" + gestureInfo.hand_side;
        }

        if (textWarning != null)
        {
            textWarning.text = "Warning:" + warning;
        }
    }
}
