using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using VRTokTok.Interaction;

namespace VRTokTok.Interaction.Crossing
{

    /// <summary>
    /// 8/4/2023-LYI
    /// 길건너기 게임 관리 클래스
    /// 길건너기에 필요한 요소들을 관리하고 작동 관리
    /// </summary>
    public class Tok_Crossing : Tok_Interact
    {
        GameManager gameMgr;
        StageManager stageMgr;

        public Crossing_Lane[] arr_lane;

        public bool isPrewarn = true;


        private void Awake()
        {
            gameMgr = GameManager.Instance;
            stageMgr = gameMgr.playMgr.currentStage;
        }


        public override void InteractInit()
        {
            base.InteractInit();

            for (int i = 0; i < arr_lane.Length; i++)
            {
                arr_lane[i].LaneInit();
            }
        }


        /// <summary>
        /// 8/7/2023-LYI
        /// 각 레인에서 플랫폼 생성 시작
        /// </summary>
        /// <returns></returns>
        public void StartCrossing()
        {
            for (int i = 0; i < arr_lane.Length; i++)
            {
                if (arr_lane[i].gameObject.activeSelf)
                {
                    arr_lane[i].crossing = this;
                    arr_lane[i].StartSpawn();
                }
            }
        }



        public override void ActiveInteraction()
        {
            base.ActiveInteraction();

            StartCrossing();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }


    }
}