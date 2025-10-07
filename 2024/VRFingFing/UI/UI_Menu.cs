using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

using Oculus.Interaction;

using VRTokTok.Character;
using VRTokTok.Manager;

namespace VRTokTok.UI
{

    /// <summary>
    /// 9/21/2023-LYI
    /// 메뉴장면에서 UI 관리 클래스
    /// 게임 모드 선택, 설정
    /// </summary>
    public class UI_Menu : MonoBehaviour
    {
        GameManager gameMgr;
        public PointableCanvas menuPointableCanvas;

        [Header("Menu Buttons")]
        [SerializeField]
        Button btn_start; //게임 시작 버튼
        [SerializeField]
        TextMeshProUGUI txt_currentStage;
        //[SerializeField]
        //Toggle btn_select;
        //[SerializeField]
        //Button btn_option;


        [Header("Selelct Scroll View")]
        [SerializeField]
        CanvasGroup menu_canvasGroup;
        [SerializeField]
        RectTransform scrollViewport;
        [SerializeField]
        GameObject efx_select;


        [SerializeField]
        Button[] arr_btn_stageType;

        List<GameObject> list_btn_stageNum_active = new List<GameObject>(); //스테이지 번호 리스트(ObjectPooling)
        List<GameObject> list_btn_stageNum_disable = new List<GameObject>(); //스테이지 번호 리스트(ObjectPooling)

        [SerializeField]
        GameObject btn_originNum;
        [SerializeField]
        Transform tr_active;
        [SerializeField]
        Transform tr_disable;

       
        public RectTransform tr_scrollTop;
        public RectTransform tr_scrollBottom;

        //캐릭터 6마리 스프라이트
        public GameObject[] arr_unlockSprite;



        //[SerializeField]
        //TextMeshProUGUI txt_stageTitle;
        //[SerializeField]
        //Image img_stageThumbnail;
        //[SerializeField]
        //TextMeshProUGUI txt_stageDescription;
        //[SerializeField]
        //Button btn_selectStart;


        [Tooltip("0: normal / 1: select")]
        [SerializeField]
        Sprite[] arr_typeSprite;

        [Tooltip("0: normal / 1: clear")]
        [SerializeField]
        Sprite[] arr_btnSprite;

        [Header("Credit")]
        [SerializeField]
        CanvasGroup credit_canvasGroup;
        [SerializeField]
        Button credit_btn_close;
        bool isCreditActive = false;

        [Header("Tutorial")]
        [SerializeField]
        GameObject tutorialMarkers;

        [Header("Properties")]
        public StageType lastStageType = StageType.NONE;
        public int lastStageNum = 0;

        public StageType currentStageType = StageType.NONE;
        public int currentStageNum = 0;

        bool isFirst = true;
        //잠금 체크용
        int lastClearStage = 0;



        public void MenuUIInit()
        {
            if (isFirst)
            {
                gameMgr = GameManager.Instance;

                lastStageType = ES3.Load(Constants.ES3.LAST_STAGE_TYPE, StageType.NONE);
                lastStageNum = ES3.Load(Constants.ES3.LAST_STAGE_NUM, 0);

                SetMenuButtonListeners();

                isFirst = false;
            }

            ButtonSelectStageType(StageType.MIXED);


            //if (lastStageType != StageType.NONE)
            //{
            //    ButtonSelectStageType(lastStageType);
            //}
            //else
            //{
            //    ButtonSelectStageType(StageType.MIXED);
            //}

            //if (currentStageType == StageType.NONE)
            //{
            //    txt_currentStage.text = (lastStageType == StageType.NONE) ?
            //   "MAZE 1" :
            //   lastStageType.ToString() + " " + lastStageNum;
            //    SetSelectEffect(lastStageNum);
            //}
            //else
            //{
            //    txt_currentStage.text = currentStageType.ToString() + " " + currentStageNum;
            //    SetSelectEffect(currentStageNum);
            //}
        }


        Coroutine fadeCoroutine = null;

