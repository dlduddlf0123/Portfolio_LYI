using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    /// <summary>
    /// 3/14/2023-LYI
    /// 연습용 방
    /// 적 소환, 제거 관련 처리
    /// </summary>
    public class RoomPractice : Room
    {
        private void Start()
        {
            RoomStart();
        }

        /// <summary>
        /// 방이 생성 되었을 때 호출
        /// 페이드 이후 방 활성화
        /// </summary>
        public override void RoomInit()
        {
            base.RoomInit();

            //stageMgr = StageManager.Instance;
            //cameraBound2D = transform.GetChild(0).GetComponent<Collider2D>();

            ////카메라 범위 설정
            //stageMgr.camConfine2D.m_BoundingShape2D = cameraBound2D;
            //stageMgr.camConfine2D.InvalidateCache();

            //캐릭터 위치 설정
            //stageMgr.playerControll.SetPlayerPos(trPlayer.position);


            //if (trSpawn == null || trSpawn.childCount == 0)
            //{
            //    stageMgr.enemySpawner.arr_spawnPos = null;
            //    stageMgr.RoomEnd();
            //    return;
            //}

            ////적 소환 위치 설정
            //arr_spawnPos = new Transform[trSpawn.childCount];
            //for (int i = 0; i < trSpawn.childCount; i++)
            //{
            //    arr_spawnPos[i] = trSpawn.GetChild(i);
            //}


            //stageMgr.enemySpawner.arr_spawnPos = arr_spawnPos;
        }


        /// <summary>
        /// 방 시작 시 처리할 공통 로직
        /// </summary>
        public override void RoomStart()
        {
            RoomInit();
            arr_roomDoor[0].DoorClose();

            stageMgr.statStage = StageStat.FIGHT;
            GameManager.Instance.statGame = SceneStatus.PRACTICE;

            ////내부 생성 캐릭터가 없을 경우 데이터 받아서 생성 진행
            //if (arr_spawnEnemy.Length <= 0)
            //{
            //    stageMgr.enemySpawner.SpawnEnemyToPoints();
            //    return;
            //}
            //else
            //{
            //    //룸 내부에 캐릭터가 배치되어있을 경우 소환 대신 현재 있는 캐릭터들 활성화
            //    for (int i = 0; i < arr_spawnEnemy.Length; i++)
            //    {
            //        // arr_spawnEnemy[i].gameObject.SetActive(true);
            //        arr_spawnEnemy[i].EnemyInit();
            //        arr_spawnEnemy[i].SpawnCharacter();

            //        StageManager.Instance.enemySpawner.AddEnemyDeathEffect(arr_spawnEnemy[i]);
            //        StageManager.Instance.enemySpawner.list_activeEnemy.Add(arr_spawnEnemy[i]);
            //    }
            //}

        }

        /// <summary>
        /// 방의 적들 모두 처치시 호출
        /// </summary>
        public override void RoomEnd()
        {
            //stageMgr.playerControll.player.GetExp(roomExp);
            //roomExp = 0;

            //isRoomClear = true;
            //arr_roomDoor[1].DoorOpen();
        }

    }
}