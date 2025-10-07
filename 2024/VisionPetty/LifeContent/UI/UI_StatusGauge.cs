using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using MoreMountains.Feedbacks;
using MoreMountains.Tools;

namespace AroundEffect
{

    public class UI_StatusGauge : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Status")]
        [SerializeField] MMProgressBar MMP_gauge;

        [SerializeField] Image img_likebg;
        [SerializeField] Sprite[] arr_barLevelSprite;

        [Header("MMF Player")]
        [SerializeField] MMF_Player mmf_open;
        [SerializeField] MMF_Player mmf_close;

        public bool isUIActive = false;
        public UnityAction onLevelChange;


        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void SliderInit(StatusLevel level, float stat, float statMax)
        {
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }

            ChangeLikeBarColor(level);

            MMP_gauge.SetBar01(stat / statMax);
       }

        /// <summary>
        /// 10/30/2024-LYI
        /// 게이지 바 색 변경
        /// </summary>
        /// <param name="level"></param>
        void ChangeLikeBarColor(StatusLevel level)
        {
            Image img = MMP_gauge.ForegroundBar.GetComponent<Image>();
            img.sprite = arr_barLevelSprite[(int)level];
            img_likebg.sprite = arr_barLevelSprite[(int)level];
            // MMP_like.SetBar01(handCharacter.Status.likeMeter / handCharacter.Status.likeMeterMax);
        }


        /// <summary>
        /// 10/7/2024-LYI
        /// 터치, 먹이 등 수치 변화 시 호출
        /// 현재 슬라이더 변화 보여주기
        /// </summary>
        public void SliderRefresh(StatusLevel level, float stat, float statMax)
        {
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }

            MMP_gauge.UpdateBar01(stat / statMax);

            //if (stat > statMax)
            //{
            //    float remainStat = stat - statMax;
            //    level++;
            //    GaugeLevelUp(level, remainStat, statMax);
            //}
            //else if (stat < 0)
            //{
            //    float remainStat = stat - statMax;
            //    level--;
            //    GaugeLevelDown(level, remainStat, statMax);
            //}
            //else
            //{
            //    MMP_gauge.UpdateBar01(stat / statMax);
            //}
        }

        public void GaugeLevelUp(StatusLevel level, float remainGauge, float statMax)
        {
            if (!this.gameObject.activeInHierarchy) { return; }

            StartCoroutine(GaugeLevelUpCoroutine(MMP_gauge, level, remainGauge, statMax));
        }
        public void GaugeLevelDown(StatusLevel level, float remainGauge, float statMax)
        {
            if (!this.gameObject.activeInHierarchy) { return; }

            StartCoroutine(GaugeLevelDownCoroutine(MMP_gauge, level, remainGauge, statMax));
        }

        IEnumerator GaugeLevelUpCoroutine(MMProgressBar bar, StatusLevel level, float remainGauge, float statMax)
        {
            bar.UpdateBar01(1);

            yield return new WaitForSeconds(bar.IncreasingDelay*2);
            //bar.SetBar01(1);
            bar.Bump();
            onLevelChange?.Invoke();

            yield return new WaitForSeconds(bar.BumpDuration);
            ChangeLikeBarColor(level);
            bar.SetBar01(0);

            yield return new WaitForSeconds(bar.BumpDuration);
            //remainGauge
            bar.UpdateBar01(remainGauge / statMax);

        }
        IEnumerator GaugeLevelDownCoroutine(MMProgressBar bar, StatusLevel level, float remainGauge, float statMax)
        {
            bar.UpdateBar01(0);

            yield return new WaitForSeconds(bar.DecreasingDelay*2);
            //bar.SetBar01(0);
            bar.Bump();
            onLevelChange?.Invoke();

            yield return new WaitForSeconds(bar.BumpDuration);
            ChangeLikeBarColor(level);
            bar.SetBar01(1);

            yield return new WaitForSeconds(bar.BumpDuration);
            //remainGauge
            bar.UpdateBar01(remainGauge / statMax);

        }



    }
}