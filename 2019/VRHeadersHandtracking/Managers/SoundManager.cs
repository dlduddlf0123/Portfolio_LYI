using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour {

    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    public bool isSfxMute = false;

    GameObject bgmObj;
    public AudioSource bgmSource { get; set; }

    public List<GameObject> list_sfx;

    //public  AudioClip bgm_gamePlay;

    public Transform sfxPool;

    private void Awake()
    {
        list_sfx = new List<GameObject>();
        bgmObj = new GameObject("bgm");
        bgmSource = GetComponent<AudioSource>();
       // bgm_gamePlay = Resources.Load<AudioClip>(Defines.RESOURCE_SOUND_BGM_0) as AudioClip;
        //sfxPool = this.transform.GetChild(1);
    }

    public AudioClip LoadClip(string _path)
    {
        return Resources.Load<AudioClip>(_path) as AudioClip;
    }

    //배경음악 재생함수(음악파일)
    public void PlayBgm(AudioClip _bgm)
    {
        //if (bgmObj.GetComponent<AudioSource>() == null)
        //{
        //    bgmSource = bgmObj.AddComponent<AudioSource>();
        //}
        if (_bgm == null)
        {
            return;
        }
        bgmSource.Stop();
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
    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        //음소거일경우 반환
        if (isSfxMute) return;

        if (sfxPool.childCount == 0)
        {
            //Sfx 사운드 오브젝트 생성
            GameObject soundObj = new GameObject("Sfx");
            //사운드 오브젝트의 위치설정
            soundObj.transform.position = pos;
            //오디오 소스 생성
            AudioSource audioSource = soundObj.AddComponent<AudioSource>();
            //재생할 소리 입력, 최소거리, 최대거리
            audioSource.clip = sfx;
            audioSource.minDistance = 10.0f;
            audioSource.maxDistance = 30.0f;
            //볼륨 조절
            audioSource.volume = sfxVolume;
            //소리 재생
            audioSource.Play();
            list_sfx.Add(soundObj);
            StartCoroutine(StopSfx(soundObj));
        }
        else
        {
            sfxPool.GetChild(0).gameObject.SetActive(true);
            AudioSource audioSource = sfxPool.GetChild(0).GetComponent<AudioSource>();
            //재생할 소리 입력, 최소거리, 최대거리
            audioSource.clip = sfx;
            audioSource.minDistance = 10.0f;
            audioSource.maxDistance = 30.0f;
            //볼륨 조절
            audioSource.volume = sfxVolume;
            //소리 재생
            audioSource.Play();
            list_sfx.RemoveAt(0);
            StartCoroutine(StopSfx(sfxPool.GetChild(0).gameObject));
            sfxPool.GetChild(0).parent = this.transform;
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
        sfxObj.transform.parent = sfxPool;

        list_sfx.Add(sfxObj);
        sfxObj.SetActive(false);
    }
}
