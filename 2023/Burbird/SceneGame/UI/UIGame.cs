using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System.Linq;
using MoreMountains.InventoryEngine;
using TMPro;
using System;

namespace Burbird
{
    /// <summary>
    /// 게임 내에서 표시되는 UI 관리
    /// 돈, 일시정지, 경험치 바, 데미지 표시 풀링 등
    /// </summary>
    public class UIGame : MonoBehaviour
    {
        StageManager stageMgr;

        [Header("Canvas")]
        public Canvas canvas_game;
        public UIPerk canvas_perk;
        public Canvas canvas_pause;
        public RectTransform canvas_result;
        public RectTransform canvas_count;
        public Canvas canvas_loading;


        [Header("Damage Text")]
        //데미지 텍스트 관련
        public GameObject origin_damageCanvas;
        Text txt_damage;
        Queue<GameObject> queue_canvas_damage = new Queue<GameObject>();

        public Transform tr_damageTextPool;

        [Header("Perk Description")]
        //4/14/2023-LYI
        //퍽 획득 시 퍽 이름, 효과 표시
        [SerializeField]
        RectTransform ui_perkDescription;
        [SerializeField]
        Image perkDescription_img_bg;
        [SerializeField]
        CanvasGroup perkDescription_group_texts;
        [SerializeField]
        TextMeshProUGUI perkDescription_txt_title;
        [SerializeField]
        TextMeshProUGUI perkDescription_txt_description;
        [SerializeField]
        Queue<Func<IEnumerator>> queue_perkCoroutine = new();
        int i_perkDescriptionMessageQueue = 0;

        //경험치 바 관련
        public RectTransform game_rt_level { get; set; }
        public Image game_level_img { get; set; }
        public TextMeshProUGUI game_level_txt { get; set; }
        Button game_btn_pause;

        public RectTransform game_rt_boss { get; set; }
        public Image game_boss_img_hp { get; set; }

        //코인 관련
        TextMeshProUGUI game_txt_coin { get; set; }


        //원본 퍽 프리팹
        [Header("ETC")]
        public GameObject perk_pause;
        public AudioClip sfx_click;
        public VariableJoystick joystick;

        //일시정지 창 관련
        TextMeshProUGUI pause_txt_room;
        public RectTransform pause_tr_perks { get; set; }
        Button pause_btn_resume;
        Button pause_btn_home;
        Button pause_tog_bgm;
        Button pause_tog_sfx;
        Button pause_tog_controll;


        //결과창 관련
        TextMeshProUGUI result_txt_stageName;
        TextMeshProUGUI result_txt_lastRoom;
        TextMeshProUGUI result_txt_clear;
        Button result_btn_home;

        //카운트다운 관련
        TextMeshProUGUI count_txt_countDown;
        Button count_btn_ad;
        Button count_btn_diamond;

        Coroutine countDownCoroutine = null;

        private void Awake()
        {
            stageMgr = StageManager.Instance;

            //Game
            game_rt_level = canvas_game.transform.GetChild(0).GetComponent<RectTransform>();
            game_level_img = game_rt_level.GetChild(0).GetComponent<Image>();
            game_level_txt = game_rt_level.GetChild(1).GetComponent<TextMeshProUGUI>();
            
            game_btn_pause = canvas_game.transform.GetChild(1).GetComponent<Button>();
            game_txt_coin = canvas_game.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();

            game_rt_boss = canvas_game.transform.GetChild(3).GetComponent<RectTransform>();
            game_boss_img_hp = game_rt_boss.GetChild(0).GetComponent<Image>();

            //Pause
            pause_txt_room = canvas_pause.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            pause_tr_perks = canvas_pause.transform.GetChild(2).GetComponent<RectTransform>();

            pause_btn_resume = canvas_pause.transform.GetChild(3).GetComponent<Button>();
            pause_btn_home = canvas_pause.transform.GetChild(4).GetComponent<Button>();
            pause_tog_bgm = canvas_pause.transform.GetChild(5).GetComponent<Button>();
            pause_tog_sfx = canvas_pause.transform.GetChild(6).GetComponent<Button>();
            pause_tog_controll = canvas_pause.transform.GetChild(7).GetComponent<Button>();

            //Result
            result_txt_stageName = canvas_result.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
            result_txt_lastRoom = canvas_result.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
            result_txt_clear = canvas_result.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
            result_btn_home = canvas_result.GetChild(4).GetComponent<Button>();

            //Count
            count_txt_countDown = canvas_count.GetChild(3).GetComponent<TextMeshProUGUI>();
            count_btn_ad = canvas_count.GetChild(4).GetComponent<Button>();
            count_btn_diamond = canvas_count.GetChild(5).GetComponent<Button>();
        }

