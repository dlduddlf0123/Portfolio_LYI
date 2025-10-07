using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{

    /// <summary>
    /// 3/24/2023-LYI
    /// 일정 주기로 정해진 방향으로 미사일을 발사
    /// EnemyShooter, Attack 활용
    /// </summary>
    public class Trap_Projectile : EnemyRangedAttack
    {
        [SerializeField]
        Transform centerTr;

        public int shotDamage;
        [Tooltip("Delay to shot (sec)")]
        public float shotDelay;
        public float shotSpeed;

        public int projectileNum;

        float shotTimer = 0;
        Vector3 shotVec;



        // Start is called before the first frame update
        void Start()
        {
            shotVec = centerTr.position + shooter.transform.right;
            
            missileDamage = shotDamage;
            missileSpeed = shotSpeed * 5;
        }


        private void Update()
        {
            shotTimer += Time.deltaTime;
            if (shotTimer > shotDelay)
            {
                if (projectileNum > 1)
                {
                    ActiveMultiStraightMissile(origin_missile, projectileNum,shotVec);
                }
                else
                {
                    ActiveMissile(origin_missile, shotVec);
                }
                shotTimer = 0;
            }
        }
    }
}