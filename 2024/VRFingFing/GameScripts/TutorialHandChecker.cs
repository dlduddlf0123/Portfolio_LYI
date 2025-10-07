using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using VRTokTok.Interaction;

namespace VRTokTok {
    public class TutorialHandChecker : Tok_Interact
    {
        GameManager gameMgr;
        FingerFollower fingerFollwer;

        public TutorialLeftHandle tutorialHandle;

        public GameObject handModel;
        public Image img_progress;
        public ParticleSystem efx_endHand;
        public ParticleSystem efx_endTutorial;

        public List<HandMarker> list_handMarker = new();
        public HandMarker handMarkerEnd;


        float progress = 0f;
        float clearTime = 2f;
        bool isDone = false;

        public override void InteractInit()
        {
            base.InteractInit();
            isInit = true;
            gameMgr = GameManager.Instance;
            fingerFollwer = gameMgr.playMgr.tokMgr.fingerFollower;


            tutorialHandle.gameObject.SetActive(true);
            tutorialHandle.StartLeftHandleTutorial();

            tutorialHandle.onHandleEnd += () => handModel.gameObject.SetActive(true);
            handModel.gameObject.SetActive(false);


            for (int i = 0; i < list_handMarker.Count; i++)
            {
                list_handMarker[i].gameObject.SetActive(false);
            }
            handMarkerEnd.gameObject.SetActive(false);

            img_progress.gameObject.SetActive(true);
            img_progress.fillAmount = 0f;

            isDone = false;
            progress = 0f;
        }


        // Update is called once per frame
        void Update()
        {
            if (!isInit)
            {
                return;
            }
            if (tutorialHandle.isTutorial)
            {
                return;
            }

            if (isDone == false)
            {
                if (gameMgr.playMgr.statPlay != Manager.PlayStatus.PLAY)
                {
                    return;
                }

                if (fingerFollwer.gameObject.activeSelf)
                {
                    progress += Time.deltaTime;
                    img_progress.fillAmount = progress / clearTime;
                    if (progress >= clearTime)
                    {
                        isDone = true;
                        EndHandTutorial();
                    }
                }
                else
                {
                    progress = 0f;
                    img_progress.fillAmount = progress / clearTime;
                }
            }
        }


        public void EndHandTutorial()
        {
            if (!isDone)
            {
                return;
            }

            handModel.gameObject.SetActive(false);
            img_progress.gameObject.SetActive(false);

            efx_endTutorial.transform.position = fingerFollwer.transform.position;

            efx_endHand.Play();
            efx_endTutorial.Play();
            gameMgr.soundMgr.PlaySfx(efx_endTutorial.transform.position, Constants.Sound.SFX_STAGE_CLEAR);

            for (int i = 0; i < list_handMarker.Count; i++)
            {
                list_handMarker[i].InteractInit();
            }
        }


        public void CheckFinish()
        {
            int count = 0;
            for (int i = 0; i < list_handMarker.Count; i++)
            {
                if (!list_handMarker[i].isActive)
                {
                    count++;
                }
            }
            if (count >= list_handMarker.Count-1)
            {
                EndTouchTutorial();
            }
        }

        public void EndTouchTutorial()
        {
            gameMgr.playMgr.currentStage.gateExit.ActiveInteraction();
            handMarkerEnd.gameObject.SetActive(true);
        }
    }
}