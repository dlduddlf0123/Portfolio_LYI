using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 떨어지는 공 막기
/// 캐릭터는 좌우로 움직이고 하늘에서 공이 랜덤으로 떨어진다
/// 손을 따라 움직이는 판넬로 공을 막아야 한다
/// </summary>
public class HelpBallMiniGame : MiniGame
{
    [Header("HelpBall")]
    public  HelpBallHeaderAI header;

    //originalPrefab
    public GameObject prefab_ball;

    Queue<GameObject> queue_ballPool = new Queue<GameObject>();
    public Transform[] arr_ballPos;
    public Transform[] arr_ballParent;

    public RogoDigital.Lipsync.LipSyncData[] arr_kantoLip;

    Coroutine currentCoroutine = null;

    public float spawnTime = 0.3f;

    protected override void DoAwake()
    {

    }

    public override void GameInit()
    {
        base.GameInit();

      //  limitTime = 30;
    }



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.GAMEPLAY &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 0)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(KantoStandUp());

            gameMgr.uiMgr.worldCanvas.StartTimer(transform.position + Vector3.up * 0.1f, 1f, () =>
            {
                gameMgr.currentEpisode.currentStage.EndInteraction();
            });


            // gameMgr.handCtrl.handFollower.ToggleHandEffect(true);

            StopGuideParticle();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(KantoCrawlingDown());

            gameMgr.uiMgr.worldCanvas.StopTimer();



            //  gameMgr.handCtrl.handFollower.ToggleHandEffect(false);

            PlayGuideParticle();
        }
    }


    //ㄱㅏ리는 도중에 좋아하는 모션, 손 떼면 다시 눕기
    IEnumerator KantoStandUp()
    {
        yield return new WaitForSeconds(0.5f);
        header.m_animator.SetTrigger("Episode01_StandUp");
        header.SetAnim(0);
    }

    IEnumerator KantoCrawlingDown()
    {
        yield return new WaitForSeconds(0.5f);
        header.m_animator.SetTrigger("Episode01_CrawlingDown");
        header.ChangeIdleAnimation(4);
    }

    /// <summary>
    /// 공 뿌리기
    /// </summary>
    void SpamBall()
    {
        StartCoroutine(SpawnBall());
    }

    IEnumerator SpawnBall()
    {
        while (gameMgr.statGame == GameStatus.GAMEPLAY)
        {
            if (queue_ballPool.Count == 0)
            {
                GameObject _go = Instantiate(prefab_ball);
                _go.transform.SetParent(arr_ballParent[1]);
                _go.transform.position = arr_ballPos[Random.Range(0, arr_ballPos.Length)].position;

                _go.GetComponent<HelpBallPrefab>().onDamage = () => BallDamage(_go);
                _go.GetComponent<HelpBallPrefab>().onDestroy = () => BallInit(_go);
            }
            else
            {
                GameObject _go = queue_ballPool.Dequeue();
                _go.transform.SetParent(arr_ballParent[1]);
                _go.transform.position = arr_ballPos[Random.Range(0, arr_ballPos.Length)].position;
                _go.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    void BallInit(GameObject _go)
    {
        _go.transform.SetParent(arr_ballParent[0]);
        queue_ballPool.Enqueue(_go);
        _go.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _go.gameObject.SetActive(false);
    }

    void BallDamage(GameObject _go)
    {
        LoseLife();

        BallInit(_go);
    }

    public override void StartMiniGame()
    {
        base.StartMiniGame();


        //list_guidePosition.Add(transform.position);
        //PlayGuideParticle();

        //gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.BACK);

        //데이터 전달
        header.arr_movePoint = arr_ballPos;

        SpamBall();

    }

    public override void EndMiniGame()
    {
        StopAllCoroutines();

        header.SetAnim(0);


        base.EndMiniGame();
    }

}
