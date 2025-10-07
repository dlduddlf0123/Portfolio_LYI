using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Perk_EnchantFreeze : Perk
    {
        public Debuff_Freeze debuff;

        protected override void DoAwake()
        {
            PerkNum = 18;
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

            debuff= player.buffHolder.AddComponent<Debuff_Freeze>();
            player.playerController.shooter.GetEffect(debuff);
        }

        public override void PerkLost()
        {
            base.PerkLost();

            debuff = player.buffHolder.GetComponent<Debuff_Freeze>();
            player.playerController.shooter.LostEffect(debuff);
        }
    }
}
