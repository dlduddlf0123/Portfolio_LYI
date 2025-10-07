using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    /// <summary>
    /// 폭파 시 생성됨
    /// 생성된 이펙트 크기만큼 주변 적들에게 데미지, 디버프 부여
    /// </summary>
    public class Explosion : MonoBehaviour
    {
        Debuff debuff;

        public BoomType explosionType;
        public int damage = 1;

        public bool isEnemy = false; //적이 쏜거면 플레이어에게 데미지
        public Collider2D bombColl;

        private void Awake()
        {
            if (GetComponent<Debuff>())
            {
                debuff = GetComponent<Debuff>();

                debuff.damage = (int)StageManager.Instance.playerControll.player.playerStatus.ATKDamage;
            }
            bombColl = GetComponent<Collider2D>();
        }
        
      Color32 CheckDMGColorType()
        {
            Color32 color;
            switch (explosionType)
            {
                case BoomType.FIRE:
                    color = new Color32(255, 170, 0, 1);
                    break;
                case BoomType.ICE:
                    color =  Color.blue;
                    break;
                case BoomType.POISON:
                    color = Color.green;
                    break;
                case BoomType.NONE:
                default:
                    color = Color.red;
                    break;
            }

            return color;
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (!isEnemy) { return; }

                Player player = StageManager.Instance.playerControll.player;

                player.GetDamage(damage, CheckDMGColorType());

                if (debuff != null)
                {
                    player.GetEffect(debuff);
                }
            }

            if (coll.gameObject.CompareTag("Enemy"))
            {
                if (isEnemy){ return;}

                Enemy enemy = coll.GetComponentInParent<Enemy>();
                enemy.GetDamage(damage, CheckDMGColorType());

                if (debuff != null)
                {
                    enemy.GetEffect(debuff);
                }
            }

        }
    }
}