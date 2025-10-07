using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Burbird
{

    public class UIPerk : MonoBehaviour
    {
        StageManager stageMgr;

        //퍽 관련
        public Perk[] arr_selectPerk = new Perk[3];

        //원본 프리팹
        public GameObject perk_select;

        private void Awake()
        {
            stageMgr = StageManager.Instance;
        }

        #region Perk UI Action

        /// <summary>
        /// 레벨 업, 게임 시작 시 등 호출
        /// 퍽 선택 창 활성화
        /// 랜덤 퍽 3가지 설정
        /// </summary>
        public virtual void PerkCanvasActive()
        {
            List<Perk> list_temp_pool = new List<Perk>();
            list_temp_pool = stageMgr.list_perk_pool.ToList();

            gameObject.SetActive(true);
            if (arr_selectPerk.Length == 0)
            {
                arr_selectPerk = transform.GetChild(1).GetComponentsInChildren<Perk>();
            }

            for (int i = 0; i < 3; i++)
            {
                arr_selectPerk[i] = list_temp_pool[Random.Range(0, list_temp_pool.Count)];
                list_temp_pool.Remove(arr_selectPerk[i]);

                if (transform.GetChild(1).GetChild(i) != null)
                {
                    Destroy(transform.GetChild(1).GetChild(i).gameObject);
                }
                Perk perk = Instantiate(arr_selectPerk[i].gameObject, transform.GetChild(1)).GetComponent<Perk>();
                perk.action_click = ()=>PerkCanvasClose(perk);
            }

            Time.timeScale = 0f;
        }

        /// <summary>
        /// 4/14/2023-LYI
        /// 퍽 클릭 시 작동
        /// UIPerk 창을 닫고, 퍽 정보를 메시지로 전달, 메시지 팝업
        /// </summary>
        /// <param name="selectedPerk"></param>
        protected virtual void PerkCanvasClose(Perk selectedPerk)
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            StageManager.Instance.ui_game.ShowPerkDescription(selectedPerk);
        }

        /// <summary>
        /// 퍽 3가지 선택 시 랜덤 퍽으로 설정
        /// </summary>
        /// <param name="arr_perk"></param>
        public virtual void PerkCanvasChange(Perk[] arr_perk)
        {
            arr_selectPerk = arr_perk;
            for (int i = 0; i < 3; i++)
            {
                arr_selectPerk[i] = arr_perk[i];
            }
        }


        #endregion

    }

}