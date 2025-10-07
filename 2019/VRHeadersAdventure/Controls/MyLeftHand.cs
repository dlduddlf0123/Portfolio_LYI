using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;
using System.Threading;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MyLeftHand : MonoBehaviour
{
    public GameObject body;

    //SteamVR_Action_Boolean rotRight = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RotRight");
    //SteamVR_Action_Boolean rotLeft = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RotLeft");
    SteamVR_Action_Boolean menuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Menu");
    public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Move");
    public SteamVR_Action_Boolean runAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Run");

    public Hand hand;
    HandPhysics handPhysics;

    public SteamVR_Input_Sources myHandType;
    public HandState statHand;

    GameManager gameMgr;
    public Character header;

    private Vector3 movement;
    bool isRun;

    // Start is called before the first frame update
    void Awake()
    {
        gameMgr = GameManager.Instance;
        hand = GetComponent<Hand>();
        handPhysics = GetComponent<HandPhysics>();

        myHandType = hand.handType;
        statHand = HandState.ORDER;
    }
    private void Start()
    {
        handPhysics.handCollider.gameObject.tag = "Player";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MenuAction();

        if (statHand == HandState.PLATFORMER &&
            gameMgr.statGame == GameState.PLAY &&
            header != null)
        {
            Vector2 m = moveAction.GetAxis(myHandType);
            isRun = runAction.GetState(myHandType);

            movement = new Vector3(m.x, 0, m.y);
            
            float rot = GameManager.Instance.mainCam.transform.eulerAngles.y;

            movement = Quaternion.AngleAxis(rot, Vector3.up) * movement;

            if (isRun)
            {
                header.mAnimator.SetBool("isMove", true);
            }
            else
            {
                header.mAnimator.SetBool("isMove", false);
            }
            header.headerCtrl.Move(movement, isRun);
        }
        else
        {
            movement = Vector2.zero;
        }
    }
    

    void MenuAction()
    {
        if (menuAction[myHandType].stateDown)
        {
            if (gameMgr.statGame == GameState.CLEAR)
            {
                gameMgr.stageMgr.StartCoroutine(gameMgr.stageMgr.ChangeCameraPosition(gameMgr.stageMgr.clearCameraPoint.position, 0.1f));
            }
            else
            {
                gameMgr.stageMgr.StartCoroutine(gameMgr.stageMgr.ChangeCameraPosition(gameMgr.stageMgr.playerPlatform.playerTr.position, 0.1f));
               // gameMgr.player.transform.localPosition +=  new Vector3(gameMgr.mainCam.transform.localPosition.x,0, gameMgr.mainCam.transform.localPosition.z);
            }
            //gameMgr.player.transform.position = gameMgr.stageMgr.arr_cameraPoints[gameMgr.stageMgr.currentCamNum].position;
            //gameMgr.player.transform.rotation = gameMgr.stageMgr.arr_cameraPoints[gameMgr.stageMgr.currentCamNum].rotation;
        }
    }

    //void RotationButton()
    //{
    //    if (rotLeft.GetStateDown(SteamVR_Input_Sources.LeftHand))
    //    {
    //        body.transform.Rotate(Vector3.down * 45);
    //    }
    //    if (rotRight.GetStateDown(SteamVR_Input_Sources.LeftHand))
    //    {
    //        body.transform.Rotate(Vector3.up* 45f);
    //    }
    //}
}
