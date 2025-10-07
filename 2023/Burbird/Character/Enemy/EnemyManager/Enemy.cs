using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using UnityEngine.Events;

namespace Burbird
{
    public enum EnemySize
    {
        NORMAL = 0,
        SMALL,
        BIG,
    }
    [SerializeField]
    public class EnemyStatus
    {
        public int hp;
        public int maxHp;

        public int ATKDamage;
        public float ShotSpeed;
        public float ATKSpeed;

        public int EXP; //경험치 수치
        public int Coin; //돈 수치

        //장비 드랍 확률, 하트 확률 등
    }

    /// <summary>
    /// 적 캐릭터의 스탯 관리, 충돌 체크, 사망 관리
    /// </summary>
    public class Enemy : Character
    {
        protected StageManager stageMgr;

        public EnemyStatus OriginStatus = new EnemyStatus();
        public EnemyStatus Status = new EnemyStatus();


        public EnemyController enemyController { get; set; } //적 캐릭터 이동
        public EnemyRangedAttack enemyAttack { get; set; }

        [Header("Enemy")]
        public TextAsset json_stat;
        public List<EnemyDeath> list_perk_enemyDeath = new List<EnemyDeath>();

        protected Canvas statCanvas;
        protected Image img_hpGauge;

        public List<Feather> list_hitedFeather = new List<Feather>();
        public Transform hitedFeatherTr { get; set; }

        //캐릭터 종류에 따라 달라지는 고유 번호
        public int EnemyCode = 0;

        public int originHP;

        public EnemySize enemySize = EnemySize.NORMAL;

        [Header("Parameter")]
        public bool isDie; //사망 여부 체크
        public bool isClone; //분신 여부 체크
        public bool isDropable; //아이템 드랍 여부 체크

        public UnityAction onEnemyDie;

        private bool isSpawned = false; //스폰 된 적이 있는지 체크

        private void Awake()
        {
            stageMgr = StageManager.Instance;
            arr_spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

            //EnemyInit();
            DoAwake();
        }

        protected virtual void DoAwake() { }
        protected virtual void StatusInit() { }

        /// <summary>
        /// 4/5/2023-LYI
        /// 적 캐릭터 스탯 적용
        /// Stage Stat 성장치 적용
        /// </summary>
        /// <param name="status"></param>
        protected virtual void StatusInit(EnemyStatus status)
        {
            //스테이지 보너스 스탯
            //10% * 룸 번호 * 스테이지 번호 
            //100 기준 1룸 1스테이지 = 110, 50룸 = 500
            float hpBonusStat = stageMgr.bonusHPPercent * stageMgr.currentRoomNum * stageMgr.stageNum;
            float dmgBonusStat = stageMgr.bonusDMGPercent * stageMgr.stageNum;

            if (IsBoss())
            {
                Status.maxHp = status.maxHp + (int)(status.maxHp * hpBonusStat*0.5f);
            }
            else
            {
                Status.maxHp = status.maxHp + (int)(status.maxHp * hpBonusStat);
            }
            Status.ATKDamage = status.ATKDamage+ (int)(status.ATKDamage * dmgBonusStat);
            Status.ATKSpeed = status.ATKSpeed;
            Status.ShotSpeed = status.ShotSpeed;

            Status.EXP = status.EXP;
            Status.Coin = status.Coin;
            

            //4/13/2023-LYI
            //적 투사체 스탯 적용
            if (enemyAttack != null)
            {
                enemyAttack.SetShooterStat(this);
            }
        }

        public void EnemyInit()
        {
           // Debug.Log(EnemyCode +  ": EnemyInit()");

            //컴포넌트 할당 생성 시 1회만
            if (!isSpawned)
            {
                stageMgr = StageManager.Instance;
                enemyController = GetComponent<EnemyController>();
                arr_spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

                base.controller = enemyController;

                if (TryGetComponent(out EnemyRangedAttack atk))
                {
                    enemyAttack = atk;
                }

                statCanvas = transform.GetChild(1).GetComponent<Canvas>();
                img_hpGauge = statCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();

                hitedFeatherTr = transform.GetChild(3);
                centerTr = transform.GetChild(4);

                isSpawned = true;
            }

            //스탯 할당
            GetEnemyStatusData();


            //체력 초기화
            Status.hp = Status.maxHp;
            originHP = Status.maxHp;

            img_hpGauge.fillAmount = 1;

            //상태 초기화
            isDie = false;
            isDropable = true;

            onEnemyDie = null;
            onEnemyDie += EnemyDeathEffect;

            ChangeSize(enemySize);
        }


