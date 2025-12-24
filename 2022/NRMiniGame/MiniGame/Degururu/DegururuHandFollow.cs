using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DegururuHandFollow : HandFollowBlock
{
    protected override IEnumerator FollowHand()
    {
        GameManager gameMgr = GameManager.Instance;

        if (isLeft)
            targetHand = gameMgr.handCtrlL.NRHandMove;
        else
            targetHand = gameMgr.handCtrlR.NRHandMove;

        while (true)
        {
            if (gameMgr.miniGameMgr.miniGameUIMgr.statMiniGameUI == MiniGameUIStat.GAME)
            {
                transform.position = targetHand.palmCenter.transform.position;
            }

            if (!targetHand.isTracking ||
                gameMgr.miniGameMgr.miniGameUIMgr.statMiniGameUI != MiniGameUIStat.GAME)
            {
                transform.position = Vector3.up * -5;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}
