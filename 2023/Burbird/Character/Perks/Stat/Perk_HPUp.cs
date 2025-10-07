using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_HPUp : Perk
    {
        int plusStat = 0;
        protected override void DoAwake()
        {
            PerkNum = 2;

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

            double statPercent =  System.Convert.ToDouble(perkInfo.status);
            plusStat = (int)(player.originHp * statPercent);
            //기본 최대 체력의 50% 증가
            player.playerStatus.maxHp += plusStat;
            player.playerStatus.hp += plusStat;
            player.HPUIRefresh();//체력값 갱신
        }

        public override void PerkLost()
        {
            base.PerkLost();

            //최대 체력 감소
            player.playerStatus.maxHp -= plusStat;

            double statPercent = System.Convert.ToDouble(perkInfo.status);
            plusStat = (int)(player.originHp * statPercent);
        }
    }
}