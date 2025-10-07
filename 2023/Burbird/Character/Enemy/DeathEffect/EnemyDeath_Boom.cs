using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    public enum BoomType
    {
        NONE = 0,
        FIRE,
        POISON,
        ICE,
    }

    /// <summary>
    /// Make explosion on die
    /// load explosion prefab
    /// get damage on touch while particle animation
    /// </summary>
    public class EnemyDeath_Boom : EnemyDeath
    {
        public GameObject boomPrefab;

        public BoomType typeBoom = BoomType.NONE;

        private void Awake()
        {
            type_death = EnemyDeathType.BOOM;
        }

        /// <summary>
        /// 적에게 사망 시 주변 적에게 피해를 주는 폭발 생성 효과 추가
        /// </summary>
        /// <param name="enemy"></param>
        public override void ActiveEnemyDeathEffect(Enemy enemy)
        {
            StageManager.Instance.enemySpawner.particleHolder.PlayParticle_BoomPlayer(typeBoom, enemy.Status.ATKDamage, enemy.transform.position);
        }

        /// <summary>
        /// 4/17/2023-LYI
        /// 적이 발사한 투사체에서 폭발할 경우 호출
        /// 플레이어에게 데미지
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="damage"></param>
        public void ActiveMissileDeathEffect(Transform tr, int damage)
        {
            GameObject go = StageManager.Instance.enemySpawner.particleHolder.PlayParticle_BoomEnemy(typeBoom, damage, tr.position);
          
            go.gameObject.SetActive(true);

            //잠시 후 컬리더 비활성화
            StartCoroutine(CheckBombCollider(go.GetComponent<Explosion>()));
        }

        IEnumerator CheckBombCollider(Explosion explosion)
        {
            explosion.bombColl.enabled = true;
            yield return new WaitForSeconds(0.3f);
            explosion.bombColl.enabled = false;
        }
    }
}