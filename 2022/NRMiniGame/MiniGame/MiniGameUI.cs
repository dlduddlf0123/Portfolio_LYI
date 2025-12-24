using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
public class MiniGameUI : MonoBehaviour
{
    GameManager gameMgr;

    //GameUI
    public GameObject stage_gameUI;
    public TextMeshProUGUI game_text_time { get; set; }
    public TextMeshProUGUI game_text_score { get; set; }
    public Image[] arr_game_img_life { get; set; }

    void Awake()
    {
        gameMgr = GameManager.Instance;

        //GameUI
        game_text_time = stage_gameUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        arr_game_img_life = stage_gameUI.transform.GetChild(1).GetComponentsInChildren<Image>();
        //game_text_score = stage_gameUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();


        UIInit();
    }

    public virtual void UIInit()
    {
        //게임 플레이 관련
        GameUIChangeScoreText(0);
        GameUIChangeTimerText(0);
        GameUIChangeLifeIcon(3);
    }
    public void GameUIChangeScoreText(int score)
    {
        if (game_text_score == null)
        {
            Debug.Log(this.gameObject.name + ": dosen't have score!");
            return;
        }
        game_text_score.text = "Score: " + score.ToString();
    }
    public void GameUIChangeTimerText(int time)
    {
        if (game_text_time == null)
        {
            Debug.Log(this.gameObject.name + ": dosen't have time!");
            return;
        }
        game_text_time.text = "Time: " + time.ToString();
    }

    /// <summary>
    /// 체력 아이콘 표시 변경
    /// </summary>
    public void GameUIChangeLifeIcon(int life)
    {
        if (arr_game_img_life == null)
        {
            Debug.Log(this.gameObject.name + ": dosen't have life!");
            return;
        }
        if (life > arr_game_img_life.Length)
        {
            Debug.LogError("Theres to many Life");
            return;
        }
        for (int i = 0; i < arr_game_img_life.Length; i++)
        {
            arr_game_img_life[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < life; i++)
        {
            arr_game_img_life[i].gameObject.SetActive(true);
        }
    }

}
