using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using VRTokTok.Character;
using VRTokTok.Manager;
using System;

namespace VRTokTok.UI
{

    /// <summary>
    /// 10/24/2023-LYI
    /// 스테이지 선택 스크롤
    /// 시작 때 부터 보이고 메인 화면으로 올 때 보인다
    /// </summary>
    public class UI_Select : MonoBehaviour
    {
        GameManager gameMgr;

        public SelectChecker[] arr_checker;

        public GameObject scroll_content;

        public List<List<SelectSphere>> list__selectSpheres = new();


        void Start()
        {
            SelectInit();
        }




        /// <summary>
        /// 10/25/2023-LYI
        /// 각 구에 스테이지 데이터 할당
        /// 클리어 여부 표시 변경용
        /// </summary>
        public void SelectInit()
        {
            if (!this.gameObject.activeSelf)
            {
                return;
            }
             
            #region ForEvent
           /* List<int> list_episodeNum = new List<int>();
            int episodeCount = 0;

            for (int gameTypeCount = 0; gameTypeCount < scroll_content.transform.childCount; gameTypeCount++)
            {
                int typeNum = Convert.ToInt32(scroll_content.transform.GetChild(gameTypeCount).name);
                list_episodeNum.Add(typeNum);
            }

            //배열 선언, 0 초기화
            int[] arr_episodeStageNum = new int[list_episodeNum.Count];
            for (int i = 0; i < arr_episodeStageNum.Length; i++)
            {
                arr_episodeStageNum[i] = 1;
            }

            //각 주제에 따른 반복문
            for (int gameTypeCount = 0; gameTypeCount < scroll_content.transform.childCount; gameTypeCount++)
            {
                //리스트 형태 선언
                List<SelectSphere> selectSpheres = new List<SelectSphere>();

                //번호 따기 x000
                //int typeNum = Convert.ToInt32(scroll_content.transform.GetChild(gameTypeCount).name);

                //잠금 체크용
                int lastClearStage = 0;
                scroll_content.transform.GetChild(gameTypeCount).localPosition = Vector3.zero + Vector3.right * 1.3f * gameTypeCount;

                //각 스피어 할당을 위한 반복문
                for (int sphereNum = 1; sphereNum < scroll_content.transform.GetChild(gameTypeCount).childCount; sphereNum++)
                {
                    //스피어 할당
                    SelectSphere sphere = scroll_content.transform.
                        GetChild(gameTypeCount).GetChild(sphereNum).GetComponent<SelectSphere>();


                    //주제번호 x + 스테이지 반복 번호 xxx
                    int stageNum = list_episodeNum[episodeCount] + arr_episodeStageNum[episodeCount]; //할당할 스테이지 번호
                  

                    //해당 스테이지번호의 클리어 정보 호출
                    bool isClear = ES3.Load<bool>(stageNum.ToString(), false);

                    //해당 스피어의 번호 할당 및 클리어 여부 변경 및 외형 변경
                    sphere.stageNum = stageNum;
                    sphere.isClear = isClear;

                    if (lastClearStage + 1 >= arr_episodeStageNum[episodeCount])
                    {
                        sphere.isLock = false;
                    }
                    //클리어한 스테이지 기록
                    if (isClear)
                    {
                        lastClearStage = arr_episodeStageNum[episodeCount];
                    }

                    sphere.RefreshMaterial();

                    selectSpheres.Add(sphere);


                    arr_episodeStageNum[episodeCount]++;
                    if (arr_episodeStageNum[episodeCount] >= 10)
                    {
                        arr_episodeStageNum[episodeCount] = 10;
                    }
                    episodeCount++;
                    if (episodeCount >= list_episodeNum.Count)
                    {
                        episodeCount = 0;
                    }
                  
                }

                list__selectSpheres.Add(selectSpheres);
            }*/
            #endregion

            #region oldBackup
            // 각 주제에 따른 반복문
            for (int gameTypeCount = 0; gameTypeCount < scroll_content.transform.childCount; gameTypeCount++)
            {
                //리스트 형태 선언
                List<SelectSphere> selectSpheres = new List<SelectSphere>();

                //번호 따기 x000
                int typeNum = Convert.ToInt32(scroll_content.transform.GetChild(gameTypeCount).name);

                //잠금 체크용
                int lastClearStage = 0;


                //각 스피어 할당을 위한 반복문
                for (int sphereNum = 1; sphereNum < scroll_content.transform.GetChild(gameTypeCount).childCount; sphereNum++)
                {
                    //스피어 할당
                    SelectSphere sphere = scroll_content.transform.
                        GetChild(gameTypeCount).GetChild(sphereNum).GetComponent<SelectSphere>();


                    //주제번호 x + 스테이지 반복 번호 xxx
                    int stageNum = typeNum + sphereNum; //할당할 스테이지 번호

                    //해당 스테이지번호의 클리어 정보 호출
                    bool isClear = ES3.Load<bool>(stageNum.ToString(), false);

                    //해당 스피어의 번호 할당 및 클리어 여부 변경 및 외형 변경
                    sphere.stageNum = stageNum;
                    sphere.isClear = isClear;

                    if (lastClearStage + 1 >= sphereNum)
                    {
                        sphere.isLock = false;
                    }
                    //클리어한 스테이지 기록
                    if (isClear)
                    {
                        lastClearStage = sphereNum;
                    }

                    sphere.RefreshMaterial();

                    selectSpheres.Add(sphere);
                }

                list__selectSpheres.Add(selectSpheres);
            }
            #endregion
        }


    }
}