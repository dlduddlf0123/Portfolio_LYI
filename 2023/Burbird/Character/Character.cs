using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


namespace Burbird
{

    /// <summary>
    /// 3/21/2023-LYI
    /// Player, Enemy 공통 동작 스크립트
    /// </summary>
    public class Character : MonoBehaviour
    {
        public CharacterController controller { get; set; }
        public Transform centerTr { get; set; }

        /// <summary>
        /// 3/21/2023-LYI
        /// 캐릭터 색 변경용 스프라이트 확인
        /// </summary>
        public SpriteRenderer[] arr_spriteRenderer { get; set; }

        [Header("Base Character")]
        public List<AdditionalEffect> list_addEffect = new List<AdditionalEffect>();


        public bool isPlayer = false; //플레이어 여부 체크

        public virtual void GetDamage(int damage, Color color)
        {

        }

        public virtual void OnSpawnCharacter()
        {

        }


        /// <summary>
        /// 3/21/2023-LYI
        /// Change character sprite colors
        /// </summary>
        /// <param name="color"></param>
        public virtual void ChangeSpritesColor(Color32 color)
        {
            if (arr_spriteRenderer.Length > 0)
            {
                for (int i = 0; i < arr_spriteRenderer.Length; i++)
                {
                    arr_spriteRenderer[i].color = color;
                }
            }
        }

        /// <summary>
        /// 해당 효과 획득 시 호출
        /// </summary>
        /// <param name="effect"></param>
        public virtual void GetEffect(AdditionalEffect effect)
        {
            AdditionalEffect e;
            Type t = effect.GetType();

            //이미 해당 효과가 있는지 체크
            if (centerTr.GetComponent(t))
            {
                e = centerTr.GetComponent(t) as AdditionalEffect;
                if (e.isStackable)
                {
                    if (e.stack < e.maxStack)
                    {
                        e.stack++;
                        e.ActiveEffect();
                    }
                    else
                    {
                        Debug.Log("Effect stack is full:" + effect.name);
                    }

                    return;
                }
                else
                {
                    //있는데 중첩 불가능한 경우 효과 갱신?
                    e.ActiveEffect();
                    return;
                }
            }
            else
            {
                //해당 효과가 없을 경우
                e = centerTr.gameObject.AddComponent(t) as AdditionalEffect;
                e.currentCharacter = this;
                e.damage = effect.damage;

                list_addEffect.Add(e);

                e.ActiveEffect();
            }

        }


        public virtual void LostEffect(AdditionalEffect effect)
        {
            //리스트에서 이미 해당 효과가 있을 때
            if (list_addEffect.Contains(effect))
            {
                //중첩 가능한 경우 획득
                if (effect.isStackable)
                {
                    AdditionalEffect stackEffect = list_addEffect.Find(e => e == effect);
                    if (stackEffect.stack >1)
                    {
                        stackEffect.stack--;
                    }
                    else
                    {
                        effect.RemoveEffect();
                        list_addEffect.Remove(effect);
                    }
                    return;
                }
                else
                {
                    //중첩 불가 안내
                    Debug.Log("Aleady has effect:" + effect.name);
                    return;
                }
            }

            effect.RemoveEffect();
            list_addEffect.Remove(effect);
        }
    }
}