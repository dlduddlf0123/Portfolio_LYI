using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Lever : MonoBehaviour
{
    SoundManager soundMgr;
    public Interactable handcheck;
    public LinearMapping linearmapping;
    public Movable moveObj;
    
    public int interactioncheck;     //상호작용 오브젝트에 신호 전달
    public bool isAutoMove = true;
    public bool isAct = true;

    private bool handstate;

    private void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
    }

    // Start is called before the first frame update
    void Start()
    {
        linearmapping.value = 0;
        handstate = false;
        interactioncheck = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(90 * linearmapping.value - 45, 0, 0);

        if (handcheck.attachedToHand)
        {
            handstate = true;
        }
        else
        {
            handstate = false;
        }

        //Auto Move to 0
        if (linearmapping.value >= 0 &&
            linearmapping.value < 1 &&
            !handstate &&
            isAutoMove)
        {
            if (linearmapping.value > 0.01)
            {
                linearmapping.value = linearmapping.value - 0.005f;
            }
            if (linearmapping.value < 0.015)
            {
                linearmapping.value = 0;
            }
        }

        if (linearmapping.value == 1 &&
            !isAct)
        {
            interactioncheck = 1;
            if (moveObj != null)
            {
                moveObj.Plus();
                moveObj.Active(true);
                soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON),Random.Range(1-0.2f,1+0.2f));
                if (handcheck.attachedToHand)
                     handcheck.attachedToHand.TriggerHapticPulse(1000);
                Debug.Log("LeverUp");
                isAct = true;
            }
        }
        if (linearmapping.value == 0 &&
            !isAct)
        {
            interactioncheck = 0;

            if (moveObj != null)
            {
                moveObj.Minus();
                moveObj.Active(false);
                soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON2), Random.Range(1 - 0.2f, 1 + 0.2f));
                if (handcheck.attachedToHand)
                    handcheck.attachedToHand.TriggerHapticPulse(1000);
                Debug.Log("LeverDown");
                isAct = true;
            }
        }

        if(linearmapping.value < 1 &&
           linearmapping.value > 0 )
        {
            isAct = false;
        }
    }
}
