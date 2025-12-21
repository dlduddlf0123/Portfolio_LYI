using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class CanyonInteraction : InteractionManager
{
    public AROcclusionManager occlusionMgr;
    public Transform bridge;
    public Transform endPoint;
    Vector3 respawnPoint;

    public PositionConstraint wallPositionConstraint;
    PositionConstraint camPosConstraint;
    ConstraintSource constraintSourceHeader;
    ConstraintSource constraintSourceBridge;

    Collider m_coll;
    public float fallingTime = 2f;

    float speed = 0.5f;

    bool isMove = false;
    public bool isOff = true;

    protected override void DoAwake()
    {
        occlusionMgr = gameMgr.arMainCamera.GetComponent<AROcclusionManager>();
        m_coll = GetComponent<Collider>();

        for (int i = 0; i < bridge.childCount; i++)
        {
            bridge.GetChild(i).GetComponent<InvisibleBridge>().bridgeNum = i;
        }

        respawnPoint = gameMgr.currentEpisode.currentStage.list_endPos[3].position;

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Header") &&
            gameMgr.statGame == GameStatus.INTERACTION &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 3)
        {
            StopAllCoroutines();

            collision.gameObject.transform.position = respawnPoint;
            collision.gameObject.GetComponent<Kanto>().StartBlink();

            gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_FAILURE);
            StartCoroutine(DialogWaitTime());

            isMove = false;
            gameMgr.currentEpisode.currentStage.arr_header[0].SetAnim(0);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameMgr.uiMgr.worldCanvas.StopTimer();
        }
    }
    public void StartMove()
    {
        if (!isMove &&
            gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(MoveToEnd());
            isMove = true;
        }
    }
    IEnumerator MoveToEnd()
    {
        Character header = gameMgr.currentEpisode.currentStage.arr_header[0];
        header.SetAnim(1); 
        while (header.transform.localPosition.x > endPoint.localPosition.x)
        {
            header.GetComponent<CharacterController>().SimpleMove(transform.forward *speed);
            yield return new WaitForSeconds(0.01f);
        }
        EndInteraction();
    }

    IEnumerator SmoothChange(ConstraintSource _default, ConstraintSource _change)
    {
        float _t = 0;
        while (_t < 1)
        {
            _t += 0.02f;

            _change.weight = _t;
            _default.weight -= 0.02f;

            camPosConstraint.SetSource(0, _default);
            camPosConstraint.SetSource(1, _change);
            wallPositionConstraint.SetSource(0, _default);
            wallPositionConstraint.SetSource(1, _change);
            yield return new WaitForSeconds(0.01f);
        }
        _change.weight = 1;
        _default.weight = 0;

        camPosConstraint.SetSource(0, _default);
        camPosConstraint.SetSource(1, _change);
        wallPositionConstraint.SetSource(0, _default);
        wallPositionConstraint.SetSource(1, _change);
    }

    public void SignalChangeCam()
    {
        camPosConstraint = gameMgr.planeGenerator.GetComponent<PositionConstraint>();

        constraintSourceHeader = wallPositionConstraint.GetSource(0);

        constraintSourceBridge = new ConstraintSource();
        constraintSourceBridge.sourceTransform = this.transform;
        constraintSourceBridge.weight = 0;

        camPosConstraint.AddSource(constraintSourceBridge);
        wallPositionConstraint.AddSource(constraintSourceBridge);

        StartCoroutine(SmoothChange(constraintSourceHeader, constraintSourceBridge));
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        // gameMgr.currentEpisode.currentStage.header.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX;

        bridge.gameObject.SetActive(true);

        for (int i = 0; i < bridge.childCount; i++)
        {
            list_guidePosition.Add(bridge.GetChild(i).position + Vector3.up * 0.5f);
        }

        if (gameMgr.isDebug)
        {
            SignalChangeCam();
        }

        //constraintSourceHeader = wallPositionConstraint.GetSource(0);

        //ConstraintSource _source = new ConstraintSource();
        //_source.sourceTransform = this.transform;
        //_source.weight = 1;

        //wallPositionConstraint.SetSource(0, _source);
        //gameMgr.planeGenerator.GetComponent<PositionConstraint>().SetSource(0, _source);

        //wallPositionConstraint.constraintActive = false;
        //gameMgr.planeGenerator.GetComponent<PositionConstraint>().constraintActive = false;


        gameMgr.handCtrl.ToggleHandOcclusion(false);

       // StartCoroutine(WallMove());

        PlayGuideParticle();
        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.FRONT);
    }

    public override void EndInteraction()
    {
        //wallPositionConstraint.constraintActive = true;
        //gameMgr.planeGenerator.GetComponent<PositionConstraint>().constraintActive = false;

        //wallPositionConstraint.SetSource(0, constraintSourceHeader);
        //gameMgr.planeGenerator.GetComponent<PositionConstraint>().SetSource(0, constraintSourceHeader);

        constraintSourceHeader.weight = 1;

        camPosConstraint.SetSource(0, constraintSourceHeader);
        wallPositionConstraint.SetSource(0, constraintSourceHeader);
        camPosConstraint.RemoveSource(1);
        wallPositionConstraint.RemoveSource(1);

        gameMgr.handCtrl.ToggleHandOcclusion(gameMgr.uiMgr.ui_setting.setting_toggle_handOcclusion.isOn);
        m_coll.enabled = false;
        gameMgr.uiMgr.worldCanvas.StopTimer();

        bridge.gameObject.SetActive(false);

        StopGuideParticle();
        StopAllCoroutines();
        gameMgr.currentEpisode.currentStage.arr_header[0].StopAllCoroutines();

        base.EndInteraction();

       // StartCoroutine(SmoothChange(constraintSourceBridge, constraintSourceHeader));

        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);

        StartCoroutine(gameMgr.LateFunc(() =>
        gameObject.SetActive(isOff),5f));
    }

    //IEnumerator WallMove()
    //{
    //    float t = 0;
    //    while (t <1)
    //    {
    //        t += Time.deltaTime;
    //        wallPositionConstraint.transform.position = Vector3.Lerp(wallPositionConstraint.transform.position, transform.position, t);
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //}
}

