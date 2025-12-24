using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;
using System.Threading;
using Valve.VR;

public class MyLeftHand : MonoBehaviour
{
    public GameObject body;

    public SteamVR_Action_Boolean rotRight = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RotRight");
    public SteamVR_Action_Boolean rotLeft = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RotLeft");

    public SteamVR_Input_Sources myHandType;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RotationButton();
    }

    void RotationButton()
    {
        if (rotLeft.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            body.transform.Rotate(Vector3.down * 45);
        }
        if (rotRight.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            body.transform.Rotate(Vector3.up* 45f);
        }
    }
}
