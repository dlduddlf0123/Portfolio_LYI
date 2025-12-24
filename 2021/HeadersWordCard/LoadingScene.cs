using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LoadingScene : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    public void LoadScene(int _sceneNum)
    {
        SceneManager.LoadScene(1);
        
        StartCoroutine(LoadSceneAsync(_sceneNum));
    }

    public IEnumerator LoadSceneAsync(int _sceneNum, UnityAction _action = null)
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation _async =  SceneManager.LoadSceneAsync(_sceneNum);
        _async.allowSceneActivation = false;
        while(!_async.isDone)
        {
            yield return null;
            if (_async.progress < 0.9f)
            {
                Debug.Log("Loading:" +_async.progress * 100 + "%");
            }
            else if(_async.progress >= 0.9f)
            {
                if (_action != null)
                {
                    _action.Invoke();
                }
                _async.allowSceneActivation = true;
            }
        }
        Destroy(this.gameObject);
    }
}
