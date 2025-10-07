using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

namespace Burbird
{
    /// <summary>
    /// 스테이지 진행 상태 체크
    /// </summary>
    public enum StageStat
    {
        NONE = 0,
        START,
        FIGHT,
        END,
        REST,
        EVENT,
        RESULT,
    }


    /// <summary>
    /// Play Scene에서 매니저 클래스
    /// 플레이어, 적 캐릭터 등 전투 관련 데이터 담당
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        public SoundManager soundMgr;

        public PlayerController2D playerControll;
        public PerkChecker perkChecker;

        public EnemySpawner enemySpawner;
        public CoinSpawner coinSpawner;
        public HeartSpawner heartSpawner;
        public ItemSpawner itemSpawner;

        public UIGame ui_game;
        public Inventory stageInven;

        public StageData currentStageData;

        //스테이지 속성, 스테이지 번호에 따라 데이터 불러오기
        public int stageNum = 1;
        public string stageName = "Stage";
        public int clearRoom = 0;
        public int maxRoom = 50;
        public int steminaForPlay = 5; //플레이에 필요한 스테미나
        public float bonusHPPercent = 0.2f; //스테이지, 룸 진행에 따라 올라갈 스탯 값
        public float bonusDMGPercent = 0.5f; //스테이지진행에 따라 올라갈 스탯 값

        /// <summary>
        /// 이 스테이지에서 진행 할 방 목록
        /// </summary>
        public List<Room> list_roomSource = new List<Room>();
        public List<Room> list_bossRoom = new List<Room>(); //보스 방
        public RestRoom restRoom; //휴식 방

        /// <summary>
        /// 이 스테이지에서 등장 할 수 있는 퍽 목록
        /// </summary>
        public List<Perk> list_perk_pool = new List<Perk>();

        public StageStat statStage = StageStat.NONE;

        public Room currentRoom; //현재 방 정보

        //카메라 바운드 세팅을 위한 가상 카메라 정보
        public Cinemachine.CinemachineConfiner2D camConfine2D;
        public Transform trPlayerPerk { get; set; }

        public AudioClip sfx_getCoin;

        public bool isEnemyDebug = false;


        //현재 정보
        public int currentRoomNum = 0;

        //재화 관련
        public int stageCoin = 0;           //게임에서 드랍되는 돈
        public int stageDiamond = 0;    //게임에서 드랍되는 다이아
        public int stagePlayerExp = 0;   //게임 종료 후 획득할 유저 경험치
                                     //재화는 게임 종료 시 데이터에 연동, 서버와 항상 소통할 것

