using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    public class EnemyDeath_Split : EnemyDeath
    {
        public int splitNum;

        void Awake()
        {
            type_death = EnemyDeathType.SPLIT;
        }

        /// <summary>
        /// 사망 효과 발동
        /// 적 캐릭터 소환
        /// </summary>
        /// <param name="enemy"></param>
        public override void ActiveEnemyDeathEffect(Enemy enemy)
        {
            if (enemy.isClone)
            {
                return;
            }

            EnemySpawner enemySpawner = StageManager.Instance.enemySpawner;
           
            for (int splitIndex = 0; splitIndex < splitNum; splitIndex++)
            {
                Enemy e;
                e = enemySpawner.SpawnTokenEnemy(enemy, enemy.transform.position);

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

                //하나는 왼쪽 하나는 오른쪽
                e.enemyController.direction = (splitIndex == 0) ? -1 : 1;

                // enemy.enemyController.originScale = enemy.transform.GetChild(0).localScale;
                // enemySpawner.list_activeEnemy.Add(e);
                e.enemyController.AI_Move(EnemyState.MOVE);

               // e.list_enemyDeath.Remove(this);
                e.onEnemyDie += () =>
                {
                    e.isClone = false;
                    //enemySpawner.list_activeEnemy.Remove(e);
                    //e.list_enemyDeath.Add(this);
                    //Destroy(this.gameObject);
                };
            }

        }
    }
}