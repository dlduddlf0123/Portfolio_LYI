using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

namespace VRTokTok.UI
{

    public class SelectSphere : MonoBehaviour
    {
        GameManager gameMgr;
        public GameObject model;
        public Renderer m_renderer;

        public Material mat_disable;
        public Material mat_select;
        public Material mat_clear;

        public MMF_Player mmf_select;
        public MMF_Player mmf_deselect;

        public int stageNum = 1001; //시작할 스테이지 번호
        public bool isClear = false;
        public bool isLock = true;
        public bool isSelect = false;

        SelectChecker[] arr_checker;
        private void Awake()
        {
            gameMgr = GameManager.Instance;
            if (gameMgr.tableMgr.ui_select != null)
            {
                arr_checker = gameMgr.tableMgr.ui_select.arr_checker;
            }
        }
        void Start()
        {

        }


        private void Update()
        {
            CheckModelActive();
        }


        /// <summary>
        /// 11/13/2023-LYI
        /// Update
        /// 스테이지 선택 활성화용 포지션 체크
        /// </summary>
        public void CheckModelActive()
        {
            //메뉴창에서만 작동
            if (gameMgr.statGame == GameStatus.MENU)
            {
                //체크 횟수를 줄이기 위해 활성화부터 체크
                if (!model.activeSelf)
                {
                    //if between both transform
                    //위칫값 확인
                    if (arr_checker[1].transform.position.x < transform.position.x &&
                        transform.position.x < arr_checker[2].transform.position.x)
                    {
                        //활성화 변경
                        model.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (arr_checker[1].transform.position.x > transform.position.x ||
                        transform.position.x > arr_checker[2].transform.position.x)
                    {
                        model.gameObject.SetActive(false);
                    }
                }
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.CompareTag("Door"))
        //    {
        //        Select();
        //    }
        //}
        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.gameObject.CompareTag("Door"))
        //    {
        //        Deselect();
        //    }
        //}



        public void Init()
        {
            RefreshMaterial();
        }

        public void CheckStageClear(int num)
        {
            stageNum = num;


        }


        public void RefreshMaterial()
        {
            if (isClear)
            {
                m_renderer.material = mat_clear;
            }
            else
            {
                if (isLock)
                {
                    m_renderer.material = mat_disable;
                }
                else
                {
                    m_renderer.material = mat_select;
                }
            }
        }

        public void Select()
        {
            isSelect = true;
            RefreshMaterial();
            if (!isLock)
            {
                mmf_select.PlayFeedbacks();
            }
        }
        public void Deselect()
        {
            isSelect = false;
            RefreshMaterial();
            if (!isLock)
            {
                mmf_deselect.PlayFeedbacks();
            }
        }

    }
}