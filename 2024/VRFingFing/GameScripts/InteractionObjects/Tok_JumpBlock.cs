using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok;
using VRTokTok.Character;
using VRTokTok.Interaction;
using MoreMountains.Feedbacks;

public class Tok_JumpBlock : Tok_Interact
{
    public Transform tr_end;

    public MMF_Player mmf_bounce;

    bool isJumping = false;

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Header"))
        {
            if (isJumping)
            {
                return;
            }

            isJumping = true;

            coll.gameObject.GetComponent<Tok_Movement>().CharacterJump(tr_end.position);


            OnJumpActive();
        }
    }


    /// <summary>
    /// 5/10/2024-LYI
    /// 작동 될 때 애니메이션 등 처리
    /// </summary>
    public void OnJumpActive()
    {
        //튕기는 효과
        //효과음
        //중복 점프 방지
        mmf_bounce.PlayFeedbacks();
        GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_INTERACTION_JUMP_BOUNCE);

        StartCoroutine(ResetWait());
    }

    IEnumerator ResetWait()
    {
        yield return new WaitForSeconds(0.2f);
        isJumping = false;
    }

}
