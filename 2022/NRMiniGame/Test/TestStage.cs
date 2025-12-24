using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStage : StageManager
{
    public override void StartStage()
    {
        gameMgr.LateFrameFunc(() =>
        AstarScan(Vector3.zero,Vector3.up));
    }
}
