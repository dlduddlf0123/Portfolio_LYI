using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Burbird
{
    /// <summary>
    /// 캐릭터에게 적용되는 추가적인 능력치들
    /// 기본스탯 및 기타 부수 효과들
    /// </summary>
    public class Ability : MonoBehaviour
    {
        public enum AbilityType
        {
            START_PERK = 0,
            MAXHP,
            ATKDMG,
            RECOVERY,
            LEVELUP_RECOVERY,
            RANGE_BLOCK,
            MELEE_BLOCK,
            ATKSPD,
            TIME_REWARD,
            ENHANCE_EQUIPMENT,
        }

        public GameObject lockImage;
        public GameObject highlight;

        [Header("Display")]
        public Image img_icon;
        public Text txt_name;
        public Text txt_level;

        public int abilityLevel;
        public int maxLevel;
        public AbilityType type_ability;

        void Awake()
        {

            SetAbilityLevel(0);
        }

        public void SetAbilityLevel(int value)
        {
            abilityLevel = value;
            txt_level.text = "Level " + abilityLevel;
        }

        /// <summary>
        /// 구매 후 최종 선택된 뒤 효과 적용 시
        /// </summary>
        public virtual void GetAbility()
        {
            abilityLevel++;
            SetAbilityLevel(abilityLevel);
        }


        public void Lock()
        {
            lockImage.gameObject.SetActive(true);
        }
        public void Unlock()
        {
            lockImage.gameObject.SetActive(false);
        }


        /// <summary>
        /// 랜덤으로 돌아가면서 선택되었을 때
        /// </summary>
        public virtual void Select()
        {
            highlight.SetActive(true);
        }
        public virtual void Deselect()
        {
            highlight.SetActive(false);
        }
    }
}