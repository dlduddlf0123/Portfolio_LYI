using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Debuff_Poison : Debuff
    {

        public float dotDamagePercent = 0.35f;
        public float dotTic = 1f;
        float currentTic = 0;

        protected override void DoAwake()
        {
            base.DoAwake();

            typeDebuff = DebuffType.DOT;

            duration = 60f;
        }
        public override void ActiveEffect()
        {
            if (isActive)
            {
                currentTic = 0;
                return;
            }
            isActive = true;
            StartCoroutine(DotDamage());

            base.ActiveEffect();
        }

        public override void RemoveEffect()
        {
            base.RemoveEffect();
        }

        IEnumerator DotDamage()
        {
            WaitForSeconds timeTic = new WaitForSeconds(dotTic);

            int ticDamage = (int)(damage * dotDamagePercent);

            while (currentTic < duration)
            {
                yield return timeTic;
                currentTic += dotTic;
                currentCharacter.GetDamage(ticDamage, new Color(0.5f, 0, 1));

                //불타는 효과, 효과음
                //GameManager.Instance.soundMgr.PlaySfx(currentCharacter.transform.position, sfx_effect);
            }

            RemoveEffect();
        }
    }
}