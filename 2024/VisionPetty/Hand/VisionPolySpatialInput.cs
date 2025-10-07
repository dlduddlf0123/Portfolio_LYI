using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Unity.PolySpatial.InputDevices;

using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

namespace AroundEffect
{
    public enum FingerType
    {
        THUMB = 0,
        INDEX,
        MIDDLE,
        RING,
        LITTLE,
    }


    /// <summary>
    /// 8/30/2024-LYI
    /// Inputs using polyspatial(vision os)
    /// ex) head pos, ray, hand touch, indirect pinch...
    /// </summary>
    public class VisionPolySpatialInput : MonoBehaviour
    {
        GameManager gameMgr;

        XRHandSubsystem XRHandSystem;

        [Header("XRHandGesture")]
        public XRHandGestureInput handInputL;
        public XRHandGestureInput handInputR;

        [Header("NearFarInteractor")]
        public NearFarInteractor primaryInteractor;
        public NearFarInteractor secondaryInteractor;


        [Header("Hand Collider")]
        public GameObject handCapsuleColliderL;
        public GameObject handCapsuleColliderR;
        public Collider[] arr_handCollL;
        public Collider[] arr_handCollR;

        public Collider fistCollL;
        public Collider fistCollR;

        [Header("Device InputSystem")]
        public InputDevice ia;

        [Header("Head Transform")]
        public Vector3 Device_headPos;
        public Quaternion Device_headRot;

        [Header("Finger Joint")]
        //private XRHandJoint[] arr_tipJointL;
        //private XRHandJoint[] arr_proximalJointL;
        //private XRHandJoint[] arr_tipJointR;
        //private XRHandJoint[] arr_proximalJointR;

        //private Vector3[] arr_tipPosL;
        //private Vector3[] arr_proximalPosL;
        //private Vector3[] arr_tipPosR;
        //private Vector3[] arr_proximalPosR;

        //private XRHandJoint[] arr_angleJointL;
        //private XRHandJoint[] arr_angleJointR;
        //public float[] arr_fingerAngleL;
        //public float[] arr_fingerAngleR;

        //public float[] fingerDistanceL;
        //public float[] fingerDistanceR;

        public Transform[] arr_trJointL;
        public Transform[] arr_trJointR;

        public bool[] isFingerCullL;
        public bool[] isFingerCullR;

        [Header("Properties")]
        //float m_ScaledThreshold;

        //public float k_fingerThreshold = 0.02f;

        public float angleThreshold = 0.6f;


        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        private void Start()
        {
            InputInit();
        }

        void OnEnable()
        {
            // enable enhanced touch support to use active touches for properly pooling input phases
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }


        public void InputInit()
        {
            GetHandSubsystem();
            SetInteractor();

            //arr_tipJointL = new XRHandJoint[5];
            //arr_proximalJointL = new XRHandJoint[5];
            //arr_tipJointR = new XRHandJoint[5];
            //arr_proximalJointR = new XRHandJoint[5];

            //arr_tipPosL = new Vector3[5];
            //arr_proximalPosL = new Vector3[5];
            //arr_tipPosR = new Vector3[5];
            //arr_proximalPosR = new Vector3[5];

            ////Angle
            //arr_angleJointL = new XRHandJoint[5];
            //arr_angleJointR = new XRHandJoint[5];
            //arr_fingerAngleL = new float[5];
            //arr_fingerAngleR = new float[5];


            //fingerDistanceL = new float[5];
            //fingerDistanceR = new float[5];

            isFingerCullL = new bool[5];
            isFingerCullR = new bool[5];

            //m_ScaledThreshold = k_fingerThreshold / gameMgr.MRMgr.VolumeCamera.transform.localScale.x;

        }

