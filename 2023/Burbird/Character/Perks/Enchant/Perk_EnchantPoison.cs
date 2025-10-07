using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_EnchantPoison : Perk
    {
        public Debuff_Poison debuff;

        protected override void DoAwake()
        {
            PerkNum = 17;
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

            debuff = player.buffHolder.AddComponent<Debuff_Poison>();
            player.playerController.shooter.GetEffect(debuff);
            //player.GetEffect(debuff);
        }

        public override void PerkLost()
        {
            base.PerkLost();

            debuff = player.buffHolder.GetComponent<Debuff_Poison>();
            player.playerController.shooter.LostEffect(debuff);
            //player.LostEffect(debuff);
        }
    }
}
