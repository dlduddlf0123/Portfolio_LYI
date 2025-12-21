using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

namespace AroundEffect
{

    public enum GameState
    {
        NONE = 0,
        WARNING,
        ARCUBE,
        SELECT,
        EPISODE,
    }
    public enum Language
    {
        KOREAN = 0,
        ENGLISH = 1,
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Manager")]
        public AddressableManager addressableMgr;
        public ObjectPoolingManager objPoolingMgr;
        public SoundManager soundMgr;

        public ARCardManager arCardMgr;

        public UI_NumberCard_Game ui_game;

        [Header("Property")]
        public Camera mainCam;

        public GameState statGame = GameState.NONE;


        public Language gameLanguage = Language.KOREAN;

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

        // Start is called before the first frame update
        void Awake()
        {
            if (FindObjectsOfType(typeof(GameManager)).Length > 1)
            {
                Destroy(this.gameObject);
                return;
            }

        }

        private void Start()
        {
            OnAppStart();
        }

        public void ChangeBGM(GameState state)
        {
            //switch (state)
            //{
            //    case GameState.NONE:
            //        break;
            //    case GameState.WARNING:
            //    case GameState.ARCUBE:
            //        soundMgr.ChangeBGMAudioSource(audio_cube);
            //        break;
            //    case GameState.SELECT:
            //        soundMgr.ChangeBGMAudioSource(audio_menu);
            //        break;
            //    case GameState.EPISODE:
            //        break;
            //    default:
            //        break;
            //}
        }


        /// <summary>
        /// 3/18/2024-LYI
        /// 앱 시작 시
        /// </summary>
        public void OnAppStart()
        {
            gameLanguage = ES3.Load<Language>(Constants.ES3.GAME_LANGUAGE, Language.KOREAN);


            //UI 초기화
            //ui_librarySelect.Init();
            //ui_popup.Init();
            //ui_warning.Init();
            //ui_setting.Init();


            //statGame = GameState.WARNING;
            //ChangeBGM(statGame);

            //인터넷 확인 후 각 에피소드 다운로드 상태 확인
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    ui_popup.OpenWarningPopup();
            //}
            //else
            //{
            //    for (int i = 0; i < ui_librarySelect.arr_episodeButton.Length; i++)
            //    {
            //        int num = i;
            //        addressableMgr.CheckLabelStatus(ui_librarySelect.arr_episodeButton[num]);
            //    }
            //}
        }
        public void OnAddreessableLoadComplete()
        {
            addressableMgr.isLoadComplete = true;


            ui_game.Init();
        }

    }
}