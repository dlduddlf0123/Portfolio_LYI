using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaneGenerator : MonoBehaviour
{
    GameManager gameMgr;
    public ARPlaneTutorial planeTutorial;

    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject dummyPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return dummyPrefab; }
        set { dummyPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    //AR Manager Objects
    ARPlaneManager m_ARPlaneManager;
    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    public bool isPlaced = false;
    public Vector3 placedPos;
    public Quaternion placedRot { get; set; }
    Vector3 spawnedScale = Vector3.zero;

    void Awake()
    {
        gameMgr = GameManager.Instance;

        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_RaycastManager = GetComponent<ARRaycastManager>();

        //placedPos = new Vector3(0, -1, 1);
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }
#endif

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (isPlaced)
        {
            return;
        }
        //if (!TryGetTouchPosition(out Vector2 touchPosition))
        //    return;
        Vector2 centerCam = new Vector2(gameMgr.arMainCamera.pixelWidth * 0.5f, gameMgr.arMainCamera.pixelHeight * 0.5f);
        if (m_RaycastManager.Raycast(centerCam, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            Vector3 camLook = gameMgr.arMainCamera.transform.position - hitPose.position;
            camLook = new Vector3(camLook.x, 0, camLook.z);

            Quaternion rot = Quaternion.LookRotation(camLook);

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(dummyPrefab, hitPose.position, rot);
                placedPos = hitPose.position;
                placedRot = rot;
                spawnedScale = spawnedObject.transform.localScale;
                planeTutorial.ChangeTutorialPlaneText();

            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = rot;
                spawnedObject.transform.localScale = spawnedScale * gameMgr.uiMgr.stageSize;

                placedPos = hitPose.position;
                placedRot = rot;

                //if (gameMgr.currentEpisode != null)
                //{
                //    gameMgr.currentEpisode.transform.position = hitPose.position;
                //    gameMgr.currentEpisode.transform.rotation = rot;
                //}

                //                gameMgr.uiMgr.txt_placedpos.text = "placePos:" + placedPos;
            }
        }
    }

    /// <summary>
    /// Iterates over all the existing planes and activates
    /// or deactivates their <c>GameObject</c>s'.
    /// </summary>
    /// <param name="value">Each planes' GameObject is SetActive with this value.</param>
    public void SetAllPlanesActive(bool value)
    {
        if (spawnedObject != null)
        {
            spawnedObject.SetActive(value);
        }
        foreach (var plane in m_ARPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }

    


    /// <summary>
    /// 생성할 프리팹 설정(맵 선택)
    /// </summary>
    /// <param name="_go"></param>
    //public void SetPlacePrefab(GameObject _go)
    //{
    //    dummyPrefab = _go;

    //    if (dummyPrefab.GetComponent<StageManager>())
    //        gameMgr.currentStage = dummyPrefab.GetComponent<StageManager>();
    //}

    public void CleanARScene()
    {
        spawnedObject.SetActive(false);
        Destroy(spawnedObject, 3f);
        spawnedObject = null;
    }


}