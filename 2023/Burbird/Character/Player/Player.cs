using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Burbird
{
    /// <summary>
    /// 캐릭터의 능력치 관련 클래스
    /// </summary>
    public class Status
    {
        //레벨
        public int level; //레벨
        public int maxLevel; //최대레벨
        public float exp; //현재 경험치
        public float maxExp; //최대 경험치

        //체력
        public int hp; //현재 체력
        public int maxHp; //최대 체력

        //공격
        public float ATKDamage; //공격력
        public float ATKSpeed; //공격 속도
        public float shotSpeed; //탄속

        //확률
        public float critChance; //치명타 확률
        public float critDamage; //치명타 배율
        public float avoidChance; //회피 확률
        public Status()
        {
            level = 0;
            maxLevel = 0;
            exp = 0;
            maxExp = 0;

            hp = 0;
            maxHp = 0;

            ATKDamage = 0f;
            ATKSpeed = 0f;
            shotSpeed = 0f;

            critChance = 0f;
            critDamage = 0f;
            avoidChance = 0f;
        }

        /// <summary>
        /// 연산자 오버로딩
        /// 클래스 간 계산을 빠르게
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Status operator +(Status a, Status b)
        {
            Status p = new Status();
            p.maxHp = a.maxHp + b.maxHp;
            p.ATKDamage = a.ATKDamage + b.ATKDamage;
            p.ATKSpeed = a.ATKSpeed + b.ATKSpeed;
            p.shotSpeed = a.shotSpeed + b.shotSpeed;
            p.critChance = a.critChance + b.critChance;
            p.critDamage = a.critDamage + b.critDamage;
            p.avoidChance = a.avoidChance + b.avoidChance;
            return p;
        }
        public static Status operator -(Status a, Status b)
        {
            Status p = new Status();
            p.maxHp = a.maxHp - b.maxHp;
            p.ATKDamage = a.ATKDamage - b.ATKDamage;
            p.ATKSpeed = a.ATKSpeed - b.ATKSpeed;
            p.shotSpeed = a.shotSpeed - b.shotSpeed;
            p.critChance = a.critChance - b.critChance;
            p.critDamage = a.critDamage - b.critDamage;
            p.avoidChance = a.avoidChance - b.avoidChance;
            return p;
        }
    }



    /// <summary>
    /// Player 캐릭터 스탯, 특성 관련 정보
    /// 데미지 계산 관련 UI 관련
    /// </summary>
    public class Player : Character
    {
        StageManager stageMgr;

        public PlayerController2D playerController { get; set; }
        public Status playerStatus = new Status();

        //버프 홀더
        //각종 캐릭터 버프나 디버프류를 게임오브젝트 형태로 가지고 있다가 적 캐릭터에게 전달하는 객체
        [Header("Player")]
        public GameObject buffHolder;

        public List<Perk> list_perk = new List<Perk>();

        //애니메이션
        protected Animator m_animator;
        protected Animator m_spriteAnim;

        protected Canvas statCanvas;
        protected Image img_hpGauge;
        protected Text txt_hp;

        [Header("Audio Clips")]
        public AudioClip sfx_getDamage;
        public AudioClip sfx_levelUp;
        public AudioClip sfx_heal;

        float invinscibleTime = 1f;
        public bool isInvincible = false; //무적, 피격무적
        public bool isShooting = true; //발사 체크

        [Header("Life Parameter")]
        public int life = 1; //목숨, 기본 1, 퍽 등으로 증가
        public bool isDie = false;
        public bool isRevive = false; //부활 했는지 여부

        //인게임에서 얻는 수치
        [Header("Perk Parameters")]
        public float perk_deathShot = 0f;
        public float perk_healMultiplier = 1f;



        /// <summary>
        /// Origin Stats
        /// 스테이지 밖에서의 스텟의 총 합 스텟
        /// 캐릭터 레벨, 장비 수치 등의 합
        /// 스텟 관련 퍽 획득 시 기준으로 사용
        /// </summary>
        public int originHp { get; set; }
        public float originATKDamage { get; set; }
        public float originATKSpeed { get; set; }
        public float originShotSpeed { get; set; }
        private void Awake()
        {
            stageMgr = StageManager.Instance;
            arr_spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

            playerController = GetComponent<PlayerController2D>();
            m_animator = transform.GetChild(0).GetComponent<Animator>();
            m_spriteAnim = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
            
            base.controller = playerController;
            base.isPlayer = true;

            statCanvas = transform.GetChild(3).GetComponent<Canvas>();
            img_hpGauge = statCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            txt_hp = statCanvas.transform.GetChild(1).GetComponent<Text>();

        }

        /// <summary>
        /// DataManager에서 플레이어 스탯 불러오기
        /// </summary>
        void LoadStatus()
        {
            playerStatus.level = 1;
            playerStatus.exp = 0;

            playerStatus.maxExp = 100; //Excel에서 레벨 당 경험치 표 만들것 100+ lev-1*50

            originATKDamage = playerStatus.ATKDamage;
            originATKSpeed = playerStatus.ATKSpeed;
            originShotSpeed = playerStatus.shotSpeed;
            originHp = playerStatus.maxHp;

            playerStatus.hp = playerStatus.maxHp;
        }

        /// <summary>
        /// 게임 시작 시 호출
        /// 룸에서 호출하지 말 것
        /// </summary>
        public void Init()
        {
            LoadStatus();
            HPUIRefresh();

            isRevive = false;
            life = 1;
        }


        /// <summary>
        /// 캐릭터에 피해 적용
        /// </summary>
        /// <param name="damage"></param>
        public void GetDamage(float damage, Vector3 point)
        {
            if (isInvincible || isDie)
            {
                return;
            }

            playerStatus.hp -= (int)damage;
            HPUIRefresh();

            playerController.KnockBack(point);

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_getDamage, Random.Range(0.7f, 1.4f));
            stageMgr.ui_game.ShowDamageText(playerController.centerTr.position, (int)damage);
            StartCoroutine(HitEffect());

            CheckBerserkPerk();

            if (playerStatus.hp <= 0)
            {
                playerStatus.hp = 0;
                HPUIRefresh();
                PlayerDie();
                return;
            }
        }

        /// <summary>
        /// 속성 데미지?
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="color"></param>
        public override void GetDamage(int damage, Color color)
        {
            if (isInvincible || isDie)
            {
                return;
            }

            playerStatus.hp -= (int)damage;
            HPUIRefresh();

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_getDamage, Random.Range(0.7f, 1.4f));
            stageMgr.ui_game.ShowDamageText(playerController.centerTr.position, (int)damage,color);
            StartCoroutine(HitEffect());

            CheckBerserkPerk();

            if (playerStatus.hp <= 0)
            {
                playerStatus.hp = 0;
                HPUIRefresh();
                PlayerDie();
                return;
            }
        }

        /// <summary>
        /// 캐릭터 체력 회복
        /// </summary>
        /// <param name="heal"></param>
        public void GetHeal(float heal)
        {
            if (isDie)
            {
                return;
            }

            int activeHeal = (int)(heal * perk_healMultiplier);

            playerStatus.hp += (int)activeHeal;

            if (playerStatus.hp >= playerStatus.maxHp)
            {
                playerStatus.hp = playerStatus.maxHp;
            }

            CheckBerserkPerk();
            HPUIRefresh();

            stageMgr.soundMgr.PlaySfx(transform.position, sfx_heal, Random.Range(0.7f, 1.4f));
            stageMgr.ui_game.ShowDamageText(playerController.centerTr.position, (int)heal, Color.green);

        }

        //버서크 퍽 획득 시 공격력 증가
        public void CheckBerserkPerk()
        {
            if (stageMgr.perkChecker.perk_berserk)
            {
                //데미지 증가량 = 1 - 현재 체력 비율 %
                //체력이 적어질수록 높아진다
                float increase = 1 - ((float)playerStatus.hp / (float)playerStatus.maxHp);
                playerStatus.ATKDamage = originATKDamage + originATKDamage * increase;
            }
        }



        /// <summary>
        /// 체력 UI 갱신
        /// </summary>
        public void HPUIRefresh()
        {
            img_hpGauge.fillAmount = (float)playerStatus.hp / (float)playerStatus.maxHp;
            txt_hp.text = playerStatus.hp.ToString();
        }

        /// <summary>
        /// 경험치 획득
        /// </summary>
        /// <param name="exp"></param>
        public void GetExp(float exp)
        {
            StartCoroutine(GaugeUpEffect(exp));
        }

        /// <summary>
        /// 최대 경험치의 % 획득
        /// </summary>
        /// <param name="amount">25% * amount</param>
        public void GetRewardExp(int amount)
        {
            float exp = 0.25f * amount;

            GetExp(playerStatus.maxExp * exp);
        }

        /// <summary>
        /// 레벨 상승, 특성 선택
        /// </summary>
        public void LevelUp()
        {
            playerStatus.level++;
            stageMgr.ui_game.ChangeLevelText(playerStatus.level);
            stageMgr.soundMgr.PlaySfx(transform.position, sfx_levelUp);

            //특성 선택 창 호출
            stageMgr.ui_game.canvas_perk.PerkCanvasActive();

            playerStatus.exp = playerStatus.exp - playerStatus.maxExp;
            playerStatus.maxExp = 100 + (float)playerStatus.level * 5f;

            stageMgr.ui_game.ChangeEXPGauge((float)playerStatus.exp / (float)playerStatus.maxExp);

            if (playerStatus.exp < 0)
            {
                playerStatus.exp = 0;
                stageMgr.ui_game.ChangeEXPGauge((float)playerStatus.exp / (float)playerStatus.maxExp);
            }
            else if (playerStatus.exp >= playerStatus.maxExp)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// 3/17/2023-LYI
        /// 모든 퍽 제거, 레벨 1로 조정
        /// </summary>
        public void LevelReset()
        {
            int count = list_perk.Count;
            for (int i = 0; i < count; i++)
            {
                playerStatus.level--;
                list_perk[i].PerkLost();
            }

            stageMgr.ui_game.ChangeLevelText(playerStatus.level);
        }

        IEnumerator GaugeUpEffect(float getEXP)
        {
            if (stageMgr.perkChecker.perk_fastLearn)
            {
                getEXP *= stageMgr.perkChecker.perk_expMultiplier;
            }

            while (getEXP > 0)
            {
                getEXP -= 5;
                playerStatus.exp += 5;
                stageMgr.ui_game.ChangeEXPGauge((float)playerStatus.exp / (float)playerStatus.maxExp);

                if (playerStatus.exp >= playerStatus.maxExp)
                {
                    LevelUp();
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
        void ActivePlayerSprite(bool active)
        {
            transform.GetChild(0).GetChild(0).gameObject.SetActive(active);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(active);
            transform.GetChild(0).GetChild(2).gameObject.SetActive(active);
        }

        /// <summary>
        /// 피격 효과, 무적
        /// </summary>
        /// <returns></returns>
        IEnumerator HitEffect()
        {
            isInvincible = true;
            for (int i = 0; i < 10 * invinscibleTime; i++)
            {
                ActivePlayerSprite(false);
                yield return new WaitForSeconds(0.05f);
                ActivePlayerSprite(true);
                yield return new WaitForSeconds(0.05f);
            }
            isInvincible = false;
            ActivePlayerSprite(true);
        }


        /// <summary>
        /// 캐릭터 사망 시
        /// </summary>
        public void PlayerDie()
        {
            if (isDie)
            {
                return;
            }

            isDie = true;

            //사망 애니메이션, 파티클, 효과음



            //목숨이 여러개면 쓰러진 뒤 부활
            if (life > 1)
            {
                PlayerRevive();
                return;
            }

            //목숨 모두 소모시 부활 한 적이 없다면
            if (!isRevive)
            {
                //카운트다운 호출
                stageMgr.ui_game.StartCountDown();
                return;
            }

            stageMgr.StageResult();
        }

        /// <summary>
        /// 마지막 목숨 사망 시 5초 카운트 다운, 광고 클릭 or 보석으로 부활
        /// </summary>
        public void PlayerCountDown()
        {
            isRevive = true;
            stageMgr.ui_game.StartCountDown();
        }

        /// <summary>
        /// 플레이어 부활
        /// </summary>
        public void PlayerRevive()
        {
            Debug.Log("PlayerRevive()");

            isDie = false;

            if (life > 0)
            {
                life--; //생명 감소
            }

            playerStatus.hp = playerStatus.maxHp; //체력 회복
            HPUIRefresh();

            //부활 파티클, 효과음, 화면 정지 효과 등

            stageMgr.soundMgr.PlaySfx(transform, sfx_heal);

            //부활 일시 무적
            StartCoroutine(HitEffect());
        }

    }
}