        /// <summary>
        /// 4/4/2023-LYI
        /// 수정할 부분
        /// dic_json을 통해 호출하는 방법에서 그냥 csv로 호출해서 비교하는 코드 쓸것
        /// </summary>
        protected void GetEnemyStatusData()
        {
            /*
            if (json_stat == null)
            {
                if (EnemyCode == 0)
                {
                    json_stat = GameManager.Instance.addressMgr.dic_jsonEnemyStatus["JsonEnemyStandard"];
                }
                else
                {
                    json_stat = GameManager.Instance.addressMgr.dic_jsonEnemyStatus[EnemyCode.ToString()];
                }
                Debug.Log("Please Check json_stat Field null");
                return;
            }

            EnemyStatus stat = JsonUtility.FromJson<EnemyStatus>(json_stat.text);
            */

            //CSV to EnemyStatus
            EnemyStatus stat = new EnemyStatus();

            List<object> list_stat;
            
           try
            {
                list_stat = GameManager.Instance.dataMgr.dic_list_enemyStat[EnemyCode];
            }
           catch (Exception e)
            {
                //EnemyCode가 잘못됐을 경우 기본 스탯 부여, 경고 메시지 
                Debug.LogError("Wrong enemy code!: " + EnemyCode +"\n"+ e);
                list_stat = GameManager.Instance.dataMgr.dic_list_enemyStat[0];
            }

            //enemyCode =  list_stat[0], 1번부터 시작
            stat.maxHp = Convert.ToInt32(list_stat[1]);
            stat.ATKDamage = Convert.ToInt32(list_stat[2]);
            stat.ShotSpeed = (float)Convert.ToDouble(list_stat[3]);
            stat.ATKSpeed = (float)Convert.ToDouble(list_stat[4]);
            stat.EXP = Convert.ToInt32(list_stat[5]);
            stat.Coin = Convert.ToInt32(list_stat[6]);

            OriginStatus = stat;
            StatusInit(stat);
        }

        /// <summary>
        /// 적 캐릭터 데미지 받을 때
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="color"></param>
        public override void GetDamage(int damage, Color color)
        {
            if (isDie)
            {
                return;
            }

            if (stageMgr.isEnemyDebug)
                Debug.Log(gameObject.name + " Get Damage:" + damage);

            Status.hp -= damage;
            img_hpGauge.fillAmount = (float)Status.hp / (float)Status.maxHp;
            stageMgr.ui_game.ShowDamageText(centerTr.position, damage, color);


           //  StartCoroutine(HitShake());

            if (Status.hp <= 0)
            {
                EnemyDie();
            }
        }

        public void SetHPGaugeImage(Image img)
        {
            img_hpGauge = img;
        }

        public void SpawnCharacter()
        {
            gameObject.SetActive(false);
            StageManager.Instance.enemySpawner.particleHolder.PlayParticle_Spawn(centerTr.position, OnSpawnCharacter, enemySize);
        }

        /// <summary>
        /// 캐릭터 소환 시 작동, 등장 효과 적용, 활성화
        /// </summary>
        public override void OnSpawnCharacter()
        {
            gameObject.SetActive(true);

            if (enemyController != null)
            {
                enemyController.AI_Move(EnemyState.MOVE);
            }
        }

        private int RandomDirection()
        {
            int a = UnityEngine.Random.Range(0, 2);
            if (a == 0)
                return -1;
            else
                return 1;
        }

        private float TimeDown(float main, float time)
        {
            if (main > 0)
                return main -= time;
            else if (main < 0)
                return main += time;
            else
                return 0;
        }

        /// <summary>
        /// 흔들림 효과
        /// 현재 피격시 적용, 발사 전에 흔들림 효과로 변경?
        /// </summary>
        /// <returns></returns>
        protected IEnumerator HitShake(float time = 0.1f)
        {
            float maxRand = 0.15f;
            float minRand = 0.05f;
            float randX = UnityEngine.Random.Range(minRand * RandomDirection(), maxRand * RandomDirection());
            float randY = UnityEngine.Random.Range(minRand * RandomDirection(), maxRand * RandomDirection());

            float t = 0;

            while (t < time)
            {
                t += 0.01f;

                randX = UnityEngine.Random.Range(minRand * RandomDirection(), maxRand * RandomDirection());
                randY = UnityEngine.Random.Range(minRand * RandomDirection(), maxRand * RandomDirection());

                transform.GetChild(0).localPosition = new Vector3(randX, randY);

                minRand = TimeDown(minRand, t);
                maxRand = TimeDown(maxRand, t);

                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localPosition = new Vector3(-randX, -randY);
                yield return new WaitForSeconds(0.01f);
            }
            transform.GetChild(0).localPosition = Vector3.zero;
        }

