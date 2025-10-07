using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

namespace VRTokTok
{
    /// <summary>
    /// 2/22/2024-LYI
    /// 테이블 등장 연출과 스테이지 생성, 제거 관리
    /// </summary>
    public class PlayDockingTable : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("PlayTable")]

        /// <summary>
        /// Where stage place
        /// </summary>
        public Transform stageAnchor;
        public Transform tr_stageDownPlane; //테이블 밑바닥의 위치, 오브젝트 초기화 체크 용도

        public MMF_Player mmf_up;
        public MMF_Player mmf_down;

        public UnityEvent onTableDown;
        public UnityEvent onTableUp;

        GameObject stageToSpawn = null;

        //생성된 스테이지 관리
        public List<GameObject> list_disableStage = new List<GameObject>();



        public bool isTableDown = true;



        public void PlayTableInit()
        {
            gameMgr = GameManager.Instance;

            mmf_up.Initialization();
            mmf_down.Initialization();

            onTableDown.AddListener(OnStageDown);
            onTableUp.AddListener(OnStageUp);

            mmf_down.Events.OnComplete = onTableDown;
            mmf_up.Events.OnComplete = onTableUp;

            stageAnchor.transform.localPosition = Vector3.up * -0.5f;
        }

        /// 2/23/2024-LYI
        /// 첫 메뉴로 돌아가기
        /// </summary>
        public void SetTableMenu()
        {
            gameMgr.playMgr.CharacterInit();

            StartTableChange();
        }

        /// <summary>
        /// 10/26/2023-LYI
        /// Display, 외부클래스에서 호출
        /// 매개변수로 들어온 스테이지를 현재 로테이션 아래에 할당
        /// 그 후 회전 시작
        /// </summary>
        /// <param name="stage"></param>
        public void SetTableStage(GameObject stage)
        {
            stageToSpawn = stage;

            StartTableChange();

        }


        public void TempEventAdd(UnityEvent target, UnityAction addFunc)
        {
            target.AddListener(addFunc);
            target.AddListener(EventReset);
        }
        public void TempEventRemove(UnityEvent target, UnityAction removeFunc)
        {
            target.RemoveListener(removeFunc);
        }
        public void EventReset()
        {
            onTableDown.RemoveAllListeners();
            onTableUp.RemoveAllListeners();

            onTableDown.AddListener(OnStageDown);
            onTableUp.AddListener(OnStageUp);
        }

        /// <summary>
        /// 6/18/2024-LYI
        /// 플레이 도중 현재 스테이지 내리기, 일시정지
        /// 현재 스테이지 비활성화?
        /// </summary>
        public void SetTableDown(UnityAction actionAfterDown = null)
        {
            if (isTableDown)
            {
                return;
            }

            if (actionAfterDown != null)
            {
                TempEventAdd(onTableDown, actionAfterDown);
            }


            gameMgr.playMgr.currentStage.StageInit();
            gameMgr.playMgr.CharacterInit();
            stageToSpawn = null;
            mmf_down.PlayFeedbacks();

        }

        /// <summary>
        /// 6/18/2024-LYI
        /// 플레이 도중 스테이지 내려진 뒤, 다시 올리기
        /// </summary>
        public void SetTableUp()
        {
            if (!isTableDown)
            {
                return;
            }

            gameMgr.playMgr.CharacterInit();
            stageToSpawn = null;
            mmf_up.PlayFeedbacks();

        }


        /// <summary>
        /// 2/23/2024-LYI
        /// 스테이지 변경 시작
        /// </summary>
        /// <param name="action"></param>
        public void StartTableChange()
        {
            if (gameMgr.playMgr.statPlay != Manager.PlayStatus.NONE)
            {
                gameMgr.ChangeGameStat(GameStatus.GAME);
            }

            if (gameMgr.playMgr.currentStage == null ||
                 isTableDown == true)
            {
                OnStageDown();
            }
            else
            {
                //이동 후 Stage Down 호출
                if (!mmf_down.IsPlaying)
                {
                    mmf_down.PlayFeedbacks();
                }
            }
        }



        /// <summary>
        /// 6/18/2024-LYI
        /// 테이블 내려간 뒤 호출
        /// 스테이지 생성
        /// 오브젝트 풀링 적용해서 중복 스테이지 최적화
        /// </summary>
        /// <param name="origin"></param>
        void SpawnStage(GameObject origin)
        {
            GameObject stage =// Instantiate(origin, stageAnchor);
            gameMgr.objPoolingMgr.CreateObject(list_disableStage, origin, stageAnchor.position, stageAnchor);
            //성능 문제로 오브젝트 풀링

            stage.transform.localPosition = Vector3.zero;
            stage.transform.localRotation = Quaternion.identity;
            stage.gameObject.SetActive(true);
            gameMgr.playMgr.currentStage = stage.GetComponent<Manager.StageManager>();
        }


        /// <summary>
        /// 10/26/2023-LYI
        /// 아래에 내려간 뒤 호출
        /// </summary>
        public void OnStageDown()
        {
            Debug.Log(gameObject.name + ": StageDown()");
            isTableDown = true;

            gameMgr.playMgr.CharacterInit();

            //스테이지 전체 비활성화
            for (int i = 0; i < stageAnchor.childCount; i++)
            {
                //6/20/2024-LYI 오브젝트 풀링 적용
                gameMgr.objPoolingMgr.ObjectInit(list_disableStage, stageAnchor.GetChild(i).gameObject, stageAnchor);
                //stageAnchor.GetChild(i).gameObject.SetActive(false);
            }


            if (gameMgr.playMgr.statPlay == Manager.PlayStatus.LOADING)
            {
                gameMgr.ChangeGameStat(GameStatus.LOADING);
            }

            if (gameMgr.playMgr.statPlay == Manager.PlayStatus.NONE)
            {
                //메인 메뉴로 돌아갈 때
                gameMgr.playMgr.cheeringSeat.ResetCheeringCharacter();
                gameMgr.ChangeGameStat(GameStatus.MENU);
            }
            else
            {
                //6/18/2024-LYI
                //스폰할 스테이지 없으면 그냥 중지
                if (stageToSpawn != null)
                {
                    gameMgr.ChangeGameStat(GameStatus.GAME);

                    //스테이지 시작할 때
                    //맵 생성
                    SpawnStage(stageToSpawn);

                    //맵 올라오는 동작 실행
                    mmf_up.PlayFeedbacks();
                }
            }
        }


        /// <summary>
        /// 2/27/2024-LYI
        /// 맵이 올라왔을 때 호출
        /// </summary>
        public void OnStageUp()
        {
            Debug.Log(gameObject.name + ": StageUp()");
            isTableDown = false;

            if (stageToSpawn == null &&
                gameMgr.playMgr.currentStage != null)
            {
                gameMgr.playMgr.currentStage.gameObject.SetActive(true);
                gameMgr.playMgr.currentStage.RestartStage();
                return;
            }

            if (gameMgr.playMgr.statPlay != Manager.PlayStatus.NONE)
            {
                //다음 스테이지 불러올 때
                gameMgr.playMgr.ReadyStage();
            }
        }
    }
}