using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OculusSampleFramework;

public enum MotionType
{
    NONE,
    CALL,
    ROLL,
}

/// <summary>
/// 손가락 끝에 닿는 모션 체크 
/// </summary>
public class HandMotion : MonoBehaviour
{
    GameManager gameMgr;
    HandInteract hand;
    OVRSkeleton skeleton;
    
    public bool isLeft = false;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLeft)
        {
            hand = gameMgr.hand[0].GetComponent<HandInteract>();
            skeleton = hand.skeleton;
        }
        else
        {
            hand = gameMgr.hand[1].GetComponent<HandInteract>();
            skeleton = hand.skeleton;
        }
        hand.handMotion = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (hand.hand.IsTracked)
        {
            GetComponent<Rigidbody>().MovePosition(skeleton.Bones[8].Transform.position);
            GetComponent<Rigidbody>().MoveRotation(skeleton.Bones[8].Transform.rotation);
            //Debug.Log("Velocity: " + GetComponent<Rigidbody>().velocity.sqrMagnitude);

            hand.isHit = (GetComponent<Rigidbody>().velocity.sqrMagnitude > 5.0f) ? true : false;
        }
    }

  
}
