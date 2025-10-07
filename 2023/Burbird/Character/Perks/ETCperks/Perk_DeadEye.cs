using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 적 투사체 이동속도 감소
    /// </summary>
    public class Perk_DeadEye: Perk
    {
        float plusStat = 0.5f;
        protected override void DoAwake()
        {
            PerkNum = 29;
        }

        private void Start()
        {

        }

        public override void PerkClick()
        {
            base.PerkClick();

        }

        public override void PerkActive()
        {
            base.PerkActive();

            perkChecker.perk_deadEye = true;

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            stageMgr.perkChecker.perk_enemyShotSpeedMultiplier -= plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            perkChecker.perk_deadEye = false;
            stageMgr.perkChecker.perk_enemyShotSpeedMultiplier += plusStat;
        }
    }
}
