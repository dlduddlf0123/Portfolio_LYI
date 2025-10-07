using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Audio;
using VRTokTok;

public class SoundManager : MonoBehaviour
{
    GameManager gameMgr;

    public float bgmVolume = 0.7f;
    public float sfxVolume = 1.0f;
    public float voiceVolume = 1.0f;

    public bool isSfxMute = false;

    public float blendSpeed = 10f;

    // GameObject bgmObj;
    public AudioSource bgmSource;

    public List<GameObject> list_sfx;

    public AudioMixerGroup mixer_bgm;
    public AudioMixerGroup mixer_sfx;

    Transform sfxPool;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        //bgmSource = GetComponent<AudioSource>();
        LoadVolume();

        list_sfx = new List<GameObject>();
        sfxPool = this.transform.GetChild(0);
    }

    private void Start()
    {
        ChangeBGMAudioSource(GetComponent<AudioSource>());
    }

    public void SaveVolume()
    {
        ES3.Save(Constants.Sound.BGM_VOLUME, bgmVolume);
        ES3.Save(Constants.Sound.SFX_VOLUME, sfxVolume);
        //PlayerPrefs.SetFloat("BGM Volume", bgmVolume);
        //PlayerPrefs.SetFloat("SFX Volume", sfxVolume);

        Debug.Log("Volume Save --  BGM:" + bgmVolume + "/SFX:" + sfxVolume);
    }

    public void ChangeSceneBGM(GameStatus scene)
    {
        switch (scene)
        {
            case GameStatus.MENU:
                ChangeBGMAudioClip(LoadClip(Constants.Sound.BGM_MENU));
                break;
            case GameStatus.SELECT:
                ChangeBGMAudioClip(LoadClip(Constants.Sound.BGM_HEADER_SELECT));
                break;
            case GameStatus.GAME:
                //ChangeBGMAudioClip(LoadClip(Constants.Sound.BGM_STAGE));
                break;
            default:
                break;
        }
    }


    public void LoadVolume()
    {
        bgmVolume = ES3.Load(Constants.Sound.BGM_VOLUME, 0.7f);
        sfxVolume = ES3.Load(Constants.Sound.SFX_VOLUME, 1.0f);
        Debug.Log("Volume Load --  BGM:" + bgmVolume + "/SFX:" + sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    public AudioClip LoadClip(string path)
    {
        if (!gameMgr.addressableMgr.isLoadComplete)
        {
            Debug.Log("!!!AddressableLoad not finished!!!");
            return null;
        }
        return  gameMgr.addressableMgr.dic_audioClip[path];
    }

    public void ChangeBGMAudioSource(AudioSource audio, bool isLerp = true)
    {
        if (audio == bgmSource)
        {
            return;
        }
        Debug.Log("BGM Change:" + audio.gameObject.name);
        if (isLerp)
        {
            StartCoroutine(ChangeBGMSourceLerp(audio));
        }
        else
        {
            bgmSource.Stop();
            bgmSource = audio;
            bgmSource.Play();
        }
    }

    public void ChangeBGMAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.Log("ChangeBGMAudioClip(): Clip is Null");
            return;
        }
        if (clip == bgmSource.clip)
        {
            Debug.Log("ChangeBGMAudioClip(): Clip is Same");
            return;
        }

        Debug.Log("SoundManager ChangeBGM: " + clip.name);
        StartCoroutine(ChangeBGMClipLerp(clip));
    }
    IEnumerator ChangeBGMSourceLerp(AudioSource audio)
    {
        float t = 0;

        if (bgmSource != null &&
            bgmSource.isPlaying)
        {
            t = bgmVolume;
            while (t > 0)
            {
                t -= 0.01f * blendSpeed;
                bgmSource.volume = t;
                yield return new WaitForSeconds(0.01f);
            }
            bgmSource.Stop();
        }
        bgmSource = audio;
        bgmSource.Play();

        while (t < bgmVolume)
        {
            t += 0.01f * blendSpeed;
            bgmSource.volume = t;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator ChangeBGMClipLerp(AudioClip clip)
    {
        float t = 0;

        if (bgmSource != null &&
            bgmSource.isPlaying)
        {
            t = bgmVolume;
            while (t > 0)
            {
                t -= 0.01f* blendSpeed;
                bgmSource.volume = t;
                yield return new WaitForSeconds(0.01f);
            }
            bgmSource.Stop();
        }
        bgmSource.clip = clip;
        bgmSource.Play();

        while (t < bgmVolume)
        {
            t += 0.01f * blendSpeed;
            bgmSource.volume = t;
            yield return new WaitForSeconds(0.01f);
        }
    }

    //배경음악 재생함수(음악파일)
    public void PlayBgm(AudioClip bgm = null)
    {
        if (bgm == null)
        {
            bgmSource.Stop();
            return;
        }
        //음악 파일 설정
        bgmSource.clip = bgm;
        //볼륨 설정
        bgmSource.volume = bgmVolume;
        //재생
        bgmSource.Play();
        //반복
        bgmSource.loop = true;
    }


    #region Play SFX
    //효과음 재생함수 (재생할 위치, 재생할 소리) 
    public void PlaySfx(Transform pos, AudioClip sfx, float pitch = 1.0f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        //음소거일경우 반환
        if (isSfxMute) return;


        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.SetParent(pos);
            soundObj.transform.position = pos.position;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.SetParent(pos);
            list_sfx[0].transform.position = pos.position;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();
            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(list_sfx[0]));
            list_sfx.RemoveAt(0);
        }

    }

    public void PlaySfx(Transform pos, string path, float pitch = 1.0f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        AudioClip sfx = LoadClip(path);

        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.position = pos.position;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.position = pos.position;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();
            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(list_sfx[0]));
            list_sfx.RemoveAt(0);
        }

    }

    //Vector3 
    public void PlaySfx(Vector3 pos, AudioClip sfx, float pitch = 1.0f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.position = pos;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.position = pos;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();
            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(list_sfx[0]));
            list_sfx.RemoveAt(0);
        }

    }

    public void PlaySfx(Vector3 pos, string path, float pitch = 1.0f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        AudioClip sfx = LoadClip(path);

        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.position = pos;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();

            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.position = pos;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();

            SetSFX(audioSource, sfx, pitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(list_sfx[0]));
            list_sfx.RemoveAt(0);
        }

    }
    public void PlaySfxRandomPitch(Vector3 pos, AudioClip sfx, float pitchRange = 0.2f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.position = pos;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();

            float randomPitch = Random.Range(1 - pitchRange, 1 + pitchRange);
            SetSFX(audioSource, sfx, randomPitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.position = pos;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();
            float randomPitch = Random.Range(1 - pitchRange, 1 + pitchRange);
            SetSFX(audioSource, sfx, randomPitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(list_sfx[0]));
            list_sfx.RemoveAt(0);
        }
    }
    public void PlaySfxRandomPitch(Vector3 pos, string path, float pitchRange = 0.2f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        AudioClip sfx = LoadClip(path);

        if (list_sfx.Count == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.position = pos;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();

            float randomPitch = Random.Range(1 - pitchRange, 1 + pitchRange);
            SetSFX(audioSource, sfx, randomPitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            list_sfx[0].SetActive(true);
            list_sfx[0].transform.position = pos;

            AudioSource audioSource = list_sfx[0].GetComponent<AudioSource>();

            float randomPitch = Random.Range(1 - pitchRange, 1 + pitchRange);
            SetSFX(audioSource, sfx, randomPitch, spatialBlend, mixer);

            StartCoroutine(StopSfx(list_sfx[0]));
            list_sfx.RemoveAt(0);
        }

    }

    /// <summary>
    /// SFX 파라미터 설정
    /// </summary>
    /// <param name="audioSource"> 재생할 소스</param>
    /// <param name="sfx">재생할 클립</param>
    /// <param name="pitch">피치 설정</param>
    /// <param name="spatialBlend">2D 3D 설정</param>
    /// <param name="mixer">효과음 할당할 믹서 설정</param>
    void SetSFX(AudioSource audioSource, AudioClip sfx, float pitch = 1.0f, float spatialBlend = 1f, AudioMixerGroup mixer = null)
    {
        Debug.Log("PlaySfx:" + sfx.name);
        //재생할 소리 입력, 최소거리, 최대거리
        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.spatialBlend = spatialBlend;
        //볼륨 조절
        audioSource.volume = sfxVolume;
        audioSource.pitch = pitch;
        //21-12-02 AudioMixer
        if (mixer == null)
            audioSource.outputAudioMixerGroup = mixer_sfx;
        else
            audioSource.outputAudioMixerGroup = mixer;
        //소리 재생
        audioSource.Play();
    }

    #endregion

    /// <summary>
    /// sfx풀로 반환
    /// </summary>
    /// <param name="sfxObj"></param>
    /// <returns></returns>
    IEnumerator StopSfx(GameObject sfxObj)
    {
        AudioSource audioSource = sfxObj.GetComponent<AudioSource>();
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        if (audioSource != null && audioSource.gameObject.activeSelf)
        {
            audioSource.Stop();
        }

        sfxObj.transform.SetParent(sfxPool);
        list_sfx.Add(sfxObj);
        sfxObj.SetActive(false);
    }
}