using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// »ç¸Á ½Ã ºÎÈ° È½¼ö Áõ°¡
    /// </summary>
    public class Perk_ExtraLife: Perk
    {
        int plusStat = 1;
        protected override void DoAwake()
        {
            PerkNum = 28;
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

            perkChecker.perk_extraLife = true;

            int csvStat = System.Convert.ToInt32(perkInfo.status);
            plusStat = csvStat;
            stageMgr.playerControll.player.life += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            int csvStat = System.Convert.ToInt32(perkInfo.status);
            plusStat = csvStat;
            perkChecker.perk_extraLife = false;
            stageMgr.playerControll.player.life -= plusStat;
        }
    }
}
