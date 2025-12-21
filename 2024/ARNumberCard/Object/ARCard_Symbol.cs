using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Text.RegularExpressions;
using UnityEngine;

namespace AroundEffect
{
    public class ARCard_Symbol : ARCard
    {

        //[Header("Symbol")]

        bool isResult = false;

        public override void ARCardInit()
        {
            base.ARCardInit();
            isSymbol = true;
        }


        public override void OnCardEnable()
        {
            base.OnCardEnable();
        }


        public override void OnCardDisable()
        {
            base.OnCardDisable();
        }

        public override void OnCardAdd(bool isLeft, ARCard card)
        {
            base.OnCardAdd(isLeft, card);

            AddSymbol(isLeft);
        }

        public override void OnCardRemove(bool isLeft)
        {
            base.OnCardRemove(isLeft);

            RemoveSymbol();
        }


        /// <summary>
        /// 8/26/2024-LYI
        /// 충돌 시 호출, 심볼로 체크
        /// 계산을 위에서하니까 딱히 쓸 이유가 없네?
        /// </summary>
        public void AddSymbol(bool isLeft)
        {
            if (isSymbol)
            {
                switch (cardName)
                {
                    //case "+":
                    //    AddPlus(isLeft);
                    //    break;
                    //case "-":
                    //    AddMinus(isLeft);
                    //    break;
                    case "=":
                        RefreshEqual();
                        break;

                    default:
                        if (leftCard == null || rightCard == null)
                        {
                            Debug.Log(cardName+ ": Need both card");
                            return;
                        }
                        if (leftCard.isSymbol || rightCard.isSymbol)
                        {
                            Debug.Log(cardName + ": Need both number");
                            return;
                        }
                        //gameMgr.arCardMgr.AddSymbolCard(isLeft, this);
                        break;
                }
            }
        }
        public void RemoveSymbol()
        {
            if (isSymbol)
            {
                //switch (cardName)
                //{
                //    case "+":
                //        RemovePlus();
                //        break;
                //    case "-":
                //       RemoveMinus();
                //        break;
                //    case "=":
                //        RemoveEqual();
                //        break;
                //}

                switch (cardName)
                {
                    case "=":
                        ResetEquial();
                        break;
                    default:
                      //  gameMgr.arCardMgr.RemoveSymbolCard(this);
                        break;
                }
            }
        }

        public override void RefreshCombineList(LinkedList<ARCard> list, string name)
        {
            base.RefreshCombineList(list, name); 
            if (cardName == "=")
            {
                RefreshEqual();
            }
        }

        public void RefreshEqual()
        {
            if (leftCard == null)
            {
                Debug.Log("Equal: Need left card");
                ResetEquial();
                return;
            }

            if (leftCard.isSymbol)
            {
                Debug.Log("Equal: Need number card");
                return;
            }

            if (isResult)
            {
                return;
            }

            isResult = true;


            //9/19/2024-LYI
            //결과 연출 재생

            //if (currentCoroutine != null)
            //{
            //    StopCoroutine(currentCoroutine);
            //    currentCoroutine = null;
            //}

            //currentCoroutine = StartCoroutine(CardResultCoroutine());

            //if (gameMgr.arCardMgr.list_linkedCard.Count > 2)
            //{
            //    result = CalculateARCardResult();
            //    txt_card.text = "= " + result.ToString();
            //}
            //else
            //{
            //    Stack<ARCard> leftStack = gameMgr.arCardMgr.StackLeft(this);

            //    string s = "";
            //    int leftCount = leftStack.Count;
            //    for (int i = 0; i < leftCount; i++)
            //    {
            //        s += leftStack.Pop().cardName;
            //    }
            //    if (leftCount > 1 && s[0] == '0')
            //    {
            //        s = s.Substring(1);
            //    }
            //    txt_card.text = "= " + s;
            //}
        }

        public void ResetEquial()
        {
            txt_card.text = "=";

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            isResult = false;
        }


        public override void RefreshCombineName()
        {
            base.RefreshCombineName();


            if (cardType == NumberType.Card_Equals)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = null;
                }

