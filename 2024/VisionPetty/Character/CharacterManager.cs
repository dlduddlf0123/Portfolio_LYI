using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    /// <summary>
    /// 240805 LYI
    /// Character scripts management, holder
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("ReadOnly")]
        public CharacterStatus Status;
        public CharacterAnimation Animation;

        [Header("Class")]
        public CharacterMovement Movement;
        public CharacterAIManager AI;
        public CharacterGestureChecker Gesture;
        public CharacterColliderManager Collider;
        public CharacterPetting Petting;
        public CharacterUI UI;
        public CharacterParticle Particle;


        private void Awake()
        {
            gameMgr = GameManager.Instance;
            CharacterInit();
        }


        public virtual void CharacterInit()
        {
            Status.charMgr = this;

            AI.charMgr = this;
            Movement.charMgr = this;
            Gesture.charMgr = this;
            Collider.charMgr = this;
            Petting.charMgr = this;
            UI.charMgr = this;
            Particle.charMgr = this;

            Status.Init();
            Animation.Init();

            Movement.Init();
            AI.Init();
            Gesture.Init();
            Collider.Init();
            Petting.Init();
            UI.Init();
            Particle.Init();

            Debug.Log(gameObject.name + "- Init()");
        }


        /// <summary>
        /// 9/10/2024-LYI
        /// 캐릭터 동작들 코루틴들 멈추기
        /// </summary>
        public virtual void Stop()
        {
            if (AI.isEvent)
            {
                return;
            }

            Movement.Stop();
            AI.Stop();
            Gesture.Stop();

        }

    }
}