        private void Start()
        {
            ChangeEXPGauge(0);
            ChangeLevelText(1);
            ChangeCoinText(0);

            //Game
            game_btn_pause.onClick.AddListener(() => PauseActiveButton(true));
            //ToggleBossHP(false);
            ui_perkDescription.gameObject.SetActive(false);

            //Pause
            pause_btn_resume.onClick.AddListener(() => PauseActiveButton(false));
            pause_btn_home.onClick.AddListener(PauseHomeButton);
            pause_tog_bgm.onClick.AddListener(PauseBGMToggle);
            pause_tog_sfx.onClick.AddListener(PauseSFXToggle);
            pause_tog_controll.onClick.AddListener(PauseControllToggle);

            //Result
            result_txt_stageName.text = stageMgr.stageName;
            result_btn_home.onClick.AddListener(ButtonResultHome);

            //Count
            count_btn_ad.onClick.AddListener(ButtonCountAD);
            count_btn_diamond.onClick.AddListener(ButtonCountDiamond);

            AddButtonSound();
        }

        public void GameUIInit()
        {
            GameManager.Instance.soundMgr.LoadVolume();
            bool isBGMOn = GameManager.Instance.soundMgr.bgmVolume == 0 ? false : true;
            bool isSFXOn = GameManager.Instance.soundMgr.sfxVolume == 0 ? false : true;

            ChangeOnOffButton(pause_tog_bgm, isBGMOn);
            ChangeOnOffButton(pause_tog_sfx, isSFXOn);
            ChangeJoystickButtonText();
        }


        void AddButtonSound()
        {
            canvas_pause.gameObject.SetActive(true);
            canvas_perk.gameObject.SetActive(true);
            canvas_result.gameObject.SetActive(true);
            canvas_count.gameObject.SetActive(true);

            Button[] arr_buttons = transform.GetComponentsInChildren<Button>();

            for (int i = 0; i < arr_buttons.Length; i++)
            {
                arr_buttons[i].onClick.AddListener(() => stageMgr.soundMgr.PlaySfx(transform.position, sfx_click));
            }

            RedrawResultInven();

            canvas_pause.gameObject.SetActive(false);
            canvas_perk.gameObject.SetActive(false);
            canvas_result.gameObject.SetActive(false);
            canvas_count.gameObject.SetActive(false);
        }

