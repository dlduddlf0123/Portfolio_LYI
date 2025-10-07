using System.Collections;
using System.Collections.Generic;

using BackEnd;
using BackendData.Base;

using LitJson;

using System;
using System.Reflection;
using System.Transactions;

using Facebook.Unity;
using UnityEngine;

namespace Burbird
{

    /// <summary>
    /// 뒤끝서버에서 유저 데이터 관련 차트 등 관련
    /// </summary>
    public class Backend_UserDataManager : MonoBehaviour
    {
        //뒤끝 콘솔에 업로드한 차트 데이터만 모아놓은 클래스
        public class Backend_Chart
        {
            public readonly BackendData.Chart.AllChart ChartInfo = new(); // 모든 차트
            public readonly BackendData.Chart.Weapon.Manager Weapon = new(); // Weapon 차트
            public readonly BackendData.Chart.Enemy.Manager Enemy = new(); // enemyChart 차트
            public readonly BackendData.Chart.Stage.Manager Stage = new(); // Stage 차트
            public readonly BackendData.Chart.Item.Manager Item = new(); // 아이템 차트
            public readonly BackendData.Chart.Shop.Manager Shop = new(); // 샵 차트
            public readonly BackendData.Chart.Quest.Manager Quest = new(); // 퀘스트 차트
        }

        // 게임 정보 관리 데이터만 모아놓은 클래스
        public class Backend_GameData
        {
            public readonly BackendData.GameData.Inventory.Manager WeaponInventory = new(); // WeaponInventory 테이블 데이터
            public readonly BackendData.GameData.Equipment.Manager WeaponEquip = new(); // WeaponEquip 테이블 데이터
            public readonly BackendData.GameData.QuestAchievement.Manager QuestAchievement = new(); // QuestAchievement 테이블 데이터
            public readonly BackendData.GameData.UserData UserData = new(); // UserData 테이블 데이터
            public readonly BackendData.GameData.ItemInventory ItemInventory = new(); // ItemInventory 테이블 데이터

            public readonly Dictionary<string, BackendData.Base.GameData>
                GameDataList = new Dictionary<string, GameData>();

            public Backend_GameData()
            {
                GameDataList.Add("내 무기 정보", WeaponInventory);
                GameDataList.Add("내 무기 장비", WeaponEquip);
                GameDataList.Add("내 퀘스트 정보", QuestAchievement);
                GameDataList.Add("내 유저 정보", UserData);
                GameDataList.Add("내 아이템 정보", ItemInventory);
            }
        }

        /// <summary>
        /// 유저 로그인 정보를 모은 클래스
        /// </summary>
        public class Backend_UserInfo
        {
            public string gamerId;
            public string countryCode;
            public string nickname;
            public string inDate;
            public string emailForFindPassword;
            public string subscriptionType;
            public string federationId;

            public Backend_UserInfo()
            {

            }

            public Backend_UserInfo(JsonData callback)
            {
                gamerId = callback["gamerId"].ToStringNullOk();
                countryCode = callback["countryCode"].ToStringNullOk();
                nickname = callback["nickname"].ToStringNullOk();
                inDate = callback["inDate"].ToStringNullOk();
                emailForFindPassword = callback["emailForFindPassword"].ToStringNullOk();
                subscriptionType = callback["subscriptionType"].ToStringNullOk();
                federationId = callback["federationId"].ToStringNullOk();
            }
        }

        public Backend_Chart Chart = new(); // 차트 모음 클래스 생성
        public Backend_GameData GameData = new(); // 게임 모음 클래스 생성
        public Backend_UserInfo UserInfo = new();
        public BackendData.Post.Manager Post = new(); // 우편 클래스 생성

        // Start is called before the first frame update
        void Start()
        {

        }

        // 뒤끝 매니저 초기화 함수
        public void Init()
        {
            var initializeBro = Backend.Initialize(true);

            // 초기화 성공시
            if (initializeBro.IsSuccess())
            {
                Debug.Log("뒤끝 초기화가 완료되었습니다.");
                CreateSendQueueMgr();
                SetErrorHandler();
            }
            //초기화 실패시
            else
            {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), initializeBro.ToString());
            }
        }

        // 모든 뒤끝 함수에서 에러 발생 시, 각 에러에 따라 호출해주는 핸들러
        private void SetErrorHandler()
        {
            Backend.ErrorHandler.InitializePoll(true);

            // 서버 점검 에러 발생 시
            Backend.ErrorHandler.OnMaintenanceError = () => {
                Debug.Log("점검 에러 발생!!!");
                StaticManager.UI.AlertUI.OpenErrorUIWithText("서버 점검 중", "현재 서버 점검중입니다.\n타이틀로 돌아갑니다.");
            };
            // 403 에러 발생시
            Backend.ErrorHandler.OnTooManyRequestError = () => {
                StaticManager.UI.AlertUI.OpenErrorUIWithText("비정상적인 행동 감지", "비정상적인 행동이 감지되었습니다.\n타이틀로 돌아갑니다.");
            };
            // 액세스토큰 만료 후 리프레시 토큰 실패 시
            Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () => {
                StaticManager.UI.AlertUI.OpenErrorUIWithText("다른 기기 접속 감지", "다른 기기에서 로그인이 감지되었습니다.\n타이틀로 돌아갑니다.");
            };
        }

        // 로딩씬에서 할당할 뒤끝 정보 클래스 초기화
        public void InitInGameData()
        {

            Chart = new();
            GameData = new();
            Post = new();
        }

        //SendQueue를 관리해주는 SendQueue 매니저 생성
        private void CreateSendQueueMgr()
        {
            var obj = new GameObject();
            obj.name = "SendQueueMgr";
            obj.transform.SetParent(this.transform);
            obj.AddComponent<SendQueueMgr>();
        }
    }
}