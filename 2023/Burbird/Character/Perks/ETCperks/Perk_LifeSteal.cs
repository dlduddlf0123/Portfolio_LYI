using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 생명력 흡수 퍽
    /// 적 캐릭터 사망 시 체력 회복
    /// </summary>
    public class Perk_LifeSteal : Perk
    {
        float plusStat = 0f;

        protected override void DoAwake()
        {
            PerkNum = 25;
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
            perkChecker.perk_lifeSteal += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();
            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;

            perkChecker.perk_lifeSteal -= plusStat;
        }
    }
}
