using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

public enum GameMode
{
    NONE = 0,
    CHARACTER_SELECT,
    CHARACTER_TRANSFORM,
    DETAIL_TRANSFORM,
    ANIMATION,
}

public class GameManager : MonoBehaviour
{
    [Header("Manager Class")]
    public AddressableManager addressMgr;
    public SoundManager soundMgr;
    public ObjectPoolingManager objPoolingMgr;

    public UIManager uiMgr;

    [Header("XR Class")]
    public XROrigin XROrigin;
    public ARPointCloudManager ARPointCloudManager { get; set; }
    public ARPlaneManager ARPlaneManager { get; set; }
    public ARRaycastManager ARRaycastManager { get; set; }

    //  public ARPlaceObject ARPlaceObject { get; set; }
    //public FacingDirectionManager facingDirectionManager { get; set; }

    [Header("XR Object")]
    public Camera mainCamera;

    public WorldCharacterController[] arr_ARPrefabs;

    public WorldCharacterController spawnARCharacter { get; set; } //현재 소환된 AR 오브젝트
    public WorldCharacterController selectARCharacter { get; set; } //현재 소환된 AR 오브젝트


    [Header("Light")]
    public Light light_ar;
    public Light light_normal;
    public bool isLightAuto = true;

    [Header("Game Status")]
    public GameMode currentMode = GameMode.NONE;

    private static GameManager s_instance = null;
    public static GameManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType(typeof(GameManager)) as GameManager;
            }
            return s_instance;
        }
    }

    void Awake()
    {
        if (s_instance != null && this != s_instance)
        {
            Debug.LogError("Cannot have two instances.");
            Destroy(gameObject);
            return;
        }
        s_instance = this;

    }

    private void Start()
    {
        if (XROrigin != null)
        {
            ARPointCloudManager = XROrigin.GetComponent<ARPointCloudManager>();
            ARPlaneManager = XROrigin.GetComponent<ARPlaneManager>();
            ARRaycastManager = XROrigin.GetComponent<ARRaycastManager>();
            // ARPlaceObject = XROrigin.GetComponent<ARPlaceObject>();
            //facingDirectionManager = XROrigin.GetComponent<FacingDirectionManager>();
        }

        ChangeLightAuto(true);
        ChangeGameMode(GameMode.CHARACTER_SELECT);
    }

    public void ChangeGameMode(GameMode mode)
    {
        currentMode = mode;

        SetARPlaneEnable(false);
        switch (mode)
        {
            case GameMode.NONE:
                break;
            case GameMode.CHARACTER_SELECT:
                break;
            case GameMode.CHARACTER_TRANSFORM:
                SetARPlaneEnable(true);
                break;
            case GameMode.ANIMATION:

                break;
            default:
                break;
        }

        uiMgr.ChangeUI(mode);
    }


    public void OnGameStart()
    {

    }

    public void OnGameEnd()
    {

    }

    public Gradient lineActive;
    public Gradient lineDisable;

    /// <summary>
    /// 12/18/2023-LYI
    /// AR Plane 활성화 여부 변경
    /// </summary>
    /// <param name="isActive"></param>
    public void SetARPlaneEnable(bool isActive)
    {
        ARPlaneManager.enabled = isActive;

        if (XROrigin.transform.childCount > 1)
        {
           // List<GameObject> list_arPlane = new List<GameObject>();
            for (int i = 0; i < XROrigin.transform.GetChild(1).childCount; i++)
            {
                GameObject arPlane = XROrigin.transform.GetChild(1).GetChild(i).gameObject;

                if (arPlane.GetComponent<ARPlane>())
                {
                    if (isActive)
                    {
                        arPlane.GetComponent<LineRenderer>().colorGradient = lineActive;
                    }
                    else
                    {
                        arPlane.GetComponent<LineRenderer>().colorGradient = lineDisable;
                    }
                   // list_arPlane.Add(XROrigin.transform.GetChild(1).GetChild(i).gameObject);
                }
            }
        }

        uiMgr.ui_characterTransform.transformRay.enabled = isActive;
    }

    /// <summary>
    /// 12/18/2023-LYI
    /// 캐릭터 선택
    /// </summary>
    /// <param name="num"></param>
    public void SetARObject(int num)
    {
        selectARCharacter = arr_ARPrefabs[num];
        uiMgr.ui_characterTransform.transformRay.prefabToPlace = selectARCharacter.gameObject;
        uiMgr.ui_characterTransform.transformRay.onSpawn = SetSpawnObject;

        if (spawnARCharacter != null)
        {
            spawnARCharacter = null;
            Destroy(uiMgr.ui_characterTransform.transformRay.m_SpawnedObject);
            uiMgr.ui_characterTransform.transformRay.m_SpawnedObject = null;
        }

        //facingDirectionManager.worldSpaceObject = selectARCharacter.gameObject;
    }

    public void SetSpawnObject()
    {
        spawnARCharacter = uiMgr.ui_characterTransform.transformRay.m_SpawnedObject.GetComponent<WorldCharacterController>();

        if (currentMode == GameMode.CHARACTER_TRANSFORM)
        {
            uiMgr.ChangeUI(currentMode);
        }
    }

    /// <summary>
    /// 12/18/2023-LYI
    /// 캐릭터 선택 해제
    /// </summary>
    public void ResetARObject()
    {
        selectARCharacter = null;
        uiMgr.ui_characterTransform.transformRay.prefabToPlace = null;
        //facingDirectionManager.worldSpaceObject = null;
    }


    public void ChangeLightAuto(bool isAcitve)
    {
        isLightAuto = isAcitve;

        light_ar.gameObject.SetActive(isAcitve);
        light_normal.gameObject.SetActive(!isAcitve);
    }
}
