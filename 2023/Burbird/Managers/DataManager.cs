using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!중요!!!!!!!!!!!!!!!!!!
    /// 유저 데이터 관리 클래스
    /// 구매 정보등 개인 데이터 관리
    /// Json 파일로 서버 통신
    /// 유저 레벨, 캐릭터 스테이터스, 장비 등 중요 데이터 가지고 있음
    /// 어드레서블 에셋 관리
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        GameManager gameMgr;

        public Status playerStat = new Status();    //플레이어 스탯, 서버에서 데이터 로드, 스테이지 내 캐릭터 생성 시 전달
        public Status characterStat = new Status();     //캐릭터 스탯
        public Status equipStat = new Status();     //장비 스탯
        public Status abilityStat = new Status(); //업그레이드 스탯

        public List<StageData> list_stageData = new List<StageData>(); //각 스테이지의 데이터들
        public Dictionary<int, List<object>> dic_list_enemyStat = new (); //적 캐릭터 스탯들

        //플레이어 주요 데이터
        public int PlayerLevel; //계정 레벨
        public float PlayerEXP; //계정 경험치
        float maxEXP; //최대 경험치

        public int Stemina; //게임 입장 시 소모
        public int MaxStemina;
        public int Coin; //인 게임 재화
        public int Diamond; //유료재화

        //상점 구매이력
        private void Awake()
        {
            gameMgr = GameManager.Instance;

        }

        public void DataManagerInit()
        {
            SetStageData();
            LoadEnemyStats();
        }

        /// <summary>
        /// Stage Data Setting(Once at Start)
        /// </summary>
        public void SetStageData()
        {
            for (int i = 0; i < gameMgr.addressMgr.dic_jsonStageData.Count; i++)
            {
                StageData data = new StageData();
                list_stageData.Add(data.SetStageData(i + 1));
            }

            //After Load Stage Data
            int stageDataNum = ES3.Load("CurrentStageNum", 1);

            gameMgr.playStageData = list_stageData[stageDataNum - 1];
        }

        public void LoadEnemyStats()
        {
            dic_list_enemyStat = gameMgr.csvLoader.ReadCSVDataDic("EnemyChart");
        }


        public void GetPlayerEXP(int exp)
        {
            PlayerEXP += exp;

            if (PlayerEXP >= maxEXP)
            {
                PlayerLevelUp();
            }

        }

        public void PlayerLevelUp()
        {
            PlayerLevel++;
            gameMgr.uiMgr.ui_world.TextChangePlayerLevel(PlayerLevel.ToString());
        }

        #region Stamina,Gold,Diamond


        /// <summary>
        /// 스테미나 획득
        /// </summary>
        /// <param name="value">획득량</param>
        public void GetStemina(int value)
        {
            Stemina += value;
            if (Stemina >= MaxStemina)
            {
                //자동회복 멈추기
            }
            SaveMainGoods();
        }
        public void RecoverStemina(int value)
        {
            Stemina += value;
            if (Stemina >= MaxStemina)
            {

                Stemina = MaxStemina;
                //회복 멈추기
            }
            SaveMainGoods();
        }

        /// <summary>
        /// 스테미나 소모
        /// </summary>
        /// <param name="value">소모량</param>
        public void UseStemina(int value)
        {
            //현재 스테미나가 부족할 경우
            if (Stemina < value)
            {
                //스테미나 부족 경고, 리턴
                StaticManager.UI.MessageUI.PopupMessage("스테미나가 부족합니다");

                // 스테미나 회복창 띄우기(광고, 결제)
                return;
            }

            Stemina -= value;
            if (Stemina < MaxStemina)
            {
                //자동회복 시작
            }
            SaveMainGoods();
        }


        /// <summary>
        /// 코인 획득
        /// </summary>
        /// <param name="value">획득량</param>
        public void GetCoin(int value)
        {
            Coin += value;

            SaveMainGoods();
        }

        /// <summary>
        /// 코인 사용
        /// </summary>
        /// <param name="value">소모량</param>
        public void UseCoin(int value)
        {
            if (Coin < value)
            {
                //코인 부족 경고, 리턴
                StaticManager.UI.MessageUI.PopupMessage("코인이 부족합니다");
                return;
            }

            Coin -= value;
            SaveMainGoods();
        }


        /// <summary>
        /// 다이아몬드 획득
        /// </summary>
        /// <param name="value">획득량</param>
        public void GetDiamond(int value)
        {
            Diamond += value;
            
            SaveMainGoods();
        }

        /// <summary>
        /// 다이아 사용
        /// </summary>
        /// <param name="value">소모량</param>
        public void UseDiamond(int value)
        {
            if (Diamond < value)
            {
                //다이아 부족 경고, 리턴
                StaticManager.UI.MessageUI.PopupMessage("다이아가 부족합니다");
                return;
            }

            Diamond -= value;

            SaveMainGoods();

        }

        #endregion

        /// <summary>
        /// 로컬 유저 데이터  세팅
        /// 서버에서 캐릭터 능력치 불러오기
        /// </summary>
        public void LocalLoadPlayerStat()
        {
            characterStat.ATKDamage = 150;
            characterStat.ATKSpeed = 0.5f;
            characterStat.maxHp = 600;
            characterStat.shotSpeed = 1;

            characterStat.critChance = 0;
            characterStat.critDamage = 1;
            characterStat.avoidChance = 0;

            MaxStemina = 20;

            //Stemina = MaxStemina;
            //Coin = 1000;
            //Diamond = 100;
            LoadMainGoods();

            RefreshAllPlayerStatus();
        }

        /// <summary>
        /// 서버를 안쓰고 플레이어 데이터 저장
        /// 재화, 플레이어 정보, 캐릭터, 인벤토리, 업그레이드, 상점
        /// </summary>
        public void SaveLocalPlayerData()
        {
            Debug.Log("Save Local Player Data()");
            if (gameMgr.uiMgr != null)
            {
                SaveMainGoods();
            }
            ES3.Save("PlayerLevel", PlayerLevel);
            ES3.Save("PlayerEXP", PlayerEXP);
        }
        public void LoadLocalPlayerData()
        {
            Debug.Log("Load Local Player Data()");
            LoadMainGoods();
            PlayerLevel = ES3.Load("PlayerLevel", 1);
            PlayerEXP = ES3.Load("PlayerEXP", 0);
        }


        /// <summary>
        /// 재화 저장, 게임 종료시나 보상획득, 재화 사용시 호출
        /// </summary>
        public void SaveMainGoods()
        {
            if (gameMgr.uiMgr != null)
            {
                gameMgr.uiMgr.ui_main.RefreshMainUI();
            }

            try
            {
                ES3.Save("Stemina", Stemina);
                ES3.Save("Coin", Coin);
                ES3.Save("Diamond", Diamond);
            }
            catch (System.IO.IOException)
            {

                StaticManager.UI.MessageUI.PopupMessage("The file is open elsewhere or there was not enough storage space");
            }
            catch (System.Security.SecurityException)
            {
                StaticManager.UI.MessageUI.PopupMessage("You do not have the required permissions");
            }
            catch (System.FormatException)
            {
                StaticManager.UI.MessageUI.PopupMessage("The data has been modified or corrupted so is no longer valid");
            }
            catch (System.ArgumentException)
            {
                StaticManager.UI.MessageUI.PopupMessage("Accessing unencrypted data with encryption enabled");
            }
        }

        /// <summary>
        /// 재화 불러오기
        /// </summary>
        void LoadMainGoods()
        {
            Stemina = ES3.Load("Stemina", MaxStemina);
            Coin = ES3.Load("Coin", 0);
            Diamond = ES3.Load("Diamond", 0);
        }


        /// <summary>
        /// 4/10/2023-LYI
        /// 플레이어 스테이터스 갱신
        /// 캐릭터 스탯(종류, 업그레이드), 장비 스탯 총합, 특성 강화에 따른 스탯 값
        /// 이후 각종 %연산 부가효과 적용 공격력 % 증가 등
        /// </summary>
        public void RefreshAllPlayerStatus()
        {
            Debug.Log("Refresh player status");

            playerStat = characterStat + equipStat + abilityStat;
        }


        #region ServerWork

        /// <summary>
        /// 서버로 유저 데이터 저장하기
        /// </summary>
        void SendUserData()
        {
            Status pStat = new Status();

        }

        /// <summary>
        /// 서버에서 유저 데이터 불러오기
        /// </summary>
        void GetUserData()
        {

        }

        void SyncUserData()
        {

        }

        /// <summary>
        /// 현재 데이터 상태와 서버의 데이터 상태 대조
        /// 비 정상적인 수치가 있을 경우 해킹으로 간주
        /// </summary>
        void CheckLocalData()
        {

        }

        void CheckDataHacked()
        {

        }

        #endregion

    }
}