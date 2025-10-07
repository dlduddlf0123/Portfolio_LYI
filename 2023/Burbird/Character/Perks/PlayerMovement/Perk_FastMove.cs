using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 이동속도 증가 퍽
    /// </summary>
    public class Perk_FastMove : Perk
    {
        public float plusStat;
        protected override void DoAwake()
        {
            PerkNum = 13;
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

            perkChecker.perk_fastMove = true;

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            player.controller.speedMultiplier += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            perkChecker.perk_fastMove = false;
            player.controller.speedMultiplier -= plusStat;
        }
    }
}