        private static StageManager s_instance = null;
        public static StageManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType(typeof(StageManager)) as StageManager;
                }
                return s_instance;
            }
        }

        void Awake()
        {
            trPlayerPerk = transform.GetChild(1);
            soundMgr = GetComponent<SoundManager>();
            perkChecker = GetComponent<PerkChecker>();
        }

        private void Start()
        {
            // GameStart();
        }

        #region Get Coin, Diamond, Items
        /// <summary>
        /// 코인 획득
        /// </summary>
        public void GetCoin(int coin)
        {
            stageCoin += coin;
            ui_game.ChangeCoinText(stageCoin);
            soundMgr.PlaySfx(playerControll.centerTr, sfx_getCoin, Random.Range(0.7f, 1.4f));
        }

        /// <summary>
        /// 다이아몬드 획득
        /// </summary>
        public void GetDiamond(int diamond)
        {
            stageDiamond += diamond;

        }

        /// <summary>
        /// 장비, 소재등 현재 게임 진행 외 아이템 획득
        /// 리스트에 아이템 추가?
        /// </summary>
        public void GetItem(InventoryItem item, int quantity)
        {
            stageInven.AddItem(item, quantity);

        }
        #endregion

        #region Stage Start Logic

        /// <summary>
        /// 스테이지 생성 시작
        /// 스테이지 내부 로딩
        /// </summary>
        /// <returns></returns>
        public IEnumerator StageLoadLogic()
        {
            Debug.Log("StageLoadLogic()");
            WaitForSeconds wait = new WaitForSeconds(0.1f);

            ui_game.canvas_loading.gameObject.SetActive(true);
            GameManager.Instance.loader.fade.FadeIn();

            GameManager.Instance.dataMgr.SetStageData();
            //오디오 변경
            soundMgr.ChangeBGMAudioSource(GetComponent<AudioSource>());

            yield return wait;

            LoadPlayerData();
            yield return wait;

            yield return StartCoroutine(LoadRoomData(stageNum));

            currentRoomNum = 0;
            currentRoom = Instantiate(restRoom);
            yield return wait;

            ui_game.canvas_loading.gameObject.SetActive(false);
            ui_game.GameUIInit();

            //페이드 후 전투 시작
            GameManager.Instance.loader.fade.FadeOut(RoomStart); 
        }

        /// <summary>
        /// 3/15/2023-LYI
        /// 연습모드 실행 시 호출
        /// </summary>
        /// <returns></returns>
        public IEnumerator PracticeLoadLogic()
        {
            Debug.Log("StageLoadLogic()");
            WaitForSeconds wait = new WaitForSeconds(0.1f);

            ui_game.canvas_loading.gameObject.SetActive(true);
            GameManager.Instance.loader.fade.FadeIn();

            GameManager.Instance.dataMgr.SetStageData();
            //오디오 변경
            soundMgr.ChangeBGMAudioSource(GetComponent<AudioSource>());

            yield return wait;

            LoadPlayerData();
            yield return wait;

            yield return StartCoroutine(LoadRoomData(stageNum));

            currentRoomNum = 0;
            currentRoom = GameObject.FindObjectOfType<Room>();
           // currentRoom = Instantiate(restRoom);
            yield return wait;

            ui_game.canvas_loading.gameObject.SetActive(false);
            ui_game.GameUIInit();

            //페이드 후 전투 시작
            GameManager.Instance.loader.fade.FadeOut(RoomStart);
        }



        /// <summary>
        /// 플레이어 캐릭터 장비 등 스탯을 불러와서 플레이어 캐릭터 생성
        /// </summary>
        public void LoadPlayerData()
        {
            playerControll.player.playerStatus = GameManager.Instance.dataMgr.playerStat;
            playerControll.player.Init();
        }

        public void StageManagerInit(StageData stage)
        {
            currentStageData = stage;

            stageNum = stage.stageNum;
            stageName = stage.stageName;

            clearRoom = 0;
            maxRoom = stage.maxRoom;

            enemySpawner.list_originEnemyPrefab = stage.list_enemy;
            // enemySpawner.list_originBossPrefab = stage.list_bossRoom; //보스 프리팹은 보스 방과 1:1 대응, 보스마다 전용 보스방 -> 보스방마다 맞는 보스 프리팹 소유

            list_perk_pool.Clear();
            list_perk_pool.AddRange(stage.list_perk);
            itemSpawner.list_originItem= stage.list_dropItem;
        }

        /// <summary> 
        /// 23-03-14 LYI
        /// 해당 번호에 따른 스테이지 데이터 호출
        /// 적 캐릭터 스탯의 기준이 됨
        /// </summary>
        /// <param name="num">stage number want to change</param>
        public void SetStage(int num)
        {
            //GetStageData(GameManager.Instance.dataMgr.list_stageData[num]);

        }

        /// <summary>
        /// 룸 데이터 불러오기
        /// </summary>
        public IEnumerator LoadRoomData(int loadStageNum = 1)
        {
            Debug.Log("LoadRoomData()");
            Dictionary<string, GameObject> dic_roomPrefab = new Dictionary<string, GameObject>();

            AddressableManager addressMgr = GameManager.Instance.addressMgr;

            //addressable 에셋 로드 요청, 완료 시 까지 대기
            yield return StartCoroutine(addressMgr.LoadRoomAssets(loadStageNum));

            //데이터 적용
            dic_roomPrefab = addressMgr.GetRoomData();

            
            //Debug.Log("Dictionary_RoomPrefab/Count:" + dic_roomPrefab.Count);
            //foreach (var item in dic_roomPrefab)
            //{
            //    Debug.Log("Dictionary_RoomPrefab/Key:" + item.Key + "/Value:" + item.Value);
            //}

            //각 데이터 할당
            restRoom = dic_roomPrefab["RoomRest"].GetComponent<RestRoom>();

            //스테이지마다 휴식은 하나, 보스룸과 기본룸은 여러개 존재 할 수 있음
            dic_roomPrefab.Remove("RoomRest");


            foreach (KeyValuePair<string, GameObject> room in dic_roomPrefab)
            {
                //보스룸은 따로 저장, 나머지는 랜덤 룸으로 저장
                if (room.Key.Contains("Boss"))
                {
                    list_bossRoom.Add(dic_roomPrefab[room.Key].GetComponent<Room>());
                }
                else
                {
                    list_roomSource.Add(dic_roomPrefab[room.Key].GetComponent<Room>());
                }
            }

            //번호순 정렬
            list_roomSource.Sort((Room c, Room d) => { return RoomSort(c, d); });

        }


        /// <summary>
        /// 각 룸 이름 떼고 숫자 비교
        /// </summary>
        int RoomSort(Room a, Room b)
        {
            string name1 = a.name;
            string name2 = b.name;
            name1 = name1.Substring(5);
            name2 = name2.Substring(5);

            int c =  int.Parse(name1);
            int d = int.Parse(name2);

            return c.CompareTo(d);
        }

        #endregion

        #region Room Life Cycle
        /// <summary>
        /// 방이 시작될 때 호출
        /// 몹스폰
        /// </summary>
        public void RoomStart()
        {
            Debug.Log("Room Start");
            statStage = StageStat.START;

            //enemySpawner.SpawnRandomEnemyWithPoints();

            currentRoom.RoomStart();

           // currentRoom.arr_roomDoor[0].DoorClose();
           // enemySpawner.SpawnEnemyToPoints();
        }


        /// <summary>
        /// 방 전투 종료 시 호출
        /// 경험치 정산, 다음 스테이지 문 개방
        /// </summary>
        public void RoomEnd()
        {
            if (GameManager.Instance.statGame == SceneStatus.PRACTICE)
            {
                return;
            }
            Debug.Log("Room End");
            statStage = StageStat.END;

            //모든 총알 지우기
            enemySpawner.projectileMgr.ResetAllMissile();

            //코인 불러모으기 작동
            coinSpawner.AbsorbCoin();
            itemSpawner.AbsorbItem();

            currentRoom.RoomEnd();

            clearRoom = currentRoomNum;

            soundMgr.bgmSource.Stop();
        }


        /// <summary>
        /// 다음 방으로 이동 시 호출
        /// 방 생성
        /// </summary>
        public void RoomNext()
        {
            Debug.Log("Room Next");

            //클리어 체크
            if (clearRoom >= maxRoom)
            {
                StageResult(true);
            }
            else
            {
                GameManager.Instance.loader.fade.StartFade(SetNextRoom);
            }
        }

        /// <summary>
        /// Fade 효과 중에 처리할 것
        /// </summary>
        void SetNextRoom()
        {
            //현재 방 비활성화
            currentRoom.RoomDisable();
            currentRoomNum++;

            playerControll.shooter.AllFeatherInit();
            //coinSpawner.AbsorbCoin

            RoomInstantiate();

            //방 활성화
            RoomStart();
        }

        /// <summary>
        /// 방 번호에 따른 방 생성
        /// </summary>
        void RoomInstantiate()
        {
            /*방을 랜덤 생성 돌릴 때 방법, 지금은 정해진 순서대로 생성방식으로 변경되서 사용되지 않음
            //다음 방 생성
            //if (currentRoomNum % 10 == 0)
            //{
            //    //10스테이지 마다 보스방
            //    currentRoom = Instantiate(list_bossRoom[currentRoomNum / 10]);
            //}
            //else if (currentRoomNum % 5 == 0)
            //{
            //    //5스테이지 마다 휴식방
            //    currentRoom = Instantiate(restRoom);
            //}
            //else
            //{
            //    //일반 방
            //    currentRoom = Instantiate(list_roomSource[0]);
            //    list_roomSource.Remove(list_roomSource[0]);
            //}*/

            currentRoom = Instantiate(list_roomSource[0]);
            list_roomSource.Remove(list_roomSource[0]);

        }
        #endregion

        #region When Stage End
        /// <summary>
        /// 스테이지 사망 시 결과창
        /// </summary>
        public void StageResult(bool isClear = false)
        {
            Debug.Log("StageResult()");
            ui_game.ActiveResult(); //결과창 활성화

            if (isClear)
            {
                //클리어 시

            }
            else
            {
                //실패 시

            }
            //인벤토리 관련 처리?
        }


        /// <summary>
        /// 결과창 표시 후 메뉴로 나갈 때 호출
        /// 스테이지 종료 시 획득할 자원 정산
        /// 데이터 매니저로 데이터 이동, 저장
        /// </summary>
        public void SavePlayerData()
        {
            DataManager dataMgr = GameManager.Instance.dataMgr;
            Inventory main = GameManager.Instance.invenChecker.mainInven;

            for (int invenIndex = 0; invenIndex < stageInven.Content.Length; invenIndex++)
            {
                if (InventoryItem.IsNull(stageInven.Content[invenIndex]))
                {
                    continue;
                }
                switch (stageInven.Content[invenIndex].ItemName)
                {
                    case "Coin": //돈
                        dataMgr.GetCoin(stageCoin);
                        break;
                    case "Diamond": //보석
                        dataMgr.GetDiamond(stageDiamond);
                        break;
                    case "Exp": //유저 경험치
                        dataMgr.GetPlayerEXP(stagePlayerExp);
                        break;
                    default:
                        stageInven.MoveItemToInventory(invenIndex, main);
                        break;
                }

            }

            //데이터 이동 후 인벤토리 저장
            GameManager.Instance.invenChecker.mainInven.SaveInventory();

            StageEnd();
        }


        /// <summary>
        /// 스테이지 종료 처리, 메인 씬으로 이동
        /// 홈버튼 클릭 시 이동
        /// </summary>
        public void StageEnd()
        {
            GameManager.Instance.loader.LoadScene((int)SceneStatus.MAIN,
               ()=> GameManager.Instance.OnGameEnd());
        }

        #endregion


        /// <summary>
        /// 랜덤 퍽 골라주기
        /// </summary>
        /// <returns></returns>
        public Perk PickRandomPerk()
        {
            Perk p;
            p = list_perk_pool[Random.Range(0, list_perk_pool.Count)];
            return p;       
        }

        /// <summary>
        /// 랜덤 퍽 배열 골라주기, 중복 제거
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public Perk[] PickRandomPerks(int range)
        {
            Perk[] arr_p = new Perk[range];
            List<Perk> pickedPerks = new List<Perk>();

            for (int i = 0; i < arr_p.Length; i++)
            {
                arr_p[i] = list_perk_pool[Random.Range(0, list_perk_pool.Count)];
                if (i > 0)
                {
                    while (pickedPerks.Contains(arr_p[i]))
                    {
                        arr_p[i] = list_perk_pool[Random.Range(0, list_perk_pool.Count)];
                    }
                }

                pickedPerks.Add(arr_p[i]);
            }

            return arr_p;
        }
    }
}
