using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnly;

public class SoundManager : MonoBehaviour
{
    GameManager gameMgr;

    public float bgmVolume = 0.7f;
    public float sfxVolume = 1.0f;

    public bool isSfxMute = false;

   // GameObject bgmObj;
    public AudioSource bgmSource;

    public List<GameObject> list_sfx;

    //Clips
    public AudioClip bgm_title { get; set; }
    public AudioClip bgm_dialog { get; set; }
    public AudioClip bgm_gamePlay { get; set; }
    public AudioClip sfx_bt { get; set; }

    Transform sfxPool;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        bgmSource = GetComponent<AudioSource>();

        list_sfx = new List<GameObject>();
        sfxPool = this.transform.GetChild(2);
    }

    private void Start()
    {
        //bgm_title = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_BGM_0) as AudioClip;
        //bgm_dialog = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_BGM_1) as AudioClip;
        //bgm_gamePlay = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_BGM_2) as AudioClip;
        //sfx_bt = gameMgr.b_sounds.LoadAsset<AudioClip>(Defines.SOUND_SFX_BUTTON) as AudioClip;
    }

    public AudioClip LoadClip(string _path)
    {
        return gameMgr.b_sounds.LoadAsset<AudioClip>(_path) as AudioClip;
    }

    //배경음악 재생함수(음악파일)
    public void PlayBgm(AudioClip _bgm = null)
    {
        if (_bgm == null)
        {
            bgmSource.Stop();
            return;
        }
        //음악 파일 설정
        bgmSource.clip = _bgm;
        //볼륨 설정
        bgmSource.volume = bgmVolume;
        //재생
        bgmSource.Play();
        //반복
        bgmSource.loop = true;
    }

    //효과음 재생함수 (재생할 위치, 재생할 소리) 
    public void PlaySfx(Transform _pos, AudioClip _sfx, float _pitch = 1.0f)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.SetParent(_pos);
            soundObj.transform.position = _pos.position;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            //재생할 소리 입력, 최소거리, 최대거리
            audioSource.clip = _sfx;
            audioSource.minDistance = 10.0f;
            audioSource.maxDistance = 30.0f;
            audioSource.spatialBlend = 1;
            //볼륨 조절
            audioSource.volume = sfxVolume;
            audioSource.pitch = _pitch;
            //소리 재생
            audioSource.Play();

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.SetParent(_pos);
            list_sfx[0].transform.position = _pos.position;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();
            //재생할 소리 입력, 최소거리, 최대거리
            audioSource.clip = _sfx;
            audioSource.minDistance = 10.0f;
            audioSource.maxDistance = 30.0f;
            audioSource.spatialBlend = 1;
            //볼륨 조절
            audioSource.volume = sfxVolume;
            audioSource.pitch = _pitch;
            //소리 재생
            audioSource.Play();
            audioSource.transform.SetParent(null);
            StartCoroutine(StopSfx(list_sfx[0].gameObject));
            list_sfx.RemoveAt(0);
        }

    }

    /// <summary>
    /// sfx풀로 반환
    /// </summary>
    /// <param name="sfxObj"></param>
    /// <returns></returns>
    IEnumerator StopSfx(GameObject sfxObj)
    {
        AudioSource audioSource = sfxObj.GetComponent<AudioSource>();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.Stop();

        sfxObj.transform.SetParent(sfxPool);
        list_sfx.Add(sfxObj);
        sfxObj.SetActive(false);
    }
}