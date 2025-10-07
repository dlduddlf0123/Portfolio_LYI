using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadOnly;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public GameManager gameMgr;

    public PlayableDirector mDirector;
    public List<TimelineAsset> list_cutScene;

    public Button btn_skip;
    
    private void Awake()
    {
        gameMgr = GameManager.Instance;
        mDirector = this.GetComponent<PlayableDirector>();
        //btn_skip.onClick.AddListener(EndCutScene);
       // btn_skip.gameObject.SetActive(false);

    }

    /// <summary>
    /// 컷씬용 캐릭터 셋팅
    /// </summary>
    public void SetCutSceneHeader()
    {
        foreach (var header in gameMgr.arr_headers)
        {
            header.StopAllCoroutines();
            header.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 컷씬 시작
    /// </summary>
    /// <param name="_sceneNum">몇번 째 컷씬</param>
    public void PlayCutScene(int _sceneNum, Transform _parent = null)
    {
        if (list_cutScene.Count <= _sceneNum)
        {
            Debug.Log("!!Dialog is Null!!");
            //gameMgr.uiMgr.ShowGoal();
            return;
        }
        gameMgr.statGame = GameState.CUTSCENE;
        Debug.Log("GameState: " + gameMgr.statGame);
      //  SetCutSceneHeader();
        mDirector.playableAsset = list_cutScene[_sceneNum];
        //btn_skip.gameObject.SetActive(true);

        if (_parent != null)
        {
            gameMgr.player.transform.SetParent(_parent);
        }
        
        mDirector.Play();
    }

    //컷씬 종료 시 호출
    public void EndCutScene()
    {
        mDirector.Stop();
        gameMgr.player.transform.parent = null;
        //btn_skip.gameObject.SetActive(false);

        //foreach (var header in gameMgr.arr_headers)
        //{
        //    if (header.headerCanvas == null)
        //    {
        //        continue;
        //    }
        //    else if (header.headerCanvas.gameObject.activeSelf == false)
        //    {
        //        header.headerCanvas.gameObject.SetActive(true);
        //        header.headerCanvas.SetDialogLanguage();
        //    }
        //}

       // gameMgr.uiMgr.ShowGoal();
    }
}




