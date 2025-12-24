using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceHandFollow : HandFollowBlock
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
                transform.position = Vector3.Lerp(transform.position, targetHand.palmCenter.transform.position, moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetHand.palmCenter.transform.rotation, moveSpeed * Time.deltaTime);
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
