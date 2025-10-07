using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_Heal : Perk
    {
        int plusStat = 0;
        protected override void DoAwake()
        {
            PerkNum = 31;
            isConsumable = true;
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
            plusStat = (int)(player.playerStatus.maxHp * (float)statPercent);
            //현재 최대 체력의 50% 회복
            player.GetHeal(plusStat);
        }

        public override void PerkLost()
        {
            base.PerkLost();
        }
    }
}