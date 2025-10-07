using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_BounceShot : Perk
    {
        protected override void DoAwake()
        {
            PerkNum = 11;
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
            perkChecker.perk_bounceShot = true;

        }

        public override void PerkLost()
        {
            base.PerkLost();
            perkChecker.perk_bounceShot = false;
        }
    }
}