using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


/// <summary>
/// 3/13/2024-LYI
/// 버튼 상태 구분용 enum
/// </summary>
public enum ButtonStatus
{
    NONE= 0,
    LOCK, //잠김 상태, 다운로드 가능
    LOADING, //다운로드 시작 전, 클릭한 뒤 불러오기 로딩 보여주기
    DOWNLOAD, //다운로드 도중일 때
    OPEN, //다운로드 완료, 클릭 가능한 상태, 클릭 시 로딩 시작
}

/// <summary>
/// 3/13/2024-LYI
/// 로딩 상태 등 버튼의 활성화 상태를 제어
///  
/// </summary>
public class UI_EpisodeButton : MonoBehaviour
{
    GameManager gameMgr;

    public Button btn_episode;

    public Image img_downloadBG;
    public Button btn_download;

    [SerializeField]
    GameObject go_downloadPercent;
    [SerializeField]
    Image img_loadGauge;
    [SerializeField]
    TextMeshProUGUI txt_loadPercent;

    [SerializeField]
    Image img_loadingRotate;



    public ButtonStatus statButton = ButtonStatus.NONE;
    public LibraryType libraryType = LibraryType.NONE;


    /// <summary>
    /// 3/13/2024-LYI
    /// 앱 실행 시 버튼 기능 할당하며 호출
    /// 버튼이 다운로드된 상태인지 체크하고 최초 보여줄 상태 변경
    /// </summary>
    public void Init()
    {
        gameMgr = GameManager.Instance;

        ChangeDownloadPercent(0);
        btn_download.onClick.AddListener(ButtonDownload);
        ChangeButtonStatus(ButtonStatus.LOCK);
    }



    /// <summary>
    /// 3/13/2024-LYI
    /// 버튼의 상태와 표시 변경
    /// </summary>
    /// <param name="stat"></param>
    public void ChangeButtonStatus(ButtonStatus stat)
    {
        statButton = stat;
        //에피소드 버튼 비활성
        btn_episode.enabled = false;
        //검은 배경 비활성, 다운로드 버튼 비활성
        img_downloadBG.gameObject.SetActive(false);
        btn_download.gameObject.SetActive(false);
        //다운로드 퍼센트 표시 비활성
        go_downloadPercent.SetActive(false);
        //로딩 표시 비활성
        img_loadingRotate.gameObject.SetActive(false);

        switch (stat)
        {
            case ButtonStatus.DOWNLOAD:
                go_downloadPercent.SetActive(true);
                img_downloadBG.gameObject.SetActive(true);
                btn_download.gameObject.SetActive(true);
                break;
            case ButtonStatus.OPEN:
                btn_episode.enabled = true;
                break;
            case ButtonStatus.LOADING:
                img_downloadBG.gameObject.SetActive(true);
                img_loadingRotate.gameObject.SetActive(true);
                break;
            case ButtonStatus.LOCK:
            case ButtonStatus.NONE:
            default:
                img_downloadBG.gameObject.SetActive(true);
                btn_download.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 3/13/2024-LYI
    /// 로딩 표시 변경
    /// </summary>
    /// <param name="percent">0~1% fillAmount</param>
    public void ChangeDownloadPercent(float percent)
    {
        img_loadGauge.fillAmount = percent;
        txt_loadPercent.text = (int)(percent * 100) + "%";
    }

    /// <summary>
    /// 3/14/2024-LYI
    /// 다운로드 버튼을 눌렀을 경우
    /// 최초 체크 시 어드레서블이 없을 경우에 다운로드 상태가 되며 작동
    /// </summary>
    public void ButtonDownload()
    {
        //인터넷 상태 확인
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //인터넷을 사용할 수 없는 경우
            //팝업창, 돌려보내기
            Debug.Log("Internet Offline!");
            gameMgr.ui_popup.OpenWarningPopup();
        }
        //else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        //{
        //    //Wi-fi인 경우 다운로드 진행
        //    gameMgr.addressableMgr.BundleDownLoad(this);
        //    ChangeButtonStatus(ButtonStatus.DOWNLOAD);
        //}
        else
        {
            //셀룰러 데이터인 경우
            //데이터 용량 체크이후 팝업창 호출
            //팝업창 버튼에 다운로드 시작 할당

            //3/25/2024-LYI
            //앱 시작 시 다운로드 사이즈를 계산하므로 계산 생략
            //다운로드 계산으로 인한 딜레이로 코드 꼬임 발생
            gameMgr.ui_popup.CheckUIPopupActive();
            if (gameMgr.ui_popup.isPopupActive)
            {
                Debug.Log("UI Popup is Active");
                return;
            }

            string title = libraryType.ToString().Substring(6);

            long downloadSize = 0;

            //다운로드 여부 체크
            if (gameMgr.addressableMgr.dic_downloadSize.ContainsKey(libraryType))
            {
                downloadSize = gameMgr.addressableMgr.dic_downloadSize[libraryType];
                if (downloadSize == 0)
                {
                    //다운로드 버튼 상태가 아님??
                    Debug.Log("오류!! 이미 다운로드 된 상태입니다. 에셋을 로드합니다");
                    gameMgr.ui_librarySelect.ButtonEpisodeSelect(this);
                }
                else
                {
                    int MBsize = (int)(downloadSize / 1048576);

                    gameMgr.ui_popup.PopupMessage(title, MBsize, () => gameMgr.addressableMgr.BundleDownLoad(this));
                }
            }
            else
            {
                //사이즈 체크 안했으면
                gameMgr.addressableMgr.CheckDownloadSize(libraryType,
                (MB) =>
                {
                    gameMgr.ui_popup.PopupMessage(title, MB,
                        () => gameMgr.addressableMgr.BundleDownLoad(this));
                });
            }
        }

    }
}

    //void CheckDownload(UI_EpisodeButton button)
    //{
    //    LibraryType type = button.libraryType;

    //    //다운로드 안됐으면 팝업
    //    if (Application.internetReachability == NetworkReachability.NotReachable)
    //    {
    //        //인터넷을 사용할 수 없는 경우
    //        //팝업창, 돌려보내기
    //        Debug.Log("Internet Offline!");
    //        gameMgr.ui_popup.OpenWarningPopup();
    //    }
    //    else
    //    {
    //        //셀룰러 데이터인 경우
    //        //데이터 용량 체크이후 팝업창 호출
    //        //팝업창 버튼에 다운로드 시작 할당
    //        string title = type.ToString().Substring(6);

    //        gameMgr.addressableMgr.CheckDownloadSize(type,
    //            (MB) =>
    //            {
    //                gameMgr.ui_popup.PopupMessage(title, MB,
    //                    () => gameMgr.addressableMgr.BundleDownLoad(button));
    //            });
    //    }
    //}


