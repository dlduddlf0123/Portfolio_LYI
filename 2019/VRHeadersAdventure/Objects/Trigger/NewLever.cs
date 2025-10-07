using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class NewLever : MonoBehaviour
{
    SoundManager soundMgr;
    public LinearMapping linearmapping;
    public Interactable handcheck;
    public Movable movable;

    public int floorState = 0; // -1이면 내려가고 +1이면 올라감
    public float speed = 0.005f;
    bool isEnd = false;
    private bool handstate;

    void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
        GetComponent<LinearDrive>().repositionGameObject = false;

        linearmapping.value = 0.5f;
        handstate = false;
    }
    private void Start()
    {

        GetComponent<LinearDrive>().repositionGameObject = true;
    }


    void Update()
    {
        if (handcheck.attachedToHand &&
            floorState == 0)
        {
            handstate = true;
        }
        else
        {
            handstate = false;

            if (linearmapping.value < 0.5)
            {
                linearmapping.value += speed * 0.005f;
            }
            if (linearmapping.value > 0.5)
            {
                linearmapping.value -= speed * 0.005f;
            }
        }


        if (linearmapping.value < 0.55f &&
            linearmapping.value > 0.45)
        {
            linearmapping.value = 0.5f;
            floorState = 0;
        }

        if (linearmapping.value >= 0.98 &&
            floorState == 0)
        {
            floorState = 1;
            if (movable != null)
            {
                movable.Plus();
                movable.Active(true);
                soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON), Random.Range(1 - 0.2f, 1 + 0.2f));
                if (handcheck.attachedToHand)
                    handcheck.attachedToHand.TriggerHapticPulse(1000);
            }
            Debug.Log("Floor UP");
            handstate = false;
        }
        else if (linearmapping.value <= 0.02 &&
            floorState == 0)
        {
            floorState = -1;
            if (movable != null)
            {
                movable.Minus();
                movable.Active(false);
                soundMgr.PlaySfx(transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON2), Random.Range(1 - 0.2f, 1 + 0.2f));
                if (handcheck.attachedToHand)
                    handcheck.attachedToHand.TriggerHapticPulse(1000);
            }
            Debug.Log("Floor Down");
            handstate = false;
        }
    }
}
