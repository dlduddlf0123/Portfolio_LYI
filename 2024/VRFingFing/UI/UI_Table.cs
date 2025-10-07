using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using MoreMountains.Feedbacks;

using VRTokTok.Character;
using VRTokTok.Manager;

namespace VRTokTok.UI
{
    /// <summary>
    /// 10/25/2023-LYI
    /// 테이블 하단의 UI 작동
    /// </summary>
    public class UI_Table : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Left Table")]
        [SerializeField]
        Button btn_menu;
        [SerializeField]
        GameObject btn_menu_disable;
        [SerializeField]
        Button btn_replay;
        [SerializeField]
        GameObject btn_replay_disable;
        [SerializeField]
        Button btn_skip;
        [SerializeField]
        GameObject btn_skip_disable;
        [SerializeField]
        TextMeshProUGUI txt_currentStage;
        [SerializeField]
        Toggle tog_passthrough;
        [SerializeField]
        Button btn_tutorial;

        [Header("Right Table")]
        [SerializeField]
        Toggle tog_bgm;
        [SerializeField]
        Toggle tog_sfx;
        [SerializeField]
        Button btn_select;
        [SerializeField]
        Button btn_credit;
        [SerializeField]
        Toggle tog_handChange;


        [Header("Select Canvas")]
        [SerializeField]
        GameObject selectCanvas;
        [SerializeField]
        MMF_Player mmf_selectOn;
        [SerializeField]
        MMF_Player mmf_selectOff;
        [SerializeField]
        Button[] arr_btnCharacterSelect;
        [SerializeField]
        GameObject[] arr_selectImg;

        public HeaderType selectedHeaderType = HeaderType.NONE;
        public bool isSelectMode = false;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        public void TableUIInit()
        {
            List<Button> list_button_sfx = new List<Button>();
            list_button_sfx.Add(btn_menu);
            list_button_sfx.Add(btn_replay);
            list_button_sfx.Add(btn_skip);
            list_button_sfx.Add(btn_select);
            list_button_sfx.Add(btn_credit);
            list_button_sfx.Add(btn_tutorial);
            for (int i = 0; i < list_button_sfx.Count; i++)
            {
                int a = i;
                list_button_sfx[i].onClick.AddListener(() => gameMgr.soundMgr.PlaySfx(list_button_sfx[a].transform.position,
                    Constants.Sound.SFX_UI_BUTTON_CLICK));
            }

            btn_menu.onClick.AddListener(gameMgr.playMgr.EndGame);
            btn_replay.onClick.AddListener(ButtonReplay);
            btn_skip.onClick.AddListener(ButtonSkip);
            btn_tutorial.onClick.AddListener(ButtonTutorial);

            tog_passthrough.onValueChanged.AddListener(TogglePassthrough);
            tog_bgm.onValueChanged.AddListener(ToggleBGM);
            tog_sfx.onValueChanged.AddListener(ToggleSFX);

            GameManager.Instance.soundMgr.LoadVolume();

            tog_bgm.isOn = (GameManager.Instance.soundMgr.bgmVolume > 0) ? true : false;
            tog_sfx.isOn = (GameManager.Instance.soundMgr.sfxVolume > 0) ? true : false;
            tog_handChange.isOn = GameManager.Instance.playMgr.tokMgr.isLeftHanded;
            tog_handChange.onValueChanged.AddListener(ToggleHandChange);


            btn_select.onClick.AddListener(ButtonSelect);

            btn_select.gameObject.SetActive(true);

            btn_credit.onClick.AddListener(ButtonCredit);

            //#if UNITY_ANDROID
            //            btn_select.gameObject.SetActive(false);
            //#else
            //            btn_select.gameObject.SetActive(true);
            //#endif

            for (int i = 0; i < arr_btnCharacterSelect.Length; i++)
            {
                int a = i;
                arr_btnCharacterSelect[i].onClick.AddListener(() => ButtonCharacterSelect(a));
            }

            selectedHeaderType = ES3.Load(Constants.ES3.LAST_SELECT_HEADER, HeaderType.KANTO);
            ButtonCharacterSelect((int)selectedHeaderType - 1, false);

            mmf_selectOff.Events.OnComplete.AddListener(() => selectCanvas.gameObject.SetActive(false));
        }


