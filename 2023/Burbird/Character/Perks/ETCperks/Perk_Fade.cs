using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// »ç¸Á ½Ã ºÎÈ° È½¼ö Áõ°¡
    /// </summary>
    public class Perk_Fade: Perk
    {
        float plusStat = 0;
        protected override void DoAwake()
        {
            PerkNum = 30;
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
            perkChecker.perk_fade = true;
            perkChecker.perk_fadeTime += plusStat;
        }

        public override void PerkLost()
        {
            base.PerkLost();

            double csvStat = System.Convert.ToDouble(perkInfo.status);
            plusStat = (float)csvStat;
            perkChecker.perk_fade = false;
            perkChecker.perk_fadeTime -= plusStat;
        }
    }
}
