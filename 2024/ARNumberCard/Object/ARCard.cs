using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Playables;
using TMPro;
using System;
using MoreMountains.Feedbacks;
using Vuforia;

namespace AroundEffect
{
    public enum NumberType
    {
        Card_And = -4,
        Card_Plus = -3,
        Card_Minus = -2,
        Card_Equals = -1,
        Card_00 = 0,
        Card_01,
        Card_02,
        Card_03,
        Card_04,
        Card_05,
        Card_06,
        Card_07,
        Card_08,
        Card_09 = 9,
        Card_10 = 10,
        Card_100 = 100,
        Card_1000 = 1000,
        Card_10000 = 10000,
        Card_Million = 1000000,
        Card_11 = 11,
        Card_12,
        Card_13,
        Card_14,
        Card_15,
        Card_16,
        Card_17,
        Card_18,
        Card_19 = 19,
        Card_20 = 20,
        Card_30 = 30,
        Card_40 = 40,
        Card_50 = 50,
        Card_60 = 60,
        Card_70 = 70,
        Card_80 = 80,
        Card_90 = 90,
        Card_10_01,
        Card_10_02,
        Card_10_03,
        Card_10_04,
        Card_10_05,
        Card_10_06,
        Card_10_07,
        Card_10_08,
        Card_10_09,
    }

    /// <summary>
    /// 8/26/2024-LYI
    /// AR Card
    /// 앞뒤 카드 데이터 등록
    /// 연출 관련
    /// 연결 시 매니저 호출
    /// </summary>
    public class ARCard : MonoBehaviour
    {
        protected GameManager gameMgr;

        ImageTargetBehaviour imageTargetBehaviour;
        DefaultObserverEventHandler arEvent;

        [Header("Link Card")]
        public ARCard leftCard;
        public ARCard rightCard;
        public LinkedList<ARCard> list_linkedCard = new();

        [Header("Link Checker")]
        [SerializeField] protected ARCard_Collider coll_left;
        [SerializeField] protected ARCard_Collider coll_right;

        [Header("Inner")]
        public PlayableDirector timelineDirector;

        public TextMeshProUGUI txt_card;
        public Character character;

        [Header("Effect")]
        [SerializeField] MMF_Player mmf_appear;
        [SerializeField] MMF_Player mmf_link;
        [SerializeField] public MMF_Player mmf_result;

        [Header("Property")]
        public NumberType cardType;
        public string cardName;
        public string combinedName;

        public bool isSymbol = false; //기호인 경우
        public bool isCombined = false;
        public bool isCalculate = false; //중복 방지
        private string startText;

        public float disapearTime = 1f;
        public bool isTimelineActive = false;

        protected float wordTermTime = 0.7f;


        protected Coroutine currentCoroutine;


        private void Awake()
        {
            startText = txt_card.text;
            ARCardInit();
        }


#if UNITY_EDITOR
        private void OnEnable()
        {
            //test
            OnCardEnable();
        }
#endif

        public virtual void ARCardInit()
        {
            gameMgr = GameManager.Instance;

            if (GetComponentInParent<ImageTargetBehaviour>())
            {
                imageTargetBehaviour = GetComponentInParent<ImageTargetBehaviour>();


            }

            if (GetComponentInParent<DefaultObserverEventHandler>())
            {
                arEvent = GetComponentInParent<DefaultObserverEventHandler>();

                arEvent.OnTargetFound.AddListener(OnCardEnable);
                arEvent.OnTargetLost.AddListener(OnCardDisable);
            }

            UnlinkCards();

        }

        public virtual void UnlinkCards()
        {
            if (coll_left !=null)
            {
                coll_left.ColldierInit();
                coll_left = null;
            }
            if (coll_right != null)
            {
                coll_right.ColldierInit();
                coll_right = null;
            }

            combinedName = "";
            isCombined = false;

            txt_card.text = startText;
        }

        /// <summary>
        /// 8/26/2024-LYI
        /// Call from vuforia?
        /// DefaultObserverEventHandler
        /// </summary>
        public virtual void OnCardEnable()
        {
            Debug.Log(cardName + ": Enable");
            gameObject.SetActive(true);

            gameMgr.arCardMgr.list_activeCard.Add(this.gameObject);
            
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            currentCoroutine = StartCoroutine(CardAppearCoroutine());

            // character.CharacterAppear(cardName);

            //if (isTimelineActive)
            //{
            //    timelineDirector.Stop();
            //}

            ////timelineDirector.playableAsset = arr_timeline[0];
            //timelineDirector.Play();

        }

