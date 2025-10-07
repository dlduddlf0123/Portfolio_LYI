using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using VRTokTok.Interaction;

namespace VRTokTok.Interaction.Bridge
{
    public class Tok_Bridge : Tok_Interact
    {
       public GameObject[][] arr__bridge;
        public Bridge_Glass[] arr_glass;

        [Header("Bridge")]
        public Tok_Button btn_question;

        bool isFirst = true;

        void Start()
        {
            //문제 버튼 활성화, 클릭 시 동작 전달
            btn_question.onActive.AddListener(StartQuestion);
            btn_question.SetInteractable(true);

        }

        public override void InteractInit()
        {
            base.InteractInit();

            if (isFirst)
            {

                arr__bridge = new GameObject[transform.childCount][];

                for (int bridgeCount = 0; bridgeCount < arr__bridge.Length; bridgeCount++)
                {
                    arr__bridge[bridgeCount] = new GameObject[transform.GetChild(bridgeCount).childCount];

                    for (int glassCount = 0; glassCount < arr__bridge[bridgeCount].Length; glassCount++)
                    {
                        arr__bridge[bridgeCount][glassCount] = transform.GetChild(bridgeCount).GetChild(glassCount).gameObject;
                    }
                }

                arr_glass = GetComponentsInChildren<Bridge_Glass>();
                isFirst = false;
            }

            for (int i = 0; i < arr_glass.Length; i++)
            {
                arr_glass[i].GlassInit();
                arr_glass[i].SetGlassSafe(false);
            }

            StartQuestion();
        }

        public void StartQuestion()
        {
            //패널 비활성화
            //for (int i = 0; i < list_panel.Count; i++)
            //{
            //    list_render_panel[i].material = arr_matDisable[i];
            //}

            ////버튼 비활성화
            //for (int i = 0; i < list_button.Count; i++)
            //{
            //    list_button[i].SetInteractable(false);
            //}

            StartCoroutine(QuestionCoroutine());
        }
        IEnumerator QuestionCoroutine()
        {
            btn_question.SetInteractable(false);


            yield return new WaitForSeconds(1f);

            int lastNum = 0; //마지막에 사용된 글래스 번호

            for (int stepCount = 0; stepCount < arr__bridge.Length; stepCount++)
            {
                int randomGlass; //해당 스텝에서 고를 유리의 번호


                //이전 값의 양 옆까지 3가지만 랜덤으로 사용
                if (stepCount != 0)
                {
                    //첫 선택이 아닌 경우
                    if (lastNum == 0)
                    {
                        //마지막 선택이 첫번째인 경우
                        randomGlass = Random.Range(lastNum, lastNum + 2);
                    }
                    else if (lastNum >= arr__bridge[stepCount].Length - 1)
                    {
                        //마지막 선택이 마지막인 경우
                        randomGlass = Random.Range(lastNum - 1, lastNum + 1);
                    }
                    else
                    {
                        //그 외 중간 값인 경우
                        //중간값은 마지막 번호의 -1부터 +1까지의 랜덤
                        randomGlass = Random.Range(lastNum - 1, lastNum + 2);
                    }
                }
                else
                {
                    //최초에 전체 랜덤
                    randomGlass = Random.Range(0, arr__bridge[stepCount].Length);
                }

                //해당 유리 선택, 가이드 하이라이트
                arr__bridge[stepCount][randomGlass].GetComponent<Bridge_Glass>().GlassSelect();
                arr__bridge[stepCount][randomGlass].GetComponent<Bridge_Glass>().SetGlassSafe(true);

                yield return new WaitForSeconds(0.2f);

                //하이라이트 해제
                arr__bridge[stepCount][randomGlass].GetComponent<Bridge_Glass>().GlassDeselect();

                lastNum = randomGlass;
            }


            //q_correct.Clear();

            //int count = 0;
            //while (count < questionCount)
            //{
            //    int r = Random.Range(0, list_panel.Count);
            //    PanelActive(r);

            //    //정답 삽입, 버튼 누를때 마다 호출하여 정답 확인
            //    q_correct.Enqueue(r);

            //    count++;

            //    yield return new WaitForSeconds(waitTime);
            //    //효과 제거
            //    PanelDisable(r);

            //    yield return new WaitForSeconds(waitTime);
            //}
            //yield return new WaitForSeconds(0.5f);

            ////정답 대기 상태
            //isAnswer = true;
            //for (int i = 0; i < list_button.Count; i++)
            //{
            //    list_button[i].SetInteractable(true);
            //}

            btn_question.SetInteractable(true);
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