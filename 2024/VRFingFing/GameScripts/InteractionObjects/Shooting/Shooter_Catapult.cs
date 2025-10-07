using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Shooting {
    public class Shooter_Catapult : MonoBehaviour
    {
        public Shooter shooter;

        [SerializeField]
        Animator anim_shooter;

    enum ShooterAnimState
        {
            IDLE = 0,
            SHOOT,
            RELOAD,
        }

        private ShooterAnimState statAnim;

        private void SetAnim(ShooterAnimState stat)
        {
            statAnim = stat;

            switch (stat)
            {
                case ShooterAnimState.IDLE:
                    break;
                case ShooterAnimState.SHOOT:
                    anim_shooter.SetTrigger("tAttack");
                    break;
                case ShooterAnimState.RELOAD:
                    break;
                default:
                    break;
            }
        }

        public void Attack()
        {
            anim_shooter.SetTrigger("tAttack");

            shooter.isAiming = false;

            shooter.aim.isAim = false;
            shooter.circleAim.isAim = false;
        }

        public void TriggerShot()
        {
            shooter.ShootArrow();


        }

    }
}