                currentCoroutine = StartCoroutine(CardResultCoroutine());
            }

        }



        public int CalculateARCardResult()
        {
            if (list_linkedCard.Count < 1)
            {
                return 0;
            }

            string expression = combinedName;

            //계산을 위한 선언
            DataTable table = new DataTable();
            //수식형 문자열을 계산
            int result = Convert.ToInt32(table.Compute(expression, string.Empty));

            return result;
        }


        protected override IEnumerator CardResultCoroutine()
        {
            WaitForSeconds waitTime = new WaitForSeconds(wordTermTime);

            yield return new WaitForSeconds(1f);

            int result = 0;
            result = CalculateARCardResult();
            txt_card.text = "= " + result.ToString();


            //리스트에 카드 목록 저장, 결과부터 왼쪽으로
            List<ARCard> list_arCard = new List<ARCard>();
            ARCard nodeCard = this;
            list_arCard.Add(nodeCard);
            while (nodeCard.leftCard != null)
            {
                nodeCard = nodeCard.leftCard;
                list_arCard.Add(nodeCard);
            }


            string expression = combinedName;

            // "=" 기호를 제거합니다.
            expression = expression.Replace("=", "");

            // 숫자와 연산자를 분리하기 위한 정규식 패턴입니다.
            string pattern = @"(\d+|[+\-*/])";

            // 정규식을 사용하여 매칭되는 항목을 찾습니다.
            MatchCollection matches = Regex.Matches(expression, pattern);

            // 결과를 저장할 리스트를 생성합니다.
            List<string> list_term = new List<string>();

            foreach (Match match in matches)
            {
                list_term.Add(match.Value);
            }

            //2중 리스트, term string에 해당하는 ARCard 각자 보관
            List<List<ARCard>> list__termARCard = new();

            //term의 각 항 list로 전환
            for (int i = 0; i < list_term.Count; i++)
            {
                List<ARCard> list_temptermCard = new List<ARCard>();

                //2자릿 수면?
                if (list_term[i].Length > 1)
                {
                    for (int j = 0; j < list_term[i].Length; j++)
                    {
                        ARCard card = list_arCard.Find((item) => item.cardName == list_term[i][j].ToString());
                        list_temptermCard.Add(card);
                    }
                }
                else
                {
                    //1자릿수는 바로 넣기
                    ARCard card=  list_arCard.Find((item) => item.cardName == list_term[i]);
                    list_temptermCard.Add(card);
                }

                list__termARCard.Add(list_temptermCard);
            }


            //다 넣었으면 이중 루프로 효과 재생 + 소리 재생
            for (int termIndex = 0; termIndex < list__termARCard.Count; termIndex++)
            {

                if (list__termARCard[termIndex].Count > 1)
                {
                    string combinedNumber = null;

                    for (int numberIndex = 0; numberIndex < list__termARCard[termIndex].Count; numberIndex++)
                    {
                        ARCard card = list__termARCard[termIndex][numberIndex];
                        if (card == null)
                        {
                            ResetEquial();
                            yield break;
                        }
                        card.mmf_result.PlayFeedbacks();
                        card.character.OnCardResult(card.cardType);

                        combinedNumber += card.cardName;
                    }

                    yield return PlayNumberTTS(this.transform.position, Convert.ToInt32(combinedNumber));
                }
                else
                {
                    ARCard card = list__termARCard[termIndex][0]; 
                    if (card == null)
                    {
                        ResetEquial();
                        yield break;
                    }

                    if (card.isSymbol)
                    {
                        if (termIndex == 0)
                        {
                            //첫 문자가 기호면 건너뛰기
                            continue;
                        }

                        card.mmf_result.PlayFeedbacks();
                        card.character.OnCardResult(card.cardType);
                        PlayCardTTS(true, card.cardName);
                    }
                    else
                    {
                        card.mmf_result.PlayFeedbacks();
                        card.character.OnCardResult(card.cardType);

                        PlayCardTTS(false, card.cardName);
                    }
                }

                yield return waitTime;
            }



            //역순으로 효과 재생
            //for (int i = list_arCard.Count - 1; i > 0; i--)
            //{
            //    list_arCard[i].mmf_result.PlayFeedbacks();
            //    list_arCard[i].character.OnCardResult(list_arCard[i].cardType);
            //    // PlayCardTTS(nodeCard.transform.position, nodeCard.cardType);

            //    yield return waitTime;

            //}

            //// 각 항목을 출력하거나 원하는 방식으로 활용합니다.
            //foreach (string term in list_term)
            //{
            //    if (int.TryParse(term, out int number))
            //    {
            //        // 숫자인 경우
            //        yield return PlayNumberTTS(transform.position, number);
            //    }
            //    else
            //    {
            //        //연산자로 시작하면 안읽기
            //        if (term != list_term[0])
            //        {
            //            // 숫자가 아닌 경우 (연산자)
            //            PlayCardTTS(isSymbol, term);
            //        }
            //    }
            //    yield return waitTime;
            //}

           // yield return StartCoroutine(TimingSync());

            //= 이후 처리
            mmf_result.PlayFeedbacks();
            character.OnCardResult(cardType);
            PlayCardTTS(isSymbol, cardName);

            yield return new WaitForSeconds(wordTermTime * 1.3f);

            yield return PlayNumberTTS(transform.position, CalculateARCardResult());

            isResult = false;

        }


        IEnumerator TimingSync()
        {
            WaitForSeconds waitTime = new WaitForSeconds(wordTermTime);

            //리스트에 카드 목록 저장, 결과부터 왼쪽으로
            List<ARCard> list_arCard = new List<ARCard>();
            ARCard nodeCard = this;
            list_arCard.Add(nodeCard);
            while (nodeCard.leftCard != null)
            {
                nodeCard = nodeCard.leftCard;
                list_arCard.Add(nodeCard);
            }


            string expression = combinedName;

            // "=" 기호를 제거합니다.
            expression = expression.Replace("=", "");

            // 숫자와 연산자를 분리하기 위한 정규식 패턴입니다.
            string pattern = @"(\d+|[+\-*/])";

            // 정규식을 사용하여 매칭되는 항목을 찾습니다.
            MatchCollection matches = Regex.Matches(expression, pattern);

            // 결과를 저장할 리스트를 생성합니다.
            List<string> terms = new List<string>();

            foreach (Match match in matches)
            {
                terms.Add(match.Value);
            }


            // 리스트 인덱스 초기화
            int arCardIndex = list_arCard.Count - 1;
            int termIndex = 0;

            // 총 단계 수 계산
            int arCardSteps = list_arCard.Count - 1;
            int termsSteps = terms.Count;

            // 총 작업 시간 설정
            float totalDuration = Mathf.Max(arCardSteps, termsSteps) * wordTermTime;

            // 시간당 진행 비율 계산
            float arCardProgressPerSecond = arCardSteps / totalDuration;
            float termsProgressPerSecond = termsSteps / totalDuration;

            // 진행 상황 초기화
            float arCardProgress = 0f;
            float termsProgress = 0f;

            while (arCardIndex >= 0 || termIndex < terms.Count)
            {
                // 현재 프레임에서의 진행량 업데이트
                arCardProgress += arCardProgressPerSecond * Time.deltaTime;
                termsProgress += termsProgressPerSecond * Time.deltaTime;

                // arCard 처리
                while (arCardIndex >= 0 && arCardProgress >= ((list_arCard.Count - 1) - arCardIndex))
                {
                    list_arCard[arCardIndex].mmf_result.PlayFeedbacks();
                    list_arCard[arCardIndex].character.OnCardResult(list_arCard[arCardIndex].cardType);
                    arCardIndex--;
                }

                // terms 처리
                while (termIndex < terms.Count && termsProgress >= termIndex)
                {
                    string term = terms[termIndex];
                    if (int.TryParse(term, out int number))
                    {
                        StartCoroutine(PlayNumberTTS(transform.position, number));
                    }
                    else
                    {
                        if (term != terms[0])
                        {
                            PlayCardTTS(isSymbol, term);
                        }
                    }
                    termIndex++;
                }

                // 다음 프레임까지 대기
                yield return null;
            }

        }
    }
}