using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornHandFire : HandFollowBlock
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
                transform.position = Vector3.Lerp(transform.position, targetHand.finger_index.transform.position, moveSpeed * Time.deltaTime);
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