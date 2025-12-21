using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using RogoDigital.Lipsync;
using Pathfinding;

using System;

/// <summary>
///   각 스테이지의 시작, 종료, 컷씬, 상호작용을 관리한다.
///   이 클래스를 상속받아 각 스테이지를 구성한다.
/// </summary>
public class StageManager : MonoBehaviour
{
    //외부 함수
    protected GameManager gameMgr;
    public EpisodeManager episodeMgr;

    //지형 관련
    public AstarPath ApathMgr;
    protected RecastGraph recastGraph;

    //타임라인, 캐릭터
    public PlayableDirector m_director;
    public Character[] arr_header;

    //사운드
    public AudioSource m_audioSource;
    protected AudioClip bgm_stage;

    //리스트들
    public List<TimelineAsset> list_timeline = new List<TimelineAsset>();   //타임라인 리스트
    public List<List<object>> list__timelineDialog = new List<List<object>>();  //대사 리스트
    public List<List<List<LipSyncData>>> list___currentLipData = new List<List<List<LipSyncData>>>();    //타임라인 번호 + 현재 대사 번[ + 캐릭터 수

    public List<InteractionManager> list_interaction = new List<InteractionManager>();  //상호작용 리스트

    public List<GameObject> list_terrain = new List<GameObject>(); //관리할 터레인들
    public List<Transform> list_endPos = new List<Transform>(); //컷씬 종료 후 포지

    public Color[] arr_nameColor;

    //현재 진행상황 변수
    public int currentTimeline = 0;     //현재 재생중인 컷 씬 번호
    public int currentDialog = 0;         //컷 씬에서 현재 대사 번호
    public int currentInteraction = 0;  //현재 플레이 중인 상호작용 번호

    public int currentTerrain = 0; //현재 활성화 된 터레인 번호(미 사용 중)

    public Transform endingPos;

    protected string stageTitle = null;
    protected string stageSubTitle = null;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        m_director = GetComponent<PlayableDirector>();
        m_audioSource = GetComponent<AudioSource>();
        if (arr_header.Length == 0)
        {
            arr_header = transform.GetChild(0).GetComponentsInChildren<Character>();
        }

