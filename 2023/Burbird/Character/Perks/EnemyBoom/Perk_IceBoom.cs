using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 적 캐릭터 사망 시 폭발로 주변 적 데미지 주는 퍽 획득
    /// 앞으로 생성되는 적은 사망 시 효과 획득
    /// EnemySpawner에서 효과 적용
    /// </summary>
    public class Perk_IceBoom : Perk
    {
        public Debuff_Burn debuff;

        float plusStat = 0f;
        protected override void DoAwake()
        {
            PerkNum = 24;
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

            perkChecker.perk_iceBoom = true;
            perkChecker.perk_iceBoomPower += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;

            perkChecker.perk_iceBoom = false;
            perkChecker.perk_iceBoomPower -= plusStat;
        }
    }
}
