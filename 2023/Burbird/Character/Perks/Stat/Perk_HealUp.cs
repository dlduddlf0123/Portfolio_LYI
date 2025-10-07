using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_HealUp : Perk
    {
        float plusStat = 0;
        protected override void DoAwake()
        {
            PerkNum = 4;
            isStackable = true;
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


            double statPercent = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)statPercent;

            //회복량 30% 증가
            player.perk_healMultiplier += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();
            player.perk_healMultiplier -= plusStat;

            double statPercent = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)statPercent;
        }
    }
}