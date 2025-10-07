using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;


namespace VRTokTok.Interaction
{
    public enum ButtonType
    {
        CLICK = 0, //누르면 들어갔다 나옴, 재작동 가능
        ONCE, //한번 누르면 들어감, 재작동 불가
        CONTINUE, //누르는 동안만 들어감, 안누르면 나옴
        TIMER, //누른 뒤 일정 시간이 지나면 나옴
    }


    /// <summary>
    /// 7/6/2023-LYI
    /// 캐릭터가 밟으면 작동하는 버튼, 지정된 상호작용 물체 작동
    /// </summary>
    public class Tok_Button : Tok_Interact
    {
        GameManager gameMgr;

        [Header("Tok Button")]
        public ButtonType type_button = ButtonType.CLICK;
        public MMF_Player[] arr_feedback; //클릭, 들어감, 나옴
        //public MMF_Player[] arr_mmfPlayer; //클릭, 들어감, 나옴
        public GameObject buttonPivot;
        public Collider col_button;

        //활성화, 비활성화 머테리얼 변경
        [Header("Material")]
        public MeshRenderer mesh_button;
        public Material mat_active;
        public Material mat_disable;

        [Header("Heavy")]
        public bool isHeavy = false; //무거운 버튼인가
        public int heavyCount = 0; //무거운걸 누르기 위한 무게
        public int weightCount = 0; //현재 무게

        [Header("Properties")]
        public int clickCount = 0;
        public bool isStartActive = true;
        public bool isPush = false;
        public bool isOnce = false;

        bool isFirst = true;

        private void Start()
        {
            InteractInit();
        }

        public override void InteractInit()
        {
            base.InteractInit();

            if (isFirst)
            {
                gameMgr = GameManager.Instance;

                arr_feedback[0].Events.OnPlay.AddListener(OnButtonClick);
                arr_feedback[0].Events.OnComplete.AddListener(OnButtonClickFinish);
                arr_feedback[1].Events.OnPlay.AddListener(OnButtonIn);
                arr_feedback[2].Events.OnPlay.AddListener(OnButtonOut);

                isFirst = false;
            }

            for (int i = 0; i < arr_feedback.Length; i++)
            {
                arr_feedback[i].Initialization();
            }

            SetButtonType(type_button);
            SetInteractable(isStartActive, false);
        }

        public void SetButtonType(ButtonType type)
        {
            type_button = type;

            switch (type)
            {
                case ButtonType.CLICK:
                    break;
                case ButtonType.ONCE:
                    isOnce = false;
                    break;
                case ButtonType.CONTINUE:
                    break;
                case ButtonType.TIMER:
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 10/9/2023-LYI
        /// 버튼 활성화 여부 결정
        /// 비활성화 시 버튼 작동하지 않음
        /// </summary>
        /// <param name="isActive"></param>
        public void SetInteractable(bool isActive, bool isFeedback = true)
        {
            if (isActive)
            {
                if (isFeedback && !isInteractable)
                {
                    arr_feedback[2].PlayFeedbacks();
                }
                else
                {
                    buttonPivot.transform.localPosition = Vector3.up *0.5f;
                }

                isInteractable = true;
                mesh_button.material = mat_active;
                isOnce = false;

                if (type_button == ButtonType.CLICK)
                {
                    if (col_button != null)
                    {
                        col_button.enabled = true;
                    }
                }
            }
            else
            {
                if (isFeedback && isInteractable)
                {
                    arr_feedback[1].PlayFeedbacks();
                }
                else
                {
                    buttonPivot.transform.localPosition = Vector3.up * -0.8f;
                }

                isInteractable = false;
                mesh_button.material = mat_disable;

                if (type_button == ButtonType.CLICK)
                {
                    if (col_button != null)
                    {
                        col_button.enabled = false;
                    }
                }
            }
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 버튼 연속 입력 방지용 딜레이
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitPush()
        {
            yield return new WaitForSeconds(0.5f);
            isPush = false;

            // SetInteractable(true);
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (!isInteractable)
            {
                return;
            }
            if (isPush)
            {
                return;
            }
            if (isHeavy)
            {
                if (weightCount < heavyCount)
                {
                    return;
                }
            }

            if (coll.gameObject.CompareTag("Header") ||
                coll.gameObject.CompareTag("Item"))
            {
                isPush = true;


                for (int i = 0; i < arr_feedback.Length; i++)
                {
                    if (arr_feedback[i].IsPlaying)
                    {
                        arr_feedback[i].StopFeedbacks();
                    }                    
                }

                switch (type_button)
                {
                    case ButtonType.CLICK:
                        clickCount++;
                        arr_feedback[0].PlayFeedbacks();
                        ActiveInteraction();

                        StartCoroutine(WaitPush());
                        //SetInteractable(false);
                        return;
                    case ButtonType.ONCE:
                        if (!isOnce)
                        {
                            arr_feedback[1].PlayFeedbacks();
                            ActiveInteraction();
                            SetInteractable(false);
                            isOnce = true;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    case ButtonType.CONTINUE:
                    default:
                        arr_feedback[1].PlayFeedbacks();
                        ActiveInteraction();
                        break;
                }

            }
        }
        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header") ||
                coll.gameObject.CompareTag("Item"))
            {
                isPush = false;

                switch (type_button)
                {
                    case ButtonType.CONTINUE:
                        arr_feedback[2].PlayFeedbacks();
                        DisableInteraction();
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// 7/2/2024-LYI
        /// 버튼 동작 관련 함수 구조 수정
        /// </summary>
        public void OnButtonClick()
        {
            if (!isInteractable)
            {
                return;
            }
            mesh_button.material = mat_disable;
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_INTERACTION_BUTTON_CLICK);
        }
        public void OnButtonClickFinish()
        {
            if (!isInteractable)
            {
                return;
            }

            mesh_button.material = mat_active;
        }
        public void OnButtonIn()
        {

            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_INTERACTION_BUTTON_ACTIVE);
        }
        public void OnButtonOut()
        {

            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_INTERACTION_BUTTON_DEACTIVATE);
        }



        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
        }


        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }



    }
}