using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction
{
    /// <summary>
    /// 7/6/2023-LYI
    /// 출구 오브젝트
    /// 모든 헤더스가 들어오면 클리어
    /// </summary>
    public class Tok_Exit : Tok_Interact
    {
        GameManager gameMgr;
        OVRScreenFade fade;
        public int maxHeaderCount;
        public int currentHeaderCount;


        private void Awake()
        {
            //fade = GetComponent<OVRScreenFade>();
        }

        public override void InteractInit()
        {
            base.InteractInit();

            gameMgr = GameManager.Instance;
            maxHeaderCount = gameMgr.playMgr.currentStage.characterNum;
            currentHeaderCount = 0;
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                currentHeaderCount++;

                CheckClear();
                //GameManager.Instance.playMgr.uiMgr.fadeCanvas.StartFade();
                //fade.FadeIn();
            }
        }
        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                currentHeaderCount--;
                //fade.FadeOut();
            }
        }


        public void CheckClear()
        {
            if (currentHeaderCount >= maxHeaderCount)
            {
                ActiveInteraction();
            }
        }

        public override void ActiveInteraction()
        {
            base.ActiveInteraction();

            GameManager.Instance.playMgr.currentStage.ClearStage();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}