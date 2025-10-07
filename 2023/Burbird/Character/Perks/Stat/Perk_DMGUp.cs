using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_DMGUp : Perk
    {

        float plusDamage = 0;
        protected override void DoAwake()
        {
            PerkNum = 1;
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
            plusDamage = player.originATKDamage * (float)statPercent;
            //공격 데미지 50% 증가
            player.playerStatus.ATKDamage += plusDamage;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double statPercent = System.Convert.ToDouble(perkInfo.status);
            plusDamage = player.originATKDamage * (float)statPercent;
            //공격 데미지 감소
            player.playerStatus.ATKDamage -= plusDamage;
        }
    }
}