using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{
    public class FlyingTeleportMonsterController : EnemyController
    {
        //캐릭터 등장, 소멸을 위한 데이터 할당
        public SpriteRenderer[] arr_sprite;

        protected override void DoAwake()
        {
            arr_sprite = GetComponentsInChildren<SpriteRenderer>();
        }
        void Start()
        {
            isKnockBackable = false;

        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                isPlayerCheck = true;
                AI_Move(EnemyState.CHASE);
            }
        }

        private void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                isPlayerCheck = true;
                AI_Move(EnemyState.CHASE);
            }
        }


        Vector3 RandomDirection()
        {
            int rand = Random.Range(0, 4);
            if (direction == 1)
            {
                rand = Random.Range(0, 2);
            }
            else
            {
                rand = Random.Range(2, 4);
            }
            switch (rand)
            {
                case 0:
                    return Vector3.right;
                case 1:
                    return Vector3.left;
                case 2:
                    return Vector3.up;
                case 3:
                    return Vector3.down;
                default:
                    return Vector3.left;
            }
        }

        void ToggleEnemyVisual(bool isOn)
        {
            for (int i = 0; i < arr_sprite.Length; i++)
            {
                arr_sprite[i].enabled = isOn;
            }
            for (int i = 0; i < arr_collider.Length; i++)
            {
                arr_collider[i].enabled = isOn;
            }

        }

        /// <summary>
        /// ???? ?? ????
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Movement()
        {
            ChangeSpriteDirection();

            Vector3 dir = RandomDirection();

            float t = 0;
            while (t < enemyStat.Status.ATKSpeed * 0.5f)
            {
                t += 0.01f;

                transform.Translate(dir * moveSpeed * speedMultiplier * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            AI_Move(EnemyState.ATTACK);
        }

        /// <summary>
        /// 플레이어 위치로 순간이동
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Chase()
        {
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(RoomTeleport());
        }

        /// <summary>
        /// 플레이어 위치로 텔레포트 이동
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayerTeleport()
        {
            Vector3 movePos = stageMgr.playerControll.centerTr.position;

            //포탈 표시 후 사라지기
            stageMgr.enemySpawner.particleHolder.PlayParticle_Spawn(transform.position,
                ()=> ToggleEnemyVisual(false));
            
            yield return new WaitForSeconds(1f);

            //1초 뒤 재등장
            //포탈 표시 후 이동
            //소환 느낌 비슷한 포탈 이펙트 호출
            stageMgr.enemySpawner.particleHolder.PlayParticle_Spawn(movePos,
                () =>
                {
                    transform.position = movePos;
                    ToggleEnemyVisual(true);
                });

            AI_Move(EnemyState.ATTACK);
        }


        /// <summary>
        /// 룸의 포인트로 랜덤 텔레포트
        /// </summary>
        /// <returns></returns>
        IEnumerator RoomTeleport()
        {
            Vector3 movePos = stageMgr.currentRoom.arr_spawnPos
                [Random.Range(0, stageMgr.currentRoom.arr_spawnPos.Length)].position;

            //포탈 표시 후 사라지기
            stageMgr.enemySpawner.particleHolder.PlayParticle_Spawn(transform.position,
                () => ToggleEnemyVisual(false));

            yield return new WaitForSeconds(1f);

            //1초 뒤 재등장
            //포탈 표시 후 이동
            //소환 느낌 비슷한 포탈 이펙트 호출
            stageMgr.enemySpawner.particleHolder.PlayParticle_Spawn(movePos,
                () =>
                {
                    transform.position = movePos;
                    ToggleEnemyVisual(true);
                });

            AI_Move(EnemyState.ATTACK);
        }


    }
}