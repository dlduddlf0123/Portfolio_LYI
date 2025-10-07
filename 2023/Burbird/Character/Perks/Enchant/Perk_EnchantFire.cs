using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_EnchantFire : Perk
    {
        public Debuff_Burn debuff;

        protected override void DoAwake()
        {
            PerkNum = 16;
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

            debuff= player.buffHolder.AddComponent<Debuff_Burn>();
            player.playerController.shooter.GetEffect(debuff);
        }

        public override void PerkLost()
        {
            base.PerkLost();

            debuff = player.buffHolder.GetComponent<Debuff_Burn>();
            player.playerController.shooter.LostEffect(debuff);
        }
    }
}
