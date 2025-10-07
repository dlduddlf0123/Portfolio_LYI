using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.UI;

using UnityEngine.AddressableAssets;
public enum SceneStatus
{
	//TITLE = 0,
	//TITLE_LOADING,
	MAIN = 0,
	GAME_LOADING,
	GAME,
	PRACTICE,
}

public enum GameLanguage
{
	ENGLISH = 0,
	KOREAN = 1,
}


namespace Burbird
{


	/// <summary>
	/// 매니저 클래스 관리, 에셋번들 관리
	/// 앱 상태 관리
	/// </summary>
	public class GameManager : MonoBehaviour
	{
		public UIManager uiMgr;
		public SoundManager soundMgr;
		public DataManager dataMgr;
		public CSVLoader csvLoader;
		public TimeManager timeMgr;

		public InventoryChecker invenChecker;
		public AddressableManager addressMgr;

		public ObjectPoolingManager objPoolingMgr;

		public StageManager stageMgr;
		public StageData playStageData;

		public Loading loader;

		public BackendManager backendMgr;

		public SceneStatus statGame;
		public GameLanguage currentLanguage = GameLanguage.KOREAN;


		public bool isDebug = false;

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
			if (s_instance != null &&
				s_instance != this)
			{
				Debug.Log("Cannot have two Singleton");
				Destroy(this.gameObject);
				return;
			}
			s_instance = this;

			statGame = (SceneStatus)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

			CreateStaticManager();

			DontDestroyOnLoad(this);

			ChangeLanguage((GameLanguage)PlayerPrefs.GetInt("Language", 0));
		}

		private void Start()
		{


		}


		/// <summary>
		/// 뒤끝서버 객체 생성
		/// </summary>
		void CreateStaticManager()
        {

			// StaticManager가 없을 경우 새로 생성
			if (FindObjectOfType(typeof(StaticManager)) == null)
			{
				var obj = Resources.Load<GameObject>("Prefabs/StaticManager");
				Instantiate(obj);
			}

		}


		/// <summary>
		/// 게임 내 모든 텍스트는 읽어오는 방식으로 사용
		/// csv 파일을 교체하여 한번에 언어변경
		/// </summary>
		/// <param name="language"></param>
		public void ChangeLanguage(GameLanguage language)
		{
			currentLanguage = language;
			PlayerPrefs.SetInt("Language", (int)language);

			switch (language)
			{
				case GameLanguage.ENGLISH:
					
					break;
				case GameLanguage.KOREAN:
					
					break;
				default:
					
					break;
			}
		}


		/// <summary>
		/// 앱 실행 시 혹은 메인화면 시작 시 실행
		/// </summary>
		public void ApplicationDataInitialize()
        {

        }

		/// <summary>
		/// 모든 앱 데이터 순차 로딩
		/// </summary>
		/// <returns></returns>
		IEnumerator DataInitializeProcess()
		{
			yield return new WaitForSeconds(1);
		}


		/// <summary>
		/// 게임 시작 시
		/// </summary>
		public void GameStart()
		{
			//if (dataMgr.Stemina < currentStage.steminaForPlay)
			//{
			//	//스테미나 부족!!
			//	//스테미나 회복창 띄우기 (광고, 결제)

			//	return;
			//}


			//게임 씬으로 이동
			loader.LoadScene((int)SceneStatus.GAME,	()=>StartCoroutine(OnGameStart()));

		}

		public IEnumerator OnGameStart()
        {
			Debug.Log("OnGameStart()");
			stageMgr = GameObject.FindObjectOfType<StageManager>();

			statGame = SceneStatus.GAME;
			stageMgr.StageManagerInit(playStageData);
			invenChecker.RefreshEquipStat();
			yield return StartCoroutine(stageMgr.StageLoadLogic());
		}
		public void OnGameEnd()
		{
			Debug.Log("OnGameEnd()");
			statGame = SceneStatus.MAIN;
			uiMgr.RefreshUIElements();
		}

		public void PracticeStart()
        {
			loader.LoadScene((int)SceneStatus.PRACTICE, () => StartCoroutine(OnPracticeStart()));
        }
		/// <summary>
		/// 3/15/2023-LYI
		/// 연습모드 시작
		/// </summary>
		/// <returns></returns>
		public IEnumerator OnPracticeStart()
        {
			Debug.Log("OnPracticeStart()");
			stageMgr = GameObject.FindObjectOfType<StageManager>();

			statGame = SceneStatus.PRACTICE;
			stageMgr.StageManagerInit(playStageData);
			invenChecker.RefreshEquipStat();
			yield return StartCoroutine(stageMgr.PracticeLoadLogic());
		}

		public void GameExit()
        {
			Application.Quit();
        }

        private void OnApplicationQuit()
        {
			dataMgr.SaveLocalPlayerData();
			timeMgr.SaveLastTime();

		}
    }

}