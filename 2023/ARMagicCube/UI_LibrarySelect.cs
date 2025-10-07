using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;
using TMPro;

using UnityEngine.XR.ARFoundation;
using MoreMountains.Feedbacks;

public class UI_LibrarySelect : MonoBehaviour
{
    GameManager gameMgr;

    public UI_EpisodeButton[] arr_episodeButton;


    [Header("Scroll")]
    public List<Sprite> list_selectButton = new List<Sprite>();

    [SerializeField]
    GameObject scrollView_episode;
    [SerializeField]
    RectTransform scrollView_content;

    [SerializeField]
    MMF_Player mmf_scrollOpen;
    [SerializeField]
    MMF_Player mmf_scrollClose;
    [SerializeField]
    bool isScrollOpen = false;

    [Header("Button")]
    [SerializeField]
    Button btn_scrollBG;
    [SerializeField]
    Button btn_scrollOpen;
    [SerializeField]
    Button btn_shop;

    [Header("Information")]
    [SerializeField]
    GameObject go_currentEpisode; //버튼으로 변경, 타이틀 선택 모드로 돌아가는 기능
    [SerializeField]
    Button btn_selectedEpisode; //버튼으로 변경, 타이틀 선택 모드로 돌아가는 기능
    [SerializeField]
    TextMeshProUGUI txt_currentEpisode;
    [SerializeField]
    GameObject titleMarker;


    [Header("Debug")]
    [SerializeField]
    TextMeshProUGUI debug_txt_currentLibrary;
    [SerializeField]
    TextMeshProUGUI txt_currentImage;
    [SerializeField]
    TextMeshProUGUI txt_fps;
    [SerializeField]
    TextMeshProUGUI txt_targetFps;
    [SerializeField]
    TextMeshProUGUI txt_addressableStat;
    [SerializeField]
    Button btn_testEnable;
    [SerializeField]
    Button btn_testDisable;
    [SerializeField]
    Button btn_30f;
    [SerializeField]
    Button btn_60f;

    public AudioClip sfx_click;