        /// <summary>
        /// 5/7/2024-LYI
        /// 버튼들 페이드 효과
        /// </summary>
        /// <param name="isActive"></param>
        public void MenuFade(bool isActive)
        {
            tutorialMarkers.SetActive(false);

            if (isCreditActive)
            {
                credit_canvasGroup.gameObject.SetActive(false);
            }

            if (isActive)
            {
                menu_canvasGroup.gameObject.SetActive(true);
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                    fadeCoroutine = null;
                }
                menu_canvasGroup.alpha = 0;
                fadeCoroutine = StartCoroutine(FadeInOut(menu_canvasGroup, true));
            }
            else
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                    fadeCoroutine = null;
                }
                menu_canvasGroup.alpha = 1;
                fadeCoroutine = StartCoroutine(FadeInOut(menu_canvasGroup, false, 5, () => menu_canvasGroup.gameObject.SetActive(false)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isActive"></param>
        public void CreditFade(bool isActive)
        {
            isCreditActive = isActive;

            if (isActive)
            {
                credit_canvasGroup.gameObject.SetActive(true);
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                    fadeCoroutine = null;
                }
                credit_canvasGroup.alpha = 0;
                fadeCoroutine = StartCoroutine(FadeInOut(credit_canvasGroup, true));
            }
            else
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                    fadeCoroutine = null;
                }
                credit_canvasGroup.alpha = 1;
                fadeCoroutine = StartCoroutine(FadeInOut(credit_canvasGroup, false, 5, () => credit_canvasGroup.gameObject.SetActive(false)));
            }
        }
        private IEnumerator FadeInOut(CanvasGroup group, bool isFadeOut, float fadingSpeed = 5f, UnityAction onFadeEnd = null)
        {
            float currentTime = 0f;
            while (currentTime < 1)
            {
                currentTime += Time.deltaTime * fadingSpeed;

                group.alpha = (isFadeOut) ?
                    Mathf.Lerp(0, 1, currentTime) :
                    Mathf.Lerp(1, 0, currentTime);

                yield return new WaitForSeconds(Time.deltaTime);
            }
            if (onFadeEnd != null)
            {
                onFadeEnd.Invoke();
            }
        }

        void ChangeCurrentStageText()
        {
            if (currentStageType == StageType.MIXED)
            {
                txt_currentStage.text = "Stage" + " " + currentStageNum.ToString();
            }
            else
            {
                txt_currentStage.text = currentStageType.ToString() + " " + currentStageNum.ToString();
            }
        }

        #region Button Func

        /// <summary>
        /// 9/22/2023-LYI
        /// Call in menuScene start
        /// 각 버튼 AddListener 호출
        /// </summary>
        public void SetMenuButtonListeners()
        {
            btn_start.onClick.AddListener(ButtonMenuStart);
            btn_start.onClick.AddListener(
                    () => gameMgr.soundMgr.PlaySfx(btn_start.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK));


            //btn_select.onValueChanged.AddListener(ButtonMenuSelect);
            //btn_option.onClick.AddListener(ButtonMenuOption);

            credit_btn_close.onClick.AddListener(CloseCredit);

            if (currentStageType != StageType.NONE)
            {
                ChangeCurrentStageText();
            }


            for (int i = 0; i < arr_btn_stageType.Length; i++)
            {
                int a = i;
                arr_btn_stageType[i].onClick.AddListener(
                    () => gameMgr.soundMgr.PlaySfx(arr_btn_stageType[a].transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK));
            }

            //arr_btn_stageType[0].onClick.AddListener(() => ButtonSelectStageType(StageType.MAZE));
            //arr_btn_stageType[1].onClick.AddListener(() => ButtonSelectStageType(StageType.CROSSING));
            //arr_btn_stageType[2].onClick.AddListener(() => ButtonSelectStageType(StageType.BLOCK));
            arr_btn_stageType[0].onClick.AddListener(() => ButtonSelectStageType(StageType.MIXED));
            // arr_btn_stageType[2].onClick.AddListener(() => ButtonSelectStageType(StageType.PRESS));
            // arr_btn_stageType[2].onClick.AddListener(() => ButtonSelectStageType(StageType.MEMORY));

            efx_select.gameObject.SetActive(false);
        }


        /// <summary>
        /// 9/22/2023-LYI
        /// 게임 타입 선택 버튼 클릭 시 호출
        /// </summary>
        public void ButtonSelectStageType(StageType type)
        {
            //버튼 스프라이트 변경
            for (int i = 0; i < arr_btn_stageType.Length; i++)
            {
                arr_btn_stageType[i].image.sprite = arr_typeSprite[0];
            }
            switch (type)
            {
                //case StageType.MAZE:
                //    arr_btn_stageType[0].image.sprite = arr_typeSprite[1];
                //    break;
                //case StageType.CROSSING:
                //    arr_btn_stageType[1].image.sprite = arr_typeSprite[1];
                //    break;
                ////case StageType.PRESS:
                ////    arr_btn_stageType[2].image.sprite = arr_typeSprite[1];
                ////    break;
                ////case StageType.MEMORY:
                ////    arr_btn_stageType[2].image.sprite = arr_typeSprite[1];
                ////    break;
                //case StageType.BLOCK:
                //    arr_btn_stageType[2].image.sprite = arr_typeSprite[1];
                //    break;
                case StageType.MIXED:
                    arr_btn_stageType[0].image.sprite = arr_typeSprite[1]; 
                    
                    currentStageType = type;

                    int mixCount = gameMgr.dataMgr.dic_mixedToStage.Count;
                    SetButtonMixedNumberActive(mixCount);
                    return;
                default:
                    arr_btn_stageType[0].image.sprite = arr_typeSprite[1];
                    break;
            }

            currentStageType = type;

            int count = gameMgr.dataMgr.list__stageType[(int)type - 1].Count;
            SetButtonSelectNumberActive(count);

            //SetSelectEffect(-1);

        }


        /// <summary>
        /// 9/25/2023-LYI
        /// 스테이지 번호 버튼 활성화 체크
        /// </summary>
        public void SetButtonSelectNumberActive(int typeButtonCount)
        {
            for (int i = 0; i < list_btn_stageNum_active.Count; i++)
            {
                list_btn_stageNum_active[i].GetComponent<MenuStageNumberButton>().ButtonDeselect();
            }
            for (int i = 0; i < list_btn_stageNum_active.Count; i++)
            {
                gameMgr.objPoolingMgr.ObjectInit(list_btn_stageNum_disable, list_btn_stageNum_active[i], tr_disable);
                // list_btn_stageNum_active[i].SetActive(false);
            }
            list_btn_stageNum_active.Clear();

            if (typeButtonCount == 0)
            {
                return;
            }

            lastClearStage = 0;

            //버튼 배열 반복
            for (int buttonIndex = 0; buttonIndex < typeButtonCount; buttonIndex++)
            {
                int localIndex = buttonIndex; //각 반복 번호 저장용 변수

                //오브젝트 생성
                GameObject go = gameMgr.objPoolingMgr.CreateObject(list_btn_stageNum_disable, btn_originNum, tr_active.position, tr_active);
                go.name = localIndex + "StageButton";
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;

                //버튼 기능 초기화
                MenuStageNumberButton numberButton = go.GetComponent<MenuStageNumberButton>();
                numberButton.Init();

                //버튼 기능 할당
                numberButton.btn_select.onClick.RemoveAllListeners();
                numberButton.btn_select.onClick.AddListener(() => ButtonSelectNumber(localIndex));
                numberButton.btn_select.onClick.AddListener(
                    () => gameMgr.soundMgr.PlaySfx(numberButton.btn_select.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK));
                
                numberButton.txt_number.text = (localIndex + 1).ToString();
                numberButton.txt_selectNumber.text = (localIndex + 1).ToString();

                //스테이지 번호 할당
                numberButton.stageNum = (int)currentStageType * 1000 + buttonIndex + 1;

                //해당하는 스테이지 클리어 여부 체크 및 활성화 변경
                //해당 스테이지번호의 클리어 정보 호출
                numberButton.isClear = ES3.Load<bool>(numberButton.stageNum.ToString(), false);

                //마지막 클리어한 스테이지의 다음스테이지인 경우
                //혹은 첫번째 버튼인 경우
                //노란색 표시, 잠금 해제
                if (lastClearStage + 1 == numberButton.stageNum ||
                    buttonIndex == 0)
                {
                    numberButton.SetButtonState(MenuStageNumberButton.ButtonState.NORMAL);
                }
                else if ((lastClearStage + 1 < numberButton.stageNum && !numberButton.isClear) ||
                    lastClearStage == 0) //클리어한 스테이지가 없는경우
                {   //마지막 클리어한 스테이지와 2단계 차이가 나고 클리어하지 않은 경우

                    numberButton.SetButtonState(MenuStageNumberButton.ButtonState.LOCK);
                }


                //클리어 한 스테이지인 경우
                if (numberButton.isClear)
                {
                    //최종 클리어 스테이지 값 갱신
                    lastClearStage = numberButton.stageNum;
                    numberButton.SetButtonState(MenuStageNumberButton.ButtonState.CLEAR);
                }


                list_btn_stageNum_active.Add(go);
                // list_btn_stageNum_active[i].SetActive(true);
            }

            SetUnlockSprite();

            StartCoroutine(SelectWaitFrame(OnButtonSetEnd));

            StartCoroutine(UpdateScrollButtonActive());
        }

        /// <summary>
        /// 5/17/2024-LYI
        /// Mixed 상태의 스테이지에 대한 할당
        /// </summary>
        /// <param name="typeButtonCount"></param>
        public void SetButtonMixedNumberActive(int typeButtonCount)
        {
            for (int i = 0; i < list_btn_stageNum_active.Count; i++)
            {
                list_btn_stageNum_active[i].GetComponent<MenuStageNumberButton>().ButtonDeselect();
            }
            for (int i = 0; i < list_btn_stageNum_active.Count; i++)
            {
                gameMgr.objPoolingMgr.ObjectInit(list_btn_stageNum_disable, list_btn_stageNum_active[i], tr_disable);
                // list_btn_stageNum_active[i].SetActive(false);
            }
            list_btn_stageNum_active.Clear();

            if (typeButtonCount == 0)
            {
                return;
            }

            lastClearStage = 0;

            //버튼 배열 반복
            for (int buttonIndex = 0; buttonIndex < typeButtonCount; buttonIndex++)
            {
                int localIndex = buttonIndex; //각 반복 번호 저장용 변수

                //오브젝트 생성
                GameObject go = gameMgr.objPoolingMgr.CreateObject(list_btn_stageNum_disable, btn_originNum, tr_active.position, tr_active);
                go.name = (localIndex+1) + "_StageButton";
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;

                //버튼 기능 초기화
                MenuStageNumberButton numberButton = go.GetComponent<MenuStageNumberButton>();
                numberButton.Init();

                //버튼 기능 할당
                numberButton.btn_select.onClick.RemoveAllListeners();
                numberButton.btn_select.onClick.AddListener(() => ButtonSelectNumber(localIndex));
                numberButton.btn_select.onClick.AddListener(
                    () => gameMgr.soundMgr.PlaySfx(numberButton.btn_select.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK));

                numberButton.txt_number.text = (localIndex + 1).ToString();
                numberButton.txt_selectNumber.text = (localIndex + 1).ToString();


                //번호와 dictionary 키 대응
                int dicKey = (int)currentStageType * 1000 + localIndex + 1;
                if (gameMgr.dataMgr.dic_mixedToStage.ContainsKey(dicKey))
                {
                    //스테이지 번호 할당
                    numberButton.stageNum = gameMgr.dataMgr.dic_mixedToStage[dicKey];
                }
                else
                {
                    //스테이지 번호 할당
                    numberButton.stageNum = dicKey;
                }

                //해당하는 스테이지 클리어 여부 체크 및 활성화 변경
                //해당 스테이지번호의 클리어 정보 호출
                numberButton.isClear = ES3.Load<bool>(dicKey.ToString(), false);

                //마지막 클리어한 스테이지의 다음스테이지인 경우
                //혹은 첫번째 버튼인 경우
                //노란색 표시, 잠금 해제
                if (lastClearStage + 1 == dicKey ||
                    localIndex == 0)
                {
                    numberButton.SetButtonState(MenuStageNumberButton.ButtonState.NORMAL);
                   // numberButton.SetButtonState(MenuStageNumberButton.ButtonState.CLEAR);
                }
                else if ((lastClearStage + 1 < dicKey && !numberButton.isClear) ||
                    lastClearStage == 0) //클리어한 스테이지가 없는경우
                {   //마지막 클리어한 스테이지와 2단계 차이가 나고 클리어하지 않은 경우

                    //6/12/2024-LYI 전체 잠금 일시 해제, 테스트 후 수정할 것
                    //numberButton.SetButtonState(MenuStageNumberButton.ButtonState.NORMAL);
                    numberButton.SetButtonState(MenuStageNumberButton.ButtonState.LOCK);
                   //numberButton.SetButtonState(MenuStageNumberButton.ButtonState.CLEAR);
                }


                //클리어 한 스테이지인 경우
                if (numberButton.isClear)
                {
                    //최종 클리어 스테이지 값 갱신
                    lastClearStage = dicKey;
                    numberButton.SetButtonState(MenuStageNumberButton.ButtonState.CLEAR);
                }


                list_btn_stageNum_active.Add(go);
                // list_btn_stageNum_active[i].SetActive(true);
            }

            SetUnlockSprite();

            StartCoroutine(SelectWaitFrame(OnButtonSetEnd));

            StartCoroutine(UpdateScrollButtonActive());
        }

        void OnButtonSetEnd()
        {
            currentStageNum = 1;
            ChangeCurrentStageText();
            SetScrollHeight();

            if (gameMgr.isTutorial)
            {
                tutorialMarkers.SetActive(true);

                //SetSelectEffect(-1);
            }
            else
            {
                tutorialMarkers.SetActive(false);
                list_btn_stageNum_active[0].GetComponent<MenuStageNumberButton>().ButtonSelect();
                //SetSelectEffect(0);
            }
        }


        IEnumerator SelectWaitFrame(UnityAction action)
        {
            yield return new WaitForEndOfFrame();
            if (action != null)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// 2/28/2024-LYI
        /// 선택 숫자 버튼 기능 할당 함수
        /// </summary>
        /// <param name="num">버튼 번호</param>
        public void ButtonSelectNumber(int num)
        {
            //클리어한 스테이지가 없는 경우 첫 번호 아니면 제외
            if (lastClearStage == 0 &&
                num > 0)
            {
                return;
            }


            MenuStageNumberButton numberButton = list_btn_stageNum_active[num].GetComponent<MenuStageNumberButton>();
            ////스테이지 번호
            //int stageNum = (int)currentStageType * 1000 + num + 1;

            // 잠김 상태 선택 막기용 예외처리
            //if (lastClearStage != 0 &&
            //    lastClearStage + 1 < numberButton.stageNum)
            //{
            //    return;
            //}
            if (lastClearStage != 0 && 
                numberButton.statButton == MenuStageNumberButton.ButtonState.LOCK)
            {
                return;
            }

            currentStageNum = num + 1;

            ChangeCurrentStageText();


            for (int i = 0; i < list_btn_stageNum_active.Count; i++)
            {
                list_btn_stageNum_active[i].GetComponent<MenuStageNumberButton>().ButtonDeselect();
            }
            numberButton.ButtonSelect();

           // SetSelectEffect(currentStageNum - 1);
        }



        /// <summary>
        /// 9/22/2023-LYI
        /// 게임 시작 버튼을 눌렀을 경우
        /// </summary>
        public void ButtonMenuStart()
        {
            if (gameMgr.playMgr.statPlay != PlayStatus.NONE)
            {
                return;
            }

            ES3.Save(Constants.ES3.LAST_STAGE_TYPE, currentStageType);
            ES3.Save(Constants.ES3.LAST_STAGE_NUM, currentStageNum);

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = null;

            //플레이 기록이 없을 경우
            if (currentStageType == StageType.NONE ||
               currentStageNum == 0)
            {
                //미로 스테이지 1 게임시작
                gameMgr.GameStart(1001);
            }
            else
            {
                //게임 시작
                int stage = (int)currentStageType * 1000 + currentStageNum;
                if (currentStageType == StageType.MIXED)
                {
                    if (gameMgr.dataMgr.dic_mixedToStage.ContainsKey(stage))
                    {
                        stage = gameMgr.dataMgr.dic_mixedToStage[stage];
                    }
                }

                gameMgr.GameStart(stage);
            }
        }

        /// <summary>
        /// 9/22/2023-LYI
        /// 스테이지 선택 후 시작 버튼을 눌렀을 경우
        /// </summary>
        //public void ButtonSelectStart()
        //{
        //    //선택이 안됐으면 돌리기
        //    if (currentStageNum == 0 ||
        //        currentStageType == StageType.NONE)
        //    {
        //        Debug.Log("Theres no selected stage");
        //    }
        //    else
        //    {
        //        ES3.Save("LastStageType", currentStageType);
        //        ES3.Save("LastStageNum", currentStageNum);

        //        //게임 시작
        //        int stage = (int)currentStageType * 1000 + currentStageNum;
        //        gameMgr.GameStart(stage);
        //    }
        //}



        #endregion

        /// <summary>
        /// 9/22/2023-LYI
        /// 선택된 스테이지 번호에 표시
        /// 6/26/2024-LYI
        /// 이펙트 변경으로 주석 처리
        /// </summary>
        /// <param name="num"></param>
        public void SetSelectEffect(int num)
        {
            if (num == -1)
            {
                efx_select.SetActive(false);
                return;
            }
            efx_select.transform.position =
                list_btn_stageNum_active[num].GetComponent<MenuStageNumberButton>().img_selected.transform.position;
            efx_select.SetActive(true);
        }


        /// <summary>
        /// 9/22/2023-LYI
        /// 각 종목에 따른 스테이지 버튼 갯수 갱신
        /// </summary>
        //void ChangeStageStats()
        //{
        //    for (int i = 0; i < list_btn_stageNum_active.Count; i++)
        //    {
        //        list_btn_stageNum_active[i].gameObject.SetActive(false);
        //    }
        //}

        /// <summary>
        /// 5/17/2024-LYI
        /// 스크롤 렉트 사이즈 변경
        /// </summary>
        public void SetScrollHeight()
        {
            int vCount = 0;
            if (list_btn_stageNum_active.Count % 5 == 0)
            {
                vCount = list_btn_stageNum_active.Count / 5;
            }
            else
            {
                vCount = list_btn_stageNum_active.Count / 5 + 1;
            }

            scrollViewport.sizeDelta = new Vector2(scrollViewport.sizeDelta.x, 30 + 120 * vCount);
        }


        /// <summary>
        /// 7/15/2024-LYI
        /// 스크롤되는 버튼들 활성화 여부 변경(Update)
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateScrollButtonActive()
        {
            WaitForSeconds second = new WaitForSeconds(0.1f);

            while (true)
            {
                if (GameManager.Instance.statGame == GameStatus.MENU)
                {
                    if (list_btn_stageNum_active.Count > 0)
                    {
                        for (int i = 0; i < list_btn_stageNum_active.Count; i++)
                        {
                            if (list_btn_stageNum_active[i].transform.position.z > tr_scrollTop.position.z || 
                                list_btn_stageNum_active[i].transform.position.z < tr_scrollBottom.position.z)
                            {
                                list_btn_stageNum_active[i].transform.GetChild(0).gameObject.SetActive(false);
                            }
                            else
                            {
                                list_btn_stageNum_active[i].transform.GetChild(0).gameObject.SetActive(true);
                            }
                        }

                    }
                }

                yield return second;
            }

        }


        /// <summary>
        /// 7/16/2024-LYI
        /// 잠금해제 캐릭터 아이콘 위치 조정
        /// </summary>
        void SetUnlockSprite()
        {
            float imgZ = -45f;
            float imgY = 5f;

            //MenuStageNumberButton btn;

            for (int i = 0; i < arr_unlockSprite.Length -1; i++)
            {
                if (gameMgr.playMgr.cheeringSeat.arr_cheeringCharacter[i].lockStageNum == 0)
                {
                    //lock이 0번으로 풀리는 경우
                    arr_unlockSprite[i].transform.SetParent(list_btn_stageNum_active[0].transform.GetChild(0));
                }
                else
                {
                    //해당 언락 번호로 parent
                    arr_unlockSprite[i].transform.SetParent(
                        list_btn_stageNum_active[gameMgr.playMgr.cheeringSeat.arr_cheeringCharacter[i].lockStageNum - 1].transform.GetChild(0));

                }

                //로컬포지션으로 위치 보정
                arr_unlockSprite[i].transform.localPosition = Vector3.zero +Vector3.up * imgY + Vector3.forward * imgZ;

                //btn = list_btn_stageNum_active[gameMgr.playMgr.cheeringSeat.arr_cheeringCharacter[i].lockStageNum - 1].GetComponent<MenuStageNumberButton>();

                //if (btn.statButton== MenuStageNumberButton.ButtonState.LOCK)
                //{
                //    arr_unlockSprite[i].transform.SetParent(btn.transform.GetChild(0));
                //}
                //else
                //{
                //    arr_unlockSprite[i].transform.SetParent(btn.transform.GetChild(0).GetChild(1));
                //}

                //arr_unlockSprite[i].transform.localPosition = Vector3.zero + Vector3.up * imgHeight2;
            }

            arr_unlockSprite[arr_unlockSprite.Length - 1].transform.SetParent(
                list_btn_stageNum_active[gameMgr.playMgr.cheeringSeat.groundTena.lockStageNum - 1].transform.GetChild(0));
            arr_unlockSprite[arr_unlockSprite.Length - 1].transform.localPosition = Vector3.zero + Vector3.up * imgY + Vector3.forward * imgZ;

            //btn = list_btn_stageNum_active[gameMgr.playMgr.cheeringSeat.groundTena.lockStageNum - 1].GetComponent<MenuStageNumberButton>();

            //if (btn.statButton == MenuStageNumberButton.ButtonState.LOCK)
            //{
            //    arr_unlockSprite[arr_unlockSprite.Length - 1].transform.SetParent(btn.transform.GetChild(0));
            //}
            //else
            //{
            //    arr_unlockSprite[arr_unlockSprite.Length - 1].transform.SetParent(btn.transform.GetChild(0).GetChild(1));
            //}

            //arr_unlockSprite[arr_unlockSprite.Length - 1].transform.localPosition = Vector3.zero + Vector3.up * imgY;

        }


        /// <summary>
        /// 5/22/2024-LYI
        /// 크레딧 열기
        /// </summary>
        public void OpenCredit()
        {
            gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_CREDIT));
            StartCoroutine(SetCreditActve(true));
        }
        public void CloseCredit()
        {
            gameMgr.soundMgr.ChangeBGMAudioClip(gameMgr.soundMgr.LoadClip(Constants.Sound.BGM_MENU));
            StartCoroutine(SetCreditActve(false));
        }


        /// <summary>
        /// 5/22/2024-LYI
        /// 서서히 메뉴 교체
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        IEnumerator SetCreditActve(bool isActive)
        {
            if (isActive)
            {
                MenuFade(false);
                yield return new WaitForSeconds(0.1f);
                yield return fadeCoroutine;
                CreditFade(true);
            }
            else
            {
                CreditFade(false);
                yield return new WaitForSeconds(0.1f);
                yield return fadeCoroutine;
                MenuFade(true);
            }
        }

    }
}