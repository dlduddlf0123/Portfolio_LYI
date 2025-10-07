using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Burbird
{

    public class EnemyDeath_Split3 : EnemyDeath
    {
        public int splitTime = 3; //분열 횟수
        public int splitNum = 2; //분열 당 나눠질 갯수

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
            EnemySpawner enemySpawner = StageManager.Instance.enemySpawner;


            EnemyDeath_Split3 split = this;
            for (int i = 0; i < enemy.list_addEffect.Count; i++)
            {
                if (enemy.list_addEffect[i].TryGetComponent<EnemyDeath_Split3>(out EnemyDeath_Split3 s))
                {
                    split = s;
                }
            }

            if (split.splitTime > 0)
            {
                split.splitTime--;
            }
            else
            {
                return;
            }

            for (int splitIndex = 0; splitIndex < splitNum; splitIndex++)
            {
                Enemy e;
                e = enemySpawner.SpawnTokenEnemy(enemy, enemy.transform.position);

                e.Status.maxHp = e.originHP /(int)Mathf.Pow(2, 3-split.splitTime);
                e.Status.hp = e.Status.maxHp;
                e.transform.localScale = Vector3.one * 0.125f * (Mathf.Pow(2, split.splitTime));

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


                //if (splitNum > 0)
                //{
                //    EnemyDeath_Split3 death =
                //        e.list_enemyDeath.Find(item =>
                //        item.type_death == EnemyDeathType.SPLIT).GetComponent<EnemyDeath_Split3>();

                //    death.splitNum--;
                //}

                e.enemyController.AI_Move(EnemyState.MOVE);

                // e.list_enemyDeath.Remove(this);
                e.onEnemyDie += () =>
                {
                    Destroy(e.gameObject);
                };
            }

        }
    }
}