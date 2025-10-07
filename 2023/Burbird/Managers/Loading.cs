using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Burbird
{
    public class Loading : MonoBehaviour
    {
        GameManager gameMgr;

        public Fade fade;

        private void Awake()
        {
            gameMgr = GameManager.Instance;

        }


        #region SceneMoveFunc

        public void LoadScene(int sceneNum, UnityAction action = null)
        {
            if (gameMgr.statGame == SceneStatus.GAME_LOADING)
            {
                return;
            }


            fade.StartFade(() =>
            {
                gameMgr.statGame = SceneStatus.GAME_LOADING;
                SceneManager.LoadScene((int)SceneStatus.GAME_LOADING);
                StartCoroutine(ChangeScene(sceneNum, action));
            });
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

        #endregion
    }
}