using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Burbird
{

    /// <summary>
    /// 플레이어 특전 클래스
    /// 이 클래스를 상속 받아서 특전효과들 제작
    /// UI Icon 표시
    /// </summary>
    public class Perk : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected StageManager stageMgr;
        protected PerkChecker perkChecker;
        protected Player player;

        public Button perk_btn_select;
        public Image perk_img_icon; //퍽 아이콘
        public TextMeshProUGUI perk_txt_description;  // 퍽 설명
        public TextMeshProUGUI perk_txt_title; // 퍽 이름

        public UnityAction action_click = null;

        //CSV에서 불러올 퍽 정보
        public PerkInfo perkInfo;
        public int PerkNum; //퍽 정보를 불러올 기준 퍽 번호, 각 스크립트에서 정의(DoAwake)

        public bool isPause = false; //일시 정지 창일 경우
        public bool isConsumable = false; //일회성 소모퍽(회복류)
        public bool isStackable = false; //중첩 가능한 퍽인가

        private void Awake()
        {
            PerkInit();
        }

        protected virtual void DoAwake() { }

        public virtual void PerkInit()
        {
            stageMgr = StageManager.Instance;
            perkChecker = stageMgr.perkChecker;

            perk_btn_select = GetComponent<Button>();

            perk_img_icon = transform.GetChild(0).GetComponent<Image>();
            perk_txt_description = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            perk_txt_title = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            //perk_btn_select.onClick.AddListener(PerkClick);


            DoAwake();
            PerkInfoInit();
        }

        public virtual void PerkInfoInit()
        {
             perkInfo = new PerkInfo(perkChecker.list__perkInfo[PerkNum]);

            perk_txt_title.text = perkInfo.name;
            perk_txt_description.text = perkInfo.description;
        }


        /// <summary>
        /// 퍽 설명 보기
        /// </summary>
        /// <param name="isActive"></param>
        public void ActiveDescription(bool isActive)
        {
            transform.GetChild(1).gameObject.SetActive(isActive);
        }

        public void PerkCopy(Perk perk)
        {
            gameObject.name = perk.gameObject.name;

            perk_img_icon.sprite = perk.perk_img_icon.sprite;
            perk_txt_description.text = perk.perk_txt_description.text;
            perk_txt_title.text = perk.perk_txt_title.text;

            isPause = perk.isPause;

            if (perk.isPause)
            {
                ActiveDescription(false);
                perk_txt_title.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 퍽 획득
        /// 플레이어의 보유 퍽 리스트에 이 퍽 추가
        /// </summary>
        public virtual void PerkClick()
        {

            Debug.Log(perk_txt_title.text + "PerkClick");


            player = stageMgr.playerControll.player;
            if (isPause)
            {
                ActiveDescription(true);
            }
            else
            {
                PerkInit();
                if (isConsumable)
                {
                    //소모성 퍽일 경우
                    PerkActive();
                    //소모성이 아니면 플레이어 퍽 리스트에 추가
                }
                else if (!isStackable)
                {
                    //if (player.list_perk.Contains(this))
                    //{
                    //    //player.list_perk.Find(p => p == this).PerkLost();
                    //    //player.list_perk.Remove(this);

                    //    Debug.Log("!! This perk is not stackable, check the Perk code");
                    //    return;
                    //}
                  
                    Perk perk = Instantiate(this, stageMgr.trPlayerPerk);
                    perk.isPause = true;
                 
                    player.list_perk.Add(perk);
                    stageMgr.list_perk_pool.Remove(this);

                    PerkActive();
                }
                else
                {
                    Perk perk = Instantiate(this, stageMgr.trPlayerPerk);
                    perk.isPause = true;
                    player.list_perk.Add(perk);

                    PerkActive();
                }
            }
            

            if (action_click != null)
            {
                action_click.Invoke();
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (isPause)
            {
                PerkClick();
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isPause)
            {
                PerkClick();
            }
            else
            {
                PerkRelease();
            }
        }

        /// <summary>
        /// 클릭 뗄 때
        /// </summary>
        public virtual void PerkRelease()
        {
            if (isPause)
            {
                ActiveDescription(false);
            }
        }

        /// <summary>
        /// 퍽 효과 적용
        /// </summary>
        public virtual void PerkActive()
        {
            PerkInit();

            Debug.Log(perk_txt_title.text + "PerkActive");
            if (!isStackable)
            {
                Perk perk = stageMgr.list_perk_pool.Find(p => p.GetType() == this.GetType());
                stageMgr.list_perk_pool.Remove(perk);
            }
            stageMgr.soundMgr.PlaySfx(transform.position, stageMgr.ui_game.sfx_click, 1, 0);
        }


        /// <summary>
        /// 퍽 손실
        /// </summary>
        public virtual void PerkLost()
        {
            Debug.Log(perk_txt_title.text + "PerkLost");
            player = stageMgr.playerControll.player;
            
            if (!isConsumable)
            {
                Perk p = stageMgr.currentStageData.list_perk.Find(t => t.GetType() == this.GetType());
                stageMgr.list_perk_pool.Add(p);
            }
            StartCoroutine(LostAct());
        }

        IEnumerator LostAct()
        {
            yield return new WaitForSeconds(0.1f);

            player.list_perk.Remove(this);
            Destroy(this.gameObject);
        }
    }
}