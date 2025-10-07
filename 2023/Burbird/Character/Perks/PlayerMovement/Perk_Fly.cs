using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// 비행 활성화 퍽
    /// </summary>
    public class Perk_Fly : Perk
    {
        protected override void DoAwake()
        {
            PerkNum = 15;
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

            perkChecker.perk_fly = true;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            perkChecker.perk_fly = false;
        }
    }
}
