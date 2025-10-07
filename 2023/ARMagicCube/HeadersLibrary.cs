using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System;

public enum CubeType
{
    NONE = 0,
    TITLE,
    INTRO,
    WORD,
    STORY,
}


/// <summary>
/// 12/2/2023-LYI
/// 번호대로 정렬되므로 번호 관리 잘 할것
/// </summary>
public enum LibraryType
{
    HEADERS_CUBE = -2,
    GUGUDAN_ALL = -1,
    NONE = 0,
    //EP001_Bee = 1,
    //EP002_Bug,
    EP018_Icecream = 1,
    EP016_Cottoncandy = 2,
    EP029_Halloween = 3,
    EP027_Vehicle = 4,
    EP026_Christmas = 5,
    EP025_Camping1 = 6,
    EP012_Bath = 7,
    EP014_Birthday = 8,
    EP010_Playground = 9,
    EP015_WashingDish = 10,
    EP028_Balloon = 11,
    EP004_Dessert = 12,
    EP019_Drink = 13,
    EP005_Fruit = 14,
    EP003_Singer = 15,
    EP009_Square = 16,
    EP020_Toy = 17,
    EP021_Baking = 18,
    EP008_Fishing1 = 19,
    EP024_Bedroom = 20,
    EP013_Sea = 21,
    EP030_Beach = 22,
    EP006_Cloud1 = 23,
    EP011_Snowman = 24,
    EP032_Rain = 25,
    EP031_Painter = 26,
    EP037_Fishing2 = 27,
    EP036_Camping2 = 28,
    EP035_Cloud2 = 29,
    EP033_Tanghulu = 30,
    EP007_Flower = 31,
    EP034_Vegetable = 32,
    // EP017_Meal,
    // EP022_Greengrocery,
    // EP023_Cleaning,
}


/// <summary>
/// 7/19/2023-LYI
/// 인식된 이미지의 이름을 기준으로 프리팹 불러오기
/// </summary>
public class HeadersLibrary : MonoBehaviour
{
    GameManager gameMgr;


    [Header("Parameter")]
    [SerializeField]
    ARTrackedImageManager arImageMgr;

    [SerializeField]
    Transform tr_active;
    [SerializeField]
    Transform tr_disable;

    //동적 생성
    public List<GameObject> list_activePrefab = new();
    public List<GameObject> list_disablePrefab = new();

    [Header("Library Status")]
    //ImageManager에서 추적할 라이브러리 목록
    public List<XRReferenceImageLibrary> list_imageLibrary = new List<XRReferenceImageLibrary>();
    //타이틀 이미지를 인식하는 라이브러리
    public XRReferenceImageLibrary titleLibrary;

    //현재 라이브러리
    public LibraryType currentLibrary;

    //각 이미지에 맞는 프리팹 홀더
    //추후 Addressable 등을 통해 데이터 전달
    //각 큐브 주제에 맞는거 들고있기?
    //인트로 인식 시 Dictionary에 데이터 할당
    public GameObject[] arr_currentMagicCube = new GameObject[12];
    public GameObject activeMagicCube = null;
    float disableTime = 5f;
    float t = 0f;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    public void Init()
    {
        //LibraryType loadType = ES3.Load("LastLibraryType", LibraryType.NONE);

        LibraryType loadType = LibraryType.NONE;
        ChangeEpisodeLibrary(loadType);
    }

    void OnEnable()
    {
        arImageMgr.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        arImageMgr.trackedImagesChanged -= OnTrackedImagesChanged;
    }


    //public void OnBeforeSerialize()
    //{
    //    list_guidPrefabOrigin.Clear();
    //    foreach (var kvp in dic_prefabDatas)
    //    {
    //        list_guidPrefabOrigin.Add(new GuidPrefab(kvp.Key, kvp.Value));
    //    }
    //}

    //public void OnAfterDeserialize()
    //{
    //    dic_prefabDatas.Clear();
    //    foreach (var entry in list_guidPrefabOrigin)
    //    {
    //        dic_prefabDatas.Add(Guid.Parse(entry.imageGuid), entry.imagePrefab);
    //    }
    //}


