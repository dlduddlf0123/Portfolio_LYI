using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Pulley : MonoBehaviour
{
    SoundManager soundMgr;
    public Interactable interact;
    public GameObject movingObject; //움직여지는 오브젝트
    public CircularDrive driveforce { get; set; }
    private Transform objectheight;

    public float keyheight; //핸들이 움직인 각도에 따라 얼만큼 움직일지 설정
    bool isAct = false;
    // Start is called before the first frame update
    private void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;
        objectheight = movingObject.GetComponent<Transform>();
        keyheight = 0.01f;
        driveforce = GetComponent<CircularDrive>();
    }

    // Update is called once per frame
    void Update()
    {
        objectheight.localPosition = new Vector3(0, driveforce.outAngle * keyheight, 0);
        if ((int)transform.localEulerAngles.y % 20 <= 1 &&
            !isAct &&
            interact.hoveringHand)
        {
           // soundMgr.PlaySfx(transform.position, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON));
            soundMgr.PlaySfx(movingObject.transform, soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON2));
            interact.hoveringHand.TriggerHapticPulse(1000);
            isAct = true;
        }
        else if ((int)transform.localEulerAngles.y % 20 >= 1)
        {
            isAct = false;
        }
    }
}
    

