using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;
using System.Threading;
using Valve.VR;
using Valve.VR.InteractionSystem;

//캐릭터에게 명령을 전달
//특수동작(물총 등)의 기능
public enum HandState
{
    ORDER=0,
    PLATFORMER,
};


public class MyRightHand : MonoBehaviour
{
    SteamVR_Behaviour_Pose controllerPose;

    //pinch trigger
    SteamVR_Action_Boolean fireAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Fire");

    //touch pad(4-direction)
    SteamVR_Action_Boolean laserAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Laser");
    SteamVR_Action_Boolean callAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Call");
    SteamVR_Action_Boolean changeRightAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ChangeRight");
    SteamVR_Action_Boolean changeLeftAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ChangeLeft");
    SteamVR_Action_Boolean changeControlAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ChangeControl");
    SteamVR_Action_Boolean jumpAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Jump");
    SteamVR_Action_Boolean specialAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Special");

    public Hand hand;
    HandPhysics handPhysics;

    public SteamVR_Input_Sources myHandType;
    public HandState statHand;

    public GameManager gameMgr;
    public Character header;

    public GameObject laser;

    Vector3 hitPoint;
    GameObject hitObj;
    float changeCoolTime = 0.0f;
    float specialCoolTime = 0.5f;

    private Vector3 jumpPos;
    private bool isJump;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        hand = GetComponent<Hand>();
        handPhysics = GetComponent<HandPhysics>();

        myHandType = hand.handType;
        statHand = HandState.ORDER;

        controllerPose = GetComponent<SteamVR_Behaviour_Pose>();
        header = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        handPhysics.handCollider.gameObject.tag = "Player";

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameMgr.statGame == GameState.PLAY)
        {
            if (changeCoolTime < 1f)
            {
                changeCoolTime += Time.deltaTime;
            }
           // ChangeControl();
           // DpadNorthAction();
            DpadSouthAction();
            if (statHand == HandState.PLATFORMER)
            {
                JumpAction();
                SpecialAction();
            }
            // DpadWestAction();
        }
    }
    
    #region Dpad Control
    //대가리 선택하기
    //누를 시 레이져 발사, 떼면 이동
    public void DpadNorthAction()
    {
        //레이져 발사
        if (laserAction.GetState(myHandType))
        {
            RaycastHit hit;
            bool rayHit = false;
            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                hitObj = hit.transform.gameObject;
                gameMgr.stageMgr.SetActiveCheckPoints(false);
                rayHit = true;
            }
            ShowLaser(hit, rayHit);
        }
        else
        {
            laser.SetActive(false);
        }

        if (laserAction.GetStateUp(myHandType) && hitObj != null)
        {
            if (hitObj != null)
            {
                Debug.Log("HitObject = " + hitObj.name);
                gameMgr.PlayEffect(gameMgr.effectTouch, hitPoint);
                CheckLaserTarget();
                hand.TriggerHapticPulse(500);
            }
            gameMgr.stageMgr.SetActiveCheckPoints(true);
        }
    }

    //누르면 부르기
    public void DpadSouthAction()
    {
        if (header == null) { return; }

        if (callAction.GetState(SteamVR_Input_Sources.RightHand))
        {
            header.MoveCharacter(this.transform.position, this.gameObject);
        }
    }


    public void JumpAction()
    {
        if (jumpAction.GetStateDown(myHandType) && header.headerCtrl.isGround)
        {
            jumpPos = header.transform.position;
            header.headerCtrl.Jump();
            isJump = true;
        }
        if (jumpAction.GetState(myHandType) && isJump)
        {
            if (header.transform.position.y < jumpPos.y + 1.5f)
            {
                header.headerCtrl.Jumping();
            }
            else
            {
                isJump = false;
            }
        }
        if (jumpAction.GetStateUp(myHandType))
        {
            isJump = false;
        }
    }

    public void DpadWestAction()
    {
        if (changeLeftAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            if (header != null)
            {
                header.SetSelect(false);
            }
            if (statHand == HandState.ORDER)
            {
                statHand = HandState.PLATFORMER;
            }
            else
            {
                statHand -= 1;
            }
            Debug.Log("HandState =" + statHand);
        }
    }
    #endregion

    /// <summary>
    /// 방아쇠 버튼
    /// </summary>
    public void SpecialAction()
    {
        if (header == null) { return; }

        if (specialCoolTime < header.Status.specialCoolTime)
        {
            specialCoolTime += Time.deltaTime;
        }

        if (specialAction[myHandType].state)
        {
            switch (statHand)
            {
                case HandState.ORDER:
                    gameMgr.missileMgr.Fire();
                    hand.TriggerHapticPulse(300);
                    break;

                case HandState.PLATFORMER:
                    if (header == null) { break; }
                    if (specialCoolTime >= header.Status.specialCoolTime)
                    {
                        header.SpecialMove();
                        specialCoolTime = 0.0f;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 오른손 컨트롤 변경(Update)
    /// </summary>
    public void ChangeControl()
    {
        if (changeControlAction.GetStateDown(myHandType) &&
            header != null &&
             changeCoolTime > 0.5f &&
             header.headerCtrl.isGround)
        {
            PlatformerActionSetOff(header);
            header.SetSelect(false);
            header = null;
            gameMgr.leftHand.header = header;
            changeCoolTime = 0.0f;
        }
    }

    public void SetRightHandControl(Character _header)
    {
        if (statHand == HandState.PLATFORMER)
        {
            statHand = HandState.ORDER;
        }
        else
        {
            statHand += 1;
        }
        Debug.Log("HandState =" + statHand);

        switch (statHand)
        {
            case HandState.ORDER:
                PlatformerActionSetOff(_header);
                break;
            case HandState.PLATFORMER:
                PlatformerActionSetOn(_header);
                break;
        }

    }

    /// <summary>
    /// 레이저가 부딪힌 사물 체크
    /// 캐릭터, 아이템, 상자, 벽 등
    /// </summary>
    public void CheckLaserTarget()
    {
        if (hitObj.CompareTag("Header"))
        {
            header = hitObj.GetComponent<Character>();
            gameMgr.leftHand.header = header;
            for (int i = 0; i < gameMgr.arr_headers.Length; i++)
            {
                gameMgr.arr_headers[i].SetSelect(false);
            }
            header.SetSelect(true);
            PlatformerActionSetOn(header);
        }
        else
        {
            if (header == null) { return; }
          //  selectHeader.MoveCharacter(hitPoint, hitObj);
        }
    }


    void ShowLaser(RaycastHit hit, bool _hit)
    {
        laser.SetActive(true);

        if (_hit)
        {
            laser.transform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, 0.5f);
            laser.transform.LookAt(hitPoint);
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, hit.distance);
        }
        else
        {
            laser.transform.position = this.transform.position;
            laser.transform.LookAt(transform.forward);
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, laser.transform.localScale.x);
        }
    }


    public void PlatformerActionSetOn(Character _header)
    {
        _header.Stop();
        statHand = HandState.PLATFORMER;
        gameMgr.leftHand.statHand = HandState.PLATFORMER;
        _header.isPlatfomer = true;
        _header.AI_Move(3);
    }

    public void PlatformerActionSetOff(Character _header)
    {
        _header.Stop();
        statHand = HandState.ORDER;
        gameMgr.leftHand.statHand = HandState.ORDER;
        _header.isPlatfomer = false;
        _header.AI_Move(3);
    }
}
