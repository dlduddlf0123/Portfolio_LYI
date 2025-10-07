using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using VRTokTok.Character;
using UnityEngine.Events;
using Oculus.Interaction;

using MoreMountains.Feedbacks;
using UnityEngine.UI;

namespace VRTokTok
{

    /// <summary>
    /// 10/30/2023-LYI
    /// 캐릭터 선택 관리 코드
    /// </summary>
    public class HeaderSelect : MonoBehaviour
    {
        GameManager gameMgr;
       public  CheeringSeat cheeringSeat;

        //응원 칸 이동 동작
        public MMF_Player mmf_selectMode;
        public MMF_Player mmf_cheeringMode;

        //캐릭터 선택 시 파티클
        public ParticleSystem efx_onSelect;
        public ParticleSystem efx_selectAura;

        //확인 버튼
        public GameObject objConfirm;
        public Button btn_confirm;

        //선택 상태 확인
        public HeaderType lastHeaderType;
        public HeaderType clickedHeaderType;

        public HeaderType selectedHeaderType;

        Coroutine currentCoroutine = null;

        /// <summary>
        /// 캐릭터 선택이 가능한 상태인가?
        /// </summary>
        public bool isSelectMode = false;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            cheeringSeat = GetComponent<CheeringSeat>();

        }


        void Start()
        {
            Init();
        }

        void Init()
        {
            lastHeaderType = HeaderType.NONE;
            clickedHeaderType = HeaderType.NONE;
            selectedHeaderType = ES3.Load(Constants.ES3.LAST_SELECT_HEADER, HeaderType.ZINO); //최초 선택 지노
            gameMgr.playMgr.selectCharacterType = selectedHeaderType;

            btn_confirm.onClick.AddListener(ButtonConfirm);
            btn_confirm.onClick.AddListener(()=>gameMgr.soundMgr.PlaySfx(btn_confirm.transform.position,
                    Constants.Sound.SFX_UI_BUTTON_CLICK));
            objConfirm.gameObject.SetActive(false);
        }

        Tok_CheeringCharacter GetCharacter(HeaderType type)
        {
            switch (type)
            {
                case HeaderType.KANTO:
                    return cheeringSeat.arr_cheeringCharacter[0];
                case HeaderType.ZINO:
                    return cheeringSeat.arr_cheeringCharacter[1];
                case HeaderType.OODADA:
                    return cheeringSeat.arr_cheeringCharacter[2];
                case HeaderType.COCO:
                    return cheeringSeat.arr_cheeringCharacter[3];
                case HeaderType.DOINK:
                    return cheeringSeat.arr_cheeringCharacter[4];
                case HeaderType.TENA:
                    return cheeringSeat.headTena;
                case HeaderType.NONE:
                default:
                    return cheeringSeat.arr_cheeringCharacter[0];
            }
        }


        /// <summary>
        /// 10/27/2023-LYI
        /// Call from select button/lever
        /// 선택 버튼이 눌리면 스테이지 이동, 캐릭터 선택 기능 활성화
        /// </summary>
        public void ActiveHeaderSelect()
        {
            Debug.Log("Active HeaderSelect()");
            selectedHeaderType = gameMgr.playMgr.selectCharacterType;

            cheeringSeat.ActiveCheeringSeat(true);

            mmf_selectMode.PlayFeedbacks();
            isSelectMode = true;


            efx_selectAura.transform.SetParent(GetCharacter(selectedHeaderType).tr_particleRoot);
            efx_selectAura.transform.localPosition = Vector3.zero;
            //efx_selectAura.transform.position = GetCharacter(selectedHeaderType).transform.position + Vector3.up * 0.04f;
            efx_selectAura.gameObject.SetActive(true);

            objConfirm.gameObject.SetActive(true);

            cheeringSeat.ResetCheeringCharacter();

            //gameMgr.ChangeGameStat(GameStatus.SELECT);

            //게임 플레이 도중 캐릭터 선택을 하는 경우
            //if (gameMgr.playMgr.statPlay == PlayStatus.PLAY)
            //{
            //    for (int i = 0; i < gameMgr.playMgr.list_activeCharacter.Count; i++)
            //    {
            //        gameMgr.playMgr.list_activeCharacter[i].m_character.CharacterDisappear();
            //    }
            //}
        }

