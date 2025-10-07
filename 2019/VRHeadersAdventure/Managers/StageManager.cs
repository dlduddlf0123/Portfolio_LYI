using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnly;

/// <summary>
/// 각 스테이지마다 존재
/// 각 스테이지의 퍼즐 갯수와 클리어 퍼즐 갯수를 알고있다
/// 각 퍼즐의 활성화 담당
/// Public을 활용하여 코드 1개로 에디터에서 확장
/// 가능?
/// </summary>
public class StageManager : MonoBehaviour
{
    GameManager gameMgr;
    CutSceneManager cutSceneMgr;

    public GameObject gameStage;
    public PlayerPlatform playerPlatform;
    public PlatformUI platformUI;

    public Character[] arr_headers;
    public Transform[] arr_movePoints;  //headers patrolmove
    public List<Checkpoint> list_checkPoints = new List<Checkpoint>(); //characters check point
    public Transform[] arr_cameraPoints; //camera change point
    public Transform clearCameraPoint;

    public List<GameObject> list_puzzle;
    public Dictionary<GameObject, bool> dic_puzzle = new Dictionary<GameObject, bool>();

    List<AudioClip> bgmClips = new List<AudioClip>();
    

    public string nextSceneName;
    public int currentCamNum = 0;
    bool isFading = false;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        cutSceneMgr = GetComponent<CutSceneManager>();

        arr_headers = transform.GetChild(0).GetComponentsInChildren<Character>();
        StageInit();
        
