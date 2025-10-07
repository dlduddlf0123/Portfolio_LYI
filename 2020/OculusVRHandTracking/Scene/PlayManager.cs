using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// parent, 각 모드에 입장할 때 이 코드를 통해서 이동한다.
/// </summary>
public class PlayManager : MonoBehaviour
{
    protected GameManager gameMgr;

    public AudioSource mAudio;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        mAudio = GetComponent<AudioSource>();

        gameMgr.currentPlay = this; //각 스테이지 이동 시 스테이지매니저 할당

        DoAwake();
    }

    protected virtual void DoAwake() { }

    /// <summary>
    /// 씬 불러오기, 초기화 수행
    /// </summary>
    public virtual void PlayStart()
    {
        gameMgr.currentPlay = this; //각 미니게임 시작 시 할당
        mAudio.Play();
    }

    /// <summary>
    /// 씬 마무리, 상호작용 모드나 메인 메뉴로 나갈 때 호출
    /// </summary>
    public virtual void PlayEnd()
    {
        mAudio.Stop();

    }

}
