using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 체력이 낮아질수록 데미지 상승 최대 2배
    /// 계수로 할건지 미정
    /// </summary>
    public class Perk_Berserk : Perk
    {
        float plusStat;
        protected override void DoAwake()
        {
            PerkNum = 26;
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

            perkChecker.perk_berserk = true;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            perkChecker.perk_berserk = false;
        }
    }
}
