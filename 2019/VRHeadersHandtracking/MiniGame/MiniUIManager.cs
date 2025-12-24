using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MiniUIManager : MonoBehaviour
{
    public RectTransform startUI { get; set; }
    public RectTransform ingameUI { get; set; }
    public Text ingameUI_Time { get; set; }
    public RectTransform gameoverUI { get; set; }
    public RectTransform clearUI { get; set; }

    public Spawner spawner;

    public float timeLimit; //게임 제한시간

    void Awake()
    {
        startUI = transform.GetChild(0).GetComponent<RectTransform>();

        ingameUI = transform.GetChild(1).GetComponent<RectTransform>();
        ingameUI_Time = transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();

        gameoverUI = transform.GetChild(2).GetComponent<RectTransform>();

        clearUI = transform.GetChild(3).GetComponent<RectTransform>();
        
    }

    void Start()
    {
        startUI.gameObject.SetActive(true);
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawner.gameState == GameState.PLAYING)
        {
            if (timeLimit >= -0.1f)
            {
                timeLimit -= Time.deltaTime;
                if (timeLimit < 0)
                {
                    spawner.gameState = GameState.CLEAR;
                    spawner.GameChange(GameState.CLEAR);
                }
            }
            ingameUI_Time.text = Mathf.Ceil(timeLimit).ToString();
        }
    }

    //손 입력
    public void ActionStart(int state)
    {
        if (spawner.gameState == GameState.NONE)
        {
            startUI.gameObject.SetActive(false);
            this.GameStart();
        }
        if (spawner.gameState == GameState.GAMEOVER ||
            spawner.gameState == GameState.CLEAR)
        {
            spawner.HandEffect(0);
            spawner.HandEffect(1);
            SceneManager.LoadScene(1);
        }
    }

    public void GameStart()
    {
        spawner.HandEffect(0);
        spawner.HandEffect(1);
        spawner.soundMgr.PlayBgm(Resources.Load<AudioClip>("Sounds/Rush"));
        spawner.GameChange(GameState.PLAYING);
        spawner.gameState = GameState.PLAYING;
        ingameUI.gameObject.SetActive(true);

    }
    public void GameOver()
    {
        spawner.soundMgr.PlaySfx(spawner.transform.position, spawner.soundMgr.sfx_gameover);
        gameoverUI.gameObject.SetActive(true);
        ingameUI.gameObject.SetActive(false);
    }
    public void GameClear()
    {
        spawner.soundMgr.PlaySfx(spawner.transform.position, spawner.soundMgr.sfx_success);
        clearUI.gameObject.SetActive(true);
        ingameUI.gameObject.SetActive(false);
    }
}

