using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 점프 높이 증가 퍽
    /// </summary>
    public class Perk_HighJump : Perk
    {
        public float plusStat;
        protected override void DoAwake()
        {
            PerkNum = 14;
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

            perkChecker.perk_highJump = true;

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            player.controller.jumpMultiplier += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();
            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;

            perkChecker.perk_highJump = false;
            player.controller.jumpMultiplier -= plusStat;
        }
    }
}
