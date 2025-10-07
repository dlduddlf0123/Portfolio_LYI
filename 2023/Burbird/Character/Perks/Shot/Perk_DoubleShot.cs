using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_DoubleShot : Perk
    {
        int plusStat = 0;
        protected override void DoAwake()
        {
            PerkNum = 6;
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
            int csvStat = System.Convert.ToInt32(perkInfo.status);
            plusStat = csvStat;
            perkChecker.perk_doubleShot += plusStat;

        }

        public override void PerkLost()
        {
            base.PerkLost();

            int csvStat = System.Convert.ToInt32(perkInfo.status);
            plusStat = csvStat;
            perkChecker.perk_doubleShot -= plusStat;
        }
    }
}