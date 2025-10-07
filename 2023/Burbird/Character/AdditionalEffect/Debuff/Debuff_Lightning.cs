using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Debuff_Lightning : Debuff
    {
        StageManager stageMgr;
        public float damagePercent = 0.25f;

        protected override void DoAwake()
        {
            base.DoAwake();

            duration = 1f;
        }

        public override void ActiveEffect()
        {
            if (isActive)
            {
                return;
            }
            isActive = true;
            StartCoroutine(LightningEffect());

            base.ActiveEffect();
        }

        public override void RemoveEffect()
        {
            base.RemoveEffect();
        }

        /// <summary>
        /// 히트한 캐릭터에게 데미지
        /// 이후 히트한 캐릭터 위치 기준 일정 범위 내 적들에게 전기 효과 부여
        /// </summary>
        /// <returns></returns>
        IEnumerator LightningEffect()
        {
            float lightningTime = 0.1f;
            if (!currentCharacter.isPlayer)
            {
                stageMgr = StageManager.Instance;
                List<Enemy> list_nearEnemy = new List<Enemy>();

                float currentDist = 0.0f; //거리 체크
                float targetDist = 5.0f; //튕기는 최소 거리 제한

                //적 캐릭터 목록 체크
                for (int i = 0; i < stageMgr.enemySpawner.list_activeEnemy.Count; i++)
                {
                    //임시 거리 측정
                    currentDist = Vector3.Distance(transform.position,
                        stageMgr.enemySpawner.list_activeEnemy[i].transform.position);

                    //임시 거리가 최소 거리 이하일 경우
                    if (currentDist < targetDist)
                    {
                        //타겟을 임시 타겟으로 지정
                        list_nearEnemy.Add(stageMgr.enemySpawner.list_activeEnemy[i]);
                    }
                }

                for (int i = 0; i < list_nearEnemy.Count; i++)
                {
                    if (list_nearEnemy[i].centerTr.GetComponent<Debuff_Lightning>() == null ||
                        list_nearEnemy[i].centerTr.GetComponent<Debuff_Lightning>().isActive == false)
                    {
                        stageMgr.playerControll.shooter.particleHolder.SetLine_Lightning(
                        currentCharacter.centerTr.position,
                        list_nearEnemy[i].centerTr.position, lightningTime);


                        list_nearEnemy[i].GetEffect(this);
                    }
                }

            }

            int dmg = (int)(damage * damagePercent);
            currentCharacter.GetDamage(dmg, Color.yellow);

            yield return new WaitForSeconds(lightningTime);
            RemoveEffect();
        }

    }
}