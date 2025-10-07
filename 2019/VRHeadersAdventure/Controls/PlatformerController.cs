using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Controller(Hand)
/// </summary>
public class PlatformerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Move");
    public SteamVR_Action_Boolean jumpAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Jump");
    public SteamVR_Action_Boolean runAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Run");

    public Character header;

    private Vector3 movement;
    private bool isJump;
    bool isRun;
    public  bool isPlatformer = false;
    private float glow;
    private SteamVR_Input_Sources hand;

    private void Awake()
    {
        hand = GetComponent<Hand>().handType;
    }
    private void Start()
    {
        header = GameManager.Instance.arr_headers[0];

    }

    private void FixedUpdate()
    {
        if (isPlatformer && GameManager.Instance.statGame == GameState.PLAY)
        {
            Vector2 m = moveAction.GetAxis(hand);
            if(isJump = jumpAction.GetStateDown(hand))
            { Debug.Log("Jump"); }
            if (isRun = runAction.GetState(hand))
            { Debug.Log("Run"); }

            movement = new Vector3(m.x, 0, m.y);
            
            glow = Mathf.Lerp(glow, jumpAction[hand].state ? 1.5f : 1.0f, Time.deltaTime * 20);
            
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
            header.headerCtrl.Move(movement,isRun);
        }
        else
        {
            movement = Vector2.zero;
            isJump = false;
            glow = 0;
        }
    }

    public void PlatformerActionSetOn(Character _header,Hand _hand)
    {
        _header.Stop();
        //actionSet.Activate(_hand.handType);
        hand = _hand.handType;
        GameManager.Instance.rightHand.statHand = HandState.PLATFORMER;
        isPlatformer = true;
        _header.isPlatfomer = true;
        _header.AI_Move(3);
    }

    public void PlatformerActionSetOff(Character _header, Hand _hand)
    {
        _header.Stop();
       // actionSet.Deactivate(_hand.handType);
        GameManager.Instance.rightHand.statHand = HandState.ORDER;
        isPlatformer = false;
        _header.isPlatfomer = false;
        _header.AI_Move(3);
    }

}
