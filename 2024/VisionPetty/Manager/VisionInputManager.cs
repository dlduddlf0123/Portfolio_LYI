using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.PolySpatial.InputDevices;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

namespace AroundEffect
{
    public enum FingerIndex
    {
        THUMB = 0,
        INDEX = 1,
        MIDDLE = 2,
        RING = 3,
        LITTLE = 4,
    }

    /// <summary>
    /// 8/22/2024-LYI
    /// 손동작 상태 정의
    /// </summary>
    public enum HandGestureState
    {
        NONE = 0,
        FIVE,
        FIST,
        PINCH,
        POINT,
    }

    /// <summary>
    /// 8/8/2024-LYI
    /// manage inputs
    /// </summary>
    public class VisionInputManager : MonoBehaviour
    {
        GameManager gameMgr;

        public XRHandGestureInput handInputL;
        public XRHandGestureInput handInputR;

        public VisionPolySpatialInput polySpatialInput;

        //get unity XRHand 
        XRHandSubsystem m_HandSubsystem;
        XRHandJoint[] arr_leftJoint;
        XRHandJoint[] arr_rightJoint;

        private Vector3[] arr_leftTipPos;
        private Vector3[] arr_rightTipPos;
        public Vector3[] Arr_leftTipPos {get {return arr_leftTipPos;}}
        public Vector3[] Arr_rightTipPos { get { return arr_rightTipPos; } }

        public HandGestureState leftGesture = HandGestureState.NONE;
        public HandGestureState rightGesture = HandGestureState.NONE;


        [Header("Properties")]
        float m_ScaledThreshold;

        const float k_PinchThreshold = 0.02f;


        //Pinch 1~4
        public bool[] arr_isLeftPinch = new bool[4];
        public bool[] arr_isRightPinch = new bool[4];


        private void Awake()
        {
            VisionInputInit();
        }

