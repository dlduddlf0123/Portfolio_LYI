using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;

public class TestKanto : Character
{
    //public int loopTime = 10;
    //public float talkTime = 0.3f;
    //public bool isTalking = false;

    public bool isGrabbable = false;

    public bool isRun = false;

    protected override void DoAwake()
    {
        characterColor = new Color32(255, 179, 0, 255);
        // arr_texture[0] = gameMgr.b_sprite.LoadAsset<Texture>("Giraffe");
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            arr_feet[i].footSound = gameMgr.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_WALK02);
        }

        m_sharedMat = arr_skin[0].sharedMaterial;
        m_sharedMat.SetFloat("_ChangeHeight", 1);
        //m_sharedMat.mainTexture = arr_texture[0];

        // StartCoroutine(ChangeColor(true));
        StartCoroutine(MoveAround());
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //gameMgr.uiMgr.worldCanvas.StartTimer(transform.position + Vector3.up * 0.1f, 2f, () => Thankyou());
            if (gameMgr.statGame == GameStatus.CUTSCENE &&
                isTouched == false)
            {
                PlayTriggerAnimation(1);
                //isTouched = true;
                //m_animator.applyRootMotion = true;
                //gameMgr.currentEpisode.currentStage.m_director.Pause();
                //gameMgr.currentEpisode.currentStage.currentDialog--;
                //Failure("만지지마!");

                //StartCoroutine(Touch());
            }
        }
    }

    IEnumerator MoveAround()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2, 6));
            if (!isRun)
            {

                Vector3 RandVec = new Vector3(Random.Range(-8 * gameMgr.uiMgr.stageSize, 8 * gameMgr.uiMgr.stageSize), 0, Random.Range(-6 * gameMgr.uiMgr.stageSize, 6 * gameMgr.uiMgr.stageSize));
                WalkToPoint(transform.parent.position + RandVec);

            }
            yield return new WaitForSeconds(5f);

        }
    }

    public void WalkToPoint(Vector3 _pos)
    {
        SetAnim(AnimationState.WALK);
        MoveCharacter(_pos, 2 * gameMgr.uiMgr.stageSize, () =>
        {
            SetAnim(AnimationState.IDLE);
        });
    }

    public void RunToPoint(Vector3 _pos)
    {
        isRun = true;
        SetAnim(AnimationState.RUN);
        MoveCharacter(_pos, 10 * gameMgr.uiMgr.stageSize, () =>
        {
            SetAnim(AnimationState.IDLE);
            isRun = false;
        });
    }
}
