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
    public class Perk_FireBoom : Perk
    {
        public Debuff_Burn debuff;

        float plusStat = 0f;
        protected override void DoAwake()
        {
            PerkNum = 22;
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

            perkChecker.perk_fireBoom= true;
            perkChecker.perk_fireBoomPower += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            perkChecker.perk_fireBoom = false;
            perkChecker.perk_fireBoomPower += plusStat;
        }
    }
}
