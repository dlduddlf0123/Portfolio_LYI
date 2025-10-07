using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_ATKSpeed : Perk
    {
        float plusStat = 0;
        float plusStat2 = 0;
        protected override void DoAwake()
        {
            PerkNum = 3;
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
            plusStat = player.originATKSpeed * (float)statPercent;
            plusStat2 = player.originShotSpeed * (float)statPercent;
            //기본 공격 속도의 50% 증가
            player.playerStatus.ATKSpeed += plusStat;
            player.playerStatus.shotSpeed += plusStat2;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double statPercent = System.Convert.ToDouble(perkInfo.status);
            plusStat = player.originATKSpeed * (float)statPercent;
            plusStat2 = player.originShotSpeed * (float)statPercent;

            player.playerStatus.ATKSpeed -= plusStat;
            player.playerStatus.shotSpeed -= plusStat2;
        }
    }
}