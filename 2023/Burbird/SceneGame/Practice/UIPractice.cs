using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace Burbird
{

    /// <summary>
    /// 3/14/2023-LYI
    /// 연습용 버튼 동작 관리
    /// </summary>
    public class UIPractice : MonoBehaviour
    {
        StageManager stageMgr;

        [SerializeField]
        Room roomPractice;

        //Player Options
        [Header("PlayerOptions")]
        [SerializeField]
        RectTransform option_player;

        [SerializeField]
        Button btn_openPlayerOption;
        [SerializeField]
        Button btn_closePlayerOption;

        [Space(10)]
        [SerializeField]
        Button player_btn_recovery;
        [SerializeField]
        Button player_btn_levelUp;
        [SerializeField]
        Button player_btn_invincible;
        [SerializeField]
        Button player_btn_shootToggle;
        [SerializeField]
        Button player_btn_reset;


        //Enemy Options
        [Space(10)]
        [Header("EnemyOptions")]
        [SerializeField]
        RectTransform option_enemy;

        [SerializeField]
        Button btn_openEnemyOption;
        [SerializeField]
        Button btn_closeEnemyOption;

        [Space(10)]
        [SerializeField]
        Button[] enemy_arr_btn_summon;
        [SerializeField]
        List<Enemy> list_enemy = new List<Enemy>();
        [SerializeField]
        List<Enemy> list_boss= new List<Enemy>();

        [SerializeField]
        Button enemy_reset;

        [Space(10)]
        [Header("StageSelelct")]
        [SerializeField]
        Button stage_btn_left;
        [SerializeField]
        Button stage_btn_right;
        [SerializeField]
        TextMeshProUGUI stage_txt_num;

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            enemy_arr_btn_summon = option_enemy.GetChild(0).GetChild(0).GetComponentsInChildren<Button>();   
        }


        // Start is called before the first frame update
        void Start()
        {
            if (!GameManager.Instance.addressMgr.isLoadComplete)
            {
                StartCoroutine(WaitforPlayerDataLoad());
            }
            InitButtons();
        }

        IEnumerator WaitforPlayerDataLoad()
        {
           while(!GameManager.Instance.addressMgr.isLoadComplete)
            {
                yield return new WaitForSeconds(0.1f);
            }
            stageMgr = GameObject.FindObjectOfType<StageManager>();

            GameManager.Instance.statGame = SceneStatus.PRACTICE;
            GameManager.Instance.dataMgr.LocalLoadPlayerStat();
            GameManager.Instance.invenChecker.RefreshEquipStat();
            StageManager.Instance.LoadPlayerData();
            StageManager.Instance.perkChecker.LoadPerkInfo();
            StageManager.Instance.StageManagerInit(GameManager.Instance.playStageData);
            StaticManager.UI.MessageUI.PopupMessage("Player stat loaded");

            StageManager.Instance.currentRoom.RoomStart();
        }

        /// <summary>
        /// 3/14/2023-LYI
        /// 버튼 기능 할당
        /// </summary>
        public void InitButtons()
        {
            //적 할당
            if (enemy_arr_btn_summon.Length >= list_enemy.Count)
            {
                for (int i = 0; i < list_enemy.Count; i++)
                {
                    int a = i;
                    enemy_arr_btn_summon[a].onClick.AddListener(() => SummonEnemy(list_enemy[a]));
                    enemy_arr_btn_summon[a].transform.GetChild(0).GetComponent<Text>().text = list_enemy[a].gameObject.name;
                }
            }

            //Option Player
            btn_openPlayerOption.onClick.AddListener(() => option_player.gameObject.SetActive(true));
            btn_closePlayerOption.onClick.AddListener(() => option_player.gameObject.SetActive(false));

            player_btn_recovery.onClick.AddListener(HealthRecovery);
            player_btn_levelUp.onClick.AddListener(LevelUp);
            player_btn_invincible.onClick.AddListener(ToggleInvincible);
            player_btn_shootToggle.onClick.AddListener(ToggleShoot);
            player_btn_reset.onClick.AddListener(PlayerReset);

            //Option Enemy
            btn_openEnemyOption.onClick.AddListener(() => option_enemy.gameObject.SetActive(true));
            btn_closeEnemyOption.onClick.AddListener(() => option_enemy.gameObject.SetActive(false));

            stage_btn_left.onClick.AddListener(StageLeftButton);
            stage_btn_right.onClick.AddListener(StageRightButton);

            stage_txt_num.text = stageMgr.stageNum.ToString();

            enemy_reset.onClick.AddListener(EnemyReset);
        }

        #region Player Buttons
        public void HealthRecovery()
        {
            stageMgr.playerControll.player.GetHeal(999999);
            StaticManager.UI.MessageUI.PopupMessage("HP Recovered");
        }
        public void LevelUp()
        {
            stageMgr.playerControll.player.LevelUp();
        }
        public void ToggleInvincible()
        {
            stageMgr.playerControll.player.isInvincible = !stageMgr.playerControll.player.isInvincible;
            StaticManager.UI.MessageUI.PopupMessage("Invincible Toggle:" + stageMgr.playerControll.player.isInvincible.ToString());
        }
        public void ToggleShoot()
        {
            stageMgr.playerControll.player.isShooting = !stageMgr.playerControll.player.isShooting;
            StaticManager.UI.MessageUI.PopupMessage("Shooting Toggle:" + stageMgr.playerControll.player.isShooting.ToString());
        }

        public void PlayerReset()
        {
            stageMgr.playerControll.player.LevelReset();


            stageMgr.playerControll.player.isDie = false;
            stageMgr.playerControll.player.isInvincible = false;

            StaticManager.UI.MessageUI.PopupMessage("Player setting reset");
        }
        #endregion

        #region Enemy Buttons
        public void StageLeftButton()
        {
            if (stageMgr.stageNum <=1)
            {
                Debug.Log("Minimum stage number!");
                return;
            }
            ChangeStageNum(stageMgr.stageNum - 1);
        }
        public void StageRightButton()
        {
            if (stageMgr.stageNum >= GameManager.Instance.dataMgr.list_stageData.Count)
            {
                Debug.Log("Maximum stage number!");
                return;
            }
            ChangeStageNum(stageMgr.stageNum + 1);
        }

        /// <summary>
        /// 3/14/2023-LYI
        /// 스테이지값 변경
        /// </summary>
        /// <param name="num"></param>
        public void ChangeStageNum(int num)
        {
            stage_txt_num.text =  num.ToString();

            stageMgr.SetStage(num);
            ChangeEnemySummonPalette(num);
        }

        /// <summary>
        /// 3/14/2023-LYI
        /// 적 캐릭터 팔레트 변경
        /// 각 스테이지 정보에 따라 생성
        /// 현재 더미데이터
        /// </summary>
        /// <param name="num"></param>
        public void ChangeEnemySummonPalette(int num)
        {
            int enemyCount = stageMgr.enemySpawner.list_originEnemyPrefab.Count;
            int bossCount = stageMgr.enemySpawner.list_originBossPrefab.Count;

            list_enemy.Clear();
            list_boss.Clear();

            list_enemy = stageMgr.enemySpawner.list_originEnemyPrefab;
            list_boss = stageMgr.enemySpawner.list_originBossPrefab;

            for (int i = 0; i < enemyCount; i++)
            {
                //버튼 생성
                //버튼에 각 프리팹 생성 기능 할당
                SummonEnemy(list_enemy[enemyCount]);
            }
            for (int i = 0; i < bossCount; i++)
            {
                //버튼 생성
                //버튼에 각 프리팹 생성 기능 할당
                SummonEnemy(list_boss[bossCount]);
            }
        }


        /// <summary>
        /// 3/14/2023-LYI
        /// 룸에서 실행?
        /// 해당 적 캐릭터 정해진 위치
        /// 혹은 포지션 랜덤위치
        /// 혹은 포지션 중 남은 위치
        /// 혹은 마우스 클릭 위치
        /// </summary>
        /// <param name="enemy"></param>
        public void SummonEnemy(Enemy enemy)
        {
           Enemy e=  stageMgr.enemySpawner.SpawnEnemy(
               enemy, roomPractice.arr_spawnPos[Random.Range(0,roomPractice.arr_spawnPos.Length)].position);
            e.isDropable = false;
        }

        /// <summary>
        /// 4/3/2023-LYI
        /// Enemy Reset, All Enemy Die
        /// </summary>
        public void EnemyReset()
        {
            Enemy[] arr_enemy = new Enemy[stageMgr.enemySpawner.list_activeEnemy.Count];
            stageMgr.enemySpawner.list_activeEnemy.CopyTo(arr_enemy);

            for (int i = 0; i < arr_enemy.Length; i++)
            {
                arr_enemy[i].EnemyDie();
            }

            StaticManager.UI.MessageUI.PopupMessage("Enemy reset");
        }

        #endregion


    }
}