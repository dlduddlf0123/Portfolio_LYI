using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    /// <summary>
    /// StageManager에서 호출
    /// 각 룸에서 적을 생성하는 역할을 담당한다
    /// 어떤 적을 생성할지는 미리 엑셀등으로 스테이지매니저에서 생성할 적 번호와 위치, 수를 받을 것
    /// 
    /// 오브젝트 풀링 할 때 이번 게임에서 중간에 나가거나 보스를 처치할 때 까지 재활용 가능하게 할 것
    /// Enemy Class로 List에 저장해서 몬스터 번호로 검색 가능하게 만들 것
    /// Enemy 소환 시 스탯 배정, CSV파일 로드해서 스탯 적용
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        StageManager stageMgr;

       public  EnemyProjectileManager projectileMgr;

        public List<Enemy> list_originEnemyPrefab = new List<Enemy>(); //일반 적 종류
        public List<Enemy> list_originBossPrefab = new List<Enemy>(); //보스 종류

        //오브젝트 풀링
        public List<Enemy> list_disableEnemy = new List<Enemy>();
        public List<Enemy> list_activeEnemy = new List<Enemy>(); //활성화 된 적, 자동 공격 시 검색

        Transform tr_active;
        
        Transform tr_disable;

        /// <summary>
        /// 적 캐릭터를 소환할 위치
        /// 각 룸 별로 위치를 표시하며 Gizmo로 확인
        /// </summary>
        public Transform[] arr_spawnPos;

        public EnemyParticleHolder particleHolder;
        public EnemyDeath_Boom[] death_boom;

        public bool isWave = false;
        public int waveCount = 0;
        void Awake()
        {
            stageMgr = StageManager.Instance;

            tr_active = transform.GetChild(0);
            tr_disable = transform.GetChild(1);
        }

        /// <summary>
        /// 적 캐릭터가 죽었을 때 호출, 리스트 정리 및 클리어 체크
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyListInit(Enemy enemy)
        {
            // go.transform.SetParent(arr_ballParent[0]);
            //queue_popcornPool.Enqueue(go);
            StageManager.Instance.playerControll.shooter.fireTarget = null;

            if (enemy.isClone)
            {
            }
            else
            {
            }

            if (!enemy.IsBoss())
            {
                list_activeEnemy.Remove(enemy);
                list_disableEnemy.Add(enemy);
                enemy.transform.SetParent(tr_disable);
            }
            else
            {
                list_activeEnemy.Remove(enemy);

            }

            CheckEnemyCount();
        }

        /// <summary>
        /// 남은 적 캐릭터 체크
        /// </summary>
        void CheckEnemyCount()
        {
            //모두 처치되면?
            if (list_activeEnemy.Count == 0)
            {
                //웨이브 모드면 다음 웨이브 전환
                if (isWave &&
                    waveCount >= 0)
                {
                    waveCount--;

                    for (int i = 0; i < arr_spawnPos.Length; i++)
                    {
                        SpawnEnemy(list_originEnemyPrefab[Random.Range(0, list_originEnemyPrefab.Count)], arr_spawnPos[i].position);
                    }
                }
                else
                {
                    //룸 클리어 처리
                    stageMgr.RoomEnd();
                }
            }
            else
            {
                if (stageMgr.playerControll.shooter.fireTarget == null)
                {
                    for (int i = 0; i < list_activeEnemy.Count; i++)
                    {
                        list_activeEnemy[i].gameObject.SetActive(true);
                    }
                }
            }
        }

        public void SpawnRandomEnemyWithPoints()
        {
            if (arr_spawnPos == null || arr_spawnPos.Length == 0) { return; }

            for (int i = 0; i < arr_spawnPos.Length; i++)
            {
                SpawnEnemy(list_originEnemyPrefab[Random.Range(0, list_originEnemyPrefab.Count)], arr_spawnPos[i].position);
            }
        }

        /// <summary>
        /// 정해진 위치에 정해진 몬스터 생성
        /// </summary>
        /// <param name="arr_name"></param>
        public void SpawnEnemyToPoints()
        {
            if (arr_spawnPos == null || arr_spawnPos.Length == 0) { return; }

            if (stageMgr.currentStageData.list__roomEnemy.Count <= stageMgr.currentRoomNum)
            {
                Debug.Log("Room Enemy List is End.\n Check the RoomEnemy CSV file");
                return;
            }
            if (stageMgr.currentRoom.GetComponent<BossRoom>())
            {
                list_activeEnemy.Add(stageMgr.currentRoom.GetComponent<BossRoom>().boss);
            }

            object[] arr_name = stageMgr.currentStageData.list__roomEnemy[stageMgr.currentRoomNum + 1].ToArray();

            for (int i = 1; i < arr_name.Length; i++)
            {
                if (arr_name[i] != null && arr_spawnPos.Length > i)
                {
                    SpawnEnemy(list_originEnemyPrefab.Find(item => item.name == arr_name[i].ToString()), arr_spawnPos[i - 1].position);
                }
            }
        }

        /// <summary>
        /// 적 오브젝트 Instantiate
        /// 비활성 리스트 중에서 원하는 몬스터 번호가 있을 경우 해당 오브젝트 Init(), Active()
        /// 오브젝트가 없으면 해당 몬스터 오브젝트 호출(StageManager에서 게임 시작 시 정해진 적 원본 오브젝트 배열을 미리 들고있을 것)
        /// </summary>
        //public void SpawnEnemy(Enemy enemyPrefab, Vector3 position)
        //{
        //    Enemy localEnemy = null;

        //    //비활성화 된 적들 중 요청 캐릭터가 이미 있으면 들고오기
        //    for (int i = 0; i < list_disableEnemy.Count; i++)
        //    {
        //        if (list_disableEnemy[i].name == enemyPrefab.name)
        //        {
        //            localEnemy = list_disableEnemy[i];
        //            list_disableEnemy.Remove(list_disableEnemy[i]);
        //            continue;
        //        }
        //    }

        //    if (tr_disable.childCount == 0 ||
        //        localEnemy == null)
        //    {
        //        localEnemy = Instantiate(enemyPrefab);
        //        localEnemy.name = enemyPrefab.name;
        //    }

        //    localEnemy.transform.SetParent(tr_active);
        //    localEnemy.transform.position = position;

        //    list_activeEnemy.Add(localEnemy);

        //    localEnemy.gameObject.SetActive(true);

        //    localEnemy.EnemyInit();
        //    localEnemy.SpawnCharacter();

        //    AddEnemyDeathEffect(localEnemy);
        //}
        public Enemy SpawnEnemy(Enemy enemyPrefab, Vector3 position)
        {
            Enemy localEnemy = null;

            //비활성화 된 적들 중 요청 캐릭터가 이미 있으면 들고오기
            for (int i = 0; i < list_disableEnemy.Count; i++)
            {
                if (list_disableEnemy[i].name == enemyPrefab.name)
                {
                    localEnemy = list_disableEnemy[i];
                    list_disableEnemy.Remove(list_disableEnemy[i]);
                    continue;
                }
            }

            if (tr_disable.childCount == 0 ||
                localEnemy == null)
            {
                localEnemy = Instantiate(enemyPrefab);
                localEnemy.name = enemyPrefab.name;
            }

            localEnemy.transform.SetParent(tr_active);
            localEnemy.transform.position = position;

            list_activeEnemy.Add(localEnemy);

            localEnemy.gameObject.SetActive(true);

            localEnemy.EnemyInit();
            localEnemy.SpawnCharacter();

            AddEnemyDeathEffect(localEnemy);

            return localEnemy;
        }

        public Enemy SpawnTokenEnemy(Enemy origin, Vector3 position)
        {
            Enemy enemy = null;

            //비활성화 된 적들 중 요청 캐릭터가 이미 있으면 들고오기
            for (int i = 0; i < list_disableEnemy.Count; i++)
            {
                if (list_disableEnemy[i].EnemyCode == origin.EnemyCode)
                {
                    enemy = list_disableEnemy[i];
                    list_disableEnemy.Remove(list_disableEnemy[i]);
                    break;
                }
            }

            if (enemy == null)
            {
                enemy = Instantiate(origin);
            }

            enemy.transform.SetParent(tr_active);
            enemy.transform.position = position;


            list_activeEnemy.Add(enemy);

            enemy.gameObject.SetActive(true);

            //if (tr_disable.childCount == 0 ||
            //    enemy == null)
            //{
            //    enemy = Instantiate(origin);
            //    enemy.name = origin.name;
            //    enemy.transform.SetParent(tr_active);
            //    enemy.transform.position = position;

            //    list_activeEnemy.Add(enemy);

            //    enemy.gameObject.SetActive(true);
            //}
            //else
            //{
            //    enemy.transform.SetParent(tr_active);
            //    enemy.transform.position = position;

            //    list_activeEnemy.Add(enemy);

            //    enemy.gameObject.SetActive(true);
            //}

            enemy.EnemyInit();
            //enemy.SpawnCharacter();

            //소형화 = 체력감소, 크기감소, 데미지는 그대로?
            enemy.ChangeSize(EnemySize.SMALL);

            AddEnemyDeathEffect(enemy);

            return enemy;
        }


        /// <summary>
        /// 적 캐릭터 사망시 추가할 효과들 체크
        /// </summary>
        public void AddEnemyDeathEffect(Enemy enemy)
        {
            bool[] arr_exist = new bool[4];
            arr_exist = EnemyBoomChecker(enemy);

            if (stageMgr.perkChecker.perk_enemyBoom)
            {
                EnemyDeath_Boom boom = death_boom[0];
                boom.typeBoom = BoomType.NONE;

                if (arr_exist[0] == false)
                {
                    enemy.list_perk_enemyDeath.Add(boom);
                }
                //기본 공격력 * 3 * 2
            }
            if (stageMgr.perkChecker.perk_fireBoom)
            {
                EnemyDeath_Boom boom = death_boom[1];
                boom.typeBoom = BoomType.FIRE;

                if (arr_exist[1] == false)
                {
                    enemy.list_perk_enemyDeath.Add(boom);
                }
                //기본 공격력 * 3 + 화상
            }
            if (stageMgr.perkChecker.perk_poisonBoom)
            {
                EnemyDeath_Boom boom = death_boom[2];
                boom.typeBoom = BoomType.POISON;

                if (arr_exist[2] == false)
                {
                    enemy.list_perk_enemyDeath.Add(boom);
                }
                //기본 공격력 * 3 + 중독
            }
            if (stageMgr.perkChecker.perk_iceBoom)
            {
                //기본 공격력 * 3 + 빙결
                EnemyDeath_Boom boom = death_boom[3];
                boom.typeBoom = BoomType.ICE;

                if (arr_exist[3] == false)
                {
                    enemy.list_perk_enemyDeath.Add(boom);
                }
            }

        }

        /// <summary>
        /// Boom 관련 퍽 효과 적용
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        bool[] EnemyBoomChecker(Enemy enemy)
        {
            bool[] arr_bool = new bool[4];

            List<int> list_boomNum = new List<int>();

            for (int i = 0; i < enemy.list_perk_enemyDeath.Count; i++)
            {
                if (enemy.list_perk_enemyDeath[i].type_death == EnemyDeathType.BOOM)
                {
                    list_boomNum.Add(i);
                }
            }

            if (list_boomNum.Count == 0)
            {
                return arr_bool;
            }

            EnemyDeath_Boom[] arr_boom = new EnemyDeath_Boom[list_boomNum.Count];
            for (int i = 0; i < list_boomNum.Count; i++)
            {
                arr_boom[i] = enemy.list_perk_enemyDeath[list_boomNum[i]].GetComponent<EnemyDeath_Boom>();
                if (arr_boom[i].typeBoom == BoomType.NONE)
                {
                    arr_bool[0] = true;
                }
                if (arr_boom[i].typeBoom == BoomType.FIRE)
                {
                    arr_bool[1] = true;
                }
                if (arr_boom[i].typeBoom == BoomType.POISON)
                {
                    arr_bool[2] = true;
                }
                if (arr_boom[i].typeBoom == BoomType.ICE)
                {
                    arr_bool[3] = true;
                }
            }


            return arr_bool;
        }
    }
}