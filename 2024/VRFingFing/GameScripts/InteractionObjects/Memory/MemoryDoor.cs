using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Interaction;

namespace VRTokTok.Interaction.Memory
{
    /// <summary>
    /// 9/26/2023-LYI
    /// 기억력 게임
    /// 문에 표시되는 색깔이 랜덤하게 빛난 순서대로 버튼을 눌러야 한다
    /// </summary>
    public class MemoryDoor : Tok_Interact
    {
        GameManager gameMgr;

        [Header("Panel")]
        public List<GameObject> list_panel = new List<GameObject>();
        List<Renderer> list_render_panel = new();
        public Material[] arr_matActive;
        public Material[] arr_matDisable;


        [Header("Button")]
        public List<Tok_Button> list_button = new List<Tok_Button>();
        public Tok_Button btn_question;

        [Header("Count")]
        public Renderer[] arr_correctCount;
        public Material correct_active;
        public Material correct_disable;

        [Header("Question")]
        public Queue<int> q_correct = new Queue<int>();

        int startCount = 0;
        public int questionCount = 3; //빛나는 횟수
        public float waitTime = 0.5f; //빛나는 사이 간격

        public bool isAnswer = false; //답변 가능 상태 확인

        public int repeatCount = 1; //반복횟수, 문제가 여러개일 경우 정답 맞춘 뒤 바로 다음문제
        public int correctCount = 0; //정답 맞춘 횟수
        public int moreQuestionCount = 1; //반복 시 늘어날 정답 수


        const string EMISSION = "_EMISSION";

        bool isFirst = true;

        Coroutine beepCoroutine = null;

        public override void InteractInit()
        {
            if (isFirst)
            {
                gameMgr = GameManager.Instance;

                startCount = questionCount;
                isFirst = false;


                //패널 할당, 비활성화
                for (int i = 0; i < list_panel.Count; i++)
                {
                    list_render_panel.Add(list_panel[i].gameObject.GetComponent<Renderer>());
                    PanelDisable(i);
                }

                //버튼 할당, 비활성화
                for (int i = 0; i < list_button.Count; i++)
                {
                    int a = i;
                    list_button[a].onActive.AddListener(() => ButtonMemory(a));
                }


                //문제 버튼 활성화, 클릭 시 동작 전달
                btn_question.onActive.AddListener(StartQuestion);
            }

            base.InteractInit();

            StopAllCoroutines();
            questionCount = startCount; //반복 관련 초기화

            isAnswer = false; //답변 불가능 상태


            //패널 비활성화
            for (int i = 0; i < list_panel.Count; i++)
            {
                PanelDisable(i);
            }
            //버튼 비활성화
            for (int i = 0; i < list_button.Count; i++)
            {
                list_button[i].SetInteractable(false, false);
            }
            //문제 버튼 활성화
            btn_question.SetInteractable(true, false);

            //카운트 버튼 활성화 체크
            if (arr_correctCount != null)
            {
                correctCount = 0;

                if (repeatCount > 1)
                {
                    arr_correctCount[0].transform.parent.gameObject.SetActive(true);

                    //비활성화 머테리얼로 변경
                    for (int i = 0; i < arr_correctCount.Length; i++)
                    {
                        arr_correctCount[i].gameObject.SetActive(false);
                    }

                    for (int i = 0; i < repeatCount; i++)
                    {
                        arr_correctCount[i].gameObject.SetActive(true);
                        arr_correctCount[i].material = correct_disable;
                    }
                }
                else
                {
                    arr_correctCount[0].transform.parent.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 10/9/2023-LYI
        /// 혼자 빛나는 하얀색 버튼을 눌러 문제 시작
        /// 누른 뒤 버튼 비활성화, 문제 시작
        /// </summary>
        public void StartQuestion()
        {
            //패널 비활성화
            for (int i = 0; i < list_panel.Count; i++)
            {
                list_render_panel[i].material = arr_matDisable[i];
            }
            btn_question.SetInteractable(false, false);

            //버튼 비활성화
            for (int i = 0; i < list_button.Count; i++)
            {
                list_button[i].SetInteractable(false);
            }

            StartCoroutine(QuestionCoroutine());
        }

        /// <summary>
        /// 9/26/2023-LYI
        /// 순서대로 문의 패널이 빛난다
        /// </summary>
        /// <returns></returns>
        IEnumerator QuestionCoroutine()
        {
            btn_question.SetInteractable(false);

            yield return new WaitForSeconds(1f);

            q_correct.Clear();

            int count = 0;
            while (count < questionCount)
            {
                int r = Random.Range(0, list_panel.Count);
                PanelActive(r);
                gameMgr.soundMgr.PlaySfx(list_panel[r].transform.position, Constants.Sound.SFX_MEMORY_PANNAL);

                //정답 삽입, 버튼 누를때 마다 호출하여 정답 확인
                q_correct.Enqueue(r);

                count++;

                yield return new WaitForSeconds(waitTime);
                //효과 제거
                PanelDisable(r);

                yield return new WaitForSeconds(waitTime);
            }
            yield return new WaitForSeconds(0.5f);

            //정답 대기 상태
            isAnswer = true;
            for (int i = 0; i < list_button.Count; i++)
            {
                list_button[i].SetInteractable(true);
            }

            btn_question.SetInteractable(true);
        }


        public void PanelActive(int r)
        {
            //반짝이는 효과
            list_render_panel[r].material = arr_matActive[r];
           // list_render_panel[r].material.EnableKeyword(EMISSION);
        }

        public void PanelDisable(int r)
        {
           // list_render_panel[r].material.DisableKeyword(EMISSION);
            list_render_panel[r].material = arr_matDisable[r];
        }

        /// <summary>
        /// 9/26/2023-LYI
        /// 각 버튼 동작 설정
        /// 각 버튼이 눌렸을 때의 정답 체크
        /// </summary>
        public void ButtonMemory(int num)
        {
            if (!isAnswer ||
                q_correct.Count == 0)
            {
                return;
            }
            int correct = q_correct.Dequeue();

            PannelOnOff(num);

            //정답일 경우
            if (num == correct)
            {
                if (q_correct.Count == 0)
                {
                    //클리어, 문 열림
                    StartCoroutine(CorrectAnswer());
                }
            }
            else  //오답일 경우
            {
                StartCoroutine(WrongAnswer());
            }

        }

        /// <summary>
        /// 10/9/2023-LYI
        /// 정답 맞췄을 시 동작
        /// 패널 초록불 보여준 뒤 정답 호출
        /// 반복 있을경우 불 들어온 뒤 다음 문제 준비
        /// </summary>
        /// <returns></returns>
        IEnumerator CorrectAnswer()
        {
            q_correct.Clear();

            correctCount++;

            //정답 패널 갱신
            for (int i = 0; i < correctCount; i++)
            {
                arr_correctCount[i].material = correct_active;
            }

            //패널 초록색
            for (int i = 0; i < list_panel.Count; i++)
            {
                list_render_panel[i].material = arr_matActive[3];
            }

            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_MEMORY_SUCCESS);

            yield return new WaitForSeconds(0.5f);

            //패널 비활성화
            for (int i = 0; i < list_panel.Count; i++)
            {
                list_render_panel[i].material = arr_matDisable[i];
            }

            //버튼 비활성화
            for (int i = 0; i < list_button.Count; i++)
            {
                list_button[i].SetInteractable(false);
            }

            btn_question.SetInteractable(false);

            yield return new WaitForSeconds(0.5f);


            RepeatQuestion();

        }


        /// <summary>
        /// 10/9/2023-LYI
        /// 잘못된 버튼을 클릭했을 경우
        /// </summary>
        /// <returns></returns>
        IEnumerator WrongAnswer()
        {
            q_correct.Clear();

            //패널 빨간색
            for (int i = 0; i < list_panel.Count; i++)
            {
                list_render_panel[i].material = arr_matActive[0];
            }
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_MEMORY_FAILURE);

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < list_panel.Count; i++)
            {
                list_render_panel[i].material = arr_matDisable[i];
            }

            //버튼 할당, 비활성화
            for (int i = 0; i < list_button.Count; i++)
            {
                list_button[i].SetInteractable(false);
            }

            btn_question.SetInteractable(true);
            //GameManager.Instance.playMgr.currentStage.RestartStage();
        }


