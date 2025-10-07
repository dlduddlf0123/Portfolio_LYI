using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainInteraction : InteractionManager
{
    Character header;
    Collider coll;

    AudioSource m_rainAudio;
    Light mainLight;

    public ParticleSystem rainParticle;
    public GameObject umbrella;

    public RogoDigital.Lipsync.LipSyncData[] arr_kantoLip;


    Coroutine currentCoroutine = null;

    protected override void DoAwake()
    {
        coll = GetComponent<Collider>();
        header = gameMgr.currentEpisode.currentStage.arr_header[0];
        mainLight = gameMgr.mainLight;
        m_rainAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (umbrella.activeSelf && gameMgr.statGame == GameStatus.INTERACTION)
        {
            umbrella.transform.position = gameMgr.handCtrl.handFollower.transform.position + Vector3.up;
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
            gameMgr.currentEpisode.currentStage.currentInteraction == 0 &&
            gameMgr.handCtrl.manoHandMove.handSide != HandSide.Palmside)
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
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(KantoCrawlingDown());

            gameMgr.uiMgr.worldCanvas.StopTimer();

            umbrella.SetActive(false);

            gameMgr.handCtrl.handFollower.ToggleHandEffect(false);

            PlayGuideParticle();
        }
    }

    IEnumerator LightBrighter()
    {
        float _t = 0;
        while (_t < 0.6f)
        {
            _t += 0.01f;
            mainLight.intensity = _t;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        stageMgr.StopAllCoroutines();

        mainLight = gameMgr.mainLight;
        mainLight.intensity = 0;
        coll.enabled = true;
        //transform.position = gameMgr.currentEpisode.currentStage.header.transform.GetChild(0).position + Vector3.up * 3f;
        //손 높이 고정
        //gameMgr.handCtrl.isHandFix = true;
        //  gameMgr.handCtrl.handCollHeight = gameMgr.currentEpisode.currentStage.header.transform.position.y + 3f;

        gameMgr.soundMgr.ChangeBGMAudioSource(m_rainAudio);

        //칸토 동작 변경
        header.ChangeIdleAnimation(4);
        umbrella.gameObject.SetActive(false);

        list_guidePosition.Add(transform.position);
        PlayGuideParticle();

        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.FRONT);
    }

    public override void EndInteraction()
    {
        StopAllCoroutines();

        header.SetAnim(0);
        coll.enabled = false;
       StartCoroutine( gameMgr.LateFunc(() => umbrella.SetActive(false), 3));
        

        gameMgr.uiMgr.worldCanvas.StopTimer();

        rainParticle.Stop();
        StopGuideParticle();

        gameMgr.soundMgr.ChangeBGMAudioSource(stageMgr.m_audioSource);
        gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_BGM_EPISODE01));
        m_rainAudio.Stop();

        base.EndInteraction();

        StartCoroutine(LightBrighter());
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

}
