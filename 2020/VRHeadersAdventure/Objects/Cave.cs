using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour
{
    public CaveExit exit1;
    public CaveExit exit2;

    public bool changeCam = false;

    [Tooltip("1번 출구로 나왔을 때 카메라")]
    public int exit1Cam;
    [Tooltip("2번 출구로 나왔을 때 카메라")]
    public int exit2Cam;
    private void Awake()
    {
        exit1 = transform.GetChild(0).GetComponent<CaveExit>();
        exit2 = transform.GetChild(1).GetComponent<CaveExit>();
    }
    private void Start()
    {
        exit1.otherExit = exit2;
        exit2.otherExit = exit1;
        exit1.changeCam = changeCam;
        exit2.changeCam = changeCam;
        exit1.camNum = exit2Cam;
        exit2.camNum = exit1Cam;
    }
}