        void OnEnable()
        {
            // enable enhanced touch support to use active touches for properly pooling input phases
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {

        }


        public void VisionInputInit()
        {
            gameMgr = GameManager.Instance;

            GetHandSubsystem();

            arr_leftJoint = new XRHandJoint[5];
            arr_rightJoint = new XRHandJoint[5];

            arr_leftTipPos = new Vector3[5];
            arr_rightTipPos = new Vector3[5];


            m_ScaledThreshold = k_PinchThreshold / gameMgr.MRMgr.VolumeCamera.transform.localScale.x;
        }

        void GetHandSubsystem()
        {
            var xrGeneralSettings = XRGeneralSettings.Instance;
            if (xrGeneralSettings == null)
            {
                Debug.LogError("XR general settings not set");
            }

            var manager = xrGeneralSettings.Manager;
            if (manager != null)
            {
                var loader = manager.activeLoader;
                if (loader != null)
                {
                    m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                    //if (!CheckHandSubsystem())
                    //    return;

                    m_HandSubsystem.Start();
                }
            }

        }



        void Update()
        {
            //조건식이 Bounded인 경우와 아닌경우 구분
            //if (GameManager.Instance.)
            //{

            //}
            UpdateBoundedInput();
            UpdateUnboundedInput();

        }

        #region Update Functions

        /// <summary>
        /// 8/22/2024-LYI
        /// Touch, indirect pinch in Bounded Volume
        /// </summary>
        void UpdateBoundedInput()
        {
            var activeTouches = Touch.activeTouches;

            if (activeTouches.Count > 0)
            {
                var primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);
                if (activeTouches[0].phase == TouchPhase.Began)
                {
                    // allow balloons to be popped with a poke or indirect pinch
                    if (primaryTouchData.Kind == SpatialPointerKind.IndirectPinch ||
                        primaryTouchData.Kind == SpatialPointerKind.Touch)
                    {
                        var touchObject = primaryTouchData.targetObject;
                        if (touchObject != null)
                        {
                            if (touchObject.GetComponentInParent<UI_Open>())
                            {
                                touchObject.GetComponentInParent<UI_Open>().OpenUI();
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 8/22/2024-LYI
        /// Check Hand state in Unbounded volume
        /// </summary>
        void UpdateUnboundedInput()
        {
            //핸드 추적 상태 체크
            var updateSuccessFlags = m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
            {
                //각 배열 손가락 상태 저장, 포지션 저장
                UpdateCheckFingerPosition(arr_leftJoint, arr_leftTipPos, true);
                //이후 제스쳐 체크
                UpdateCheckGestureState(true, arr_leftJoint, arr_leftTipPos, leftGesture);
            }

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            {
                //각 배열 손가락 상태 저장, 포지션 저장
                UpdateCheckFingerPosition(arr_rightJoint, arr_rightTipPos, false);
                //이후 제스쳐 체크
                UpdateCheckGestureState(false, arr_rightJoint, arr_rightTipPos, rightGesture);
            }

        }


        /// <summary>
        /// 8/22/2024-LYI
        /// Check fingerTip position in joint, save
        /// </summary>
        /// <param name="arr_joint"></param>
        /// <param name="arr_tipPos"></param>
        void UpdateCheckFingerPosition(XRHandJoint[] arr_joint, Vector3[] arr_tipPos, bool isLeft)
        {
            //5 finger 0:Thumb~4:Little
            for (int i = 0; i < 5; i++)
            {
                int fingerIndex = i;
                arr_joint[fingerIndex] = GetHandJoint(isLeft, fingerIndex);
                arr_tipPos[fingerIndex] = GetFingerTipPosition(arr_joint[fingerIndex]);
            }
        }

        /// <summary>
        /// 8/22/2024-LYI
        /// Get hand joint with id
        /// </summary>
        /// <param name="isLeft"></param>
        /// <param name="fingerIndex">0:Thumb~4:Little</param>
        /// <returns></returns>
        XRHandJoint GetHandJoint(bool isLeft, int fingerIndex)
        {
            XRHandJoint resultJoint = new XRHandJoint();

            //XRHandJointID   ThumbTip = 6, IndexTip = 11,  MiddleTip = 16, RingTip = 21, LittleTip = 26
            // 6 + (5*index) 
            if (isLeft)
                resultJoint = m_HandSubsystem.leftHand.GetJoint((XRHandJointID)(6 + 5 * fingerIndex));
            else
                resultJoint = m_HandSubsystem.rightHand.GetJoint((XRHandJointID)(6 + 5 * fingerIndex));


            return resultJoint;
        }

        Vector3 GetFingerTipPosition(XRHandJoint joint)
        {
            Vector3 resultPos = Vector3.zero;
            if (joint.TryGetPose(out Pose fingerPose))
            {
                //카메라 상대 좌표
                // adjust transform relative to the PolySpatial Camera transform
                resultPos = gameMgr.MRMgr.VolumeCamera.transform.InverseTransformPoint(fingerPose.position);
            }
            return resultPos;
        }


        /// <summary>
        /// 8/22/2024-LYI
        /// Check gesture state in Unbounded volume
        /// </summary>
        void UpdateCheckGestureState(bool isLeft, XRHandJoint[] arr_joint, Vector3[] arr_fingerPos, HandGestureState stat)
        {

            //pinch, pointing, open, fist 

            //movement gesture

            for (int i = 0; i < arr_isLeftPinch.Length; i++)
            {
                if (isLeft)
                {
                    arr_isLeftPinch[i] = CheckPinchState(arr_joint, arr_fingerPos, i+1);
                }
                else
                {
                    arr_isRightPinch[i] = CheckPinchState(arr_joint, arr_fingerPos, i+1);
                }
            }


        }


        /// <summary>
        /// 8/22/2024-LYI
        /// Check Pinch
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        public bool CheckPinchState(XRHandJoint[] arr_joint, Vector3[] arr_fingerPos, int finger)
        {
            if (arr_joint[(int)FingerIndex.THUMB].trackingState != XRHandJointTrackingState.None &&
              arr_joint[finger].trackingState != XRHandJointTrackingState.None)
            {
                float pinchDistance = Vector3.Distance(arr_fingerPos[(int)FingerIndex.THUMB], arr_fingerPos[finger]);
                if (pinchDistance < m_ScaledThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        #endregion

    }
}