        /// <summary>
        /// 10/29/2024-LYI
        /// NearFar Interactor 상호작용 시 이벤트 추가
        /// </summary>
        public void SetInteractor()
        {
            primaryInteractor.selectEntered.AddListener(NearFar_OnSelect);
            primaryInteractor.selectExited.AddListener(NearFar_OnDeselect);

            secondaryInteractor.selectEntered.AddListener(NearFar_OnSelect);
            secondaryInteractor.selectExited.AddListener(NearFar_OnDeselect);
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
                    XRHandSystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                    //if (!CheckHandSubsystem())
                    //    return;

                    XRHandSystem.Start();
                }
            }

        }

        #region Update Func

        void Update()
        {
            Device_headPos = gameMgr.MRMgr.XR_Origin.Camera.transform.position;
            Device_headRot = gameMgr.MRMgr.XR_Origin.Camera.transform.rotation;

            EnhancedSpatialTouchInteractorUpdate();
            FingerStateUpdate();
            FingerColliderUpdate();
        }

        void EnhancedSpatialTouchInteractorUpdate()
        {
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

            if (activeTouches.Count > 0)
            {
                // 첫 번째 활성 터치에 대해 PolySpatial 데이터를 가져옴
                SpatialPointerState primaryTouchData =
                    EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);


                // 제스처 종류가 IndirectPinch 또는 DirectPinch인지 확인
                if (primaryTouchData.Kind == SpatialPointerKind.DirectPinch)
                {
                    Vector3 interactionPosition = primaryTouchData.interactionPosition;
                    GameObject targetObject = primaryTouchData.targetObject;

                    // 여기서 핀치 제스처에 따른 로직을 추가

                    if (activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        // 핀치 시작 이벤트 처리
                        OnStartPinch(interactionPosition, targetObject);
                    }
                    else if (activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Ended)
                    {
                        // 핀치 종료 이벤트 처리
                        OnEndPinch();
                    }
                }

                //원거리 핀치일 때
                if (primaryTouchData.Kind == SpatialPointerKind.IndirectPinch)
                {
                    Vector3 interactionPosition = primaryTouchData.interactionPosition;
                    GameObject targetObject = primaryTouchData.targetObject;


                    if (activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        if (gameMgr.statGame != GameStatus.MINIGAME)
                        {
                            if (targetObject.layer == LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER))
                            {
                                //캐릭터 선택 효과
                                CharacterManager charMgr = targetObject.GetComponentInParent<CharacterManager>();

                                gameMgr.lifeMgr.OnCharacterSelect(charMgr);
                            }
                        }

                        // 핀치 시작 이벤트 처리
                        OnStartPinch(interactionPosition, targetObject);
                    }
                    else if (activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Ended)
                    {
                        // 핀치 종료 이벤트 처리
                        OnEndPinch();
                    }

                }

                if (primaryTouchData.Kind == SpatialPointerKind.IndirectPinch ||
                       primaryTouchData.Kind == SpatialPointerKind.Touch)
                {
                    Vector3 interactionPosition = primaryTouchData.interactionPosition;
                    GameObject targetObject = primaryTouchData.targetObject;

                    if (activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        if (targetObject != null)
                        {
                            if (targetObject.layer == LayerMask.NameToLayer(Constants.Layer.LAYERMASK_UI))
                            {
                                if (targetObject.GetComponentInParent<Button>())
                                {
                                    targetObject.GetComponentInParent<Button>().onClick?.Invoke();
                                }
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 10/29/2024-LYI
        /// 손가락 상태 업데이트
        /// 각 손가락의 위치 체크
        /// 손가락이 접혔는지 체크
        /// 접혔으면 컬리더 비활성화
        /// </summary>
        void FingerStateUpdate()
        {
            if (XRHandSystem != null)
            {
                //UpdateCheckFingerPosition(arr_tipJointL, arr_tipPosL, true, true);
                //UpdateCheckFingerPosition(arr_tipJointR, arr_tipPosR, false, true);
                //UpdateCheckFingerPosition(arr_proximalJointL, arr_proximalPosL, true, false);
                //UpdateCheckFingerPosition(arr_proximalJointR, arr_proximalPosR, false, false);

                for (int i = 0; i < arr_trJointL.Length; i++)
                {
                    if (i == 0)
                    {
                        isFingerCullL[i] = arr_trJointL[i].localRotation.x > angleThreshold - 0.2f;
                        isFingerCullR[i] = arr_trJointR[i].localRotation.x > angleThreshold - 0.2f;
                    }
                    else
                    {
                        isFingerCullL[i] = arr_trJointL[i].localRotation.x > angleThreshold;
                        isFingerCullR[i] = arr_trJointR[i].localRotation.x > angleThreshold;
                    }
                }

                //for (int i = 0; i < arr_fingerAngleL.Length; i++)
                //{
                //    arr_angleJointL[i] = GetHandAngleJoint(true, i);
                //    arr_angleJointR[i] = GetHandAngleJoint(false, i);

                //    float r1,r2;
                //    if (arr_angleJointL[i].TryGetRadius(out r1))
                //    {
                //        arr_fingerAngleL[i] = r1;
                //        isFingerCullL[i] = (arr_fingerAngleL[i] < angleThreshold);
                //    }
                //    if (arr_angleJointR[i].TryGetRadius(out r2))
                //    {
                //        arr_fingerAngleR[i] = r2;
                //        isFingerCullR[i] = (arr_fingerAngleR[i] < angleThreshold);
                //    }

                //}

                //for (int i = 0; i < isFingerCullL.Length; i++)
                //{
                //    int fingerIndex = i;
                //    isFingerCullL[fingerIndex] = CheckFingerState(true, (FingerType)fingerIndex);
                //    isFingerCullR[fingerIndex] = CheckFingerState(false, (FingerType)fingerIndex);
                //}
            }
        }

        /// <summary>
        /// 10/29/2024-LYI
        /// 손가락 컬리더 상태 전환
        /// </summary>
        void FingerColliderUpdate()
        {
            for (int i = 0; i < isFingerCullL.Length; i++)
            {
                arr_handCollL[i].gameObject.SetActive(!isFingerCullL[i]);
                arr_handCollR[i].gameObject.SetActive(!isFingerCullR[i]);
            }
        }

        public void FistColliderUpdate(bool isLeft, bool isActive)
        {
            if (isLeft)
            {
                fistCollL.gameObject.SetActive(isActive);
            }
            else
            {
                fistCollR.gameObject.SetActive(isActive);
            }
        }

        #endregion


        #region Finger State(미사용)

        void UpdateCheckFingerPosition(XRHandJoint[] arr_joint, Vector3[] arr_tipPos, bool isLeft, bool isTip)
        {
            //5 finger 0:Thumb~4:Little
            for (int i = 0; i < 5; i++)
            {
                int fingerIndex = i;
                arr_joint[fingerIndex] = GetHandJoint(isLeft, isTip,fingerIndex);
                arr_tipPos[fingerIndex] = GetJointPosition(arr_joint[fingerIndex]);
            }
        }
        XRHandJoint GetHandJoint(bool isLeft, bool isTip, int fingerIndex)
        {
            XRHandJoint resultJoint = new XRHandJoint();

            //XRHandJointID   ThumbTip = 6, IndexTip = 11,  MiddleTip = 16, RingTip = 21, LittleTip = 26
            //Hand Proximal ThumbTip = 4, IndexTip = 8,  MiddleTip = 13, RingTip = 18, LittleTip = 23
            // 6 + (5*index)

            int startPoint = 0;
            if (isTip)
            {
                startPoint = 6;
            }
            else
            {
                startPoint = 3;
            }
            if (isLeft)
                resultJoint = XRHandSystem.leftHand.GetJoint((XRHandJointID)(startPoint + 5 * fingerIndex));
            else
                resultJoint = XRHandSystem.rightHand.GetJoint((XRHandJointID)(startPoint + 5 * fingerIndex));

            return resultJoint;
        }
        Vector3 GetJointPosition(XRHandJoint joint)
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

        public bool CheckFingerState(bool isLeft, FingerType type)
        {
            //XRHandJoint[] arr_tipJoint;
            //Vector3[] arr_tipPos;
            //XRHandJoint[] arr_proximalJoint;
            //Vector3[] arr_proximalPos;


            //if (isLeft)
            //{
            //    arr_tipJoint = arr_tipJointL;
            //    arr_tipPos = arr_tipPosL;
            //    arr_proximalJoint = arr_proximalJointL;
            //    arr_proximalPos = arr_proximalPosL;
            //}
            //else
            //{
            //    arr_tipJoint = arr_tipJointR;
            //    arr_tipPos = arr_tipPosR;
            //    arr_proximalJoint = arr_proximalJointR;
            //    arr_proximalPos = arr_proximalPosR;
            //}

            //if (arr_proximalJoint[(int)type].trackingState != XRHandJointTrackingState.None &&
            //  arr_tipJoint[(int)type].trackingState != XRHandJointTrackingState.None)
            //{
            //    float cullDistance = Vector3.Distance(arr_proximalPos[(int)type], arr_tipPos[(int)type]);
            //    if (isLeft)
            //        fingerDistanceL[(int)type] = cullDistance;      
            //    else  
            //        fingerDistanceR[(int)type] = cullDistance;

            //    Debug.Log(type.ToString() +  " - CullDistance: " + cullDistance + "--ScaleTreshold: " + m_ScaledThreshold);

            //    if (cullDistance < m_ScaledThreshold)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
            return false;
        }


        XRHandJoint GetHandAngleJoint(bool isLeft, int fingerIndex)
        {
            XRHandJoint resultJoint = new XRHandJoint();

            //XRHandJointID   ThumbTip = 6, IndexTip = 11,  MiddleTip = 16, RingTip = 21, LittleTip = 26
            //Hand Proximal ThumbTip = 4, IndexTip = 8,  MiddleTip = 13, RingTip = 18, LittleTip = 23
            // 6 + (5*index)

            //4 + 5*i ,9, 14 ,19 ,24

            if (fingerIndex == 0)
            {
                if (isLeft)
                    resultJoint = XRHandSystem.leftHand.GetJoint(XRHandJointID.ThumbDistal);
                else
                    resultJoint = XRHandSystem.rightHand.GetJoint(XRHandJointID.ThumbDistal);
            }
            else
            {
                if (isLeft)
                    resultJoint = XRHandSystem.leftHand.GetJoint((XRHandJointID)(4 + 5 * fingerIndex));
                else
                    resultJoint = XRHandSystem.rightHand.GetJoint((XRHandJointID)(4 + 5 * fingerIndex));

            }


            return resultJoint;
        }

        #endregion



        /// <summary>
        /// 11/25/2024-LYI
        /// pinch 동작 시
        /// </summary>
        /// <param name="pinchPos"></param>
        /// <param name="targetGO"></param>
        void OnStartPinch(Vector2 pinchPos, GameObject targetGO)
        {
            Debug.Log("Input - OnStartPinch()");

            if (gameMgr.MRMgr.AR_PlaneGenerator.statPlane == ARPlaneMode.MOVE)
            {
                if (targetGO.CompareTag(Constants.TAG.TAG_GROUND))
                {
                    //gameMgr.MRMgr.AR_PlaneGenerator.CreateDummy();

                    gameMgr.MRMgr.tr_MRAnchor.position = pinchPos;
                    //gameMgr.MRMgr.AR_PlaneGenerator.MoveDummy(pinchPos);
                }

            }
            
            //SetCapsuleActive(true, false);
            //SetCapsuleActive(false, false);
        }

        void OnEndPinch()
        {
            Debug.Log("Input - OnEndPinch()");
            //SetCapsuleActive(true, true);
            //SetCapsuleActive(false, true);
        }


        void GetHeadTransform()
        {
            InputDevice head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (head.isValid)
            {
                if (head.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
                {
                    //머리 위치 받아옴
                    Device_headPos = pos;
                }
                if (head.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
                {
                    //머리 회전 받아옴
                    Device_headRot = rot;
                }
            }

        }



        /// <summary>
        /// 9/9/2024-LYI
        /// 각 손의 제스쳐에 따라 손 캡슐(터치 용) 활성화 상태 변경
        /// </summary>
        /// <param name="isLeft">left hand = true</param>
        /// <param name="isActive">active = true</param>
        public void SetCapsuleActive(bool isLeft,bool isActive)
        {
            if (isLeft)
            {
                handCapsuleColliderL.gameObject.SetActive(isActive);
            }
            else
            {
                handCapsuleColliderR.gameObject.SetActive(isActive);
            }
        }



        public void NearFar_OnSelect(SelectEnterEventArgs args)
        {
            if (args.interactorObject != null)
            {
                bool isLeft = (args.interactorObject.handedness == InteractorHandedness.Left);
                SetCapsuleActive(isLeft, false);
            }
        }
        public void NearFar_OnDeselect(SelectExitEventArgs args)
        {
            if (args.interactorObject != null)
            {
                bool isLeft = (args.interactorObject.handedness == InteractorHandedness.Left);
                SetCapsuleActive(isLeft, true);
            }
        }





        private void OnDestroy()
        {
            ResetInteractor();
        }
        public void ResetInteractor()
        {
            primaryInteractor.selectEntered.RemoveAllListeners();
            primaryInteractor.selectExited.RemoveAllListeners();

            secondaryInteractor.selectEntered.RemoveAllListeners();
            secondaryInteractor.selectExited.RemoveAllListeners();
        }


    }//class
}//namespace