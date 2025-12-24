using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 플레이 하는 캐릭터
/// 물체와 충돌했을 때 반응
/// 이동, 점프 입력과 연계되는 애니메이션 재생
/// </summary>
public class RunCharacter : MonoBehaviour
{
    protected NRHandMove targetHand;
    protected float moveSpeed = 8f;

    Animator m_anim;

    bool isLeft = false;
    bool isGround = true;
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
    }
    void OnEnable()
    {
        StartCoroutine(FollowHand());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }


    public void SetAnim(int animNum)
    {
        switch(animNum)
        {
            case 0:
                
                break;
        }    
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
