using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public enum EnemyDeathType
    {
        NONE = 0,
        SPLIT,
        BOOM,
        SUMMON,
    }


    /// <summary>
    /// Effect when enemy die
    /// can be stack
    /// </summary>
    public class EnemyDeath : AdditionalEffect
    {
        public EnemyDeathType type_death = EnemyDeathType.NONE;
        void Start()
        {

        }


        public virtual void ActiveEnemyDeathEffect(Enemy enemy)
        {

        }

        public virtual void ActiveMissileDeathEffect()
        {

        }
    }
}