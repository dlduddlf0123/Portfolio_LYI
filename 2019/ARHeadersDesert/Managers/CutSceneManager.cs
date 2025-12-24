using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.ReadOnly;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutSceneManager : MonoBehaviour
{
    public GameManager gameMgr;

    public PlayableDirector mDirector;
    public List<TimelineAsset> list_cutScene;

    public int cutSceneCount;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        mDirector = this.GetComponent<PlayableDirector>();

        cutSceneCount = 0;
    }

    /// <summary>
    /// 컷씬용 캐릭터 셋팅
    /// </summary>
    public void SetCutSceneHeader()
    {
        foreach (var header in gameMgr.list_Headers)
        {
            header.StopAllCoroutines();
            header.gameObject.SetActive(false);
            header.SetBodyColor(1.0f);
        }

    }
    /// <summary>
    /// 컷씬 시작
    /// </summary>
    /// <param name="_sceneNum">몇번 째 컷씬</param>
    public void PlayCutScene(int _sceneNum)
    {
        gameMgr.statGame = GameState.DIALOG;
        Debug.Log("GameState: " + gameMgr.statGame);
        SetCutSceneHeader();
        mDirector.playableAsset = list_cutScene[_sceneNum - 1];
        mDirector.Play();
        cutSceneCount = 0;
    }

    //컷씬 종료 시 호출
    public void EndCutScene()
    {
        gameMgr.uiMgr.ShowGoal();
        gameMgr.cutSceneMgr.mDirector.Stop();
        gameMgr.cutSceneMgr.cutSceneCount = 0;

        foreach (var header in gameMgr.list_Headers)
        {
            if (header.headerCanvas.gameObject.activeSelf == false)
            {
                header.headerCanvas.gameObject.SetActive(true);
                header.headerCanvas.SetDialogLanguage();
            }
        }
    }
}




