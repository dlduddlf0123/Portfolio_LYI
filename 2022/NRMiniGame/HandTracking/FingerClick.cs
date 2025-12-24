using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerClick : MonoBehaviour
{
   // public GameObject finger1;
    public GameObject finger2;

    Vector3 lastGroundClickPos = Vector3.zero;
    int groundClickCount = 0;

    private void Start()
    {
//        ManomotionManager.OnManoMotionFrameProcessed += HandFingerMove;
    }

    void HandFingerMove()
    {
      //  TrackingInfo trackingInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
      ////  Vector3 _fingerPos = trackingInfo.finger_tips[1];
      //  Vector3 _fingerPos2 = trackingInfo.skeleton.joints[8];

      //  float _depth = trackingInfo.depth_estimation;

      //  float depthEstimation = Mathf.Clamp(trackingInfo.depth_estimation, 0.4f, 1);

      // // finger1.transform.position = ManoUtils.Instance.CalculateNewPosition(_fingerPos, _depth);
      //  finger2.transform.position = ManoUtils.Instance.CalculateNewPositionSkeletonJointDepth(_fingerPos2, _depth);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {

            if (lastGroundClickPos != Vector3.zero &&
                Vector3.Distance(lastGroundClickPos,finger2.transform.position) < 0.1f)
            {
                groundClickCount++;

                Debug.Log("Ground Count:" + groundClickCount);
            }
            else
            {
                groundClickCount = 0;
            }

            lastGroundClickPos = finger2.transform.position;

            if (groundClickCount > 2)
            {
                GroundClickAct();
            }
        }
    }

    void GroundClickAct()
    {
        Debug.Log("Ground Clicked!");
        groundClickCount = 0;
    }
}