    /// <summary>
    /// 3/18/2024-LYI
    /// 프리팹 목록 갱신
    /// </summary>
    /// <param name="list">생성할 프리팹 리스트</param>
    public void SetDictionaryPrefab(List<GameObject> list = null)
    {
        arr_currentMagicCube = new GameObject[12];

        if (list_activePrefab.Count > 0)
        {
            List<GameObject> listToRemove = new();
            for (int i = 0; i < list_activePrefab.Count; i++)
            {
                list_activePrefab[i].GetComponent<MagicCube>().StopTimeline();
                gameMgr.objPoolingMgr.ObjectInit(list_disablePrefab, list_activePrefab[i], tr_disable);

                listToRemove.Add(list_activePrefab[i]);
            }

            list_activePrefab.RemoveAll((item) => listToRemove.Contains(item));

            Debug.Log("List Active Prefab Cleared");
            list_activePrefab.Clear();
        }

        if (list != null)
        {
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject prefab = gameMgr.objPoolingMgr.CreateObject(list_disablePrefab, list[i], transform.position, tr_disable);

                    prefab.SetActive(false);

                    list_activePrefab.Add(prefab);
                }

                list_activePrefab.CopyTo(arr_currentMagicCube);

                Debug.Log("CurrentMagicCube:" + arr_currentMagicCube[0].name + "  Length: " + arr_currentMagicCube.Length);
            }
        }
        else
        {
            Debug.Log("CurrentMagicCube:" + "Empty");
        }
    }


    /// <summary>
    /// 11/6/2023-LYI
    /// 각 버튼 동작
    /// 버튼 클릭 시 에피소드 변경
    /// 현재 재생 중인 에피소드 초기화?
    /// 추가적인 배경 변경?
    /// </summary>
    /// <param name="type"></param>
    public void ChangeEpisodeLibrary(LibraryType type)
    {
        //같은 라이브러리면 되돌리기
        if (type == currentLibrary)
        {
            return;
        }
        //3/22/2024-LYI
        //NONE일 경우 타이틀 구분 라이브러리 상태로 변경
        if (type == LibraryType.NONE)
        {
            arImageMgr.referenceLibrary = titleLibrary;
            currentLibrary = type;

            gameMgr.ui_librarySelect.ChangeCurrentLibraryText(type);


            //활성화 된 오브젝트들 비활성화
            DisableActiveMagicCube();
            //팝업 활성화 된 경우 비활성화
            gameMgr.ui_popup.ButtonCancle();

            Debug.Log("Current Library: Title_" + arImageMgr.referenceLibrary.ToString());
            return;
        }

        currentLibrary = type;
        ES3.Save("LastLibraryType", currentLibrary);

        gameMgr.ui_librarySelect.ChangeCurrentLibraryText(type);


        string episodeName = type.ToString();

        //Addressable에서 로드한 프리팹 리스트 불러오기
        if (gameMgr.addressableMgr.dic__allMagicCube.ContainsKey(episodeName))
        {
            SetDictionaryPrefab(gameMgr.addressableMgr.dic__allMagicCube[episodeName]);
        }
        else
        {
            SetDictionaryPrefab();
        }

        if (list_imageLibrary.Count >= (int)type)
        {

            if (list_imageLibrary[(int)type - 1] != null)
            {
                arImageMgr.referenceLibrary = list_imageLibrary[(int)type - 1];
            }
            else
            {
                arImageMgr.referenceLibrary = list_imageLibrary[list_imageLibrary.Count - 1];
            }
        }
        else
        {
            arImageMgr.referenceLibrary = list_imageLibrary[list_imageLibrary.Count-1];
        }


        Debug.Log("Current Library: Episode_" + arImageMgr.referenceLibrary.ToString());


        //arImageMgr.referenceLibrary = arr_imageLibrary[(int)type - 1];
        //Debug.Log("Current Library: " + arr_imageLibrary[(int)type - 1].name);
    }


    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (currentLibrary == LibraryType.NONE)
            {
                OnTitleRecognized(trackedImage);
            }
            else
            {
                ActivePrefab(trackedImage);
            }
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            if (currentLibrary == LibraryType.NONE)
            {
                //OnTitleRecognized(trackedImage);
            }
            else
            {
                ActivePrefab(trackedImage);
            }
        }
        //foreach (var trackedImage in eventArgs.removed)
        //{
        //    DisablePrefab(trackedImage);
        //}
    }

    /// <summary>
    /// 3/22/2024-LYI
    /// 타이틀 인식 시 라이브러리 변경 함수
    /// </summary>
    /// <param name="trackedImage"></param>
    void OnTitleRecognized(ARTrackedImage trackedImage)
    {
        //활성화된 매직큐브 비활성화
        DisableActiveMagicCube();

        //이미지 체크
        for (int i = 0; i < list_imageLibrary.Count; i++)
        {
           // Debug.Log(list_imageLibrary[i].name +"==" + trackedImage.referenceImage.name);
            //이름이 일치하는 경우
            if (list_imageLibrary[i].name == trackedImage.referenceImage.name)
            {

                //해당 라이브러리 다운로드 체크 호출
                //중복 돌려보낼것

                //!! 이미지 라이브러리와 버튼 순서가 같아야 한다
                UI_EpisodeButton button =  gameMgr.ui_librarySelect.arr_episodeButton[i];

                LibraryType type = button.libraryType;

                //버튼 상태로 예외처리
                if (button.statButton == ButtonStatus.DOWNLOAD ||
                    button.statButton == ButtonStatus.LOADING)
                {
                    Debug.Log(trackedImage.referenceImage.name + button.statButton.ToString());
                    return;
                }


                //다운로드 된 상태면 변경
                //사이즈 체크
                if (gameMgr.addressableMgr.dic_downloadSize.ContainsKey(type))
                {
                    long downloadSize = gameMgr.addressableMgr.dic_downloadSize[type];
                    if (downloadSize == 0)
                    {
                        gameMgr.ui_librarySelect.ButtonEpisodeSelect(button);
                    }
                    else
                    {
                        button.ButtonDownload();
                    }
                }
                else
                {
                    //사이즈 체크 진행
                    button.ButtonDownload();
                }

            }
        }

    }

    void ActivePrefab(ARTrackedImage trackedImage)
    {
        //Debug.Log("TrackedImageName: " + trackedImage.name);
        //Debug.Log("LastName: " + lastImageName);
        //t = 0;
        gameMgr.ui_librarySelect.ChangeCurrentReferenceImageText(trackedImage);

        if (arr_currentMagicCube[0] == null)
        {
            Debug.Log("Array is Empty!!");
            return;
        }

        for (int i = 0; i < 12; i++)
        {
            if (trackedImage.referenceImage.name == "ARImage" + (i + 1))
            {
                Debug.Log("ImageName:" + currentLibrary.ToString() + " ARImage" + (i + 1));
                Debug.Log("ObjectName: " + arr_currentMagicCube[i].name);

                activeMagicCube = arr_currentMagicCube[i];
                SetGameObjectPosition(activeMagicCube, trackedImage);
            }
        }

        //else if (trackedImage.trackingState == TrackingState.Limited)
        //{
        //    t += Time.deltaTime;
        //    if (t > disableTime)
        //    {
        //        DisablePrefab(trackedImage);
        //        t = 0;
        //    }
        //    // Debug.Log("TrackingState false?" + trackedImage.trackingState.ToString());


        //    //  DisablePrefab(trackedImage);
        //}
        //else
        //{
        //    t = 0;
        //    DisablePrefab(trackedImage);
        //}
    }

    void SetGameObjectPosition(GameObject go, ARTrackedImage trackedImage)
    {
        gameMgr.ui_librarySelect.SetMarkerActive(false);
        GameObject prefab = go;

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            t = 0;
            if (list_activePrefab.Count > 1)
            {
                for (int i = 0; i < list_activePrefab.Count; i++)
                {
                    if (list_activePrefab[i] != go)
                    {
                        if (list_activePrefab[i].activeSelf)
                        {
                            list_activePrefab[i].GetComponent<MagicCube>().StopTimeline();
                            list_activePrefab[i].SetActive(false);
                        }
                    }
                }
            }

            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;

            prefab.SetActive(true);
            prefab.GetComponent<MagicCube>().PlayTimeline();
        }
        else
        {
            t += Time.deltaTime;

            if (t > disableTime)
            {
                prefab.GetComponent<MagicCube>().StopTimeline();
                prefab.SetActive(false);
                t = 0;
            }
        }
    }

    /// <summary>
    /// 3/25/2024-LYI
    /// 활성화 된 매직큐브들 비활성화
    /// </summary>
    void DisableActiveMagicCube()
    {
        gameMgr.ui_librarySelect.SetMarkerActive(true);
        if (activeMagicCube != null)
        {
            for (int i = 0; i < list_activePrefab.Count; i++)
            {
                if (list_activePrefab[i].activeSelf)
                {
                    list_activePrefab[i].GetComponent<MagicCube>().StopTimeline();
                    list_activePrefab[i].SetActive(false);
                }
            }
            activeMagicCube.GetComponent<MagicCube>().StopTimeline();
            activeMagicCube.SetActive(false);
            activeMagicCube = null;
        }
    }

    void UpdatePrefab(ARTrackedImage trackedImage)
    {
        if (list_activePrefab.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < list_activePrefab.Count; i++)
        {
            list_activePrefab[i].transform.position = trackedImage.transform.position;
            list_activePrefab[i].transform.rotation = trackedImage.transform.rotation;
        }

        gameMgr.ui_librarySelect.ChangeCurrentReferenceImageText(trackedImage);
    }

    void DisablePrefab(ARTrackedImage trackedImage)
    {
        if (list_activePrefab.Count <= 0)
        {
            return;
        }

        Debug.Log("Disable Prefab");
        for (int i = 0; i < list_activePrefab.Count; i++)
        {
            if (list_activePrefab[i].name != trackedImage.referenceImage.name)
            {
                list_activePrefab[i].GetComponent<MagicCube>().StopTimeline();
                gameMgr.objPoolingMgr.ObjectInit(list_disablePrefab, list_activePrefab[i], tr_disable);

                list_activePrefab.RemoveAt(i);
            }
        }

    }


    #region TestCode
    public void TestEnable()
    {
        if (arr_currentMagicCube[0] == null)
        {
            Debug.Log("Array is Empty!!");
            return;
        }
        for (int i = 0; i < arr_currentMagicCube.Length; i++)
        {
            if (arr_currentMagicCube[i] == null)
            {
                Debug.Log("Array :" + i + " is Empty!!");
                return;
            }
        }

        for (int i = 0; i < list_activePrefab.Count; i++)
        {
            if (list_activePrefab[i].name == arr_currentMagicCube[11].name + "(Clone)")
            {
                Debug.Log("Aleady Activated!: " + list_activePrefab[i].name);
                GameObject activeGo = list_activePrefab[i];

                activeGo.SetActive(true);
                return;
            }
        }

        GameObject prefab = gameMgr.objPoolingMgr.CreateObject(list_disablePrefab, arr_currentMagicCube[11],
            Vector3.zero, tr_disable);

        prefab.SetActive(true);
        prefab.GetComponent<MagicCube>().PlayTimeline();
        list_activePrefab.Add(prefab);
    }

    public void TestUpdate()
    {
        if (list_activePrefab.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < list_activePrefab.Count; i++)
        {
            list_activePrefab[i].transform.position = Vector3.zero;
            list_activePrefab[i].transform.rotation = Quaternion.identity;
        }
    }

    public void TestDisable()
    {
        if (list_activePrefab.Count <= 0)
        {
            return;
        }

        // Queue<int> removeNum = new();
        for (int i = 0; i < list_activePrefab.Count; i++)
        {
            list_activePrefab[i].GetComponent<MagicCube>().StopTimeline();
            gameMgr.objPoolingMgr.ObjectInit(list_disablePrefab, list_activePrefab[i], tr_disable);

            list_activePrefab.RemoveAt(i);
        }

        gameMgr.ui_librarySelect.ChangeCurrentReferenceImageText();
    }
    #endregion
}
