using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    /// <summary>
    /// 240805 LYI
    /// UI element management
    /// 
    /// </summary>
    public class RaceUIManager : MonoBehaviour
    {
        GameManager gameMgr;

        bool isInit = false;

        public void UIInit()
        {
            if (!isInit)
            {
                gameMgr = GameManager.Instance;

                isInit = true;
            }
        }

    }
}