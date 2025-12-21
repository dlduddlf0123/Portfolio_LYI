using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.IO;
using UnityEngine.Events;

public enum GameStatus
{
	NONE = 0,
	MENU = 1,
	ARPLANE = 2,
	SELECT = 3,
	CUTSCENE = 4,
	GAME = 5,
}

public enum GameLanguage
{
	ENGLISH = 0,
	KOREAN = 1,
}

public class GameManager : MonoBehaviour {

	public UIManager uiMgr;
	public SoundManager soundMgr;
	public DialogManager dialogMgr;
	public EpisodeManager[] arr_episode;
	public EpisodeManager currentEpisode = null;

	//AR
	public ARSession arSession;
	public ARPlaneManager arPlaneMgr;
	public PlaneGenerator planeGenerator;
	public HandController handCtrl;

	public GameStatus statGame = GameStatus.NONE;
	public GameLanguage currentLanguage = GameLanguage.KOREAN;

	public AssetBundle b_stagePrefab { get; set; }
	public AssetBundle b_sound { get; set; }
	public AssetBundle b_lipsync { get; set; }
	public AssetBundle b_sprite { get; set; }

	public AssetBundle[] b_arr_voice { get; set; }

	public AssetBundle b_csvkor { get; set; }
	public AssetBundle b_csveng { get; set; }
	public AssetBundle b_currentCSV { get; set; }

	public bool isDebug = false;
	public float waitTime = 0.3f;


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
	void Awake () {
        if (s_instance != null &&
			s_instance != this)
        {
			Debug.LogError("Cannot have two Singleton");
			Destroy(this.gameObject);
			return;
        }
		s_instance = this;

		b_stagePrefab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "stageprefab"));
		b_sound = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sound"));
		b_csvkor = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "csvkor"));
		b_csveng= AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "csveng"));
		b_sprite = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sprite"));

		b_arr_voice = new AssetBundle[5];
		//      for (int i = 0; i < b_arr_voice.Length; i++)
		//      {
		//	b_arr_voice[i] = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "voice_ep" + (i + 1)));
		//}
		b_arr_voice[3] = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "voice_ep" + (3 + 1)));

		ChangeLanguage((GameLanguage)PlayerPrefs.GetInt("Language", 0));
		ARSessionSetActive(false);
	}

    private void Start()
    {
		statGame = GameStatus.MENU;
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

    public void ARSessionSetActive(bool _isActive)
	{
		arSession.enabled = _isActive;
	}

	public void ARContentScale()
	{
		Camera.main.transform.parent.GetComponent<ARSessionOrigin>().MakeContentAppearAt(Camera.main.transform,Vector3.zero);
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


}
