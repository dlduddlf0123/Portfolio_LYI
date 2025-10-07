using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;
using UnityEngine.Events;
using System;

using VRTokTok.Character;
using VRTokTok.Interaction;

using Pathfinding;


namespace VRTokTok.Manager
{

    /// <summary>
    /// 8/22/2023-LYI
    /// 게임 종류에 따른 스테이지 타입 설정
    /// 스테이지 타입에 따른 기믹, 플레이어 조작 변경 등에 사용됨
    /// </summary>
    public enum StageType
    {
        NONE = 0,
        MAZE = 1,
        MACHINE = 2,
        SHOOTING = 3,
        CROSSING = 4,
        PRESS = 5,
        MEMORY = 6,
        BRIDGE = 7,
        BLOCK = 8,
        MIXED = 9,
    }

    /// <summary>
    /// 8/23/2023-LYI
    /// 각 스테이지 로드 시 불러올 변수 데이터 클래스
    /// CSV로 불러오기
    /// </summary>
    public class StageData
    {
        public StageType typeStage = StageType.NONE;

        public int playCharaterCount = 0;

        public int timeLimit = 0;
        public int maxCount = 0;

        public StageData(int characters, int limit, int count)
        {
            this.playCharaterCount = characters;
            this.timeLimit = limit;
            this.maxCount = count;
        }
    }

    /// <summary>
    /// 8/23/2023-LYI
    /// 스테이지 플레이 기록용 클래스
    /// 최초 실행 시 전체 데이터 검색 후 각 스테이지 기록 불러오기
    /// </summary>
    public class StagePlayData
    {

    }



    /// <summary>
    /// 8/22/2023-LYI
    /// 각 스테이지 로드 시에 게임 플레이 매니저에 데이터 전달
    /// 각 스테이지의 시작과 끝 관리
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        PlaySceneManager playMgr;

        public AstarPath astarPath;

        public StageData Data;
        public StageType typeStage = StageType.NONE; //mixed 컨셉으로 인한 스테이지 할당 추가

        public Transform appearFeedbackTr;

        [Header("Load Intro")]
        //public GameObject[] arr_appearObjects;
        public MMF_Player[] arr_feedback;
        public Transform[] arr_startPos;
        public Tok_Gate gateStart;
        public Tok_Gate gateExit;

        [Header("Tilemap")]
        public GridLayout[] arr_tilemap;

        [Header("Interaction")]
        public List<Tok_Interact> list_interact = new List<Tok_Interact>();
        public Tok_Interact[] arr_startInteract;



        [Header("Stage Property")]
        public List<HeaderType> list_activeType = new();

        public int stageNum = 0;
        public int currentTime = 0;
        public int currentTokCount = 0;

        public UnityAction onStageStart;

        [Header("DataDebug Property")]
        public bool isDebug = false;
        public int characterNum = 1;
        public int limitTime = 0;
        public int maxTokCount = 0;

        public int keyCount = 0;
        public List<Tok_Key> list_key = new List<Tok_Key>();

        Coroutine co_time = null;

        private void Awake()
        {
            playMgr = GameManager.Instance.playMgr;
        }

        private void Start()
        {
        }

        /// <summary>
        /// 8/24/2023-LYI
        /// 스테이지 시작 시 수치 관련 초기화
        /// </summary>
        public void StageInit()
        {
            if (Data == null)
            {
                SetStageData();
            }
            currentTokCount = 0;
            currentTime = Data.timeLimit;

            // playMgr.uiMgr.ui_game.ChangeTokCountText(0, maxTokCount);

            //열쇠 관련 초기화
            keyCount = 0;
            list_key.Clear();

            for (int i = 0; i < list_interact.Count; i++)
            {
                list_interact[i].InteractInit();
            }

        }

        /// <summary>
        /// 8/24/2023-LYI
        /// 스테이지 로드 시 호출
        /// 스테이지 번호에 따른 데이터 할당
        /// AddressableManager에서 CSV 호출하여 데이터 할당
        /// </summary>
        public void SetStageData()
        {
            //stageNum = Convert.ToInt32(gameObject.name);
            //초기 개발용 디버그 이후 DataManager 등 이용하여
            //CSV데이터에서 행열로 불러오기
            if (isDebug)
            {
                string num = gameObject.name;
                if (num.Contains("(Clone)"))
                {
                    num = num.Replace("(Clone)", "");
                }
                stageNum = 0;

                try
                {
                     stageNum = Convert.ToInt32(num);
                }
                catch
                {
                    stageNum = 1001;
                }
                StageData data = new StageData(characterNum, limitTime, maxTokCount);
                data.typeStage = typeStage;
                    //(StageType)(stageNum / 1000);
                Data = data;
            }

            Tok_Interact[] tok_Interacts = GetComponentsInChildren<Tok_Interact>();
            list_interact.Clear();
            for (int i = 0; i < tok_Interacts.Length; i++)
            {
                list_interact.Add(tok_Interacts[i]);
            }
        }


