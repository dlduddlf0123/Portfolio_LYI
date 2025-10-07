using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public enum DebuffType
    {
        NONE = 0,
        DOT,
        STUN,
        SLOW,
        AIRBORNE,

    }
    public class Debuff : AdditionalEffect
    {

        public DebuffType typeDebuff = DebuffType.NONE;


        protected override void DoAwake()
        {
            base.DoAwake();
        }
        public override void ActiveEffect()
        {
            base.ActiveEffect();

        }

        public override void RemoveEffect()
        {
            base.RemoveEffect();
        }

    }
}