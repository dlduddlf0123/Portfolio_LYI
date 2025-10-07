using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.XR.CoreUtils;
using Unity.PolySpatial;
using UnityEngine.XR.ARFoundation;

namespace AroundEffect
{
    public enum GameStatus
    {
        TITLE = 0,
        LOADING = 1,
        SURFACE,
        LIFE,
        RACING,
        MINIGAME,
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        public AddressableManager addressableMgr;
        public SoundManager soundMgr;
        public ObjectPoolingManager objPoolingMgr;
        public DataManager dataMgr;
        public InventoryManager invenMgr;

        public LifeManager lifeMgr;
        // public RaceManager raceMgr;

        [Header("MR")]
        public MRManager MRMgr;

        [Header("Properties")]
        public GameStatus statGame;

        //Save Datas
        public int language; //0:korean 1: english
        public bool isTutorial = false; //True인 경우 실행 시 튜토리얼 진행


        //싱글톤 
        private static GameManager s_instance = null;
        public static GameManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                }
                return s_instance;
            }
        }


        void Awake()
        {
            if (FindObjectsOfType(typeof(GameManager)).Length > 1)
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this);
        }

        void Start()
        {

        }



        public void OnAddreessableLoadComplete()
        {
            addressableMgr.isLoadComplete = true;



        }


        /// <summary>
        /// 11/13/2023-LYI
        /// 게임 상태 바뀔 때의 처리 변경
        /// 사운드, ui 등 상태변경
        /// </summary>
        /// <param name="stat"></param>
        public void ChangeGameStat(GameStatus stat)
        {
            Debug.Log("ChangeGameStat: " + stat.ToString());

            statGame = stat;
            //switch (stat)
            //{
            //    case GameStatus.MENU:
            //        break;
            //    case GameStatus.LOADING:
            //        break;
            //    case GameStatus.SELECT:
            //        break;
            //    case GameStatus.GAME:
            //        break;
            //    default:
            //        break;
            //}

            soundMgr.ChangeSceneBGM(statGame);
        }


        /// <summary>
        /// 7/13/2023-LYI
        /// 메인메뉴에서 게임 시작 시 호출
        /// 씬 이동, 플레이매니저 할당
        /// 플레이매니저에서 스테이지 시작하도록 호출
        /// </summary>
        public void GameStart(int stageNum = 0)
        {


        }


    }
}
    