        /// <summary>
        /// 적 사망시 호출
        /// </summary>
        public virtual void EnemyDie()
        {
            if (stageMgr.isEnemyDebug)
                Debug.Log("EnemyDie: " + gameObject.name);

            isDie = true;

            //3/17/2023-LYI
            //아이템 드랍여부 체크 추가
            if (isDropable)
            {

                if (!isClone)
                {
                    //경험치 축적
                    stageMgr.currentRoom.roomExp += Status.EXP;

                    //아이템 드랍
                    stageMgr.itemSpawner.SpawnItem(centerTr.position);
                }

                //코인 드랍
                for (int i = 0; i < UnityEngine.Random.Range(1, 4); i++)
                {
                    stageMgr.coinSpawner.SpawnCoin(centerTr.position, Status.Coin);
                }

                //하트 드랍
                stageMgr.heartSpawner.SpawnHeart(centerTr.position);
            }


            //생명력 회복퍽 체크
            if (stageMgr.perkChecker.perk_lifeSteal > 0)
            {
                Player player = stageMgr.playerControll.player;
                player.GetHeal(player.playerStatus.maxHp * stageMgr.perkChecker.perk_lifeSteal);
            }


            //(old) 몸에 박힌 깃털 제거하기
            for (int i = 0; i < list_hitedFeather.Count; i++)
            {
                list_hitedFeather[i].FeatherFall();
            }

            StartCoroutine(DieAct());
        }

        /// <summary>
        /// 사망 시 효과 처리
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator DieAct()
        {
            if (controller != null)
            {
                enemyController.Stop();
            }

            //사망 시 이벤트(자폭 등)
            onEnemyDie.Invoke();

            //효과 제거
            if (list_addEffect.Count > 0)
            {
                for (int i = 0; i < list_addEffect.Count; i++)
                {
                    list_addEffect[i].RemoveEffect();
                }
            }
            list_addEffect.Clear();


            //적 캐릭터 사망 모션
            transform.GetChild(0).gameObject.SetActive(false);

            enemyController.m_rigidbody2D.simulated = false;
            statCanvas.gameObject.SetActive(false);

            stageMgr.enemySpawner.EnemyListInit(this);

            //if (enemyAttack != null)
            //{
            //  //  yield return StartCoroutine(enemyAttack.MissileCheck());
            //}

            yield return new WaitForSeconds(0.01f);

            //초기화 처리
            transform.GetChild(0).gameObject.SetActive(true);

            enemyController.m_rigidbody2D.simulated = true;
            statCanvas.gameObject.SetActive(true);

            //EnemyInit();

            gameObject.SetActive(false);
        }


        /// <summary>
        /// 캐릭터 사망 시 작동
        /// </summary>
        protected virtual void EnemyDeathEffect()
        {
            for (int i = 0; i < list_perk_enemyDeath.Count; i++)
            {
                GetEffect(list_perk_enemyDeath[i]);                
                list_perk_enemyDeath[i].ActiveEnemyDeathEffect(this);
            }
        }

        /// <summary>
        /// 보스인지 아닌지 구별
        /// </summary>
        /// <returns></returns>
        public bool IsBoss()
        {
            char a = EnemyCode.ToString()[0];
            if (a == '1')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 적 캐릭터 사이즈 변경
        /// 크기에 따라 능력치 변경 적용
        /// </summary>
        /// <param name="size"></param>
        public void ChangeSize(EnemySize size)
        {
            switch (size)
            {
                case EnemySize.NORMAL:
                    Status.maxHp = originHP;
                    Status.hp = Status.maxHp;
                    transform.localScale = Vector3.one;
                    break;
                case EnemySize.SMALL:
                    Status.maxHp = originHP/2;
                    Status.hp =Status.maxHp;
                    transform.localScale = Vector3.one *0.7f;
                    break;
                case EnemySize.BIG:
                    Status.maxHp = originHP * 2;
                    Status.hp = Status.maxHp;
                    transform.localScale = Vector3.one * 1.3f;
                    break;
                default:
                    break;
            }
        }

    }
}