using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    GameManager gameMgr;

    //child(0) Title UI
    public RectTransform titleUI { get; set; }
    public Button title_bt_start { get; set; }
    //child(3) Music Select UI
    public RectTransform musicSelectUI { get; set; }
    public Button select_bt_music { get; set; }
    public Text select_text_songName { get; set; }
    public Button select_bt_left { get; set; }
    public Button select_bt_right { get; set; }
    public Button select_bt_start { get; set; }
    public Text select_score { get; set; }
    public Button select_bt_tutorial { get; set; }

    public int currentSongNum = 1;

    // Start is called before the first frame update
    void Awake()
    {
        gameMgr = GameManager.Instance;

        //Title UI
        titleUI = this.transform.GetChild(0).GetComponent<RectTransform>();
        title_bt_start = titleUI.transform.GetChild(2).GetComponent<Button>();
        title_bt_start.onClick.AddListener(() => { musicSelectUI.gameObject.SetActive(true); titleUI.gameObject.SetActive(false); });
        //Music Select UI
        musicSelectUI = this.transform.GetChild(3).GetComponent<RectTransform>();
        select_bt_music = musicSelectUI.transform.GetChild(2).GetComponent<Button>();
        select_text_songName = select_bt_music.transform.GetChild(0).GetComponent<Text>();
        select_bt_left = musicSelectUI.transform.GetChild(3).GetComponent<Button>();
        select_bt_right = musicSelectUI.transform.GetChild(4).GetComponent<Button>();
        select_bt_start = musicSelectUI.transform.GetChild(5).GetComponent<Button>();
        select_score = musicSelectUI.transform.GetChild(7).GetComponent<Text>();
        select_bt_tutorial = musicSelectUI.transform.GetChild(9).GetComponent<Button>();

        //Character Select UI

    }
    private void Start()
    {
        select_text_songName.text = gameMgr.table_songdata.Row[currentSongNum].Col[0].ToString();
        select_bt_left.onClick.AddListener(() => { MusicChoice(1); });
        select_bt_right.onClick.AddListener(() => { MusicChoice(2); });
        select_bt_start.onClick.AddListener(() => { gameMgr.LoadScene(1); musicSelectUI.gameObject.SetActive(false); });
        select_score.text = "High Score : " + gameMgr.highScore;
        select_bt_tutorial.onClick.AddListener(() => { musicSelectUI.gameObject.SetActive(false); });
    }

    /// <summary>
    /// 1은 왼쪽버튼, 2는 왼쪽버튼
    /// </summary>
    /// <param name="i"></param>
    void MusicChoice(int i)
    {
        if(i==1)
        {
            currentSongNum--;
            if(currentSongNum ==0)
            {
                currentSongNum = gameMgr.openSongNum;
            }
            select_text_songName.text = gameMgr.table_songdata.Row[currentSongNum].Col[0].ToString();
        }
        if(i==2)
        {
            currentSongNum++;
            if(currentSongNum == gameMgr.openSongNum +1)
            {
                currentSongNum = 1;
            }
            select_text_songName.text = gameMgr.table_songdata.Row[currentSongNum].Col[0].ToString();
        }
    }

}