        public virtual void OnCardDisable()
        {
            Debug.Log(cardName + ": Lost");

            UnlinkCards();

            gameMgr.arCardMgr.list_activeCard.Remove(this.gameObject);
            gameObject.SetActive(false);

            if (imageTargetBehaviour!= null)
            {
                imageTargetBehaviour.enabled = false;
                imageTargetBehaviour.enabled = true;
            }

            //if (currentCoroutine !=null)
            //{
            //    StopCoroutine(currentCoroutine);
            //    currentCoroutine = null;
            //}

            //currentCoroutine = StartCoroutine(DelayDisappear());
        }


        /// <summary>
        /// 8/26/2024-LYI
        /// 양 컬리더 상태 확인?
        /// 아랫쪽에서 호출
        /// </summary>
        public virtual void CheckColliders()
        {

        }


        #region Card Functions

        public virtual void OnCardAdd(bool isLeft, ARCard card)
        {
            //gameMgr.arCardMgr.list_linkedCard.Add(arCard); //링크 등록

            if (isLeft)
            {
                leftCard = card;

                isCombined = true;
                CheckCombineList();
            }
            else
            {
                rightCard = card;
                isCombined = true;
                CheckCombineList();
            }

            Debug.Log(cardName + "-OnCardAdd:" + card.cardName);
        }

        public virtual void OnCardRemove(bool isLeft)
        {
            //gameMgr.arCardMgr.list_linkedCard.Remove(arCard); //링크 제거
            Debug.Log(cardName + "-OnCardRemove");

            if (isLeft)
            {
                leftCard = null;
            }
            else
            {
                rightCard = null;
            }

            if (leftCard == null && rightCard == null)
            {
                combinedName = "";
                list_linkedCard.Clear();
                isCombined = false;
            }
            else
            {
                CheckCombineList();
            }

            //gameMgr.arCardMgr.ResetAllCard();
        }

        /// <summary>
        /// 9/24/2024-LYI
        /// 오른쪽 카드가 null인 경우만 진행?
        /// </summary>
        public virtual void CheckCombineList()
        {
            if (rightCard != null)
            {
                return;
            }


            RefreshCombineName();
        }


        /// <summary>
        /// 8/27/2024-LYI
        /// 합쳐질 때 데이터 갱신
        /// </summary>
        public virtual void RefreshCombineList(LinkedList<ARCard> list, string name)
        {
            list_linkedCard = list;
            combinedName = name;
        }

        /// <summary>
        /// 8/27/2024-LYI
        /// 합쳐질 때 이름 저장
        /// </summary>
       public virtual void RefreshCombineName()
        {
            if (!isCombined)
            {
                combinedName = "";
                list_linkedCard.Clear();
                return;
            }

            if (isCalculate)
            {
                return;
            }
            isCalculate = true;

            list_linkedCard.Clear();

            string leftName = "", rightName = "";

            //왼쪽 카드 계산
            if (leftCard != null)
            {
                leftName = leftCard.cardName;

                Stack<ARCard> leftStack = gameMgr.arCardMgr.StackLeft(this);
                if (leftStack.Count > 0)
                {
                    leftName = "";
                    int leftCount = leftStack.Count;
                    for (int i = 0; i < leftCount; i++)
                    {
                        ARCard stackCard = leftStack.Pop();
                        leftName += stackCard.cardName;
                        list_linkedCard.AddFirst(stackCard);
                    }
                }
            }

            list_linkedCard.AddLast(this);

            //오른쪽 카드 계산
            if (rightCard != null)
            {
                rightName = rightCard.cardName;
                Queue<ARCard> rightQueue = gameMgr.arCardMgr.QueueRight(this);
                if (rightQueue.Count > 0)
                {
                    rightName = "";
                    int rightCount = rightQueue.Count;
                    for (int i = 0; i < rightCount; i++)
                    {
                        ARCard queueCard = rightQueue.Dequeue();
                        if (queueCard.cardName != "=")
                        {
                            rightName += queueCard.cardName;
                        }
                        list_linkedCard.AddLast(queueCard);
                    }
                }
            }

            combinedName = leftName + cardName + rightName;
            if (cardName == "=")
            {
                combinedName = leftName;
            }

            LinkedListNode<ARCard> linknode = list_linkedCard.First;

            while (true)
            {
                linknode.Value.RefreshCombineList(list_linkedCard, combinedName);
                if (linknode.Next == null)
                {
                    break;
                }
                linknode = linknode.Next;
                if (linknode.Value.cardName == cardName)
                {
                    if (linknode.Next == null)
                    {
                        break;
                    }
                    linknode = linknode.Next;
                }

            }

            Debug.Log(cardName + "- RefreshCombineName: " + combinedName);

            if (list_linkedCard.Last.Value.cardName == "=")
            {
                list_linkedCard.Last.Value.RefreshCombineList(list_linkedCard, combinedName);
                //list_linkedCard.Last.Value.GetComponent<ARCard_Symbol>().AddEqual();
            }

            isCalculate = false;

            //9/19/2024-LYI
            //링크 효과 추가?
            //일단 주석 처리
            //if (currentCoroutine != null)
            //{
            //    StopCoroutine(currentCoroutine);
            //    currentCoroutine = null;
            //}

            //currentCoroutine = StartCoroutine(CardLinkCoroutine());


            //if (leftCard != null)
            //{
            //    leftCard.combinedName = combinedName;
            //}
            //if (rightCard != null)
            //{
            //    rightCard.combinedName = combinedName;
            //}
        }

