using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageManager : PlayManager
{
    public MiniGameManager[] arr_miniGame;
    public MiniGameType currentMiniGame = MiniGameType.NONE;

    public StageUI stageUI;
    public StatusUI statUI;

    public Character[] headers;
    public Character interactHeader;

    public Transform[] arr_headersMovePoints;
    public Transform headersTransform;

    public Transform interactionTransform;

    public GameObject playerObj;
    public Camera mainCam;
    public PlayerHand[] hand;
    public GameObject handInteractObject;


    protected override void DoAwake()
    {
        headers = headersTransform.GetComponentsInChildren<Character>();

        ChangeSelectCharacter(headers[0]);

        gameMgr.hand = hand;
        gameMgr.mainCam = mainCam;
        gameMgr.statGame = GameState.INTERACTION;
    }


    public void ChangeSelectCharacter(Character _header)
    {
        interactHeader = _header;
        //selectPointer.transform.position = selectHeader.transform.position + Vector3.up * 1.3f;
        //selectPointer.transform.SetParent(selectHeader.transform);
        //selectPointer.SetActive(true);
    }

    /// <summary>
    /// OVR카메라 이동, 플레이어 위치 이동
    /// </summary>
    /// <param name="_moveTr"></param>
    public void MovePlayer(Transform _moveTr)
    {
        StartCoroutine(FadeMove(_moveTr));
    }

    IEnumerator FadeMove(Transform _moveTr)
    {
        gameMgr.StartCoroutine(gameMgr.Fading());
        yield return new WaitForSeconds(0.5f);

        playerObj.transform.position = new Vector3(_moveTr.position.x, playerObj.transform.position.y, _moveTr.position.z);
        playerObj.transform.rotation = _moveTr.rotation;
       // handInteractObject.transform.position = playerObj.transform.position;
    }

    /// <summary>
    /// 칸토 활성화
    /// </summary>
    public override void PlayStart()
    {
        base.PlayStart();
        gameMgr.statGame = GameState.INTERACTION;
        interactHeader.gameObject.SetActive(true);

    }

    /// <summary>
    /// 칸토 비활성화
    /// </summary>
    public override void PlayEnd()
    {
        base.PlayEnd();
        interactHeader.gameObject.SetActive(false);

    }


}