        /// <summary>
        /// 6/13/2024-LYI
        /// 게임 플레이와 메뉴 상태에서 호출
        /// 게임 플레이 메뉴 활성화 설정
        /// </summary>
        /// <param name="isActive"></param>
        public void SetLeftTableMenuButton(bool isActive)
        {
            btn_menu_disable.gameObject.SetActive(!isActive);
            btn_menu.gameObject.SetActive(isActive);

            btn_replay_disable.gameObject.SetActive(!isActive);
            btn_replay.gameObject.SetActive(isActive);

            btn_skip_disable.gameObject.SetActive(!isActive);
            btn_skip.gameObject.SetActive(isActive);

            txt_currentStage.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 6/13/2024-LYI
        /// 스테이지 번호 표시 변경
        /// currentStage에서 번호 넣으면 9000번대로 변경
        /// </summary>
        /// <param name="stageNum"></param>
        public void ChangeCurrentStageText(int stageNum)
        {
            int mixedNum = 0;

            if (gameMgr.dataMgr.dic_StageToMixed.ContainsKey(stageNum))
            {
                mixedNum = gameMgr.dataMgr.dic_StageToMixed[stageNum];
                mixedNum -= 9000; //9000빼서 2자리, 3자리수로 변경
            }

            string s = mixedNum.ToString(); //한글자 떼고 스트링

            txt_currentStage.text = "Stage\n" + s;
        }

        /// <summary>
        /// 6/13/2024-LYI
        /// 현재 스테이지 다시시작
        /// </summary>
        public void ButtonReplay()
        {
            if (gameMgr.playMgr.statPlay == PlayStatus.NONE ||
              gameMgr.playMgr.statPlay == PlayStatus.READY ||
              gameMgr.playMgr.statPlay == PlayStatus.LOADING ||
              gameMgr.statGame == GameStatus.MENU)
            {
                //이미 메뉴면 돌려보냄
                //로딩 중에 누르면 돌려보냄
                return;
            }

            gameMgr.playMgr.currentStage.RestartStage(false);
        }


        /// <summary>
        /// 6/13/2024-LYI
        /// 현재 스테이지 스킵, 다음스테이지로
        /// </summary>
        public void ButtonSkip()
        {
            if (gameMgr.playMgr.statPlay == PlayStatus.NONE ||
               gameMgr.playMgr.statPlay == PlayStatus.READY ||
               gameMgr.playMgr.statPlay == PlayStatus.LOADING ||
               gameMgr.statGame == GameStatus.MENU)
            {
                //이미 메뉴면 돌려보냄
                //로딩 중에 누르면 돌려보냄
                return;
            }
            gameMgr.playMgr.currentStage.SkipStage();
        }


        /// <summary>
        /// 7/5/2024-LYI
        /// 튜토리얼 스테이지(1000) 플레이
        /// </summary>
        public void ButtonTutorial()
        {
            gameMgr.ChangeGameStat(GameStatus.GAME);
            gameMgr.GameStart(1000);
        }


        /// <summary>
        /// 10/25/2023-LYI
        /// 패스스루 기능 활성화
        /// </summary>
        public void TogglePassthrough(bool isActive)
        {
            if (isActive)
            {
                RenderSettings.skybox = null;
                gameMgr.tableMgr.passThroughObjects.SetActive(false);

                gameMgr.soundMgr.PlaySfx(tog_passthrough.transform.position,Constants.Sound.SFX_UI_BUTTON_CLICK);
                gameMgr.mainCam.clearFlags = CameraClearFlags.SolidColor; //갱신해서 작동시키기 위함
                gameMgr.ovrMgr.isInsightPassthroughEnabled = true;
            }
            else
            {
                RenderSettings.skybox = gameMgr.tableMgr.mat_skybox;
                gameMgr.tableMgr.passThroughObjects.SetActive(true);

                gameMgr.soundMgr.PlaySfx(tog_passthrough.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK);
                gameMgr.mainCam.clearFlags = CameraClearFlags.Skybox; //갱신해서 작동시키기 위함
                gameMgr.ovrMgr.isInsightPassthroughEnabled = false;
            }
        }

        public void ToggleBGM(bool isActive)
        {
            gameMgr.soundMgr.SetBGMVolume(isActive ? 0.7f : 0);
            gameMgr.soundMgr.PlaySfx(tog_bgm.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK);
            GameManager.Instance.soundMgr.SaveVolume();
        }

        public void ToggleSFX(bool isActive)
        {
            gameMgr.soundMgr.sfxVolume = isActive ? 1 : 0;
            gameMgr.soundMgr.PlaySfx(tog_sfx.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK);
            GameManager.Instance.soundMgr.SaveVolume();
        }
        public void ToggleHandChange(bool isLeft)
        {
            gameMgr.playMgr.tokMgr.ChangeMainHand(isLeft);
            gameMgr.soundMgr.PlaySfx(tog_sfx.transform.position, Constants.Sound.SFX_UI_BUTTON_CLICK);
        }


        /// <summary>
        /// 5/8/2024-LYI
        /// Select 버튼 클릭 시 동작
        /// </summary>
        public void ButtonSelect()
        {
            SetLeftTableMenuButton(false);

            //6/18/2024-LYI
            //게임 플레이 도중인 경우
            if (gameMgr.statGame == GameStatus.GAME)
            {
                if (!gameMgr.tableMgr.playTable.isTableDown)
                {
                    gameMgr.tableMgr.playTable.SetTableDown(()=> SetSelectActive(true));
                }
                else
                {
                    SetSelectActive(false);

                    if (gameMgr.playMgr.currentStage != null)
                    {
                        gameMgr.playMgr.currentStage.gameObject.SetActive(true);
                    }
                    gameMgr.tableMgr.playTable.SetTableUp();
                    SetLeftTableMenuButton(true);
                }
            }
            else
            {
                SetSelectActive(!isSelectMode);
            }
        }


        /// <summary>
        /// 5/22/2024-LYI
        /// 크레딧 창 열기
        /// </summary>
        public void ButtonCredit()
        {
            gameMgr.tableMgr.ui_menu.OpenCredit();
        }


        /// <summary>
        /// 5/8/2024-LYI
        /// 캐릭터 선택 활성화 변경
        /// </summary>
        /// <param name="isActive"></param>
        public void SetSelectActive(bool isActive)
        {
            if (isActive)
            {
                mmf_selectOn.PlayFeedbacks();
                selectCanvas.SetActive(true);
                isSelectMode = true;
                gameMgr.playMgr.cheeringSeat.headerSelect.ActiveHeaderSelect();
            }
            else
            {
                mmf_selectOff.PlayFeedbacks();
                isSelectMode = false;
                gameMgr.playMgr.cheeringSeat.headerSelect.DisableHeaderSelect();
            }
            
        }


        public void ButtonCharacterSelect(int num, bool sfx = true)
        {
            //if (selectedHeaderType == (HeaderType)num+1)
            //{
            //    return;
            //}
            if (gameMgr.addressableMgr.isLoadComplete && sfx)
            {
                gameMgr.soundMgr.PlaySfx(arr_btnCharacterSelect[num].transform.position,
                    Constants.Sound.SFX_UI_BUTTON_CLICK);
            }

            selectedHeaderType = (HeaderType)num + 1;
            gameMgr.playMgr.selectCharacterType = selectedHeaderType;

            ES3.Save(Constants.ES3.LAST_SELECT_HEADER, selectedHeaderType);

            for (int i = 0; i < arr_btnCharacterSelect.Length; i++)
            {
                arr_btnCharacterSelect[i].gameObject.SetActive(true);
                arr_selectImg[i].SetActive(false);
            }

            arr_btnCharacterSelect[num].gameObject.SetActive(false);
            arr_selectImg[num].SetActive(true);

            //이후 적용 동작
            //게임 플레이 도중 캐릭터 선택을 하는 경우
            if (gameMgr.playMgr.statPlay == PlayStatus.PLAY)
            {
                gameMgr.playMgr.currentStage.RestartStage(false);

            }


        }
    }
}