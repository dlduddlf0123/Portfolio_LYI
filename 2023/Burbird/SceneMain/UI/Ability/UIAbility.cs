using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace Burbird
{
    public class UIAbility : MonoBehaviour
    {


        GameManager gameMgr;

        public Text txt_title; //UI 상단 제목
        public Text txt_cost; //업그레이드 비용 = 기본 * abilityLevel

        public Button btn_upgrade; //업그레이드 버튼

        [Header("Get Screen")]
        public Button btn_getScreen; //업그레이드 결과 스크린
        public TextMeshProUGUI txt_getStatusName; //강화된 결과값 보여주기
        public TextMeshProUGUI txt_getStatusStat; //강화된 결과값 보여주기
        public TextMeshProUGUI txt_getStatusUp; //강화된 결과값 보여주기
        [SerializeField]
        private Ability getAbility;

        public Ability[] arr_ability;

        List<List<object>> list__abilityStat = new ();

        public int allAbilityLevel = 0; //전체 어빌리티 레벨(가격에 영향)
        public int upgradeCost = 300;

        bool isActing = false;

        void Awake()
        {
            gameMgr = GameManager.Instance;

            arr_ability = transform.GetComponentsInChildren<Ability>();
        }

        private void Start()
        {
            btn_upgrade.onClick.AddListener(Button_AbilityUpgrade);
            btn_getScreen.onClick.AddListener(CloseGetAbilityScreen);
            list__abilityStat = GameManager.Instance.csvLoader.ReadCSVDatas2("AbilityChart");
            Init();
        }

        public void Init()
        {
            RefreshUI();
        }

        public void ChangeTitle(string text)
        {
            txt_title.text = text;
        }

        /// <summary>
        /// 코스트 갱신, 각 어빌리티 레벨 표시 적용 등
        /// </summary>
        public void RefreshUI()
        {
            txt_cost.text = (upgradeCost + upgradeCost * allAbilityLevel).ToString();
        }

        /// <summary>
        /// 업그레이드 버튼 클릭 시
        /// </summary>
        public void Button_AbilityUpgrade()
        {
            if (isActing && IsUpgradeAble())
            {
                return;
            }

            //서버 검증이후 작동할 것


            //코스트 지불
           // gameMgr.dataMgr.UseCoin(upgradeCost);

            //획득 효과 보여주기
            StartCoroutine(GetRandomAbility());
        }

        /// <summary>
        /// 강화 가능 여부 체크
        /// </summary>
        private bool IsUpgradeAble()
        {
            //비용 체크
            if (gameMgr.dataMgr.Coin < upgradeCost)
            {
                StaticManager.UI.MessageUI.PopupMessage("업그레이드 비용이 부족합니다");
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 랜덤 어빌리티 획득 효과 보여주기
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetRandomAbility()
        {
            isActing = true;

            Ability getAbility = null;
            Ability lastAbility = null;

            float waitTime = 0.1f;
            WaitForSeconds Wait = new WaitForSeconds(waitTime);
            
            for (int i = 0; i < 18; i++)
            {
                if (lastAbility != null)
                {
                    lastAbility.Deselect();
                }

                lastAbility = getAbility;

                yield return Wait;
                waitTime += 0.01f;
                Wait = new WaitForSeconds(waitTime);

                getAbility = arr_ability[Random.Range(0, arr_ability.Length)];
                getAbility.Select();
            }

            if (lastAbility != null)
            {
                lastAbility.Deselect();
            }

            waitTime = 0.3f;
            Wait = new WaitForSeconds(waitTime);
            for (int i = 0; i < 4; i++)
            {
                yield return Wait;
                getAbility.Select();
                yield return Wait;
                getAbility.Deselect();
            }

            for (int i = 0; i < arr_ability.Length; i++)
            {
                arr_ability[i].Deselect();
            }

            getAbility.GetAbility();

            //업그레이드 이후 동작
            isActing = false;
            allAbilityLevel++;
            RefreshUI();
            OpenGetAbilityScreen(getAbility);
        }


        /// <summary>
        /// 룰렛 이후 결정된 어빌리티 확인 창
        /// </summary>
        /// <param name="ability"></param>
        void OpenGetAbilityScreen(Ability ability)
        {
            btn_getScreen.gameObject.SetActive(true);

            getAbility.img_icon.sprite = ability.img_icon.sprite;
            getAbility.txt_name.text = ability.txt_name.text;
            getAbility.txt_level.text = ability.txt_level.text;

            List<object> list_abilityStat = list__abilityStat[(int)ability.type_ability];

            int upStat = 0;

            if (ability.abilityLevel  <= 1)
            {
                upStat = 0;
                txt_getStatusUp.gameObject.SetActive(false);
            }
            else
            {
                upStat = System.Convert.ToInt32(list_abilityStat[ability.abilityLevel]) - System.Convert.ToInt32(list_abilityStat[ability.abilityLevel - 1]);
                txt_getStatusUp.gameObject.SetActive(true);
            }

            txt_getStatusName.text = list_abilityStat[0].ToString();
            txt_getStatusStat.text = "+" + list_abilityStat[ability.abilityLevel].ToString();
            txt_getStatusUp.text = "+" + upStat + "▲";
        }
        void CloseGetAbilityScreen()
        {
            btn_getScreen.gameObject.SetActive(false);
        }

        /// <summary>
        /// 어빌리티 스탯 전체 갱신, 플레이어 총 스탯 갱신
        /// </summary>
        public void ApplyAbilityStat()
        {
            for (int i = 0; i < arr_ability.Length; i++)
            {
                SetAbilityStat(arr_ability[i]);
            }
            gameMgr.dataMgr.RefreshAllPlayerStatus();
        }

        /// <summary>
        /// 각 어빌리티 획득 시 스탯 적용
        /// </summary>
        /// <param name="ability"></param>
        void SetAbilityStat(Ability ability)
        {
            if (ability.abilityLevel == 0)
            {
                return;
            }
            int abilityNum = (int)ability.type_ability;

            switch (ability.type_ability)
            {
                case Ability.AbilityType.START_PERK:
                    if (ability.abilityLevel > 0)
                    {
                        //시작퍽 활성화
                    }
                    break;
                case Ability.AbilityType.MAXHP:
                    gameMgr.dataMgr.abilityStat.maxHp =  System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.ATKDMG:
                    gameMgr.dataMgr.abilityStat.ATKDamage = System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.RECOVERY:
                  //  gameMgr.dataMgr.abilityStat. = System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.LEVELUP_RECOVERY:
                  //  gameMgr.dataMgr.abilityStat.= System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.RANGE_BLOCK:
                  //  gameMgr.dataMgr.abilityStat.ATKDamage = System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.MELEE_BLOCK:
                  //  gameMgr.dataMgr.abilityStat.ATKDamage = System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.ATKSPD:
                    gameMgr.dataMgr.abilityStat.ATKSpeed = (float)System.Convert.ToDouble(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                case Ability.AbilityType.TIME_REWARD:
                    //  gameMgr.dataMgr.abilityStat.ATKDamage = System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    if (ability.abilityLevel > 0)
                    { //시간 보상 활성화
                    }
                        break;
                case Ability.AbilityType.ENHANCE_EQUIPMENT:
                  // gameMgr.dataMgr.abilityStat.ATKDamage = System.Convert.ToInt32(list__abilityStat[abilityNum][ability.abilityLevel]);
                    break;
                default:
                    break;
            }
        }


    }
}