        DoAwake();
    }

    protected virtual void DoAwake() { }

    private void Update()
    {
        if (gameMgr.isDebug)
        {
            gameMgr.uiMgr.ui_game.debug_txt_timeline.text = "CamPos: " + gameMgr.arMainCamera.transform.parent.position;
            gameMgr.uiMgr.ui_game.debug_txt_interaction.text = "StagePos: " + transform.parent.position;
        }
    }

    public virtual void AstarScan()
    {
        recastGraph = ApathMgr.data.recastGraph;
        recastGraph.SnapForceBoundsToScene();
        ApathMgr.graphs[0].Scan();
    }

    public virtual void AstarScan(Vector3 _center, Vector3 _size)
    {
        recastGraph = ApathMgr.data.recastGraph;
        recastGraph.SnapForceBoundsToScene();
        recastGraph.forcedBoundsCenter += _center;
        recastGraph.forcedBoundsSize += _size;
        ApathMgr.graphs[0].Scan();

    }

    public virtual void SetLipSyncFiles()
    {
        int count = 0;
        for (int timelineIndex = 0; timelineIndex < list__timelineDialog.Count; timelineIndex += 2)
        {
            List<List<LipSyncData>> list_temp_voice = new List<List<LipSyncData>>();

            for (int dialogIndex = 1; dialogIndex < list__timelineDialog[timelineIndex].Count; dialogIndex++)
            {
                List<LipSyncData> temp_voice = new List<LipSyncData>();
                string[] _headerNum = list__timelineDialog[timelineIndex][dialogIndex].ToString().Split('&');

                for (int characterIndex = 0; characterIndex < _headerNum.Length; characterIndex++)
                {
                    temp_voice.Add(gameMgr.currentEpisode.arr_voice[count]);
                    count++;
                }
                list_temp_voice.Add(temp_voice);
            }

            list___currentLipData.Add(list_temp_voice);
        }
    }

    /// <summary>
    /// 헤더 캔버스에서 대사, 음성 출력
    /// </summary>
    /// <param name="_canvas"></param>
    public void NextDialog(HeaderCanvas _canvas)
    {
        if (gameMgr.statGame == GameStatus.CUTSCENE)
        {
            StageManager _stage = gameMgr.currentEpisode.currentStage;

            //캐릭터가 한명일 경우
            _canvas.ShowCutsceneText(list__timelineDialog, currentTimeline, currentDialog);
            currentDialog++;
        }
    }

    /// <summary>
    /// 2열 CSV 캐릭터구분, 대사구분 방식
    /// 어떤 캐릭터의 대사를 출력할지 결정되어있e
    /// </summary>
    /// <param name="_canvas"></param>
    public void NextDialogLip(HeaderCanvas _canvas)
    {
        if (gameMgr.statGame == GameStatus.CUTSCENE &&
            currentDialog + 1 < list__timelineDialog[currentTimeline * 2].Count)
        {
            StageManager _stage = gameMgr.currentEpisode.currentStage;

            if (IsMultipleCharacterTalk())
            {
                //캐릭터가 1명 이상 얘기할 경우 자르기, 반복
                MultipleCharacterTalk(_canvas);
            }
            else if (Convert.ToInt32(list__timelineDialog[currentTimeline * 2][currentDialog + 1]) == 9)
            {
                //현재 모든 대가리들 동시 대사 출력
                AllHeadersTalk(_canvas);
            }
            else
            {
                //캐릭터가 한명일 경우
                _canvas.ShowCutsceneText(list__timelineDialog, currentTimeline * 2 + 1, currentDialog);
                //if (gameMgr.currentEpisode.currentStage.list___currentLipData.Count != 0)
                //{
                //    _canvas.currentLipData = _stage.list___currentLipData[_stage.currentTimeline][_stage.currentDialog][0];
                //}
                currentDialog++;
            }
        }
    }

    public bool IsMultipleCharacterTalk()
    {
        if (arr_header.Length > 1)
        {
            return list__timelineDialog[currentTimeline * 2][currentDialog + 1].ToString().Contains("&");
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 캐릭터가 1명 이상 얘기할 경우 DialogNum진행 안하기, 반복
    /// </summary>
    /// <param name="_canvas"></param>
    void MultipleCharacterTalk(HeaderCanvas _canvas)
    {
        StageManager _stage = gameMgr.currentEpisode.currentStage;

        string[] _headerNum = list__timelineDialog[currentTimeline * 2][currentDialog + 1].ToString().Split('&');

        _canvas.ShowCutsceneText(list__timelineDialog, currentTimeline * 2 + 1, currentDialog);


        for (int i = 0; i < _headerNum.Length; i++)
        {
            if (gameMgr.currentEpisode.currentStage.list___currentLipData.Count != 0)
            {
                _canvas.currentLipData = _stage.list___currentLipData[_stage.currentTimeline][_stage.currentDialog][i];
            }

            //헤더캔버스가 캐릭터 수 만큼 활성화가 되었을 때 다음 대사로 넘길 조건
            //if (_canvas == _stage.arr_header[i].headerCanvas &&
            //    i == Convert.ToInt32(_headerNum[_headerNum.Length-1]))
            //{
            //}
        }
        currentDialog++;
    }

    /// <summary>
    /// 모든 대가리들 동시에 얘기하기
    /// 등장하지 않거나 등의 이유로 마지막 대가리는 포함되지 않는다 
    /// </summary>
    /// <param name="_canvas"></param>
    void AllHeadersTalk(HeaderCanvas _canvas)
    {
        StageManager _stage = gameMgr.currentEpisode.currentStage;

        //대사 출 AllHeadersTalk
        _canvas.ShowCutsceneText(list__timelineDialog, currentTimeline * 2 + 1, currentDialog);

        //음성 출력 반복
        //대가리들 마지막만 빼고
        for (int i = 0; i < arr_header.Length; i++)
        {
            if (gameMgr.currentEpisode.currentStage.list___currentLipData.Count != 0)
            {
                _canvas.currentLipData = _stage.list___currentLipData[_stage.currentTimeline][_stage.currentDialog][i];
            }

            //다음 대사로 넘길 조건
            if (_canvas == _stage.arr_header[i].headerCanvas &&
                i == arr_header.Length - 1)
            {
                currentDialog++;
            }
        }

    }


    /// <summary>
    /// 모든 대가리들 위치 조정
    /// </summary>
    public virtual void SetInteractionPos()
    {
        if (arr_header.Length > 1
            && currentTimeline < list_endPos.Count)
        {
            for (int i = 0; i < arr_header.Length; i++)
            {
                StartCoroutine(SmoothPosReset(i, list_endPos[currentTimeline].position));
                arr_header[i].transform.rotation = list_endPos[currentTimeline].rotation;
                arr_header[i].transform.GetChild(0).localPosition = Vector3.zero;
                arr_header[i].transform.GetChild(0).rotation = Quaternion.identity;
            }
        }
    }
    public IEnumerator SmoothPosReset(int _headerNum, Vector3 _targetPos, bool isLocal = false)
    {
        float t = 0f;
        if (isLocal)
        {
            while (t < 1f)
            {
                arr_header[_headerNum].transform.localPosition = Vector3.Lerp(arr_header[_headerNum].transform.localPosition, _targetPos, t);
                t += 0.02f;
                yield return new WaitForSeconds(0.01f);
            }
            arr_header[_headerNum].transform.localPosition = _targetPos;
        }
        else
        {
            while (t < 1f)
            {
                arr_header[_headerNum].transform.position = Vector3.Lerp(arr_header[_headerNum].transform.position, _targetPos, t);
                t += 0.02f;
                yield return new WaitForSeconds(0.01f);
            }
            arr_header[_headerNum].transform.position = _targetPos;
        }
    }
    public IEnumerator SmoothRotReset(int _headerNum, Quaternion _targetRot,bool isLocal = false)
    {
        float t = 0f;

        if (isLocal)
        {
            while (t < 1f)
            {
                arr_header[_headerNum].transform.localRotation = Quaternion.Lerp(arr_header[_headerNum].transform.localRotation, _targetRot, t);
                t += 0.02f;
                yield return new WaitForSeconds(0.01f);
            }
            arr_header[_headerNum].transform.localRotation = _targetRot;
        }
        else
        {

            while (t < 1f)
            {
                arr_header[_headerNum].transform.rotation = Quaternion.Lerp(arr_header[_headerNum].transform.rotation, _targetRot, t);
                t += 0.02f;
                yield return new WaitForSeconds(0.01f);
            }
            arr_header[_headerNum].transform.rotation = _targetRot;
        }
    }
    public IEnumerator SmoothTransformReset(int _headerNum, Transform _targetTr)
    {
        float t = 0f;
        while (t < 1f)
        {
            arr_header[_headerNum].transform.position = Vector3.Lerp(arr_header[_headerNum].transform.position, _targetTr.position, t);
            arr_header[_headerNum].transform.rotation = Quaternion.Lerp(arr_header[_headerNum].transform.rotation, _targetTr.rotation, t);
            t += 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        arr_header[_headerNum].transform.position = _targetTr.position;
        arr_header[_headerNum].transform.rotation = _targetTr.rotation;
    }


    #region =============Game Main Flow Func===========
    /// <summary>
    /// 컷 씬 재생 시작
    /// </summary>
    /// <param name="_sceneNum"></param>
    public virtual void PlayCutscene(int _sceneNum)
    {
        if (list_timeline.Count <= _sceneNum)
        {
            Debug.Log("Dialog is null");
            if (gameMgr.currentEpisode.arr_stage.Length - 1 <= gameMgr.currentEpisode.currentStageNum)
            {
                EndStage();
            }
            else
            {
                gameMgr.currentEpisode.NextStage();
            }
            return;
        }
        Debug.Log(gameObject.name + " PlayCutscene:" + _sceneNum);

        gameMgr.statGame = GameStatus.CUTSCENE;
        //gameMgr.handCtrl.handColl.SetActive(false);

        gameMgr.uiMgr.ui_game.game_btn_skip.gameObject.SetActive(gameMgr.isDebug);

        gameMgr.uiMgr.UIGameTimelineFrameToggle(true);

        m_director.playableAsset = list_timeline[_sceneNum];
        m_director.Play();
    }
    public virtual void PlayCutscene()
    {
        if (list_timeline.Count <= currentTimeline)
        {
            Debug.Log("Dialog is null");
            if (gameMgr.currentEpisode.arr_stage.Length - 1 <= gameMgr.currentEpisode.currentStageNum)
            {
                EndStage();
            }
            else
            {
                gameMgr.currentEpisode.NextStage();
            }
            return;
        }
        Debug.Log(gameObject.name + " PlayCutscene: " + currentTimeline);

        gameMgr.statGame = GameStatus.CUTSCENE;
        //gameMgr.handCtrl.handColl.SetActive(false);
        gameMgr.uiMgr.ui_game.game_btn_skip.gameObject.SetActive(gameMgr.isDebug);
        gameMgr.uiMgr.UIGameTimelineFrameToggle(true);

        m_director.playableAsset = list_timeline[currentTimeline];
        m_director.Play();
    }
    public virtual void PlayCutscene(TimelineAsset _timeline)
    {
        if (list_timeline.Count <= currentTimeline)
        {
            Debug.Log("Dialog is null");
            if (gameMgr.currentEpisode.arr_stage.Length - 1 <= gameMgr.currentEpisode.currentStageNum)
            {
                EndStage();
            }
            else
            {
                gameMgr.currentEpisode.NextStage();
            }
            return;
        }
        Debug.Log(_timeline.name + "PlayCutscene");

        gameMgr.statGame = GameStatus.CUTSCENE;
        gameMgr.uiMgr.ui_game.game_btn_skip.gameObject.SetActive(gameMgr.isDebug);
        gameMgr.uiMgr.UIGameTimelineFrameToggle(true);

        m_director.playableAsset = _timeline;
        m_director.Play();
    }

    /// <summary>
    /// 컷 씬 종료, 상호작용 시작
    /// 변수 초기화
    /// </summary>
    public virtual void EndCutScene()
    {
        Debug.Log(gameObject.name + " EndCutscene: " + currentTimeline);
        m_director.Stop();
        // gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(false);
        gameMgr.statGame = GameStatus.INTERACTION;

        ResetCutScenePosition();

        currentDialog = 0;
        currentTimeline++;

        gameMgr.uiMgr.ui_game.game_txt_dialog.gameObject.SetActive(false);
        gameMgr.uiMgr.UIGameTimelineFrameToggle(false);

        //gameMgr.uiMgr.fadeCanvas.StartFade(() => StartInteraction(), 5);
        StartInteraction();
    }
    void ResetCutScenePosition()
    {
        for (int i = 0; i < arr_header.Length; i++)
        {
            arr_header[i].StopAllCoroutines();

            arr_header[i].transform.localPosition = list_endPos[currentTimeline].localPosition;
            arr_header[i].transform.rotation = list_endPos[currentTimeline].rotation;
            //arr_header[i].transform.localPosition += arr_header[i].transform.GetChild(0).localPosition;

            arr_header[i].transform.GetChild(0).localPosition = Vector3.zero;
            arr_header[i].transform.GetChild(0).localRotation = Quaternion.identity;

            //arr_header[i].StartCoroutine(SmoothPosReset(i, list_endPos[currentTimeline].localPosition, true));
            //arr_header[i].StartCoroutine(SmoothRotReset(i, list_endPos[currentTimeline].rotation));


            arr_header[i].SetAnim(0);
        }


    }

    public virtual void StartInteraction()
    {
        if (list_interaction.Count <= currentInteraction)
        {
            EndStage();
            return;
        }

        // header.transform.position = list_endPos[currentInteraction].position;

        ResetInteractionPosition();

        list_interaction[currentInteraction].gameObject.SetActive(true);
        list_interaction[currentInteraction].StartInteraction();
    }
    public virtual void EndInteraction()
    {
        ResetInteractionPosition();
        list_interaction[currentInteraction].EndInteraction();
        // list_interaction[currentInteraction].gameObject.SetActive(false);
    }

    void ResetInteractionPosition()
    {
        for (int i = 0; i < arr_header.Length; i++)
        {
            arr_header[i].m_richAI.enabled = false;
            arr_header[i].transform.GetChild(0).transform.localPosition = Vector3.zero;
            arr_header[i].gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 게임 시작
    /// 스테이지 불러오기
    /// 초기화, BGM 재생
    /// </summary>
    public virtual void StartStage()
    {
        Debug.Log(gameObject.name + "StartStage");
        //gameMgr.uiMgr.game_btn_back.gameObject.SetActive(true);
        currentTimeline = 0;
        currentDialog = 0;
        currentInteraction = 0;

        //for (int i = 0; i < list_endPos.Count; i++)
        //{
        //    //list_endPos[i].GetComponent<Collider>().enabled = true;
        //    list_endPos[i].GetComponent<Renderer>().enabled = gameMgr.isDebug;
        //}

        gameMgr.soundMgr.ChangeBGMAudioSource(m_audioSource);
        //gameMgr.soundMgr.PlayBgm(bgm_stage); //BGM 직접 넣는 것으로 변경됨, 상호작용 시 현재 오디오 저장 후 되돌릴것!

        PlayCutscene(0);
    }

    /// <summary>
    /// 게임 종료, 스테이지 종료, 중간 나가기 버튼 등
    /// 스테이지 선택 창으로 이동
    /// </summary>
    public virtual void EndStage()
    {
        Debug.Log(gameObject.name + "EndStage");
        currentTimeline = 0;
        currentDialog = 0;
        currentInteraction = 0;

        gameMgr.uiMgr.fadeCanvas.StartFade(() =>
        {
            episodeMgr.EndEpisode();
        });
    }
    #endregion

    public void FadeSignal()
    {
        gameMgr.uiMgr.fadeCanvas.StartFade(null, 5, 1f);
    }

    public void SignalFadeTime(float _time)
    {
        gameMgr.uiMgr.fadeCanvas.StartFade(null, 5, _time);
    }

    public void SignalParticleStart(ParticleSystem _particle)
    {
        _particle.Play();
    }

    public void SignalEndingCredit()
    {
        gameMgr.uiMgr.endingCreditCanvas.gameObject.SetActive(true);
        gameMgr.soundMgr.ChangeBGMAudioSource(gameMgr.uiMgr.endingCreditCanvas.GetComponent<AudioSource>());
        if (endingPos != null)
        {
            gameMgr.uiMgr.endingCreditCanvas.transform.position = endingPos.position;
            gameMgr.uiMgr.endingCreditCanvas.transform.rotation = endingPos.rotation;
        }
        else
        {
            gameMgr.uiMgr.endingCreditCanvas.transform.position = Vector3.up * 0.1f;
            gameMgr.uiMgr.endingCreditCanvas.transform.rotation = Quaternion.identity;
        }
    }

    public void NavigationSignal()
    {
        arr_header[0].m_animator.applyRootMotion = true;
        m_director.Pause();
        arr_header[0].SetAnim(1);
        arr_header[0].MoveCharacter(list_endPos[currentTimeline].position, 1, () =>
        {
            StartCoroutine(SmoothTransformReset(0, list_endPos[currentTimeline]));
            arr_header[0].m_animator.applyRootMotion = false;
            m_director.Play();
        });
    }

    public virtual void NavigationSignal(Transform _tr)
    {
        arr_header[0].m_animator.applyRootMotion = true;
        m_director.Pause();
        arr_header[0].SetAnim(1);
        arr_header[0].MoveCharacter(_tr.position, 1, () =>
        {
            StartCoroutine(SmoothTransformReset(0, _tr));
            arr_header[0].m_animator.applyRootMotion = false;
            m_director.Play();
        });
    }

}