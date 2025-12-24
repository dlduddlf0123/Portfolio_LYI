using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatform : Movable
{
    public Transform playerTr;
    Vector3 startPos;
    public int currentNum = 0;

    private void Awake()
    {
        startPos = this.transform.position;
       // GameManager.Instance.player.transform.SetParent(transform);
    }
    
    public override void Plus()
    {
        StageManager stageMgr = GameManager.Instance.stageMgr;
        if (currentNum < stageMgr.arr_cameraPoints.Length-1)
        {
            currentNum++;
            stageMgr.StartCoroutine(stageMgr.ChangeCameraPosition(currentNum));
            this.transform.position = stageMgr.arr_cameraPoints[currentNum].position;
        }
    }

    public override void Minus()
    {
        StageManager stageMgr = GameManager.Instance.stageMgr;
        if (currentNum > 0)
        {
            currentNum--;
            stageMgr.StartCoroutine(stageMgr.ChangeCameraPosition(currentNum));
            this.transform.position = stageMgr.arr_cameraPoints[currentNum].position;
        }
    }
}
