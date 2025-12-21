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
    public LipSync lipMaster;
    public Character header;
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

    //현재 진행상황 변수
    public int currentTimeline = 0;     //현재 재생중인 컷 씬 번호
    public int currentDialog = 0;         //컷 씬에서 현재 대사 번호
    public int currentInteraction = 0;  //현재 플레이 중인 상호작용 번호

    public int currentTerrain = 0; //현재 활성화 된 터레인 번호(미 사용 중)

    protected string stageTitle = null;
    protected string stageSubTitle = null;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        m_director = GetComponent<PlayableDirector>();
        m_audioSource = GetComponent<AudioSource>();
        arr_header = transform.GetChild(0).GetComponentsInChildren<Character>();

        DoAwake();
    }

    protected virtual void DoAwake() { }

    private void Update()
    {
        if (gameMgr.isDebug)
        {
            gameMgr.uiMgr.debug_txt_timeline.text = "CurrentCutScene: " + currentTimeline;
            gameMgr.uiMgr.debug_txt_interaction.text = "CurrentInteraction: " + currentInteraction;
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
            currentDialog + 1 < list__timelineDialog[currentTimeline*2].Count)
        {
            StageManager _stage = gameMgr.currentEpisode.currentStage;

            if (list__timelineDialog[currentTimeline * 2][currentDialog + 1].ToString().Contains("&"))
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
                if (gameMgr.currentEpisode.currentStage.list___currentLipData.Count != 0)
                {
                    _canvas.currentLipData = _stage.list___currentLipData[_stage.currentTimeline][_stage.currentDialog][0];
                }
                currentDialog++;
            }
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

            //다음 대사로 넘길 조건
            if (_canvas == _stage.arr_header[i].headerCanvas &&
                i == 1)
            {
                currentDialog++;
            }
        }
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
        for (int i = 0; i <arr_header.Length; i++)
        {
            if (gameMgr.currentEpisode.currentStage.list___currentLipData.Count != 0)
            {
                _canvas.currentLipData = _stage.list___currentLipData[_stage.currentTimeline][_stage.currentDialog][i];
            }

            //다음 대사로 넘길 조건
            if (_canvas == _stage.arr_header[i].headerCanvas &&
                i == arr_header.Length-1)
            {
                currentDialog++;
            }
        }

    }

    public void ScrollRight()
    {
        StartCoroutine(ScrollAction());
    }

    IEnumerator ScrollAction()
    {
        float t = 0;
        Vector3 start = transform.position;
        Vector3 end = transform.position + -transform.right * 1.65f;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(start, end, t);
            t += Time.deltaTime * 2f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    #region =============Terrain Active Func============
    public void TerrainInit()
    {
        if (list_terrain.Count == 0)
        {
            return;
        }
        for (int i = 0; i < list_terrain.Count; i++)
        {
            list_terrain[i].gameObject.SetActive(false);
        }
        list_terrain[0].gameObject.SetActive(true);
        currentTerrain = 0;
    }

    public void NextTerrainActive()
    {
        currentTerrain++;
        list_terrain[currentTerrain].gameObject.SetActive(true);
    }
    public void PrevTerrainRemove()
    {
        if (currentTerrain == 0)
        {
            return;
        }
        list_terrain[currentTerrain - 1].gameObject.SetActive(false);
    }

    public void SetTimelineEndPos()
    {
        if (arr_header.Length > 1)
        {
            for (int i = 0; i < arr_header.Length; i++)
            {
                arr_header[i].transform.position += arr_header[i].transform.GetChild(0).localPosition;
                arr_header[i].transform.rotation = arr_header[i].transform.GetChild(0).rotation;
                arr_header[i].transform.GetChild(0).localPosition = Vector3.zero;
                arr_header[i].transform.GetChild(0).rotation = Quaternion.identity;
            }
        }


        if (currentTimeline < list_endPos.Count)
        {
            header.transform.position = list_endPos[currentTimeline].position;
            header.transform.rotation = list_endPos[currentTimeline].rotation;
            header.transform.GetChild(0).localPosition = Vector3.zero;
            header.transform.GetChild(0).rotation = Quaternion.identity;
        }
    }

    #endregion

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
        gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(true);


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
        gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(true);

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
        gameMgr.uiMgr.game_btn_skip.gameObject.SetActive(true);

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
        gameMgr.statGame = GameStatus.GAME;

        header.StopAllCoroutines();
        header.transform.position = list_endPos[currentTimeline].position;
        header.transform.rotation = list_endPos[currentTimeline].rotation;
        header.transform.GetChild(0).localPosition = Vector3.zero;
        header.transform.GetChild(0).localRotation = Quaternion.identity;

        header.SetAnim(0);

        currentDialog = 0;
        currentTimeline++;

        //gameMgr.uiMgr.fadeCanvas.StartFade(() => StartInteraction(), 5);
        StartInteraction();
    }

    public void StartInteraction()
    {
        if (list_interaction.Count <= currentInteraction)
        {
            EndStage();
            return;
        }

        // header.transform.position = list_endPos[currentInteraction].position;

        ResetPosition();

        list_interaction[currentInteraction].gameObject.SetActive(true);
        list_interaction[currentInteraction].StartInteraction();
    }
    public void EndInteraction()
    {
        ResetPosition();
        list_interaction[currentInteraction].EndInteraction();
        // list_interaction[currentInteraction].gameObject.SetActive(false);
    }

    void ResetPosition()
    {
        for (int i = 0; i < arr_header.Length; i++)
        {
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
        gameMgr.soundMgr.PlayBgm(bgm_stage);

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

        gameMgr.soundMgr.bgmSource.Stop();

        gameMgr.uiMgr.fadeCanvas.StartFade(() =>
        {
            episodeMgr.EndStage();
        });
    }
    #endregion

    public void FadeSignal()
    {
        gameMgr.uiMgr.fadeCanvas.StartFade(null,20,0.3f);
    }

    public void SignalParticleStart(ParticleSystem _particle)
    {
        _particle.Play();
    }

    public void NavigationSignal()
    {
        header.m_animator.applyRootMotion = true;
        m_director.Pause();
        header.SetAnim(1);
        header.MoveCharacter(list_endPos[currentTimeline].position, 1, () =>
         {
             header.transform.position = list_endPos[currentTimeline].position;
             header.transform.rotation = list_endPos[currentTimeline].rotation;
             header.m_animator.applyRootMotion = false;
             m_director.Play();
         });
    }

    public virtual void NavigationSignal(Transform _tr)
    {
        header.m_animator.applyRootMotion = true;
        m_director.Pause();
        header.SetAnim(1);
        header.MoveCharacter(_tr.position, 1, () =>
         {
             header.transform.position = _tr.position;
             header.transform.rotation = _tr.rotation;
             header.m_animator.applyRootMotion = false;
             m_director.Play();
         });
    }

}

