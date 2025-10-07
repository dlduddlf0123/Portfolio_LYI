using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class BossRoom : Room
    {
        public Enemy boss;
        public NPCInteraction npc;
        public NPCInteraction perfectNpc;


        public override void RoomStart()
        {
            base.RoomStart();

            stageMgr.ui_game.ToggleBossHP(true);
            boss.SetHPGaugeImage(stageMgr.ui_game.game_boss_img_hp);

            if (npc != null)
            {
                npc.gameObject.SetActive(false);
            }
            if (perfectNpc != null)
            {
                perfectNpc.gameObject.SetActive(false);
            }
        }

        public override void RoomEnd()
        {
            stageMgr.ui_game.ToggleBossHP(false);

            base.RoomEnd();

            //플레이어 체력 체크, 최대 체력이면
            if (stageMgr.playerControll.player.playerStatus.hp == stageMgr.playerControll.player.playerStatus.maxHp)
            {
                //악마 소환
                if (perfectNpc != null)
                {
                    perfectNpc.gameObject.SetActive(false);
                }
            }

            //룰렛 소환
            if (npc != null)
            {
                npc.gameObject.SetActive(true);
                npc.NPCInit(npc.transform);
            }

        }

    }
}