        /// <summary>
        /// 7/2/2024-LYI
        /// 패널 점등 진행
        /// </summary>
        /// <param name="pannelNum"></param>
        public void PannelOnOff(int pannelNum)
        {
            if (beepCoroutine != null)
            {
                StopCoroutine(beepCoroutine);

                for (int i = 0; i < list_panel.Count; i++)
                {
                    PanelDisable(i);
                }
            }
            beepCoroutine = StartCoroutine(PannelBeep(pannelNum));
        }

        /// <summary>
        /// 7/2/2024-LYI
        /// 패널 점등 효과
        /// </summary>
        /// <returns></returns>
        IEnumerator PannelBeep(int pannelNum)
        {
            PanelActive(pannelNum);
            gameMgr.soundMgr.PlaySfx(list_panel[pannelNum].transform.position, Constants.Sound.SFX_MEMORY_PANNAL);

            yield return new WaitForSeconds(waitTime);
            //효과 제거
            PanelDisable(pannelNum);
        }


        /// <summary>
        /// 10/9/2023-LYI
        /// 정답 이후 반복횟수가 있을 경우 호출
        /// 문제 반복 및 최종적인 체크
        /// </summary>
        void RepeatQuestion() 
        {
            //정답 횟수 체크
            if (correctCount < repeatCount)
            {
                questionCount += moreQuestionCount;
                //정답 횟수가 부족하면 다음 문제 재생
                StartCoroutine(QuestionCoroutine());
            }
            else
            {
                //버튼 전체 비활성화
                for (int i = 0; i < list_button.Count; i++)
                {
                    list_button[i].SetInteractable(false);
                }
                btn_question.SetInteractable(false);

                //정답 횟수가 충분하면 작동
                ActiveInteraction();
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