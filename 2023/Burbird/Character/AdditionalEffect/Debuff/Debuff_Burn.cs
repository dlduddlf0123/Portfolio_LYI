using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    /// <summary>
    /// 화상 디버프 부여
    /// 짧은 지속시간, 중간 데미지
    /// </summary>
    public class Debuff_Burn : Debuff
    {
       
        public float dotDamagePercent = 0.18f;
        public float dotTic = 0.25f;
        float currentTic = 0;

        protected override void DoAwake()
        {
            base.DoAwake();

            typeDebuff = DebuffType.DOT;

            duration = 2f;
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
                currentCharacter.GetDamage(ticDamage, new Color(1,0.5f,0));

                //불타는 효과, 효과음
                //GameManager.Instance.soundMgr.PlaySfx(currentCharacter.transform.position, sfx_effect);
            }

            RemoveEffect();
        }
    }
}