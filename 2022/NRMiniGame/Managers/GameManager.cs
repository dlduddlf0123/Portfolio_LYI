using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public enum GameStatus
{
	NONE = 0,
	MENU = 1,
	GAMESELECT,
	CHARACTERSELECT,
	READY,
	GAMEPLAY,
	RESULT,
}

public enum GameLanguage
{
	ENGLISH = 0,
	KOREAN = 1,
}

public class GameManager : MonoBehaviour
{
	public SoundManager soundMgr { get; set; }
	public CSVManager csvMgr { get; set; }
	public MiniGameManager miniGameMgr { get; set; }

	public UIManager uiMgr;
	public NRUIManager nrUiMgr;

	public EpisodeManager[] arr_episode;
	public EpisodeManager currentEpisode = null;


	//AR
	public Camera mainCamera;

	public HandController handCtrlL;
	public HandController handCtrlR;

	public Light mainLight;

	public GameStatus statGame = GameStatus.NONE;
	public GameLanguage currentLanguage = GameLanguage.ENGLISH;

	public AssetBundle b_stagePrefab { get; set; }
	public AssetBundle b_sound { get; set; }
	public AssetBundle b_lipsync { get; set; }
	public AssetBundle b_sprite { get; set; }

	public AssetBundle[] b_arr_voice { get; set; }

	public AssetBundle b_csvkor { get; set; }
	public AssetBundle b_csveng { get; set; }
	public AssetBundle b_currentCSV { get; set; }

	public GameObject particle_touch;

	public bool isDebug = false;
	public float waitTime = 0.3f;
	public bool isTutorial = false;
	public bool isEducation = false;

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

	// Use this for initialization
	void Awake()
	{
		if (s_instance != null &&
			s_instance != this)
		{
			Debug.LogError("Cannot have two Singleton");
			Destroy(this.gameObject);
			return;
		}
		s_instance = this;

		soundMgr = GetComponent<SoundManager>();
		csvMgr = GetComponent<CSVManager>();
		miniGameMgr = GetComponent<MiniGameManager>();

		b_stagePrefab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "stageprefab"));
		b_sound = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sound"));
		//b_csvkor = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "csvkor"));
		b_csveng = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "csveng"));
		//b_sprite = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sprite"));

		b_arr_voice = new AssetBundle[1];
		for (int i = 0; i < b_arr_voice.Length; i++)
		{
			b_arr_voice[i] = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "voice_ep" + (i + 1)));
		}

		ChangeLanguage((GameLanguage)PlayerPrefs.GetInt("Language", 0));
	}

	private void Start()
	{
		statGame = GameStatus.MENU;

		isTutorial = true;

#if !UNITY_EDITOR
isDebug = false;
#endif
	}

	public void ChangeLanguage(GameLanguage _language)
	{
		currentLanguage = _language;
		PlayerPrefs.SetInt("Language", (int)_language);

		switch (_language)
		{
			case GameLanguage.ENGLISH:
				b_currentCSV = b_csveng;
				break;
			case GameLanguage.KOREAN:
				b_currentCSV = b_csvkor;
				break;
			default:
				b_currentCSV = b_csveng;
				break;
		}
	}



	/// <summary>
	/// 스테이지 선택 완료 시 해당 위치에 스테이지 생성 후 시작
	/// 버튼으로 동작
	/// </summary>
	public void SetStage(int _stageNum)
	{
        if (currentEpisode != null)
        {
			Debug.LogError("Episode Prefab Already Exist!");
			return;
        }

        if (!isTutorial)
        {
			_stageNum = 5;
        }

		GameObject stage = Instantiate(arr_episode[_stageNum].gameObject);

		stage.transform.localScale = Vector3.one * uiMgr.stageSize;
		currentEpisode = stage.GetComponent<EpisodeManager>();

		currentEpisode.ActiveStage(0);
	}

	public void GameStart()
    {
		uiMgr.SetUIActive(UIWindow.GAME, false);

		uiMgr.fadeCanvas.StartFade(() =>
		{

			currentEpisode.currentStage.gameObject.SetActive(true);
			currentEpisode.currentStage.StartStage();
		});
	}


	public IEnumerator LateFunc(UnityAction _action, float _time = 1f)
	{
		yield return new WaitForSeconds(_time);
		_action.Invoke();
	}
	public IEnumerator LateFrameFunc(UnityAction _action, float _frame = 1f)
	{
		for (int i = 0; i < _frame; i++)
		{
			yield return new WaitForEndOfFrame();
		}
		_action.Invoke();
	}

	public void PlayParticleEffect(Vector3 _pos, GameObject _go)
	{
		ParticleSystem _particle = Instantiate(_go).GetComponent<ParticleSystem>();
		_particle.transform.position = _pos;

		if (_particle.transform.childCount != 0)
		{
			ParticleSystem[] arr_particle = _particle.GetComponentsInChildren<ParticleSystem>();

			for (int index = 0; index < arr_particle.Length; index++)
			{
				arr_particle[index].Play();
			}
		}
		else
		{
			_particle.Play();
		}

		Destroy(_particle.gameObject, _particle.main.duration + 1);
	}
	public void PlayParticleEffect(Vector3 _pos, string _path)
	{
		ParticleSystem _particle = Instantiate(b_stagePrefab.LoadAsset<GameObject>(_path)).GetComponent<ParticleSystem>();
		_particle.transform.position = _pos;

		if (_particle.transform.childCount != 0)
		{
			ParticleSystem[] arr_particle = _particle.GetComponentsInChildren<ParticleSystem>();

			for (int index = 0; index < arr_particle.Length; index++)
			{
				arr_particle[index].Play();
			}
		}
		else
		{
			_particle.Play();
		}

		Destroy(_particle.gameObject, _particle.main.duration + 1);
	}
	public void PlayTouchParticle(Vector3 _pos)
	{
		ParticleSystem _particle = Instantiate(particle_touch).GetComponent<ParticleSystem>();
		_particle.transform.position = _pos;

		if (_particle.transform.childCount != 0)
		{
			ParticleSystem[] arr_particle = _particle.GetComponentsInChildren<ParticleSystem>();

			for (int index = 0; index < arr_particle.Length; index++)
			{
				arr_particle[index].Play();
			}
		}
		else
		{
			_particle.Play();
		}

		Destroy(_particle.gameObject, _particle.main.duration + 1);
	}

	//public void CheckInternetActive()
	//   {
	//       if (Application.internetReachability == NetworkReachability.NotReachable)
	//       {
	//		uiMgr.ShowInternetError();		
	//       }
	//       else
	//	{
	//		StartCoroutine(CheckInternetAccess());
	//	}
	//   }
	//IEnumerator CheckInternetAccess()
	//   {
	//	WWW www = new WWW("http://www.google.com");
	//	yield return www;
	//	if (!www.isDone ||
	//		www.bytesDownloaded == 0 ||
	//		www.error != null)
	//	{
	//		uiMgr.ShowInternetError();
	//	}
	//}

	public void QuitApp()
	{
		Application.Quit();
		
	}

}