        //StartCoroutine(OnStageStart());
    }
    private void Start()
    {
        bgmClips.Add(gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_LOBBY));
        bgmClips.Add(gameMgr.soundMgr.LoadClip(Defines.SOUND_BGM_STAGE1));
        StartCoroutine(OnStageStart());
        gameMgr.soundMgr.PlayBgm(bgmClips[0]);
    }

    /// <summary>
    /// 스테이지 초기화
    /// </summary>
    public void StageInit()
    {
        dic_puzzle.Clear();
        for (int i = 0; i < list_puzzle.Count; i++)
        {
            dic_puzzle[list_puzzle[i]] = false;
        }
        
        gameMgr.stageMgr = this;
        gameMgr.statGame = GameState.PLAY;
        gameMgr.arr_headers = arr_headers;
        gameMgr.rightHand.header = arr_headers[0];
        gameMgr.leftHand.header = arr_headers[0];
        arr_headers[0].SetSelect(true);
        gameMgr.rightHand.PlatformerActionSetOn(arr_headers[0]);
    }

    /// <summary>
    /// 퍼즐 클리어 여부 설정
    /// </summary>
    /// <param name="_key">퍼즐이 해결된 오브젝트</param>
    /// <param name="_clear">true or false</param>
    public void SetPuzzleClear(GameObject _key, bool _clear)
    {
        if (_key != null)
        {
            dic_puzzle[_key] = _clear;
        }
    }

    /// <summary>
    /// 체크포인트 활성화 변경(레이저 사용시 일시적으로 꺼지는 용도)
    /// </summary>
    public void SetActiveCheckPoints(bool _active)
    {
        if (_active == list_checkPoints[0].gameObject.activeSelf) { return; }
        foreach (var check in list_checkPoints)
        {
            check.gameObject.SetActive(_active);
        }
    }

    /// <summary>
    /// 현재 체크포인트에 따른 대가리들의 패트롤 위치 변경
    /// </summary>
    public void ChangeHeadersMovePoint(int _pointNum)
    {
        for (int i = 0; i < arr_headers.Length; i++)
        {
            arr_headers[i].movePoint = arr_movePoints[_pointNum];
            arr_headers[i].movePoints = arr_headers[i].movePoint.GetComponentsInChildren<Transform>();

            if (_pointNum == arr_movePoints.Length-1)
            {
                arr_headers[i].mNavAgent.enabled = false;
                arr_headers[i].transform.position = arr_movePoints[arr_movePoints.Length - 1].GetChild(i).position;
                arr_headers[i].mNavAgent.enabled =true;
            }
        }
    }

    /// <summary>
    /// 플레이어 카메라 위치 변경(페이드/순간이동)
    /// </summary>
    /// <param name="_camNum">이동할 카메라 번호</param>
    public IEnumerator ChangeCameraPosition(int _camNum)
    {
        if (arr_cameraPoints.Length < _camNum + 1 || isFading) { yield break; }
        isFading = true;

        Valve.VR.SteamVR_Fade.Start(Color.black, 0.3f);
        yield return new WaitForSeconds(0.1f);
        Valve.VR.SteamVR_Fade.Start(Color.clear, 0.3f);

        currentCamNum = _camNum;
        playerPlatform.currentNum = _camNum;

        playerPlatform.transform.position = arr_cameraPoints[_camNum].position;
        gameMgr.player.transform.position = playerPlatform.playerTr.position;
        
        isFading = false;
    }

    public IEnumerator ChangeCameraPosition(int _camNum, float _fadeTime)
    {
        if (arr_cameraPoints.Length < _camNum + 1 || isFading || currentCamNum == _camNum) { yield break; }
        isFading = true;

        Valve.VR.SteamVR_Fade.Start(Color.black, _fadeTime);
        yield return new WaitForSeconds(0.1f);
        Valve.VR.SteamVR_Fade.Start(Color.clear, _fadeTime);

        currentCamNum = _camNum;
        playerPlatform.currentNum = _camNum;

        playerPlatform.transform.position = arr_cameraPoints[_camNum].position;
        gameMgr.player.transform.position = playerPlatform.playerTr.position;

        isFading = false;
    }

    public IEnumerator ChangeCameraPosition(Vector3 _pos, float _fadeTime)
    {
        isFading = true;
        Valve.VR.SteamVR_Fade.Start(Color.black, _fadeTime);
        yield return new WaitForSeconds(0.1f);
        Valve.VR.SteamVR_Fade.Start(Color.clear, _fadeTime);

        gameMgr.player.transform.position = _pos;
        isFading = false;
    }


    /// <summary>
    /// 게임 시작, 각 스테이지 요소, 캐릭터 초기화
    /// </summary>
    public void GameStart()
    {
        gameMgr.statGame = GameState.PLAY;
        gameMgr.soundMgr.PlayBgm(bgmClips[1]);
        StageInit();

        //if (platformUI)
        //{
        //    platformUI.OnStartUI();
        //}
    }

    /// <summary>
    /// 게임 종료, 상호작용 모드 입장
    /// </summary>
    public void GameClear()
    {
        if (gameMgr.rightHand.statHand == HandState.PLATFORMER)
        {
            gameMgr.rightHand.PlatformerActionSetOff(gameMgr.rightHand.header);
        }
        if (gameMgr.rightHand.header!= null)
        {
            gameMgr.rightHand.header.SetSelect(false);
        }

        if (gameMgr.statGame == GameState.CLEAR)
        {
            gameMgr.ChangeScene(gameMgr.stageMgr.nextSceneName);
        }
        else
        {
            gameMgr.statGame = GameState.CLEAR;
            gameMgr.soundMgr.PlayBgm();
            StartCoroutine(ChangeCameraPosition(clearCameraPoint.position,0.3f));
            ChangeHeadersMovePoint(arr_movePoints.Length-1);

            foreach (var header in arr_headers)
            {
                header.SetPlayerGameMode();
            }
            Debug.Log("GameState = "+gameMgr.statGame);
        }
    }

    /// <summary>
    /// 게임 시작 시 연출
    /// </summary>
    public IEnumerator OnStageStart()
    {
        // gameMgr.soundMgr.PlayBgm(gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_BGM_STAGE1));

        yield return StartCoroutine(ChangeCameraPosition(0));
        //시작 연출 등
        cutSceneMgr.PlayCutScene(0,playerPlatform.transform);
    }

    /// <summary>
    /// 게임 클리어 시 연출
    /// </summary>
    public IEnumerator OnStageClear()
    {
       // platformUI.OnClearUI();

        yield return StartCoroutine(ChangeCameraPosition(clearCameraPoint.position, 0.3f)); ;
        //클리어 연출 등
        cutSceneMgr.PlayCutScene(1);
    }
}
