using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NRKernal; //NReal SDK

public enum FingerIndex
{
    PALM = 0,
    INDEX = 1,
    THUMB = 2,
}

public class NRHandMove : MonoBehaviour
{
    public NRHand nrHandMgr;
    HandState nrHandState;

    public GameObject palmCenter { get; set; }
    public GameObject finger_index { get; set; }
    public GameObject finger_thumb { get; set; }

    /// <summary>
    /// 0:Palm,1:Index,2:Thumb
    /// </summary>
    public HandFollower[] arr_handFollwer { get; set; }


    public Vector3 handPos = Vector3.zero;

//   public  HandSide handSide;

    public float handDepth = 0f;
    public bool isPinch = false;
    public bool isTracking = false;

    private void Awake()
    {
        palmCenter = transform.GetChild(0).GetChild(0).gameObject;
        finger_index = transform.GetChild(0).GetChild(1).gameObject;
        finger_thumb = transform.GetChild(0).GetChild(2).gameObject;

        arr_handFollwer = new HandFollower[3];
        for (int i = 0; i < 3; i++)
        {
            arr_handFollwer[i] = transform.GetChild(1).GetChild(i).GetComponent<HandFollower>();
        }

        nrHandState = nrHandMgr.GetHandState();
    }

    private void Start()
    {


    }


    void Update()
    {
        isPinch= nrHandState.isPinching ;
        isTracking = nrHandState.isTracked;

        palmCenter.transform.position = nrHandState.GetJointPose(HandJointID.Palm).position;
        palmCenter.transform.rotation = nrHandState.GetJointPose(HandJointID.Palm).rotation;
        finger_index.transform.position = nrHandState.GetJointPose(HandJointID.IndexTip).position;
        finger_index.transform.rotation = nrHandState.GetJointPose(HandJointID.IndexTip).rotation;
        finger_thumb.transform.position = nrHandState.GetJointPose(HandJointID.ThumbTip).position;
        finger_thumb.transform.rotation = nrHandState.GetJointPose(HandJointID.ThumbTip).rotation;
    }

    /// <summary>
    /// toggle hand raycast 
    /// </summary>
    /// <param name="_isActive"></param>
    /// <param name="_handNum">0:Palm,1:Index,2:Thumb</param>
    public void HandRayToggle(bool _isActive, FingerIndex _handNum = FingerIndex.INDEX)
    {

    }

}
