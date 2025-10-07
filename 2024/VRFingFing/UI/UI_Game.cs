using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

/// <summary>
/// 7/13/2023-LYI
/// 게임 플레이중 정보 표시 UI
/// </summary>
public class UI_Game : MonoBehaviour
{
    GameManager gameMgr;

    [SerializeField]
    TextMeshProUGUI txt_title; //현재 스테이지 이름
    [SerializeField]
    TextMeshProUGUI txt_tokCount; //클릭 횟수 제한 표시
    [SerializeField]
    TextMeshProUGUI txt_time; //시간 제한 표시

    [SerializeField]
    Button btn_pause; //일시정지

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        btn_pause.onClick.AddListener(PauseButton);
    }

    public void PauseButton()
    {
        //gameMgr.playMgr.uiMgr.ui_pause.transform.GetChild(0).gameObject.SetActive(true);
       // gameMgr.playMgr.uiMgr.ui_pause.interactable.enabled = true;
    }

    public void ChangeStageText(string stageName, int stageNum)
    {
        txt_title.text = stageName + stageNum;
    }


    /// <summary>
    /// 8/24/2023-LYI
    /// 시간 활성화 여부 변경
    /// </summary>
    /// <param name="isActive">활성화 할 것인가</param>
    public void ChangeTimeActive(bool isActive)
    {
        txt_time.gameObject.SetActive(isActive);
    }
    /// <summary>
    /// 7/17/2023-LYI
    /// 다른 함수에서 호출용 텍스트 변경 함수
    /// </summary>
    /// <param name="time"></param>
    public void ChangeTimeText(int time)
    {
        txt_time.text = "Time: " + time;
    }
    /// <summary>
    /// 7/17/2023-LYI
    /// 다른 함수에서 호출용 텍스트 변경 함수
    /// </summary>
    public void ChangeTokCountText(int count, int maxCount)
    {
        txt_tokCount.text = "TokCount: " + count + "/" + maxCount;
    }

}