        #region Pause UI Action
        /// <summary>
        /// 일시 정지 시 퍽 목록 새로고침
        /// </summary>
        /// <param name="list_perk"></param>
        public void PauseRefreshPerk(List<Perk> list_perk)
        {
            int gap = 0;
            if (pause_tr_perks.childCount < list_perk.Count)
            {
                gap = list_perk.Count - pause_tr_perks.childCount;
                if (gap < 0)
                    return;

                for (int i = 0; i < list_perk.Count; i++)
                {
                    Perk perk;
                    if (i < gap)
                    {
                        perk = Instantiate(perk_pause, pause_tr_perks.transform).GetComponent<Perk>();
                    }
                    else
                    {
                        perk =  stageMgr.trPlayerPerk.GetChild(i).GetComponent<Perk>();
                    }
                    perk.PerkCopy(list_perk[i]);
                }
            }
            else
            {
                if (pause_tr_perks.childCount > list_perk.Count)
                {
                    gap = pause_tr_perks.childCount - list_perk.Count;
                    if (gap < 0)
                        return;

                    for (int i = pause_tr_perks.childCount - gap; i < pause_tr_perks.childCount; i++)
                    {
                        pause_tr_perks.GetChild(i).gameObject.SetActive(false);
                    }

                    for (int i = 0; i < list_perk.Count; i++)
                    {
                        Perk perk = pause_tr_perks.GetChild(i).GetComponent<Perk>();
                        perk.PerkCopy(list_perk[i]);
                        perk.gameObject.SetActive(true);
                    }
                }
                else
                {
                    gap = list_perk.Count - pause_tr_perks.childCount;
                    if (gap < 0)
                        return;

                    for (int i = 0; i < pause_tr_perks.childCount; i++)
                    {
                        Perk perk = pause_tr_perks.GetChild(i).GetComponent<Perk>();
                        perk.PerkCopy(list_perk[i]);
                        perk.gameObject.SetActive(true);
                    }
                    for (int i = pause_tr_perks.childCount; i < list_perk.Count; i++)
                    {
                        Perk perk = Instantiate(perk_pause, pause_tr_perks.transform).GetComponent<Perk>();
                        perk.PerkCopy(list_perk[i]);
                        perk.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void PauseRoomTextChange(int stageNum, int roomNum, int maxRoom)
        {
            pause_txt_room.text = "Stage" + stageNum + " - Room " + roomNum + " / " + maxRoom;
        }

        void PauseActiveButton(bool isActive)
        {
            if (isActive)
            {
                //일시정지 활성화시
                canvas_pause.gameObject.SetActive(true);
                Time.timeScale = 0f;

                //특성창 갱신
                PauseRefreshPerk(stageMgr.playerControll.player.list_perk);
                PauseRoomTextChange(stageMgr.stageNum, stageMgr.currentRoomNum, stageMgr.maxRoom);
            }
            else
            {
                canvas_pause.gameObject.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        void PauseHomeButton()
        {
            stageMgr.StageEnd();
            PauseActiveButton(false);
        }

        /// <summary>
        /// 버튼 누를 시 이미지 변경
        /// </summary>
        /// <param name="button">이미지를 변경할 버튼</param>
        /// <param name="isOn"></param>
        void ChangeOnOffButton(Button button, bool isOn)
        {
            if (isOn)
            {
                button.image.color = new Color32(49, 122, 255, 255);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "ON";
            }
            else
            {
                button.image.color = new Color32(207, 0, 28, 255);
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "OFF";
            }
        }

        void PauseBGMToggle()
        {
            if (stageMgr.soundMgr.bgmVolume == 0)
            {
                Debug.Log("BGM On");
                stageMgr.soundMgr.bgmVolume = 1;
                ChangeOnOffButton(pause_tog_bgm, true);
            }
            else
            {
                Debug.Log("BGM Off");
                stageMgr.soundMgr.bgmVolume = 0;
                ChangeOnOffButton(pause_tog_bgm, false);
            }
            stageMgr.soundMgr.bgmSource.volume = stageMgr.soundMgr.bgmVolume;
            stageMgr.soundMgr.SaveVolume();
        }
        void PauseSFXToggle()
        {
            if (stageMgr.soundMgr.sfxVolume == 0)
            {
                Debug.Log("SFX On");
                stageMgr.soundMgr.sfxVolume = 1;
                ChangeOnOffButton(pause_tog_sfx, true);
            }
            else
            {
                Debug.Log("SFX Off");
                stageMgr.soundMgr.sfxVolume = 0;
                ChangeOnOffButton(pause_tog_sfx, false);
            }

            stageMgr.soundMgr.SaveVolume();
        }
        /// <summary>
        /// 4/20/2023-LYI
        /// 조이스틱 조작 방식 변경
        /// </summary>
        void PauseControllToggle()
        {
            JoystickType type = joystick.joystickType;
            if (type >= JoystickType.Dynamic)
            {
                joystick.SetMode(JoystickType.Fixed);
            }
            else
            {
                type++;
                joystick.SetMode(type);
            }
            ChangeJoystickButtonText();
        }
        void ChangeJoystickButtonText()
        {
            switch (joystick.joystickType)
            {
                case JoystickType.Fixed:
                    pause_tog_controll.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fixed";
                    break;
                case JoystickType.Floating:
                    pause_tog_controll.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Floating";
                    break;
                case JoystickType.Dynamic:
                    pause_tog_controll.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dynamic";
                    break;
                default:
                    pause_tog_controll.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fixed";
                    break;
            }
        }
        #endregion

        #region Game UI Action
        public void ChangeEXPGauge(float fill)
        {
            game_level_img.fillAmount = fill;
        }
        public void ChangeLevelText(int level)
        {
            game_level_txt.text = "Level " + level;
        }
        public void ChangeCoinText(int coin)
        {
            game_txt_coin.text = coin.ToString();
        }
        public void ChangeBossHP(float fill)
        {
            game_boss_img_hp.fillAmount = fill;
        }

        /// <summary>
        /// 경험치바 가리고 보스 체력 표시
        /// </summary>
        /// <param name="isBoss">보스 체력을 표시해야 하는가?</param>
        public void ToggleBossHP(bool isBoss)
        {
            game_rt_level.gameObject.SetActive(!isBoss);
            game_rt_boss.gameObject.SetActive(isBoss);
        }
        #endregion

        #region Damage UI Action

        /// <summary>
        /// 데미지에 따른 데미지 텍스트 효과
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dmg"></param>
        public void ShowDamageText(Vector3 pos, int dmg)
        {
            GameObject go = CreateText(pos, dmg);
            StartCoroutine(DamageTextMove(go));
        }

        /// <summary>
        /// 220701 데미지 텍스트 색깔 설정 추가
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dmg"></param>
        /// <param name="color"></param>
        public void ShowDamageText(Vector3 pos, int dmg, Color color)
        {
            GameObject go = CreateText(pos, dmg);
            go.transform.GetChild(0).GetComponent<Text>().color = color;
            StartCoroutine(DamageTextMove(go));
        }

        /// <summary>
        /// 220701
        /// 텍스트 생성과 이동 분리
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dmg"></param>
        /// <returns></returns>
        GameObject CreateText(Vector3 pos, int dmg)
        {
            GameObject go;

            go = GameManager.Instance.objPoolingMgr.CreateObject(queue_canvas_damage, origin_damageCanvas, pos,tr_damageTextPool);

            go.transform.GetChild(0).GetComponent<Text>().text = dmg.ToString();
            go.transform.GetChild(0).GetComponent<Text>().color = Color.red;

            return go;
        }


        /// <summary>
        /// 데미지 텍스트 위로 올라가는 움직임
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dmg"></param>
        /// <returns></returns>
        public IEnumerator DamageTextMove(GameObject go)
        {
            Text text = go.transform.GetChild(0).GetComponent<Text>();
            //text.transform.localScale = Vector3.one;

            Vector3 start = text.rectTransform.localPosition;

            text.rectTransform.localPosition -= Vector3.up * 30f + Vector3.right * UnityEngine.Random.Range(-10f, 11f);
            float t = 0;
            while (t < 0.2f)
            {
                t += 0.01f;

                text.rectTransform.localPosition += Vector3.up * 1f;

                yield return new WaitForSeconds(0.01f);
            }

            text.rectTransform.localPosition = start;
            //queue_canvas_damage.Enqueue(go);
            //go.SetActive(false);

            GameManager.Instance.objPoolingMgr.ObjectInit(queue_canvas_damage, go, tr_damageTextPool);
        }
        #endregion

        #region Result UI Action
        public void ButtonResultHome()
        {
            stageMgr.SavePlayerData();
        }

        public void RedrawResultInven()
        {
            MMInventoryEvent.Trigger(MMInventoryEventType.Redraw, null, stageMgr.stageInven.name, null, 0, 0, stageMgr.stageInven.PlayerID);
        }

        public void ActiveResult()
        {
            canvas_result.gameObject.SetActive(true);
            stageMgr.statStage = StageStat.RESULT;
            MMInventoryEvent.Trigger(MMInventoryEventType.ContentChanged, null, "MainInventory", null, 0, 0, "Stage");

            result_txt_stageName.text = stageMgr.stageName;
            result_txt_lastRoom.text = stageMgr.clearRoom.ToString(); //클리어한 룸
        }

        #endregion

        #region CountDown UI Action

        /// <summary>
        /// 카운트다운 광고 버튼 동작
        /// </summary>
        public void ButtonCountAD()
        {
            Debug.Log("ButtonCountAd()");

            //광고 호출 시도
            Debug.Log("Load Ad Success");
            //광고 호출 실패 시 인스턴트 메시지

            Debug.Log("Load Ad Success Failed");
            Debug.Log("Sorry, we don't have more Ad for today");


        }

        /// <summary>
        /// 부활 영상 광고 보상
        /// </summary>
        public void AdRewardRevive()
        {
            if (countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
            }
            stageMgr.playerControll.player.PlayerRevive();

            canvas_count.gameObject.SetActive(false);
        }


        /// <summary>
        /// 카운트 다운 보석 버튼 동작
        /// </summary>
        public void ButtonCountDiamond()
        {
            Debug.Log("ButtonCountDiamond()");

            DataManager dataMgr = GameManager.Instance.dataMgr;
            if (dataMgr.Diamond >= 100)
            {
                if (countDownCoroutine != null)
                {
                    StopCoroutine(countDownCoroutine);
                }
                //다이아 100개 소모
                GameManager.Instance.dataMgr.Diamond -= 100;

                stageMgr.playerControll.player.PlayerRevive();
                canvas_count.gameObject.SetActive(false);
            }
            else
            {
                //GameManager 인스턴트 시스템 메시지, 화면 중앙에 2초 노출
                StaticManager.UI.MessageUI.PopupMessage("You need more Diamond");
            }
        }

        /// <summary>
        /// 플레이어 사망시 카운트다운 팝업, 광고, 보석 버튼으로 부활
        /// </summary>
        public void StartCountDown()
        {
            canvas_count.gameObject.SetActive(true);
            countDownCoroutine = StartCoroutine(CountDown());
        }

        IEnumerator CountDown()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);
            int t = 5;
            while (t > 0)
            {
                count_txt_countDown.text = t.ToString();
                yield return wait;
                t--;
            }

            canvas_count.gameObject.SetActive(false);
            //사망처리 완료, 결과창 호출
            stageMgr.StageResult();
        }

        #endregion


        #region Perk Description Message Popup
        /// <summary>
        /// 4/14/2023-LYI
        /// Show get perk description with image, text
        /// Active after perk canvas disabled
        /// if there is multiple perks to display, it stacks in queue
        /// </summary>
        public void ShowPerkDescription(Perk perk)
        {
            if (i_perkDescriptionMessageQueue < 1)
            {
                StartCoroutine(PerkDescriptionAnimation(perk));
                i_perkDescriptionMessageQueue++;
            }
            else
            {
                queue_perkCoroutine.Enqueue(() => PerkDescriptionAnimation(perk));
                i_perkDescriptionMessageQueue++;
            }
        }

        /// <summary>
        /// 4/14/2023-LYI
        /// 퍽 효과 보여주고 사라지는 효과
        /// 3초간 표시되었다가 사라진다
        /// </summary>
        /// <returns></returns>
        IEnumerator PerkDescriptionAnimation(Perk perk)
        {
            perkDescription_txt_title.text = perk.perkInfo.name;
            perkDescription_txt_description.text = perk.perkInfo.description;

            //등장 효과
            //중앙에서 좌우로 열리며 등장
            ui_perkDescription.gameObject.SetActive(true);
            perkDescription_img_bg.rectTransform.localScale = new Vector3(0, 1, 1);

            perkDescription_group_texts.alpha = 0;
            float t = 0;
            float fadeSpeed = 10f;
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (t < 1)
            {
                t += 0.01f * fadeSpeed;
                perkDescription_img_bg.rectTransform.localScale = new Vector3(t, 1, 1);
                perkDescription_group_texts.alpha = t;
                yield return wait;
            }
            perkDescription_img_bg.rectTransform.localScale = Vector3.one;
            perkDescription_group_texts.alpha = 1;

            //메시지 표시 대기 시간
            yield return new WaitForSeconds(2f);

            t = 0;
            //소멸 효과
            //좌우가 좁아지며 사라지기
            while (t < 1)
            {
                t += 0.01f * fadeSpeed;
                perkDescription_img_bg.rectTransform.localScale = new Vector3(1 - t, 1, 1);
                perkDescription_group_texts.alpha = 1 - t;
                yield return wait;
            }

            perkDescription_group_texts.alpha = 0;
            perkDescription_img_bg.rectTransform.localScale = new Vector3(0, 1, 1);

            ui_perkDescription.gameObject.SetActive(false);

            //다음 메시지 실행
            i_perkDescriptionMessageQueue--;
            if (queue_perkCoroutine.Count > 0)
            {
                StartCoroutine(queue_perkCoroutine.Dequeue()());
            }

        }
        #endregion

    }
}