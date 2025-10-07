using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Burbird
{

    /// <summary>
    /// 3/24/2023-LYI
    /// 투사체가 부딪혔을 때 해당 위치에 적 소환
    /// 여러개의 투사체를 발사하면 여러 곳에 적 소환
    /// 주로 소형 적
    /// 궁수의전설에서 유령 같은것
    /// </summary>
    public class EnemyDeath_Summon : EnemyDeath
    {
        public Enemy summonEnemy; //소환할 적

        void Awake()
        {
            type_death = EnemyDeathType.SUMMON;

        }

        /// <summary>
        /// 사망 효과 발동
        /// 적 캐릭터 소환
        /// </summary>
        public override void ActiveMissileDeathEffect()
        {
            EnemySpawner enemySpawner = StageManager.Instance.enemySpawner;

            Enemy e;
            e = enemySpawner.SpawnTokenEnemy(summonEnemy, transform.position);

            //클론으로 소환됐을 때에 대한 처리??
            e.isClone = true;

            //3/20/2023-LYI
            //연습모드 드랍 통제
            e.isDropable = !(GameManager.Instance.statGame == SceneStatus.PRACTICE);

            //4/13/2023-LYI
            //소환 시 발사하는 항목 비활성화 코드
            //발사체 매커니즘 변경으로 필요 없어짐
            //if (e.enemyAttack != null)
            //{
            //    if (e.enemyAttack.shooter != null)
            //    {
            //        if (e.enemyAttack.shooter.list_activeMissile.Count != 0)
            //        {
            //            for (int missileIndex = 0; missileIndex < e.enemyAttack.shooter.list_activeMissile.Count; missileIndex++)
            //            {
            //                e.enemyAttack.shooter.list_activeMissile[missileIndex].gameObject.SetActive(false);
            //            }
            //        }
            //    }
            //}

            e.enemyController.AI_Move(EnemyState.MOVE);

        }
    }
}