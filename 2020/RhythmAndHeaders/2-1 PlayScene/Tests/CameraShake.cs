using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    PlayManager ingameMgr;
    public float ShakeAmount;
    float ShakeTime;
    public Transform initialPosition;

    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }
    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
    }
    // Update is called once per frame
    void Update()
    {
        if(ShakeTime>0)
        {
        }
    }
}
