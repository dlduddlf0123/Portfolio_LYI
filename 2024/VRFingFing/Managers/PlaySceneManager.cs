using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using VRTokTok.Character;
using VRTokTok.UI;
using UnityEngine.Events;

namespace VRTokTok.Manager
{
    /// <summary>
    /// 현재 게임 진행 상태 체크, 저장
    /// </summary>
    public enum PlayStatus
    {
        NONE = 0,
        LOADING,
        READY,
        PLAY,
        CLEAR,
    }


    /// <summary>
    /// 7/6/2023-LYI
    /// 플레이 데이터 관리 클래스
    /// 각 씬의 내용물 관리
    /// 각 스테이지 시작과 끝 관리
    /// 게임 플로우 관련 관리
    /// 클리어 판정 관리
    /// </summary>
    public class PlaySceneManager : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Managers")]
        // public UIManager uiMgr;
        public TokTokManager tokMgr;
        public StageManager currentStage; //현재 작동 중인 스테이지
        public StageManager lastStage; //마지막 플레이한 스테이지

        [Header("Play")]
        public PlayStatus statPlay = PlayStatus.NONE;

        public Transform tr_stageAnchor; //스테이지의 기준점이 될 위치값

        [Header("CharacterArray")]
        public CheeringSeat cheeringSeat;
        public Tok_Movement[] arr_character;
        public Tok_Movement[] arr_navCharacter;

        public Tok_Movement[] arr_currentCharacter;

        public Transform[] arr_startPosition;


        [Header("Current Playing")]
        //게임 플레이시 사용할 메인 캐릭터 번호
        //처음에 조종할 캐릭터는 해당 번호의 캐릭터로 진행
        //여러 캐릭터가 등장할 경우 해당 번호를 제외하고 순서대로 혹은 랜덤
        //옵션 창 등에서 캐릭터 변경할 때 이 변수로 변경
        public HeaderType selectCharacterType;

        /// <summary>
        /// 10/16/2023-LYI
        /// 현재 활성화 되어있는 캐릭터 표시
        /// Tok 입력 체크 관련 호출
        /// </summary>
        public List<Tok_Movement> list_activeCharacter = new();


        private void Awake()
        {
            gameMgr = GameManager.Instance;
            gameMgr.playMgr = this;
        }

        private void Start()
        {
            if (currentStage == null &&
                !gameMgr.addressableMgr.isStartDebug)
            {
                currentStage = FindObjectOfType<StageManager>();

                if (currentStage != null)
                {
                    gameMgr.ChangeGameStat(GameStatus.GAME);
                    gameMgr.tableMgr.playTable.SetTableStage(currentStage.gameObject);
                }
            }
        }


        /// <summary>
        /// 4/4/2024-LYI
        /// 캐릭터 목록 선택
        /// </summary>
        /// <param name="type"></param>
        public void SetCharacterArray(StageType type)
        {
            if (arr_currentCharacter != null)
            {
                CharacterInit();
            }
            arr_currentCharacter = null;
            switch (type)
            {
                case StageType.MAZE:
                    arr_currentCharacter = arr_navCharacter;
                    break;
                default:
                    arr_currentCharacter = arr_character;
                    break;
            }

        }


        /// <summary>
        /// 8/24/2023-LYI
        /// 캐릭터들 상태 초기화, 비활성화
        /// 씬 전환 시 혹은 스테이지 로드 시 호출
        /// </summary>
        public void CharacterInit()
        {
            for (int i = 0; i < arr_currentCharacter.Length; i++)
            {
                arr_currentCharacter[i].transform.position = Vector3.zero;
                arr_currentCharacter[i].Init();
                arr_currentCharacter[i].gameObject.SetActive(false);
            }
            tokMgr.selectMarker.OffTok();
        }


        /// <summary>
        /// 8/24/2023-LYI
        /// 캐릭터들 등장 효과
        /// 게임 시작이나 다시시작 시 호출
        /// 0~5까지 6종의 캐릭터 선택 호출
        /// </summary>
        public IEnumerator CharacterAppear(HeaderType header, float time = 0.5f)
        {
            list_activeCharacter.Clear();

            int headerNum = (int)header - 1;
            arr_currentCharacter[headerNum].transform.position = currentStage.gateStart.efx_portal.transform.position;// arr_startPosition[0].position;
            arr_currentCharacter[headerNum].transform.rotation = Quaternion.Euler(Vector3.up * 90f); //arr_startPosition[0].rotation;
            //arr_character[headerNum].gameObject.SetActive(true);

            arr_currentCharacter[headerNum].moveMarker.position = arr_startPosition[0].position;
            arr_currentCharacter[headerNum].moveMarker.SetParent(arr_startPosition[0]);

            currentStage.gateStart.SetPortal(true);
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_STAGE_PORTAL);

