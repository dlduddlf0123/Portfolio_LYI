using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO
/// 게임 조작 정상화
/// UI 디자인 변경 필요
/// 효과음과 이펙트 넣을 
/// </summary>
public class TireRunInteract : InteractionManager
{
    AudioSource m_audio;

    public RunningTire player; //Wheel + Headers

    public Material loopTerrainMat;

    public Transform loopTree;
    public Transform treePool;

    public Queue<GameObject> queue_tree = new Queue<GameObject>();

    public GameObject prefab_tree_coll;

    public float moveSpeed = 2f;
    const float maxTime = 100f;
    public float gameTime = maxTime;
    public bool isGame = false;

    public int gameOverCount = 0;


    //UI
    public RectTransform uiCanvas;
    public GameObject[] arr_heartUI;
    public Text txt_timer;
    public GameObject txt_gameOver;

    public Button btn_start;
    public Button btn_restart;
    public Button btn_skip;

    protected override void DoAwake()
    {
        m_audio = GetComponent<AudioSource>();
        arr_heartUI = new GameObject[3];
        for (int i = 0; i < uiCanvas.GetChild(0).childCount; i++)
        {
            arr_heartUI[i] = uiCanvas.GetChild(0).GetChild(i).gameObject;
        }
        txt_timer = uiCanvas.GetChild(1).GetComponent<Text>();
    }

    private void Start()
    {
        loopTerrainMat.mainTextureOffset = Vector3.zero;

        txt_gameOver.SetActive(false);
        btn_start.gameObject.SetActive(true);
        btn_skip.gameObject.SetActive(false);
        btn_restart.gameObject.SetActive(false);
    }

    private void Update()
    {
        loopTerrainMat.mainTextureOffset += Vector2.up * moveSpeed * Time.deltaTime;
        loopTree.transform.localPosition += Vector3.forward  * 10f * moveSpeed * Time.deltaTime;
    }


    IEnumerator SpawnTree()
    {
        while (isGame)
        {
            GameObject _tree;
            if (queue_tree.Count == 0)
            {
                _tree = Instantiate(prefab_tree_coll, loopTree);
                _tree.GetComponent<TreeRunColl>().runMgr = this;
                queue_tree.Enqueue(_tree);
            }
            else
            {
                _tree = queue_tree.Dequeue();
                _tree.SetActive(true);
            }

            _tree.transform.position = treePool.GetChild(Random.Range(0, 2)).transform.position + Vector3.right * Random.Range(-1f, 1f);
            _tree.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 1f), 0);
            _tree.transform.localScale = Vector3.one * Random.Range(0.4f, 0.6f);

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }


    IEnumerator RunGameTimer()
    {
        while (isGame &&
            gameTime >0)
        {
            gameTime -= 1;
            txt_timer.text =  gameTime.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        if (isGame && player.hp != 0)
        {
            GameClear();
        }
    }

    public void SetHeartUI(int _hp)
    {
        //2,1,0 GameOver
        if (_hp <= arr_heartUI.Length)
        {
            for (int i = _hp; i < arr_heartUI.Length; i++)
            {
                arr_heartUI[i].SetActive(false);
            }
        }
    }


    public void GameStart()
    {
        Debug.Log("Run Game Start!");
        isGame = true;

        uiCanvas.gameObject.SetActive(true);
        for (int i = 0; i < arr_heartUI.Length; i++)
        {
            arr_heartUI[i].SetActive(true);
        }

        player.hp =player.maxHP;
        gameTime = maxTime;
        txt_timer.text = gameTime.ToString();
        txt_gameOver.SetActive(false);

        StartCoroutine(SpawnTree());
        StartCoroutine(RunGameTimer());
    }

    public void GameClear()
    {
        Debug.Log("Run Game Clear!");
        isGame = false;
        EndInteraction();
    }

    public void GameOver()
    {
        StopAllCoroutines();
        Debug.Log("Run Game Over!");
        txt_gameOver.SetActive(true);
        isGame = false;
        gameOverCount++;

        btn_restart.gameObject.SetActive(true);
        if (gameOverCount > 2)
        {
            btn_skip.gameObject.SetActive(true);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();

        gameMgr.soundMgr.ChangeBGMAudioSource(m_audio);
        gameMgr.soundMgr.PlayBgm();

        gameMgr.currentEpisode.currentStage.gameObject.SetActive(false);
        transform.GetChild(0).GetComponent<Animator>().SetBool("isRoll", true);

        player.ResetGyro();

        txt_gameOver.SetActive(false);
        btn_start.gameObject.SetActive(true);
        btn_skip.gameObject.SetActive(false);
        btn_restart.gameObject.SetActive(false);
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        transform.GetChild(0).GetComponent<Animator>().SetBool("isRoll", false);

        uiCanvas.gameObject.SetActive(false);
        gameMgr.currentEpisode.currentStage.currentInteraction++;
        gameMgr.soundMgr.ChangeBGMAudioSource(gameMgr.currentEpisode.currentStage.m_audioSource);

        Debug.Log(gameObject.name + "EndInteraction");
        gameMgr.statGame = GameStatus.CUTSCENE;
        GetComponent<UnityEngine.Playables.PlayableDirector>().Play();
    }

    public void GoBackToStage()
    {
        gameMgr.currentEpisode.currentStage.gameObject.SetActive(true);
        gameMgr.currentEpisode.currentStage.PlayCutscene(2);
        gameObject.SetActive(false);
    }
}