using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace Burbird
{
    /// <summary>
    /// 부가 효과, 버프 / 디버프
    /// </summary>
    public class AdditionalEffect : MonoBehaviour
    {
        public Character currentCharacter;

        public AudioClip sfx_effect;

        public UnityEvent onEffectActive;
        public UnityEvent onEffectRemove;

        public bool isActive = false;

        public int damage;
        public float duration;
        
        //중첩이 가능할 경우
        public bool isStackable = false;
        public int stack = 0;
        public int maxStack = 0;

        private void Awake()
        {

            DoAwake();
        }

        protected virtual void DoAwake() { }
        public virtual void ActiveEffect()
        {
            if (onEffectActive != null)
            {
                onEffectActive.Invoke();
            }
        }


        public virtual void RemoveEffect()
        {
            isActive = false;
            StopAllCoroutines();

            if (onEffectRemove != null)
            {
                onEffectRemove.Invoke();
            }
        }

    }
}