    float pollingTime = 1f;
    float time;
    int frameCount;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }


    public void Init()
    {
        //버튼들 클릭 효과음 할당
        Button[] allButtons = GetComponentsInChildren<Button>();
        for (int i = 0; i < allButtons.Length; i++)
        {
            allButtons[i].onClick.AddListener(() => gameMgr.soundMgr.PlaySfx(Vector3.zero, sfx_click));
        }

        //각 버튼 기능할당
        btn_scrollBG.onClick.AddListener(ToggleScrollView);
        btn_scrollOpen.onClick.AddListener(ToggleScrollView);
        btn_shop.onClick.AddListener(ButtonShop);

        //상단 에피소드 버튼
        btn_selectedEpisode.onClick.AddListener(ButtonCurrentEpisode);
        btn_selectedEpisode.onClick.AddListener(() => gameMgr.soundMgr.PlaySfx(Vector3.zero, sfx_click));

        //에피소드 선택 버튼 긁어오기
        arr_episodeButton = scrollView_content.transform.GetComponentsInChildren<UI_EpisodeButton>();

        //각 버튼 에피소드 스프라이트 변환
        Dictionary<string, Sprite> dic_selectSprite = new();

        for (int i = 0; i < list_selectButton.Count; i++)
        {
            if (!dic_selectSprite.ContainsKey(list_selectButton[i].name))
            {
                dic_selectSprite.Add(list_selectButton[i].name, list_selectButton[i]);
            }
        }

        //버튼 기능 설정
        for (int i = 0; i < arr_episodeButton.Length; i++)
        {
            //for문 내 매개변수 전달용 저장 변수
            int num = 0;
            num = i;
            //이름 변환
            string libraryName = ((LibraryType)num + 1).ToString();

            //에피소드 선택 버튼 기능 초기화
            arr_episodeButton[num].Init();
            arr_episodeButton[num].libraryType = ((LibraryType)num + 1);
            arr_episodeButton[num].btn_episode.image.sprite = dic_selectSprite[libraryName];
            arr_episodeButton[num].img_downloadBG.sprite = dic_selectSprite[libraryName];

            //에피소드 선택 버튼 기능 할당
            arr_episodeButton[num].btn_episode.onClick.AddListener(() => ButtonEpisodeSelect(arr_episodeButton[num]));
        }

        //스크롤 위치 맨 위로
        scrollView_content.position = Vector3.zero;

        //디버그 기능들
        btn_testEnable.onClick.AddListener(TestButtonEnable);
        btn_testDisable.onClick.AddListener(TestButtonDisable);

        btn_30f.onClick.AddListener(() => ChangeFrameRate(30));
        btn_60f.onClick.AddListener(() => ChangeFrameRate(60));

        //상단 UI 상태 변환
        ChangeCurrentLibraryText(LibraryType.NONE);
    }


    public void ChangeFrameRate(int frame)
    {
        Application.targetFrameRate = frame;
        txt_targetFps.text = "TargetFPS: " + Application.targetFrameRate;
    }

    public void ChangeAddressableStatus(bool isLoad)
    {
        if (!isLoad)
        {
            txt_currentEpisode.text = "Loading...";
        }
        else
        {
            txt_currentEpisode.text = "Ready";
        }
        txt_addressableStat.text = "AddressableLoad = " + isLoad.ToString();
    }

    private void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            txt_fps.text = "FPS: " + frameRate.ToString();

            time -= pollingTime;
            frameCount = 0;
        }
    }

    #region TestCode
    void TestButtonEnable()
    {
        gameMgr.libraryHeader.TestEnable();

        StartCoroutine(UpdateCheck());
    }
    void TestButtonDisable()
    {
        gameMgr.libraryHeader.TestDisable();
        StopAllCoroutines();
    }

    IEnumerator UpdateCheck()
    {
        while (true)
        {
            gameMgr.libraryHeader.TestUpdate();
            yield return null;
        }
    }
    #endregion

    /// <summary>
    /// 3/13/2024-LYI
    /// 에피소드 선택 버튼을 눌렀을 경우
    /// </summary>
    /// <param name="num"></param>
    public void ButtonEpisodeSelect(UI_EpisodeButton button)
    {
        //이미 로드됐는지 확인
        if (!gameMgr.addressableMgr.list_loadedLibrary.Contains(button.libraryType))
        {
            button.ChangeButtonStatus(ButtonStatus.LOADING);

            //번들 로드 진행
            //이후 프리팹 생성
            StartCoroutine(gameMgr.addressableMgr.LoadMagicCubeAssets(button.libraryType,
                () =>
                {
                    gameMgr.libraryHeader.ChangeEpisodeLibrary(button.libraryType);

                    button.ChangeButtonStatus(ButtonStatus.OPEN);
                }));

        }
        else
        {
            //비활성화 된 프리팹 활성화
            gameMgr.libraryHeader.ChangeEpisodeLibrary(button.libraryType);
        }

    }



    #region Current Episode UI

    /// <summary>
    /// 3/22/2024-LYI
    /// 에피소드 버튼 클릭 시
    /// 현재 에피소드 해제하기, 마커 보여주기
    /// </summary>
    public void ButtonCurrentEpisode()
    {
        //선택 안된 상태일 경우
        if (gameMgr.libraryHeader.currentLibrary == LibraryType.NONE)
        {
            return;
        }


        //선택 된 상태인경우
        gameMgr.libraryHeader.ChangeEpisodeLibrary(LibraryType.NONE);

    }


    /// <summary>
    /// 3/18/2024-LYI
    /// 화면 상단 UI 표시 변경
    /// </summary>
    /// <param name="type"></param>
    public void ChangeCurrentLibraryText(LibraryType type)
    {
        if (type == LibraryType.NONE)
        {
            btn_selectedEpisode.gameObject.SetActive(false);
            SetMarkerActive(true);
            txt_currentEpisode.text = "Episode";
            return;
        }

        btn_selectedEpisode.gameObject.SetActive(true);

        string s = type.ToString().Substring(6);
        txt_currentEpisode.text = s;
       //debug
        debug_txt_currentLibrary.text = "CurrentLibrary: " + type.ToString();
    }

    #endregion

    /// <summary>
    /// 3/18/2024-LYI
    /// Call from HeadersLibrary
    /// 현재 선택된 이미지 텍스트변경
    /// </summary>
    /// <param name="trackedImage"></param>

    public void ChangeCurrentReferenceImageText(ARTrackedImage trackedImage = null)
    {
        if (trackedImage == null)
        {
            txt_currentImage.text = "CurrentImage: NULL";
        }
        else
        {
            txt_currentImage.text = "CurrentImage: " + trackedImage.referenceImage.name;
        }
    }

    /// <summary>
    /// 3/18/2024-LYI
    /// 스크롤뷰 선택 시 표시 변경
    /// </summary>
    public void ToggleScrollView()
    {
        gameMgr.ui_popup.CheckUIPopupActive();

        scrollView_episode.gameObject.SetActive(true);
        if (isScrollOpen)
        {
            gameMgr.statGame = GameState.ARCUBE;
            gameMgr.ChangeBGM(gameMgr.statGame);

            mmf_scrollClose.PlayFeedbacks();
            btn_scrollBG.gameObject.SetActive(false);
            isScrollOpen = false;

            gameMgr.arSession.enabled = true;
            gameMgr.xrOrigin.enabled = true;

           // ChangeCurrentLibraryText(gameMgr.libraryHeader.currentLibrary);
        }
        else
        {
            gameMgr.statGame = GameState.SELECT;
            gameMgr.ChangeBGM(gameMgr.statGame);

            mmf_scrollOpen.PlayFeedbacks();
            btn_scrollBG.gameObject.SetActive(true);
            isScrollOpen = true;

            gameMgr.arSession.enabled = false;
            gameMgr.xrOrigin.enabled = false;

           // ChangeCurrentLibraryText(LibraryType.NONE);
        }
    }
    public void ToggleScrollView(bool isUp)
    {
        if (isUp == isScrollOpen)
        {
            return;
        }
        isScrollOpen = !isUp;

        scrollView_episode.gameObject.SetActive(true);
        if (isScrollOpen)
        {
            ChangeCurrentLibraryText(gameMgr.libraryHeader.currentLibrary);

            gameMgr.statGame = GameState.ARCUBE;
            gameMgr.ChangeBGM(gameMgr.statGame);

            mmf_scrollClose.PlayFeedbacks();
            btn_scrollBG.gameObject.SetActive(false);
            isScrollOpen = false;

            gameMgr.arSession.enabled = true;
            gameMgr.xrOrigin.enabled = true;
        }
        else
        {
            ChangeCurrentLibraryText(LibraryType.NONE);

            gameMgr.statGame = GameState.SELECT;
            gameMgr.ChangeBGM(gameMgr.statGame);

            mmf_scrollOpen.PlayFeedbacks();
            btn_scrollBG.gameObject.SetActive(true);
            isScrollOpen = true;

            gameMgr.arSession.enabled = false;
            gameMgr.xrOrigin.enabled = false;
        }
    }


    /// <summary>
    /// 3/25/2024-LYI
    /// 마커 활성화 변경
    /// </summary>
    /// <param name="isActive"></param>
    public void SetMarkerActive(bool isActive)
    {
        titleMarker.gameObject.SetActive(isActive);
    }

    public void ButtonShop()
    {
        //추후 상점 관련 안내창 띄우기
        //안내창에서 누르면 인터넷 브라우저로 스마트 스토어 웹페이지 띄우기
    }

}
