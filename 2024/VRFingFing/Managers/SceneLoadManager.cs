using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// 7/13/2023-LYI
/// 씬 전환 시 처리
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    GameManager gameMgr;
    Fade fade;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
       // fade = gameMgr.fade;
    }


    #region SceneMoveFunc
    public void LoadScene(GameStatus scene, UnityAction action = null)
    {
        if (gameMgr.statGame == GameStatus.LOADING)
        {
            return;
        }

        fade.StartFadeMiddle(() =>
        {
            gameMgr.ChangeGameStat(GameStatus.LOADING);

            SceneManager.LoadScene((int)GameStatus.LOADING);
            StartCoroutine(ChangeScene((int)scene, action));
        }, 5, 1);
    }
    public void LoadScene(int sceneNum, UnityAction action = null)
    {
        if (gameMgr.statGame == GameStatus.LOADING)
        {
            return;
        }

        fade.StartFadeMiddle(() =>
        {
            gameMgr.ChangeGameStat(GameStatus.LOADING);
            SceneManager.LoadScene((int)GameStatus.LOADING);
            StartCoroutine(ChangeScene(sceneNum, action));
        }, 5, 1);
    }

    /// <summary>
    /// 7/13/2023-LYI 
    /// 장면 갯수가 많아 string 형식 오버로딩 추가
    /// </summary>
    /// <param name="sceneName">장면 이름</param>
    /// <param name="action">로딩 후 작동시킬 함수</param>
    public void LoadScene(string sceneName, UnityAction action = null)
    {
        if (gameMgr.statGame == GameStatus.LOADING)
        {
            return;
        }

        fade.StartFadeMiddle(() =>
        {
            gameMgr.ChangeGameStat(GameStatus.LOADING);
            SceneManager.LoadScene("Loading");
            StartCoroutine(ChangeScene(sceneName, action));
        }, 5, 1);
    }


    //Scene 전환시 호출, 비동기 로딩 후 로딩이 끝나면 전환
    public IEnumerator ChangeScene(int sceneNum, UnityAction action = null)
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneNum);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            yield return null;
            if (async.progress < 0.9f)
            {
                Debug.Log("Loading:" + async.progress * 100 + "%");
            }
            else if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);
                async.allowSceneActivation = true;
                Debug.Log("Scene Activated");

            }
        }

        if (action != null)
        {
            action.Invoke();
        }
    }
    public IEnumerator ChangeScene(string sceneName, UnityAction action = null)
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            yield return null;
            if (async.progress < 0.9f)
            {
                Debug.Log("Loading:" + async.progress * 100 + "%");
            }
            else if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.1f);
                async.allowSceneActivation = true;
                Debug.Log("Scene Activated");

            }
        }

        if (action != null)
        {
            action.Invoke();
        }
    }



    #endregion
}