using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using BackEnd;

namespace Burbird
{

    /// <summary>
    /// 게임 내 시간, 재 접속까지의 시간 관련 체크 클래스
    /// 방치형 보상, 스테미나 회복, 이벤트 시간 등 체크에 사용
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public int recoverMinute = 10;

        public DateTime currentLoginTime; //이번 로그인 된 시간
        public DateTime lastLoginTime; //이번 이전 마지막 로그인 한 시간

        // Start is called before the first frame update
        void Start()
        {
            RecoverSteminaOnLogin();
        }

        /// <summary>
        /// 첫 게임 실행 시 호출할 것
        /// 현재 시간 - 최종 접속 시간 / 20분 만큼 스테미나 회복
        /// 호출 전에 현재 스테미나량 체크 
        /// </summary>
        public void RecoverSteminaOnLogin()
        {
            int recoverStemina = 0;

            //회복량은 시간 차에 범위만큼
            DateTime lastTime = LoadLastTime();
            DateTime currentTime = CheckWebTime();

            //현재 게임에서 사용될 수 있는 전역변수 초기화
            currentLoginTime = currentTime;
            lastLoginTime = lastTime;

            TimeSpan timeSpan = currentTime - lastTime;

            int spentMinute =  (int)timeSpan.Minutes;

            int spentSecond = (int)timeSpan.Seconds;

            recoverStemina = spentMinute / recoverMinute;

            StartCoroutine(WaitForLoad(() =>
            GameManager.Instance.dataMgr.RecoverStemina(recoverStemina)));
        }

        IEnumerator WaitForLoad(UnityEngine.Events.UnityAction action)
        {
            while (!GameManager.Instance.addressMgr.isLoadComplete)
            {
                yield return new WaitForSeconds(1f);
            }

            action.Invoke();
        }


        /// <summary>
        /// 게임 종료, 로그아웃 시 시간 저장
        /// </summary>
        public void SaveLastTime()
        {
            //현재 시간 저장
            DateTime gameOpenTime = CheckWebTime();

            string save = gameOpenTime.ToString("yyyy:MM:dd:HH:mm:ss");
            PlayerPrefs.SetString("LogoutTime", save);

            //총 플레이 타임에 이번 플레이 시간 추가 저장
            float playTime = PlayerPrefs.GetFloat("WholePlayTime");
            playTime += Time.realtimeSinceStartup;
            PlayerPrefs.SetFloat("WholePlayTime", playTime);
        }

        public DateTime LoadLastTime()
        {
            string logoutTime =  PlayerPrefs.GetString("LogoutTime", "1111:01:01:01:01:01");
            DateTime resert = StringToDateTime(logoutTime);
            return resert;
        }

        /// <summary>
        /// DateTime 저장 시 string으로 저장, 불러올 때 string을 DateTime으로 변환
        /// </summary>
        /// <param name="dateTime">DateTime을 다음 형식으로 저장할 것 "yyyy:MM:dd:HH:mm:ss"</param>
        /// <returns></returns>
        public DateTime StringToDateTime(string dateTime)
        {
            DateTime resert;

            string[] time = dateTime.Split(":");
            int y, M, d, h, m, s;
            y = Convert.ToInt32(time[0]);
            M = Convert.ToInt32(time[1]);
            d = Convert.ToInt32(time[2]);
            h = Convert.ToInt32(time[3]);
            m = Convert.ToInt32(time[4]);
            s = Convert.ToInt32(time[5]);

            resert = new DateTime(y, M, d, h, m, s);
            return resert;
        }


        /// <summary>
        /// 뒤끝서버에서 현재 시간 불러오기(동기)
        /// </summary>
        /// <returns></returns>
        public DateTime CheckWebTime()
        {
            BackendReturnObject servertime = Backend.Utils.GetServerTime();

            string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
            DateTime parsedDate = DateTime.Parse(time);
            return parsedDate;
        }


    }
}