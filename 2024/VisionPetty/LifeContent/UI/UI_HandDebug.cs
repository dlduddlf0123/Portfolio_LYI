using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


namespace AroundEffect
{

    public class UI_HandDebug : MonoBehaviour
    {
        GameManager gameMgr;

        [SerializeField] Text[] arr_txt_leftFingerTipPosition;
        [SerializeField] Text[] arr_txt_leftFingerState;

        [SerializeField] Text[] arr_txt_rightFingerTipPosition;
        [SerializeField] Text[] arr_txt_rightFingerState;


        private void Awake()
        {
            HandDebugInit();

        }


        public void HandDebugInit()
        {
            gameMgr = GameManager.Instance;

        }


        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < 5; i++)
            {
                ChangeText(arr_txt_leftFingerTipPosition[i], gameMgr.MRMgr.polySpatialInput.arr_trJointL[i].localRotation.x.ToString());
                ChangeText(arr_txt_rightFingerTipPosition[i], gameMgr.MRMgr.polySpatialInput.arr_trJointR[i].localRotation.x.ToString());
            }
            for (int i = 0; i < 5; i++)
            {
                ChangeText(arr_txt_leftFingerState[i], gameMgr.MRMgr.polySpatialInput.isFingerCullL[i].ToString());
                ChangeText(arr_txt_rightFingerState[i], gameMgr.MRMgr.polySpatialInput.isFingerCullR[i].ToString());
            }
        }

        void ChangeText(Text uiText, string innerText)
        {
            if (uiText != null)
            {
                uiText.text = innerText;
            }
        }
    }
}