        #endregion


        #region Effect Coroutines


        /// <summary>
        /// 9/19/2024-LYI
        /// 사라질 때 지연
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator DelayDisappear()
        {
            float t = 0f;

            if (t < disapearTime)
            {
                t += Time.deltaTime;
                yield return null;
            }

            Debug.Log(cardName + ": Disable");
            //timelineDirector.Stop();
            UnlinkCards();

            gameObject.SetActive(false);
        }



        /// <summary>
        /// 9/19/2024-LYI
        /// 카드가 등장할 때 코루틴
        /// 캐릭터 등장, 카드 등장
        /// 이후 숫자 읽기
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CardAppearCoroutine()
        {
            mmf_appear.PlayFeedbacks();
            character.OnCardAppear(cardType);

            yield return new WaitForSeconds(0.5f);
            PlayCardTTS(isSymbol, cardName);

        }

        /// <summary>
        /// 9/19/2024-LYI
        /// 카드 연결됐을 때 효과
        /// 숫자만 연결됐을 때?
        /// 안 쓸 수도 있음
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CardLinkCoroutine()
        {
            mmf_link.PlayFeedbacks();
            character.OnCardLink();
            yield return null;
        }

        /// <summary>
        /// 9/19/2024-LYI
        /// 카드 결과물 연출 효과
        /// 계산식 보고 맨 앞부터 숫자와 점프하며 읽기
        /// =에서만 호출되도록 할 것
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CardResultCoroutine()
        {
            WaitForSeconds waitTime = new WaitForSeconds(wordTermTime);


            List<ARCard> list_arCard = new List<ARCard>();
            ARCard nodeCard = this;
            list_arCard.Add(nodeCard);
            while (nodeCard.leftCard != null)
            {
                nodeCard = nodeCard.leftCard;
                list_arCard.Add(nodeCard);
            }

            for (int i = list_arCard.Count - 1; i > 0; i--)
            {
                list_arCard[i].mmf_result.PlayFeedbacks();
                list_arCard[i].character.OnCardResult(list_arCard[i].cardType);
                // PlayCardTTS(nodeCard.transform.position, nodeCard.cardType);

                yield return waitTime;
               
            }

           // LinkedListNode<ARCard> linkNode = list_linkedCard.Last;

            //while (true)
            //{
            //    if (linkNode == null)
            //    {
            //        break;
            //    }
            //    //단어 움직임 모션 + 캐릭터 움직임 + 단어 읽기
            //    ARCard nodeCard = linkNode.Value;
            //    nodeCard.mmf_result.PlayFeedbacks();
            //    nodeCard.character.OnCardResult(nodeCard.cardType);
            //   // PlayCardTTS(nodeCard.transform.position, nodeCard.cardType);

            //    yield return waitTime;

            //    linkNode = linkNode.Previous;
            //}

            //완료됐을 때 처리
            //정답 외치기, 모션, 단어

            //각 항별 발음 + 기호 발음 + ... + = + 결과 
            //아랫 단에서 처리

        }

        #endregion


        #region TTS Sound


        /// <summary>
        /// 9/20/2024-LYI
        /// 함수 호출 시 현재 카드에 따른 소리
        /// </summary>
        public void PlayCardTTS(bool symbol, string name)
        {
            if (symbol)
            {
                switch (name)
                {
                    case "+":
                        gameMgr.soundMgr.PlaySfx(transform.position, String_GetRegionTTS(NumberType.Card_Plus));
                        break;
                    case "-":
                        gameMgr.soundMgr.PlaySfx(transform.position, String_GetRegionTTS(NumberType.Card_Minus));
                        break;
                    case "=":
                        gameMgr.soundMgr.PlaySfx(transform.position, String_GetRegionTTS(NumberType.Card_Equals));
                        break;
                }
            }
            else
            {
                //숫자로 바로
                gameMgr.soundMgr.PlaySfx(transform.position, String_GetRegionTTS((NumberType)Convert.ToInt32(name)));
            }

        }
        public void PlayCardTTS(Vector3 pos, NumberType type)
        {
            gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS(type));
        }
        /// <summary>
        /// 9/20/2024-LYI
        /// 카드 데이터를 읽는게 아닌 숫자를 읽는것
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        public IEnumerator PlayNumberTTS(Vector3 pos, int number)
        {
            switch (gameMgr.gameLanguage)
            {
                case Language.KOREAN:
                    yield return StartCoroutine(PlayNumberInKorean(pos, number));
                    break;
                case Language.ENGLISH:
                   yield return  StartCoroutine(PlayNumberInEnglish(pos, number));
                    break;
                default:
                    break;
            }

            yield return null;
        }