            yield return new WaitForSeconds(time);

            arr_currentCharacter[headerNum].OnCharacterAppear();
            list_activeCharacter.Add(arr_currentCharacter[headerNum]);

            yield return new WaitForSeconds(time);

            currentStage.gateStart.SetPortal(false);

            //yield return new WaitForSeconds(time);

            //arr_character[headerNum].OnCharacterReady();
            //tokMgr.SelectCharacter(arr_character[(int)selectCharacterType - 1]);

        }

        public IEnumerator CharactersAppear(HeaderType[] headers, float time = 0.5f)
        {
            list_activeCharacter.Clear();

            for (int i = 0; i < arr_currentCharacter.Length; i++)
            {
                for (int type = 0; type < headers.Length; type++)
                {
                    if (arr_currentCharacter[i].m_character.typeHeader == headers[type])
                    {
                        list_activeCharacter.Add(arr_currentCharacter[i]);
                    }
                }
            }

            for (int i = 0; i < list_activeCharacter.Count; i++)
            {
                list_activeCharacter[i].transform.position = arr_startPosition[i].position;
                list_activeCharacter[i].transform.rotation = arr_startPosition[i].rotation;
                list_activeCharacter[i].gameObject.SetActive(true);
                list_activeCharacter[i].OnCharacterAppear();
                yield return new WaitForSeconds(time);
                list_activeCharacter[i].OnCharacterReady();
            }

            tokMgr.SelectCharacter(arr_currentCharacter[(int)selectCharacterType - 1]);
        }



        /// <summary>
        /// 7/6/2023-LYI
        /// 스테이지 시작 시 호출
        /// 게임매니저에 연결
        /// </summary>
        public void LoadStage(int stageNum = 0)
        {
            gameMgr.playMgr = this;

            statPlay = PlayStatus.LOADING;
            if (gameMgr.tableMgr.playTable.isTableDown)
            {
                gameMgr.ChangeGameStat(GameStatus.LOADING);
            }

            Debug.Log("PlayScene Loading");


            //목록에 없는 숫자 불러오면 0으로 변경
            if (!gameMgr.addressableMgr.dic_assetLocation.ContainsKey(stageNum.ToString()))
            {
                Debug.Log("!!Wrong Stage Number!!");
                stageNum = 0;
            }

            if (stageNum == 0)
            {
                stageNum = ES3.Load(Constants.ES3.LAST_STAGE, 1001);
            }

            //6/27/2024-LYI 타입별 배경음 변경 추가
            ChangeStageBGM(stageNum);

            if (gameMgr.addressableMgr.list_loadedStageNum.Contains(stageNum))
            {
                OnStageLoaded(stageNum);
            }
            else
            {
                gameMgr.tableMgr.playTable.SetTableStage(null);

                //기존 스테이지가 올라와 있는 경우 아래로 내린 후 로딩 진행하도록 변경
                gameMgr.addressableMgr.LoadStageAsset(stageNum, () => OnStageLoaded(stageNum));
            }

        }

        /// <summary>
        /// 6/13/2024-LYI
        /// Stage Loading 방식 변화로 분리
        /// 이미 스테이지가 로딩되어있으면 바로 실행
        /// 스테이지가 로딩 안되었으면 로딩 후 실행
        /// </summary>
        /// <param name="stageNum"></param>
        public void OnStageLoaded(int stageNum)
        {
            GameObject stage = gameMgr.addressableMgr.dic_stagePrefab[stageNum.ToString()];

            //스테이지 데이터 초기화, 스테이지 등장 연출 시작
            //currentStage = stage.GetComponent<StageManager>();

            ES3.Save<int>(Constants.ES3.LAST_STAGE, stageNum);

            gameMgr.tableMgr.ui_table.ChangeCurrentStageText(stageNum);

            //10/25/2023-LYI
            //스테이지 회전하면서 등장하는 효과 넣을 것
            //회전한 이후 스테이지 데이터 할당 및 캐릭터 등장, 시작
            //기존의 로딩 개념을 대체?
            gameMgr.tableMgr.playTable.SetTableStage(stage);
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 시작시 초기화 용도
        /// </summary>
        public void ReadyStage()
        {
            gameMgr.ChangeGameStat(GameStatus.GAME);
            statPlay = PlayStatus.READY;

            if (currentStage == null)
            {
                Debug.Log("ReadyStage(): Stage is missing!");
                return;
            }
            currentStage.StageInit();

            SetCharacterArray(currentStage.Data.typeStage);

            //캐릭터 세팅
            CharacterInit();

            // uiMgr.ui_pause.PauseLoad();

            currentStage.gameObject.SetActive(true);
            currentStage.OnStageLoad();
        }

        /// <summary>
        /// 8/31/2023-LYI
        /// 스테이지 클리어 이후 처리
        /// 현재 스테이지 비활성화, 이후 삭제?
        /// </summary>
        public void DisableStage()
        {
            currentStage.gameObject.SetActive(false);
        }

        /// <summary>
        /// 7/6/2023-LYI
        /// 스테이지 클리어 시 바로 다음스테이지로 이동
        /// 현재 스테이지 비활성화
        /// </summary>
        public void LoadNextStage()
        {
            //10/26/2023-LYI 기존 비활성화를 회전 이후 비활성화로 수정
            //DisableStage();

            if (currentStage != null)
            {
                lastStage = currentStage;
            }

            //6/12/2024-LYI
            //코드 추가로 공용 지역변수 추가
            int currentStageNum = currentStage.stageNum;

            //When playing type is mixed
            if (gameMgr.tableMgr.ui_menu.currentStageType == StageType.MIXED)
            {
                int mixedNum = 0;

                if (gameMgr.dataMgr.dic_StageToMixed.ContainsKey(currentStageNum))
                {
                    //Mixed 인 경우 스테이지 번호 가져오기
                    //9000번대로 변환
                    mixedNum = gameMgr.dataMgr.dic_StageToMixed[currentStageNum];
                }

                int nextStage = 0;
                if (gameMgr.dataMgr.dic_mixedToStage.ContainsKey(mixedNum + 1))
                {
                    nextStage = gameMgr.dataMgr.dic_mixedToStage[mixedNum + 1];
                }
                else
                {
                    EndGame();
                    return;
                }

                //AddressManager에서 스테이지 번호가 있을 경우 다음 스테이지 로딩
                if (gameMgr.addressableMgr.dic_assetLocation.ContainsKey(nextStage.ToString()))
                {
                    LoadStage(nextStage);
                }
                else
                {
                    //스테이지 번호가 목록에 없으면 메뉴로
                    EndGame();
                }
            }
            else
            {
                //AddressManager에서 스테이지 번호가 있을 경우 다음 스테이지 로딩
                if (gameMgr.addressableMgr.dic_assetLocation.ContainsKey((currentStageNum + 1).ToString()))
                {
                    LoadStage(currentStageNum + 1);
                    // gameMgr.fade.StartFadeMiddle(() => LoadStage(currentStage.stageNum + 1));
                }
                else
                {
                    //스테이지 번호가 목록에 없으면 메뉴로
                    EndGame();
                }
            }
            //gameMgr.loadMgr.LoadScene(currentStageNum++);
        }

        /// <summary>
        /// 7/6/2023-LYI
        /// 스테이지 종료 시 호출, 메뉴버튼에서 호출
        /// 메인메뉴로 나가기
        /// </summary>
        public void EndGame()
        {
            if (statPlay == PlayStatus.NONE ||
                statPlay == PlayStatus.READY ||
                statPlay == PlayStatus.LOADING ||
                gameMgr.statGame == GameStatus.MENU)
            {
                //이미 메뉴면 돌려보냄
                //로딩 중에 누르면 돌려보냄
                return;
            }

            statPlay = PlayStatus.NONE;

            gameMgr.tableMgr.OnTableMenu();

            currentStage = null;

            //if (statPlay == PlayStatus.PLAY)
            //{
            //    for (int i = 0; i < list_activeCharacter.Count; i++)
            //    {
            //        list_activeCharacter[i].m_character.CharacterDisappear();
            //    }
            //}


            // cheeringSeat.ResetCheeringCharacter();

            //gameMgr.fade.StartFadeMiddle(() => gameMgr.loadMgr.LoadScene(SceneStatus.MENU));
            // gameMgr.currentEpisode = null;
            //     gameMgr.uiMgr.endingCreditCanvas.gameObject.SetActive(false);
            //     gameMgr.uiMgr.ui_game.game_btn_skip.gameObject.SetActive(false);

            //uiMgr.SetUIActive(UIWindow.TITLE, false); //UI 변경

            //Destroy(gameObject);

        }

        /// <summary>
        /// 6/26/2024-LYI
        /// 전체 게임 클리어 순간 체크
        /// </summary>
        public bool IsGameAllCleared()
        {
            //마지막 플레이 스테이지가 있는 경우
            if (lastStage != null)
            {
                //마지막 스테이지가 클리어 된 경우
                bool isClear = ES3.Load<bool>(lastStage.stageNum.ToString());
                if (isClear)
                {
                    //마지막 스테이지 번호 스테이지 목록에 있을 때 
                    if (gameMgr.dataMgr.dic_StageToMixed.ContainsKey(lastStage.stageNum))
                    {
                        int mixedNum = gameMgr.dataMgr.dic_StageToMixed[lastStage.stageNum];
                        mixedNum -= 9000;
                        //마지막 스테이지 번호가 스테이지목록의 마지막일 때
                        if (mixedNum >= gameMgr.dataMgr.dic_mixedToStage.Count)
                        {
                            //전체 클리어 체크
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 6/27/2024-LYI
        /// 게임 타입에 따른 배경음 변화 추가
        /// </summary>
        public void ChangeStageBGM(int stageNum)
        {
            if (stageNum == 1000)
            {
                //튜토리얼인 경우
                gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_MAZE));

            }
            else
            {
                StageType type = (StageType)(stageNum / 1000);

                switch (type)
                {
                    case StageType.MAZE:
                        gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_MAZE));
                        break;
                    case StageType.CROSSING:
                        gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_CROSSING));
                        break;
                    case StageType.MEMORY:
                        gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_MEMORY));
                        break;
                    case StageType.BLOCK:
                        gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_BLOCK));
                        break;
                    default:
                        gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_STAGE));
                        break;
                }

            }
        }


        /// <summary>
        /// 7/16/2024-LYI
        /// 스테이지 클리어 시 일시정지하고 응원석 언락을 위해 불러오는 경우
        /// </summary>
        public void ClearCheerUnlock()
        {
            cheeringSeat.headerSelect.mmf_selectMode.Events.OnComplete.AddListener(() =>
            {
                cheeringSeat.OnCharacterUnlock();
                cheeringSeat.headerSelect.mmf_selectMode.Events.OnComplete.RemoveAllListeners();
            });

            cheeringSeat.headerSelect.mmf_cheeringMode.Events.OnComplete.AddListener(() =>
            {
                LoadNextStage();
                cheeringSeat.headerSelect.mmf_cheeringMode.Events.OnComplete.RemoveAllListeners();
            });

            //테이블 다운, OnTableUp에서 CheeringStage 열기
            gameMgr.tableMgr.playTable.SetTableDown(()=>cheeringSeat.headerSelect.ActiveHeaderSelect());


        }


        /// <summary>
        /// 4/12/2024-LYI
        /// 손잡이 이동 시 네비 관련 처리
        /// </summary>
        public void OnGrabStart()
        {
            if (statPlay == PlayStatus.PLAY)
            {
                //미로인 경우만 작동
                if (currentStage.Data.typeStage == StageType.MAZE)
                {
                    //네비 일시정지
                    for (int i = 0; i < list_activeCharacter.Count; i++)
                    {
                        list_activeCharacter[i].Stop();
                    }
                }
            }
        }

        /// <summary>
        /// 4/12/2024-LYI
        /// 손잡이 이동 시 네비 관련 처리
        /// </summary>
        public void OnGrabEnd()
        {
            if (statPlay == PlayStatus.PLAY)
            {
                //미로인 경우만 작동
                if (currentStage.Data.typeStage == StageType.MAZE)
                {
                    currentStage.SetNavigationGraph();
                }
            }
        }

    }
}