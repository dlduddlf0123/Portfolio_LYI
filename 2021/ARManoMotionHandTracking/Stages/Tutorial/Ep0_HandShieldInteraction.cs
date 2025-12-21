using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ep0_HandShieldInteraction : InteractionManager
{
    Character header;
    Collider coll;
    AudioSource m_rainAudio;

    Queue<GameObject> queue_ball;
    public GameObject ball;
    public GameObject umbrella;
    public Transform ballPool;

    public RogoDigital.Lipsync.LipSyncData[] arr_kantoLip;


    Coroutine currentCoroutine = null;

    protected override void DoAwake()
    {
        coll = GetComponent<Collider>();
        coll.enabled = false; 
        header = gameMgr.currentEpisode.currentStage.arr_header[0];
        m_rainAudio = GetComponent<AudioSource>();
        queue_ball = new Queue<GameObject>();
        e_handIcon = HandIcon.BACK;
        umbrella.gameObject.SetActive(false);
    }

    void Update()
    {
        if (umbrella.activeSelf && gameMgr.statGame == GameStatus.INTERACTION)
        {
            umbrella.transform.position = gameMgr.handCtrl.handFollower.transform.position + Vector3.up * 0.7f;
        }
    }
    public override IEnumerator DialogWaitTime()
    {
        gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.gameObject.SetActive(true);
        while (gameMgr.statGame == GameStatus.INTERACTION &&
            arr_LoopDialog.Length > 0)
        {
            //  int rand = Random.Range(0, arr_LoopDialog.Length);
            for (int i = 0; i < arr_LoopDialog.Length; i++)
            {
                yield return new WaitForSeconds(5f);
                gameMgr.currentEpisode.currentStage.arr_header[0].headerCanvas.ShowText(arr_LoopDialog[i], 5);
                gameMgr.currentEpisode.currentStage.arr_header[0].m_lipSync.Play(arr_kantoLip[i]);
                yield return new WaitForSeconds(5f);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION &&
            gameMgr.handCtrl.manoHandMove.handSide != HandSide.Palmside)
        {
            if (queue_ball.Count == 0)
            {
                GameObject go = Instantiate(ball, ballPool.transform);
                go.transform.position = transform.position + Vector3.up * 15f;
                go.transform.localScale = Vector3.one * gameMgr.uiMgr.stageSize;
                StartCoroutine(gameMgr.LateFunc(() => {
                    go.gameObject.SetActive(false);
                    queue_ball.Enqueue(go);
                }, 5));
            }
            else
            {
                GameObject go = queue_ball.Dequeue();
                go.transform.position = transform.position + Vector3.up * 15f;
                go.GetComponent<Rigidbody>().velocity = Vector3.zero;
                go.SetActive(true);
                StartCoroutine(gameMgr.LateFunc(() =>
                {
                    go.gameObject.SetActive(false);
                    queue_ball.Enqueue(go);
                }, 5));
            }

            gameMgr.uiMgr.worldCanvas.StartTimer(transform.position + Vector3.up * 0.1f, 3f, () =>
            {
                gameMgr.currentEpisode.currentStage.EndInteraction();
            });

            umbrella.SetActive(true);

            gameMgr.handCtrl.handFollower.ToggleHandEffect(true);

            StopGuideParticle();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            gameMgr.statGame == GameStatus.INTERACTION)
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();

            umbrella.SetActive(false);

            gameMgr.handCtrl.handFollower.ToggleHandEffect(false);

            PlayGuideParticle();
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        stageMgr.StopAllCoroutines();

        coll.enabled = true;

        ballPool.gameObject.SetActive(true);
        umbrella.gameObject.SetActive(false);

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        header.SetAnim(0);
        coll.enabled = false;

        ballPool.gameObject.SetActive(false);
        StartCoroutine(gameMgr.LateFunc(() => umbrella.SetActive(false), 3));


        gameMgr.uiMgr.worldCanvas.StopTimer();

        StopGuideParticle();

        base.EndInteraction();
    }
}
