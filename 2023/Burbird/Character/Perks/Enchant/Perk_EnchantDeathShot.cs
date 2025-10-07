using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_EnchantDeathShot : Perk
    {
        public float plusStat = 0f;

        protected override void DoAwake()
        {
            PerkNum = 20;
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

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;

            //즉사확률 1% 증가
            player.perk_deathShot += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            player.perk_deathShot -= plusStat;
        }
    }
}
