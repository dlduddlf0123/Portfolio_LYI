using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    /// <summary>
    /// 8/22/2024-LYI
    /// Life contents UI element management
    /// 
    /// </summary>
    public class LifeUIManager : MonoBehaviour
    {
        GameManager gameMgr;

        public UI_HandDebug ui_handDebug;

        public UI_OnHand ui_handLeft;
        public UI_OnHand ui_handRight;


        bool isInit = false;

        public void UIInit()
        {
            if (!isInit)
            {
                gameMgr = GameManager.Instance;

                isInit = true;
            }
        }



        public void StatUIRefresh()
        {
            ui_handLeft.SliderRefresh();
            ui_handRight.SliderRefresh();
        }

        public void StatUILevelUp(StatusLevel level, float remainGauge, float statMax)
        {
            ui_handLeft.SliderLevelUp(level, remainGauge, statMax);
            ui_handRight.SliderLevelUp(level, remainGauge, statMax);
        }
        public void StatUILevelDown(StatusLevel level, float remainGauge, float statMax)
        {
            ui_handLeft.SliderLevelDown(level, remainGauge, statMax);
            ui_handRight.SliderLevelDown(level, remainGauge, statMax);
        }

    }
}