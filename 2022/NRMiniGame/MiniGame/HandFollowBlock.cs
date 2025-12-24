using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HandFollowBlock : MonoBehaviour
{
    protected NRHandMove  targetHand;
    protected  float moveSpeed = 8f;

    public bool isLeft = false;

    public ParticleSystem vfx_hand;
    public AudioClip sfx_hand;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(FollowHand());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 손이 오브젝트와 닿았을 때 이펙트
    /// 닿는 오브젝트에서 호출
    /// </summary>
    public void HandCollEffect()
    {

    }

    protected virtual IEnumerator FollowHand()
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