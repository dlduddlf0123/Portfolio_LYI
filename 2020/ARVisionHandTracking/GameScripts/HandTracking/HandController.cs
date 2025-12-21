using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;

public class HandController : MonoBehaviour
{
    [SerializeField] SwiftForUnity swiftUnity;
    [SerializeField] private Vector2 _handPos;

    public ARCameraManager arCamMgr;
    public Text text;

    GameManager gameMgr;
    public GameObject handColl;
    public GameObject handFollower;

    public Vector3 handWorldPos = Vector3.zero;


    public bool isHandFix = false;
    public float handCollHeight = 3f;

    public float handPosZ = 4f;
    public float handPosRight = 0.6f;
    public float handPosFront = 0;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        LoadHandPose();
    }

    private void OnEnable()
    {
        swiftUnity.OnHandDetected += OnHandDetectorCompleted;
        arCamMgr.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        swiftUnity.OnHandDetected -= OnHandDetectorCompleted;
        arCamMgr.frameReceived -= OnCameraFrameReceived;

    }

    public void SaveHandPose()
    {
        PlayerPrefs.SetFloat("HandPosZ", handPosZ);
        PlayerPrefs.SetFloat("HandPosRight", handPosRight);
    }
    public void LoadHandPose()
    {
        handPosZ = PlayerPrefs.GetFloat("HandPosZ", handPosZ);
        handPosRight = PlayerPrefs.GetFloat("HandPosRight", handPosRight);
    }


    /// <summary>
    /// Update?
    /// </summary>
    /// <param name="eventArgs"></param>
    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
#if !UNITY_EDITOR && UNITY_IOS
        var cameraParams = new XRCameraParams
        {
            zNear = Camera.main.nearClipPlane,
            zFar = Camera.main.farClipPlane,
            screenWidth = Screen.width,
            screenHeight = Screen.height,
            screenOrientation = Screen.orientation
        };


        XRCameraFrame frame;

        if (arCamMgr.subsystem.TryGetLatestFrame(cameraParams, out frame))
        {
            if (swiftUnity.IsIdle)
            {
                swiftUnity.StartDetect(frame.nativePtr);
            }
        }
#endif

    }

    //public void OnHandRecognized(object sender, HandRecognizedArgs e)
    //{
    //    _handPos = e.hand[0].handPosition;
    //    GameManager.Instance.uiMgr.uiText.text = "OnHandRecognized";

    //    handColl.transform.position = Camera.main.transform.forward + new Vector3(_handPos.x, _handPos.y, 0.1f);
    //}

    ////   /// <summary>
    ////   /// 손의 위치가 인식되었을 때 작동 Update
    ////   /// </summary>
    //public void OnHandTracking(object _sender, string _data)
    //{

    //    Vector3 vec = Camera.main.ScreenToWorldPoint(new Vector3(_handPos.x * Camera.main.pixelWidth, _handPos.y * Camera.main.pixelHeight, handPosZ));

    //    handColl.transform.position = vec;
    //    text.text = "손 위치: " + vec;
    //}

    //public void ChangeMoveDirection(MoveDirection _dir)
    //{
    //    moveDirection = _dir;
    //}

    //public void ChangeToXY()
    //{
    //    moveDirection = MoveDirection.XY;
    //    Debug.Log("MoveDirection: " + moveDirection);
    //}
    //public void ChangeToXZ()
    //{
    //    moveDirection = MoveDirection.XZ;
    //    Debug.Log("MoveDirection: " + moveDirection);
    //}
    //public void ChangeToYZ()
    //{
    //    moveDirection = MoveDirection.YZ;
    //    Debug.Log("MoveDirection: " + moveDirection);
    //}

    private void OnHandDetectorCompleted(object sender, Vector2 pos)
    {
        if (!handColl.activeSelf)
        {
            return;
        }
        //var handPos = new Vector3();
        //handPos.x = pos.x;
        //handPos.y = 1 - pos.y;
        //handPos.z = handPosZ;//m_Cam.nearClipPlane;
        //var handWorldPos = handPos;

        //switch (moveDirection)
        //{
        //    case MoveDirection.XY:
        //        handWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Abs(pos.x) * Camera.main.pixelWidth, Mathf.Abs(handPosZ), pos.y * Camera.main.pixelHeight));
        //        handColl.transform.position = handWorldPos + Vector3.right * handColl.transform.lossyScale.x * 0.5f;
        //        break;
        //    case MoveDirection.XZ:
        //        handWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Abs(pos.x) * Camera.main.pixelWidth, pos.y * Camera.main.pixelHeight, Mathf.Abs(handPosZ)));
        //        handColl.transform.position = handWorldPos + Vector3.right * handColl.transform.lossyScale.x * 0.5f;
        //        break;
        //    case MoveDirection.YZ:
        //        handWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Abs(handPosZ), Mathf.Abs(pos.x) * Camera.main.pixelWidth, pos.y * Camera.main.pixelHeight));
        //        handColl.transform.position = handWorldPos + Vector3.right * handColl.transform.lossyScale.x * 0.5f;
        //        break;
        //}

        handWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Abs(pos.x) * Camera.main.pixelWidth, pos.y * Camera.main.pixelHeight, Mathf.Abs(handPosZ)));
        handColl.transform.position = handWorldPos + transform.right * handPosRight + transform.forward * handPosFront;
       

        text.text = "손 위치: " + handColl.transform.position;

        Vector3 camLook = Camera.main.transform.position - handColl.transform.position;
        handColl.transform.rotation = Quaternion.LookRotation(camLook);
        handColl.transform.rotation = new Quaternion(0, handColl.transform.rotation.y, 0, handColl.transform.rotation.w);
    }
}
