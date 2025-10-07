using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
namespace VRTokTok
{

    /// <summary>
    /// 10/26/2023-LYI
    /// 테이블 회전과 스테이지 생성, 제거 관리
    /// </summary>
    public class PlayRotationTable : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Table Spin")]
        public Transform tr_spinPoint;

        /// <summary>
        /// Stage Anchor 0:Up 1:Down
        /// </summary>
        public Transform[] arr_tr_stage;
        public GameObject[] arr_stoneBoard;

        public MMF_Player mmf_spin;

        bool isUp = true;

        public UnityEvent OnSpinEnd;

        private void Awake()
        {
            gameMgr = GameManager.Instance;


        }

        void Start()
        {
            PlayTableInit();
        }

        public void PlayTableInit()
        {
            mmf_spin.Initialization();

            OnSpinEnd.AddListener(SpinEnd);

            mmf_spin.Events.OnComplete = OnSpinEnd;

            tr_spinPoint.localRotation = Quaternion.identity;
            isUp = true;

            for (int i = 0; i < arr_stoneBoard.Length; i++)
            {
                arr_stoneBoard[i].SetActive(true);
            }
        
        }

        public void DisableStone()
        {
            for (int i = 0; i < arr_stoneBoard.Length; i++)
            {
                arr_stoneBoard[i].SetActive(false);
            }
        }

        /// <summary>
        /// 10/26/2023-LYI
        /// 테이블 돌멩이로 회전시키기, 이후 TableInit?
        /// </summary>
        public void SetTableInit()
        {
            //for (int i = 0; i < arr_tr_stage.Length; i++)
            //{
            //    for (int j = 0; j < arr_tr_stage[i].childCount; i++)
            //    {
            //        arr_tr_stage[i].GetChild(i).gameObject.SetActive(false);
            //    }
            //}

            

            if (isUp)
            {
                for (int i = 0; i < gameMgr.playMgr.list_activeCharacter.Count; i++)
                {
                    gameMgr.playMgr.list_activeCharacter[i].SetParentMenu(arr_tr_stage[0].transform);
                }
                arr_stoneBoard[1].SetActive(true);
            }
            else
            {
                for (int i = 0; i < gameMgr.playMgr.list_activeCharacter.Count; i++)
                {
                    gameMgr.playMgr.list_activeCharacter[i].SetParentMenu(arr_tr_stage[1].transform);
                }
                arr_stoneBoard[0].SetActive(true);
            }

            SpinStage();
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
            if (isUp)
            {
                stage.transform.SetParent(arr_tr_stage[1]);
                arr_stoneBoard[1].SetActive(false);
            }
            else
            {
                stage.transform.SetParent(arr_tr_stage[0]);
                arr_stoneBoard[0].SetActive(false);
            }


            stage.transform.localPosition = Vector3.zero;
            stage.transform.localRotation = Quaternion.identity;
            stage.gameObject.SetActive(true);
            
            SpinStage();

        }


   

        /// <summary>
        /// 10/26/2023-LYI
        /// 스테이지 회전 시작
        /// </summary>
        /// <param name="action"></param>
        public void SpinStage()
        {
            mmf_spin.PlayFeedbacks();
            // StartCoroutine(SpinCoroutine(action));

        }

        /// <summary>
        /// 10/26/2023-LYI
        /// 아래에 내려간 스테이지 비활성화
        /// </summary>
        public void SpinEnd()
        {
            Debug.Log(gameObject.name + ": SpinEnd()");


            for (int i = 0; i < gameMgr.playMgr.list_activeCharacter.Count; i++)
            {
                gameMgr.playMgr.list_activeCharacter[i].ResetParent();
            }

            if (isUp)
            {
                for (int i = 0; i < arr_tr_stage[0].childCount; i++)
                {
                    arr_tr_stage[0].GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < arr_tr_stage[1].childCount; i++)
                {
                    arr_tr_stage[1].GetChild(i).gameObject.SetActive(false);
                }
            }

            isUp = !isUp;


            if (gameMgr.statGame == GameStatus.MENU)
            {
                //스핀으로 메뉴로 돌아갔을 때
                gameMgr.playMgr.cheeringSeat.ResetCheeringCharacter();
                gameMgr.playMgr.CharacterInit();
                gameMgr.tableMgr.ChangeTableUIStatus();
            }
            else
            {
                gameMgr.tableMgr.ChangeTableUIStatus();

                DisableStone();
                //스핀으로 다음 스테이지 불러올 때
                gameMgr.playMgr.ReadyStage();
            }
        }



        /// <summary>
        /// 10/26/2023-LYI
        /// 스테이지 회전 동작
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator SpinCoroutine(UnityAction action = null)
        {
            Quaternion start = tr_spinPoint.rotation;
            Quaternion desitination = Quaternion.Euler(tr_spinPoint.rotation * Vector3.forward * -180);

            float rotateTime = 2f;
            float t = 0f;
            WaitForSeconds wait = new WaitForSeconds(0.01f);
           
            while (t < 1)
            {
                t += 0.01f / rotateTime;

                tr_spinPoint.rotation = Quaternion.Lerp(start, desitination, t);

                yield return wait;
            }

            if (action != null)
            {
                action.Invoke();
            }
        }




    }
}