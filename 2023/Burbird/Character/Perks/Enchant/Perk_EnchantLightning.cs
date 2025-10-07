using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_EnchantLightning : Perk
    {
        public Debuff_Lightning debuff;

        protected override void DoAwake()
        {
            PerkNum = 19;
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

            debuff = player.buffHolder.AddComponent<Debuff_Lightning>();
            player.playerController.shooter.GetEffect(debuff);
        }

        public override void PerkLost()
        {
            base.PerkLost();

            debuff = player.buffHolder.GetComponent<Debuff_Lightning>();
            player.playerController.shooter.LostEffect(debuff);
        }
    }
}
