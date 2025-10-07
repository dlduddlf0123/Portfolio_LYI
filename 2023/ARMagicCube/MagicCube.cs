using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Playables;


public class MagicCube : MonoBehaviour
{
    public AudioSource bgm_episode;
    public PlayableDirector director;

    public CubeType typeCube = CubeType.NONE;

    float directorTime = 0f;

    public virtual void MagicCubeInit()
    {
        directorTime = 0;
    }


    private void Update()
    {
        if (director.state == PlayState.Playing)
        {
            directorTime += Time.deltaTime;

            if (directorTime >= director.playableAsset.duration)
            {
                OnTimeLineEnd();
            }
        }
        //else
        //{
        //    directorTime = 0f;
        //}
    }


    public void PlayTimeline()
    {
        if (director.state == PlayState.Playing)
        {
            return;
        }

        if (bgm_episode != null)
        {
            GameManager.Instance.statGame = GameState.EPISODE;
            GameManager.Instance.soundMgr.ChangeBGMAudioSource(bgm_episode);
        }
        director.Play();
    }

    public void StopTimeline()
    {
        MagicCubeInit();
        if (director.state == PlayState.Playing)
        {
            if (GameManager.Instance.statGame != GameState.SELECT)
            {
                GameManager.Instance.statGame = GameState.ARCUBE;
                GameManager.Instance.ChangeBGM(GameState.ARCUBE);
            }
            director.Stop();
        }
    }

    /// <summary>
    /// 12/4/2023-LYI
    /// 각 큐브 타임라인 끝났을 때의 처리
    /// 큐브의 형태는 미리 지정
    /// </summary>
    public void OnTimeLineEnd()
    {
        Debug.Log(gameObject.name + ": TimelineEnd() - type:" + typeCube.ToString());
        switch (typeCube)
        {
            case CubeType.NONE:
                director.Pause();
                break;
            case CubeType.TITLE:
                director.Pause();
                break;
            case CubeType.INTRO:
                director.Pause();
                break;
            case CubeType.WORD:
                //director.
                //director.Play();
                break;
            case CubeType.STORY:
                director.Pause();
                break;
            default:
                director.Pause();
                break;
        }
    }

}