        public void TokCountPlus()
        {
            currentTokCount++;
         //   playMgr.uiMgr.ui_game.ChangeTokCountText(currentTokCount, maxTokCount);
        }

        /// <summary>
        /// 7/17/2023-LYI
        /// 시간제한 작동
        /// 제한 0이면 시간 비활성화
        /// </summary>
        public void TimeTic()
        {
            if (currentTime == 0)
            {
               // playMgr.uiMgr.ui_game.ChangeTimeActive(false);
                return;
            }
          //  playMgr.uiMgr.ui_game.ChangeTimeActive(true);


            if (co_time != null)
            {
                StopCoroutine(co_time);
                co_time = null;
            }
            co_time = StartCoroutine(TimeTicCoroutine());
        }
        IEnumerator TimeTicCoroutine()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            int tic = currentTime;
            while (playMgr.statPlay == PlayStatus.PLAY &&
                tic > 0)
            {
                tic -= 1;
              // playMgr.uiMgr.ui_game.ChangeTimeText(tic);
                yield return wait;
            }


            //시간초과로 게임오버
            if (tic <= 0)
            {

            }

        }

        /// <summary>
        /// 8/23/2023-LYI
        /// StageLoad에서 호출
        /// 스테이지 오브젝트 생성 이후에 작동된다
        /// </summary>
        public void OnStageLoad()
        {
            Debug.Log("Stage Load");
            SetStartPosition();
            StageCharacterActiveCheck();

            //StageStart();

            StartCoroutine(StageAppearCoroutine());
        }


        /// <summary>
        /// 9/4/2023-LYI
        /// 스테이지 로드 시 StartPosition 위치 변경
        /// </summary>
        public void SetStartPosition()
        {
            for (int i = 0; i < arr_startPos.Length; i++)
            {
                playMgr.arr_startPosition[i].position = arr_startPos[i].position;
                //playMgr.arr_startPosition[i].rotation = arr_startPos[i].rotation;
            }

        }

        public HeaderType[] GetActiveHeaderTypes()
        {
            list_activeType.Clear();
            HeaderType[] stageHeader = new HeaderType[Data.playCharaterCount];
            List<HeaderType> allType = new List<HeaderType>();

            for (int i = 1; i < 7; i++)
            {
                allType.Add((HeaderType)i);
            }

            //첫번째 캐릭터는 선택된 캐릭터
            stageHeader[0] = playMgr.selectCharacterType;
            allType.Remove(playMgr.selectCharacterType);

            for (int i = 0; i < allType.Count; i++)
            {
                for (int j = 1; j < stageHeader.Length; j++)
                {
                    if (stageHeader[j] == HeaderType.NONE &&
                        stageHeader[j] != stageHeader[j - 1])
                    {
                        stageHeader[j] = allType[i];
                        allType.RemoveAt(i);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            for (int i = 0; i < stageHeader.Length; i++)
            {
                list_activeType.Add(stageHeader[i]);
            }

            return stageHeader;
        }

        /// <summary>
        /// 8/25/2023-LYI
        /// 캐릭터 활성화 여부 결정
        /// 스테이지에서 여러 캐릭터가 사용되는 경우 메인캐릭터 외의 추가 캐릭터는 랜덤 혹은 순서대로 사용
        /// </summary>
        public void StageCharacterActiveCheck()
        {
            Debug.Log("Stage Character Check");
            playMgr.CharacterInit();

            if (Data.playCharaterCount > 1)
            {
               // playMgr.cheeringSeat.ResetCheeringCharacter();
                //응원석 준비 체크
                playMgr.cheeringSeat.SetCheeringCharacter(GetActiveHeaderTypes());
            }
            else
            {

                HeaderType[] stageHeader = new HeaderType[1] { playMgr.selectCharacterType };

              //  playMgr.cheeringSeat.ResetCheeringCharacter();
                playMgr.cheeringSeat.SetCheeringCharacter(stageHeader);
            }
        }


        /// <summary>
        /// 8/23/2023-LYI
        /// 맵 등장효과 각자 재생
        /// 이후 캐릭터 등장
        /// </summary>
        /// <returns></returns>
        IEnumerator StageAppearCoroutine(bool isRestart = false)
        {
            float appearTime = 0.5f;

            ////오브젝트 일단 비활성화
            //for (int i = 0; i < arr_feedback.Length; i++)
            //{
            //    arr_feedback[i].transform.parent.gameObject.SetActive(false);
            //}

            //yield return new WaitForSeconds(appearTime);

            ////한개씩 활성화 하며 효과 재생
            //for (int i = 0; i < arr_feedback.Length; i++)
            //{
            //    arr_feedback[i].transform.parent.gameObject.SetActive(true);
            //    arr_feedback[i].PlayFeedbacks();
            //    yield return new WaitForSeconds(appearTime);
            //}

            if (Data.playCharaterCount > 1)
            {
                yield return StartCoroutine(playMgr.CharactersAppear(list_activeType.ToArray(), appearTime));
            }
            else
            {
                yield return StartCoroutine(playMgr.CharacterAppear(playMgr.selectCharacterType, appearTime));
            }

            //스테이지 시작
            StageStart(isRestart);
        }


        /// <summary>
        /// 4/4/2024-LYI
        /// 네비게이션 스캔
        /// </summary>
        /// <param name="gridGraph"></param>
        /// <param name="grid"></param>
        public void SetNavigationGraph()
        {
            if (astarPath.data.gridGraph != null)
            {
                GridGraph gridGraph = astarPath.data.gridGraph;
                GridLayout grid = arr_tilemap[0];

                gridGraph.AlignToTilemap(grid);
                gridGraph.SetGridShape(InspectorGridMode.Grid);

                gridGraph.width = 35;
                gridGraph.depth = 35;
                gridGraph.nodeSize = 0.04f;

                gridGraph.center = transform.position;
                gridGraph.isometricAngle = 0;

                Debug.Log("GridGraph Scan: " + gridGraph.center + " / " + transform.position);
                gridGraph.Scan();
            }
            else if (astarPath.data.recastGraph != null)
            {
                RecastGraph recastGraph = astarPath.data.recastGraph;

                recastGraph.SnapForceBoundsToScene();
                recastGraph.forcedBoundsCenter = transform.position;
                recastGraph.forcedBoundsSize = new Vector3(recastGraph.forcedBoundsSize.x, 1, recastGraph.forcedBoundsSize.z);

                Debug.Log("RecastGraph Scan: " + recastGraph.forcedBoundsCenter + " / " + transform.position);
                recastGraph.Scan();

            }

        }



        /// <summary>
        /// 8/23/2023-LYI 
        /// 스테이지 시작 시 처리
        /// </summary>
        public void StageStart(bool isRestart = false)
        {
            Debug.Log("StageStart!");
            playMgr.statPlay = PlayStatus.PLAY;

            if (Data.typeStage == StageType.CROSSING)
            {
                for (int i = 0; i < arr_startInteract.Length; i++)
                {
                    arr_startInteract[i].ActiveInteraction();
                }
            }

            if (Data.typeStage == StageType.MAZE)
            {
                //네비게이션 스캔
                SetNavigationGraph();
            }


            if (!isRestart)
            {
                playMgr.tokMgr.TokGroundInit();
                playMgr.cheeringSeat.OnStart();

                TimeTic();

                for (int i = 0; i < arr_startInteract.Length; i++)
                {
                    arr_startInteract[i].ActiveInteraction();
                }

                if (onStageStart != null)
                {
                    onStageStart.Invoke();
                }
            }
        }

        /// <summary>
        /// 7/13/2023-LYI
        /// 스테이지 재시작 시 호출
        /// 사망하거나 하면 페이드? 후 다시시작
        /// 옵션에서 다시시작 클릭 시 다시시작
        /// </summary>
        public void RestartStage(bool isFail = false)
        {
            Debug.Log("Restart Stage()");

            playMgr.statPlay = PlayStatus.READY;
            StageInit();
            playMgr.CharacterInit();

            if (isFail)
            {
                playMgr.cheeringSeat.OnFail();
            }

            StartCoroutine(StageAppearCoroutine(true));
            //if (Data.playCharaterCount > 1)
            //{
            //    StartCoroutine(playMgr.CharactersAppear(list_activeType.ToArray()));
            //}
            //else
            //{
            //    StartCoroutine(playMgr.CharacterAppear(playMgr.selectCharacterType));
            //}

        }


        /// <summary>
        /// 8/22/2023-LYI
        /// 스테이지 클리어 시 호출
        /// 클리어 축하 효과 등 호출 뒤 NextStage 호출
        /// 혹은 UI 호출하여 축하 후 NextStage 버튼 작동
        /// </summary>
        public void ClearStage()
        {
            if (playMgr.statPlay == PlayStatus.CLEAR) { return; }
            Debug.Log("!!Stage Clear!!");
            playMgr.statPlay = PlayStatus.CLEAR;

            bool isNewClear = ES3.Load<bool>(stageNum.ToString(), false) ? false : true;

            ES3.Save<bool>(stageNum.ToString(), true); //게임 클리어 여부 저장

            if (GameManager.Instance.dataMgr.dic_StageToMixed.ContainsKey(stageNum))
            {
                ES3.Save<bool>(GameManager.Instance.dataMgr.dic_StageToMixed[stageNum].ToString(), true); //게임 클리어 여부 저장
            }


            //for (int i = 0; i < playMgr.list_activeCharacter.Count; i++)
            //{
            //    playMgr.list_activeCharacter[i].transform.SetParent(this.transform);
            //    playMgr.list_activeCharacter[i].OnClear();
            //}

            //새로 클리어한 스테이지인 경우
            if (isNewClear)
            {
                //잠금해제 체크
                //잠금해제가 되는 경우
                //현재 스테이지가 캐릭터의 해제 스테이지와 같은 경우
                //Check Unlock Stage
                bool isUnlock = playMgr.cheeringSeat.IsCharacterUnlock(stageNum);
                if (isUnlock == true)
                {
                    //스테이지 최초클리어 했고 언락 가능한 스테이지인 경우
                    //언락 시퀀스 진행
                    playMgr.ClearCheerUnlock();
                    return;
                }
            }

            StartCoroutine(ClearCoroutine(playMgr.LoadNextStage));
        }

        /// <summary>
        /// 6/13/2024-LYI
        /// 스테이지 스킵 처리
        /// </summary>
        public void SkipStage()
        {
            if (playMgr.statPlay == PlayStatus.LOADING) { return; }
            Debug.Log("!!Stage Skip!!");
            playMgr.statPlay = PlayStatus.LOADING;

            // ES3.Save(stageNum.ToString(), true); //게임 클리어 여부 저장

            //if (GameManager.Instance.dataMgr.dic_StageToMixed.ContainsKey(stageNum))
            //{
            //    ES3.Save(GameManager.Instance.dataMgr.dic_StageToMixed[stageNum].ToString(), true); //게임 클리어 여부 저장
            //}

            playMgr.CharacterInit();

            playMgr.LoadNextStage();
        }

        /// <summary>
        /// 8/31/2023-LYI
        /// 스테이지 클리어했을 때 성공 관련 동작 보여주고 나서 다음 스테이지로 이동
        /// </summary>
        /// <returns></returns>
        IEnumerator ClearCoroutine(UnityAction action)
        {
            for (int i = 0; i < playMgr.list_activeCharacter.Count; i++)
            {
                playMgr.list_activeCharacter[i].OnTokTok(gateExit.tr_center.position, gateExit.tr_center.gameObject);
            }

            int moveCount = playMgr.list_activeCharacter.Count;
            while (moveCount > 0)
            {
                moveCount = playMgr.list_activeCharacter.Count;
                for (int i = 0; i < playMgr.list_activeCharacter.Count; i++)
                {
                    if (playMgr.list_activeCharacter[i].isMove == false)
                    {
                        moveCount--;
                    }
                }
                yield return null;
            }

            for (int i = 0; i < playMgr.list_activeCharacter.Count; i++)
            {
                playMgr.list_activeCharacter[i].moveMarker.position = gateExit.tr_jump.position;
                playMgr.list_activeCharacter[i].moveMarker.SetParent(gateExit.tr_jump);
                playMgr.list_activeCharacter[i].OnClear();
            }

            playMgr.cheeringSeat.OnClear();
            yield return new WaitForSeconds(2f);

            //7/16/2024-LYI
            //해금 연출을 위해 클리어 이후 동작 분화
            if (action != null)
            {
                action.Invoke();
            }
        }

    }
}