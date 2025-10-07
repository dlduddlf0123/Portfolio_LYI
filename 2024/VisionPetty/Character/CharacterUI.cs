using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using MoreMountains.Feedbacks;
using MoreMountains.Tools;

namespace AroundEffect
{

    /// <summary>
    /// 10/31/2024-LYI
    /// 캐릭터 머리 위의 UI
    /// </summary>
    public class CharacterUI : MonoBehaviour
    {
        GameManager gameMgr;
        public CharacterManager charMgr;

        public UI_StatusGauge statGaugeUI;

        [SerializeField] MMF_Player mmf_appear;
        [SerializeField] MMF_Player mmf_disappear;

        float disableTime = 0;
        public float disableMaxTime = 5f;
        public bool isActive = false;

        Coroutine currentCoroutine = null;

        private void Awake()
        {


        }


        public void Init()
        {
            statGaugeUI.SliderInit(charMgr.Status.level_like, charMgr.Status.likeMeter, charMgr.Status.likeMeterMax);

        }

        public void EnableUI()
        {
            if (!isActive)
            {
                statGaugeUI.SliderInit(charMgr.Status.level_like, charMgr.Status.likeMeter, charMgr.Status.likeMeterMax);
                mmf_appear.PlayFeedbacks();
                isActive = true;
            }

            if (currentCoroutine !=null)
            {
                disableTime = disableMaxTime;
                return;
            }
            currentCoroutine = StartCoroutine(AutoDisableCoroutine());

        }

        public void DisableUI()
        {
            isActive = false;

            mmf_disappear.PlayFeedbacks();
        }

        public void RefreshUI()
        {
            EnableUI();
            statGaugeUI.SliderRefresh(charMgr.Status.level_like, charMgr.Status.likeMeter, charMgr.Status.likeMeterMax);
        }

        IEnumerator AutoDisableCoroutine()
        {
            disableTime = disableMaxTime;
            while (disableTime > 0)
            {
                yield return new WaitForSeconds(0.01f);
                disableTime -= 0.01f;
            }
            DisableUI();
        }


    }
}