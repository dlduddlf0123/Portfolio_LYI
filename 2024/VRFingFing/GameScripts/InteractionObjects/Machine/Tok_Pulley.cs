using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Machine
{

    /// <summary>
    /// 11/2/2023-LYI
    /// 도르래 동작
    /// 물리로 테스트 했으나 너무 불안정함으로 직접 구현
    /// 최초에 거리 총량을 포지션 차이로 인식
    /// 최대 높이와 최소 높이 공통으로 사용
    /// Update로 계속 움직이기
    /// </summary>
    public class Tok_Pulley : Tok_Interact
    {
        public Tok_Platform[] arr_platform;

        public Transform tr_top;
        public Transform tr_bottom;


        public float maxLength = 1f;
        public float moveSpeed = 1f;

        // Start is called before the first frame update
        void Start()
        {

        }




        public override void InteractInit()
        {
            base.InteractInit();


            //for (int i = 0; i < arr_pulleyRigs.Length; i++)
            //{
            //    arr_pulleyRigs[i].isKinematic = true;
            //    arr_pulleyRigs[i].velocity = Vector3.zero;
            //}

            maxLength = tr_top.position.y - tr_bottom.position.y;

        }


        private void Update()
        {
            WeightCheck();
        }

        public void WeightCheck()
        {
            if (arr_platform[0].weight < arr_platform[1].weight)
            {
                //왼쪽이 무거움
                if (arr_platform[0].transform.position.y > tr_bottom.position.y)
                {
                    arr_platform[0].transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
                    arr_platform[1].transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                }
            }
            else if (arr_platform[0].weight > arr_platform[1].weight)
            {
                //오른쪽이 무거움
                if (arr_platform[1].transform.position.y > tr_bottom.position.y)
                {
                    arr_platform[0].transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                    arr_platform[1].transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                //같은 무게
                if (arr_platform[0].transform.position.y > arr_platform[1].transform.position.y)
                {
                    arr_platform[0].transform.Translate(Vector3.down* moveSpeed * Time.deltaTime);
                    arr_platform[1].transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                }
                else if (arr_platform[0].transform.position.y < arr_platform[1].transform.position.y)
                {
                    arr_platform[0].transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                    arr_platform[1].transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
                }
            }
        }




        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
        }



        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }


    }
}