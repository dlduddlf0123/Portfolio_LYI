using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class Debuff_Freeze : Debuff
    {
        bool isFrozen = false;
        protected override void DoAwake()
        {
            base.DoAwake();

            duration = 1f;
        }
        public override void ActiveEffect()
        {
            if (isActive && currentCharacter.controller == null)
            {
                return;
            }
            isActive = true;
            StartCoroutine(FreezeEffect());

            base.ActiveEffect();
        }

        public override void RemoveEffect()
        {
            isFrozen = false;

            base.RemoveEffect();
        }

        IEnumerator FreezeEffect()
        {
            if (!isFrozen)
            {
                //스턴 이펙트, 사운드
                isFrozen = true;
                typeDebuff = DebuffType.STUN;

                if (currentCharacter.isPlayer)
                {
                    //플레이어는 조작불가
                    currentCharacter.controller.isStun = true;
                }
                else
                {
                    //적 캐릭터는 AI 정지
                    currentCharacter.controller.GetComponent<EnemyController>().Stop();

                }


                yield return new WaitForSeconds(duration);
                if (currentCharacter.isPlayer)
                {
                    //플레이어는 조작불가
                    currentCharacter.controller.isStun = false;
                }
                else
                {
                    //적 캐릭터는 AI 정지
                    currentCharacter.controller.GetComponent<EnemyController>().AI_Move(EnemyState.IDLE);

                }
            }

            typeDebuff = DebuffType.SLOW;

            //슬로우 이펙트, 사운드

            float originSpeed = currentCharacter.controller.speedMultiplier;
            currentCharacter.controller.speedMultiplier = 0.1f;

            yield return new WaitForSeconds(duration * 2);

            currentCharacter.controller.speedMultiplier = originSpeed;
        }


    }
}