        private IEnumerator PlayNumberInKorean(Vector3 pos, int number)
        {
            if (number > 99999)
            {
                Debug.LogWarning("지원하지 않는 숫자 범위입니다.");
                yield break;
            }

            WaitForSeconds wordTerm = new WaitForSeconds(wordTermTime);
            WaitForSeconds shortTerm = new WaitForSeconds(wordTermTime * 0.7f);

            if (number < 0)
            {
                gameMgr.soundMgr.PlaySfx(transform.position, "AR_NumberCard_Minus_ENG");
                yield return wordTerm;
                number = Mathf.Abs(number);
            }

            if (number < 10)
            {
                gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)number));
                yield break;
            }



            int[] units = { 10000, 1000, 100, 10, 1 };

            for (int i = 0; i < units.Length; i++)
            {
                int unitValue = number / units[i];
                if (unitValue > 0)
                {
                    //10 단위
                    if (i == 3)
                    {
                        gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)(unitValue * 10)));
                        yield return wordTerm;
                    }
                    else
                    {
                        // '일백', '일십'에서 '일' 생략
                        if (!(unitValue == 1 &&
                            units[i] >= 10))
                        {
                            gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)unitValue));
                            yield return shortTerm;
                        }

                        if (units[i] != 1) // 일의 자리는 단위명 없음
                        {
                            gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)units[i]));
                            yield return wordTerm;
                        }
                    }
                }
                number %= units[i]; // 다음 자리수로 이동
            }
        }
        private IEnumerator PlayNumberInEnglish(Vector3 pos, int number)
        {
            if (number > 99999)
            {
                Debug.LogWarning("Number out of supported range.");
                yield break;
            }

            WaitForSeconds wordTerm = new WaitForSeconds(wordTermTime);

            if (number < 0)
            {
                gameMgr.soundMgr.PlaySfx(transform.position, String_GetRegionTTS(NumberType.Card_Minus));
                yield return wordTerm;
                number = Mathf.Abs(number);
            }

            if (number < 10)
            {
                gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)number));
                yield break;
            }



            int thousands = number / 1000;
            int remainder = number % 1000;

            if (thousands > 0)
            {
                yield return PlayBelowThousand(pos, thousands);
                gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)1000));
                yield return wordTerm;
            }

            if (remainder > 0)
            {
                yield return PlayBelowThousand(pos, remainder);
            }
        }

        private IEnumerator PlayBelowThousand(Vector3 pos, int number)
        {
            WaitForSeconds wordTerm = new WaitForSeconds(wordTermTime);
            WaitForSeconds shortTerm = new WaitForSeconds(wordTermTime * 0.5f);

            int hundreds = number / 100;
            int remainder = number % 100;

            if (hundreds > 0)
            {
                gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)hundreds));
                yield return shortTerm;
                gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)100));
                yield return wordTerm;

                // 영국식 영어에서는 'and'를 사용
                //gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS(NumberType.Card_And));
                //yield return wordTerm;
            }

            if (remainder > 0)
            {

                if (remainder < 10)
                {
                    gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)remainder));
                    yield return wordTerm;
                }
                else if (remainder < 20)
                {
                    gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)remainder));
                    yield return wordTerm;
                }
                else
                {
                    int tens = remainder / 10;
                    int units = remainder % 10;

                    gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)(tens*10)));
                    yield return wordTerm;

                    if (units > 0)
                    {
                        gameMgr.soundMgr.PlaySfx(pos, String_GetRegionTTS((NumberType)units));
                        yield return wordTerm;
                    }
                }
            }
        }



        private readonly Dictionary<Language, string> LanguageCodes = new Dictionary<Language, string>
{
    { Language.KOREAN, "KOR" },
    { Language.ENGLISH, "ENG" },
    // 새로운 언어 추가 가능
};

        public  string String_GetRegionTTS(NumberType cardType)
        {
            // 현재 게임 언어 가져오기
            Language currentLanguage = GameManager.Instance.gameLanguage;

            // 언어 코드 가져오기
            if (!LanguageCodes.TryGetValue(currentLanguage, out string languageCode))
            {
                Debug.LogWarning("지원하지 않는 언어입니다.");
                return null;
            }

            // 파일명 생성
            string fileName = null;

            fileName = $"AR_Number{cardType}_{languageCode}";

            return fileName;
        }

        #endregion

    }
}