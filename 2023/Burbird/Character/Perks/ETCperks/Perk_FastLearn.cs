using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// ∞Ê«Ëƒ° »πµÊ∑Æ ¡ı∞°
    /// </summary>
    public class Perk_FastLearn: Perk
    {
        float plusStat = 0.5f;
        protected override void DoAwake()
        {
            PerkNum = 27;
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

            perkChecker.perk_fastLearn= true;

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            stageMgr.perkChecker.perk_expMultiplier += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            perkChecker.perk_fastLearn = false;
            stageMgr.perkChecker.perk_expMultiplier -= plusStat;
        }
    }
}