        /// <summary>
        /// 10/27/2023-LYI
        /// Call from confirm button
        /// 완료 버튼 혹은 셀렉트 버튼 재 클릭 시 호출
        /// 무대 원래 위치로 다시 이동
        /// 아우라 등 이펙트 비활성화
        /// 선택된 캐릭터 저장
        /// </summary>
        public void DisableHeaderSelect()
        {
            //#if UNITY_ANDROID
            //            cheeringSeat.ActiveCheeringSeat(false);
            //#endif

            Debug.Log("Deactive HeaderSelect()");

            isSelectMode = false;
            mmf_cheeringMode.PlayFeedbacks();

            efx_selectAura.Stop();
            efx_selectAura.gameObject.SetActive(false);
            efx_onSelect.Stop();

            objConfirm.gameObject.SetActive(false);

            ES3.Save(Constants.ES3.LAST_SELECT_HEADER, selectedHeaderType);

            cheeringSeat.CheckCharacterLock();

            //게임 플레이 도중 캐릭터 선택을 하는 경우
            if (gameMgr.playMgr.statPlay == PlayStatus.PLAY)
            {
                cheeringSeat.SetCheeringCharacter(gameMgr.playMgr.currentStage.GetActiveHeaderTypes());
               // gameMgr.playMgr.currentStage.RestartStage(false);

                //gameMgr.ChangeGameStat(GameStatus.GAME);
            }
            else
            {
                //gameMgr.ChangeGameStat(GameStatus.MENU);
            }
        }


        /// <summary>
        /// 10/30/2023-LYI
        /// 확인 버튼 클릭 신
        /// </summary>
        public void ButtonConfirm()
        {
            gameMgr.tableMgr.ui_table.ButtonSelect();
        }


        /// <summary>
        /// 10/27/2023-LYI
        /// TokSelect에서 호출, 캐릭터 터치 했을 때의 처리
        /// </summary>
        /// <param name="header"></param>
        public void SelectTok(Tok_CheeringCharacter header)
        {
            if (!isSelectMode)
            {
                return;
            }
            clickedHeaderType = header.typeHeader; //선택 할당


            //마지막 선택과 다른 경우
            //if (lastHeaderType != clickedHeaderType)
            //{
            //    //1차 선택
            //    //해당 캐릭터 어텐션, 제자리 점프
            //    //선택 이펙트 표시
            //    header.PlayCheeringAnim(0);

            //    efx_onSelect.transform.position = header.transform.position + Vector3.up * 0.04f;
            //    efx_onSelect.Play();
            //}
            ////마지막 선택과 같은 경우
            //else if (lastHeaderType == clickedHeaderType)
            //{
            //    //2차 선택
            //    //선택된 캐릭터 변경
            //    //해당 캐릭터 신나는 모션
            //    //선택 이펙트 표시
            //    //아우라 위치 변경, 활성화


            //    selectedHeaderType = clickedHeaderType;
            //    gameMgr.playMgr.selectCharacterType = selectedHeaderType;

            //    header.PlayCheeringAnim(2);

            //    efx_onSelect.transform.position = header.transform.parent.position + Vector3.up * 0.04f;
            //    efx_onSelect.Play();

            //    efx_selectAura.transform.position = header.transform.parent.position + Vector3.up * 0.04f;
            //    efx_selectAura.Play();
            //}

            selectedHeaderType = clickedHeaderType;
            gameMgr.playMgr.selectCharacterType = selectedHeaderType;

            header.PlayCheeringAnim(2);

            //효과음 추가
            gameMgr.soundMgr.PlaySfx(header.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK);

            efx_onSelect.transform.position = header.transform.parent.position + Vector3.up * 0.04f;
            efx_onSelect.Play();


            efx_selectAura.transform.SetParent(header.tr_particleRoot);
            efx_selectAura.transform.localPosition = Vector3.zero;
            //efx_selectAura.transform.position = header.transform.parent.position + Vector3.up * 0.04f;
            efx_selectAura.gameObject.SetActive(true);
            efx_selectAura.Play();


            lastHeaderType = clickedHeaderType; //선택 밀기

            //if (currentCoroutine != null)
            //{
            //    StopCoroutine(currentCoroutine);
            //}
            //currentCoroutine = StartCoroutine(SelectTimer()); //선택 타이머 가동
        }

        /// <summary>
        /// 10/27/2023-LYI
        /// 선택 취소 타이머
        /// 더블클릭 체크용
        /// </summary>
        /// <returns></returns>
        IEnumerator SelectTimer()
        {
            yield return new WaitForSeconds(2f);
            lastHeaderType = HeaderType.NONE;
        }
    }
}