using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MiniGameType
{
    NONE = -1,
    DEGURURU = 0,
    BOUNCE,
    POPCORN,
    RUN,
    SKI,
    SLAP,
}

public enum MiniGameMode
{
    EASY,
    NORMAL,
    HARD,
    EXTREME
}

/// <summary>
/// 게임 시작 시 미니게임 관리
/// 미니게임 프리팹 가지고 생성, 제거 관리
/// 게임 시작과 끝 처리 받기
/// 게임 모드에 따라 다른 형태로 미니게임 생성
/// 현재 캐릭터 정보 관리
/// </summary>
public class MiniGameManager : MonoBehaviour
{
    GameManager gameMgr;

    public MiniGameUIManager miniGameUIMgr;

    public MiniGame[] arr_miniGame;
    public MiniGame currentMiniGame;

    public List<ParticleSystem> list_guideParticle = new List<ParticleSystem>();
    public Transform particlePool;

    public int currentMinigameNum = 0;

    public Vector3 miniGamePos;

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        particlePool = transform.GetChild(transform.childCount - 1);
    }

    /// <summary> 21-12-30
    /// 점수 저장하기
    /// 현재 진행중인 미니게임의 게임타입을 텍스트로 저장
    /// 최고 점수 저장
    /// </summary>
    public void SaveScore()
    {
        //PlayerPrefs.SetInt(currentMiniGame.typeMiniGame.ToString(), currentMiniGame.gameScore);
        PlayerPrefs.SetInt(currentMiniGame.typeMiniGame.ToString(), currentMiniGame.highScore);
    }

    /// <summary> 21-12-30
    /// 최고 점수 불러오기
    /// 미니게임 타입으로 받아오기
    /// 혹은 ReadOnly 문서에 정의한 뒤 안전하게 불러오기
    /// </summary>
    public int LoadScore()
    {
        Debug.Log("LoadScore(): " + currentMiniGame.typeMiniGame.ToString() + "score load");
        return PlayerPrefs.GetInt(currentMiniGame.typeMiniGame.ToString(), 0);

        Debug.LogError("LoadScore(): Game Type Error!!");
        return 0;
    }




    /// <summary>
    /// 미니게임 생성 관리
    /// </summary>
    public void CreateMiniGame(int miniGameNum)
    {
        if (currentMiniGame != null)
        {
            Debug.LogError("Episode Prefab Already Exist!");
            return;
        }

        GameObject miniGame = Instantiate(arr_miniGame[miniGameNum].gameObject);
        

        //미니게임 위치 변경
        miniGame.transform.position = miniGamePos;

        //_miniGame.transform.position = gameMgr.planeGenerator.placedPos;
        //_miniGame.transform.rotation = gameMgr.planeGenerator.placedRot;
        //_miniGame.transform.localScale = Vector3.one * stageSize;

        //생성 뒤 현재 미니게임 갱신
        currentMiniGame = miniGame.GetComponent<MiniGame>();

        gameMgr.soundMgr.ChangeBGMAudioSource(currentMiniGame.m_audioSource,false);

        miniGameUIMgr.UIInit(currentMiniGame);
    }

    /// <summary>
    /// 미니게임 제거 관리
    /// </summary>
    public void DestroyMiniGame()
    {
        Destroy(currentMiniGame.gameObject);
        currentMiniGame = null;

        miniGameUIMgr.UIDisable();
    }

}
