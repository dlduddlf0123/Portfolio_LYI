using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System.IO;
using ReadOnly;

public class GameManager : MonoBehaviour
{
    public UIManager uiMgr;
    public SoundManager soundMgr;

    public WordCardManager wordCardMgr;



    //AssetBundles
    public AssetBundle b_prefab { get; set; }
    public AssetBundle b_sprite { get; set; }
    public AssetBundle b_sound { get; set; }
    public AssetBundle b_csvdata { get; set; }
    public AssetBundle b_animator { get; set; }


    public ParticleSystem[] arr_particles;
    public Camera mainCam;

    public int screenWidth = 1440;
    public int screenHeight = 2960;

    //Save Datas
    public int language; //0:korean 1: english



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

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        
        b_sound = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sounds"));
    }

    private void Start()
    {
       // soundMgr.PlayBgm(soundMgr.LoadClip(Defines.SOUND_BGM_STAGE1));
    }

    //void LoadWords()
    //{
    //    GameObject go = new GameObject();

    //    for (int i = 0; i < 10; i++)
    //    {
    //        WordCard _word = Instantiate(go).GetComponent<WordCard>();
    //        if (_word.subject ==  WordSubject.FRUIT &&
    //            _word.num == i)
    //        {
    //            wordCardMgr.list_wordCard.Add("fruit", _word);
    //        }
    //        else
    //        {
    //            Debug.LogError("Error: Word Subject is Wrong!!");
    //        }
            
    //    }
       
    //}


    #region 이펙트 관련 함수
    /// <summary>
    /// 파티클 재생 함수
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_go"></param>
    public void PlayEffect(Vector3 _position, ParticleSystem _p)
    {
        _p.transform.position = _position;
        _p.Play();
    }
    public void PlayEffect(Transform _position, ParticleSystem _p,string _path)
    {
        _p.transform.position = _position.position;
        _p.Play();
        soundMgr.PlaySfx(_position, soundMgr.LoadClip(_path));
    }


    public void HandEffect(Transform _hand, int _particle = 0, string _clip = Defines.SOUND_SFX_CLICK)
    {
        PlayEffect(_hand.position, arr_particles[_particle]);
        soundMgr.PlaySfx(_hand.transform, soundMgr.LoadClip(_clip));
    }

    public void PlayParticleEffect(Vector3 _pos, GameObject _go)
    {
        ParticleSystem _particle = Instantiate(_go).GetComponent<ParticleSystem>();
        _particle.transform.position = _pos;
        _particle.transform.localScale = Vector3.one * uiMgr.stageSize;

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
        ParticleSystem _particle = Instantiate(b_prefab.LoadAsset<GameObject>(_path)).GetComponent<ParticleSystem>();
        _particle.transform.position = _pos;
        _particle.transform.localScale = Vector3.one * uiMgr.stageSize;

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
    #endregion

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
