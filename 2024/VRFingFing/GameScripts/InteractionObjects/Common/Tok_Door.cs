using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

namespace VRTokTok.Interaction
{
    /// <summary>
    /// 9/25/2023-LYI
    /// 문 작동 방식에 따른 분류
    /// 기본은 아무동작 없음
    /// 열쇠는 부딛혔을 때 열쇠로 상호작용 작동
    /// 2/21/2024-LYI
    /// 색깔에 따른 동작 분류로 필요가 없어져서 제거
    /// </summary>
    //public enum DoorType
    //{
    //    NORMAL = 0,
    //    KEY,
    //}

    /// <summary>
    /// 7/6/2023-LYI
    /// 기본적인 문
    /// 각종 장치와 상호작용을 통해 열린다
    /// </summary>
    public class Tok_Door : Tok_Interact
    {
        [Header("Tok_Door")]
        public Renderer m_Door2;
        public Material[] arr_matDoor;

        [SerializeField]
        Animator m_animator;

        [SerializeField]
        Collider m_collider;

        public MMFeedbacks[] arr_feedbacks;

        public MMF_Player mmf_open;
        public MMF_Player mmf_close;

        public bool isOpen = false;


        public override void InteractInit()
        {
            base.InteractInit();

            for (int i = 0; i < arr_feedbacks.Length; i++)
            {
                arr_feedbacks[i].Initialization();
            }
            isInteractable = true;

            if (m_renderer != null)
            {
                SetColor(interactColor);
            }

            DoorClose();
        }

        /// <summary>
        /// 2/19/2024-LYI
        /// 문용 색 지정 추가
        /// </summary>
        /// <param name="color"></param>
        public override void SetColor(InteractColor color)
        {
            if (arr_matDoor == null)
            {
                base.SetColor(color);
                return;
            }

            if (m_renderer == null)
            {
                return;
            }

            switch (color)
            {
                case InteractColor.BLUE:
                    m_renderer.material = arr_matDoor[0];
                    m_Door2.material = arr_matDoor[0];
                    break;
                case InteractColor.GREEN:
                    m_renderer.material = arr_matDoor[1];
                    m_Door2.material = arr_matDoor[1];
                    break;
                case InteractColor.YELLOW:
                    m_renderer.material = arr_matDoor[2];
                    m_Door2.material = arr_matDoor[2];
                    break;
                case InteractColor.RED:
                    m_renderer.material = arr_matDoor[3];
                    m_Door2.material = arr_matDoor[3];
                    break;
                default:
                    //m_renderer.material = arr_matDoor[0];
                    break;
            }
        }

        private void OnCollisionEnter(Collision coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                if (isInteractable)
                {
                    Manager.StageManager stageMgr = GameManager.Instance.playMgr.currentStage;

                    Tok_Key openKey = null;
                    if (stageMgr.keyCount > 0)
                    {
                        for (int i = 0; i < stageMgr.keyCount; i++)
                        {
                            //문과 열쇠 검증
                            if (interactColor == stageMgr.list_key[i].interactColor)
                            {
                                openKey = stageMgr.list_key[i];
                            }
                        }

                        if (openKey != null)
                        {
                            ActiveInteraction();
                            openKey.UseKey();

                            isInteractable = false;
                        }
                    }
                    //if (coll.gameObject.GetComponent<Tok_Character>().item.key != null)
                    //{
                    //    ActiveInteraction();
                    //}
                }
            }
        }

        public void DoorOpen()
        {
            //재시작 시 보이는것 수정
            if (isOpen)
            {
                return;
            }
            isOpen = true;
            m_collider.enabled = false;

            if (mmf_open != null)
            {
                mmf_open.PlayFeedbacks();
            }

            GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_INTERACTION_DOOR_OPEN);

            if (m_animator != null)
            {
                // m_animator.SetBool("isOpen", true);
            }
        }
        public void DoorClose()
        {
            if (!isOpen)
            {
                return;
            }
            isOpen = false;
            m_collider.enabled = true;

            if (mmf_close != null)
            {
                mmf_close.PlayFeedbacks();
            }

            if (m_animator != null)
            {
                // m_animator.SetBool("isOpen", false);
            }
        }


        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            DoorOpen();
        }


        public override void DisableInteraction()
        {
            base.DisableInteraction();
            DoorClose();
        }

    }
}