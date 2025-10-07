using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class RestRoom : Room
    {
        public Angel angelOrigin;

        public bool isAngelUsed = false;
        public bool isStart = false; //첫 방인지

        public override void RoomInit()
        {
            base.RoomInit();
            stageMgr.statStage = StageStat.REST;

            isStart = (stageMgr.currentRoomNum <= 0) ? true : false;

            isRoomClear = true;

            if (isStart)
            {
                //첫 방일 경우 특성 선택
                //if (GameManager.Instance.dataMgr.characterStat.abillity.)
                //{

                //}
            }
            else
            {
                SpawnAngel();
            }
        }

        public override void RoomStart()
        {
            RoomInit();
        }

        void SpawnAngel()
        {
            if (isAngelUsed)
            {
                return;
            }
            Angel go = Instantiate(angelOrigin);
            go.gameObject.SetActive(true);
            go.NPCInit(trSpawn);
        }

    }
}