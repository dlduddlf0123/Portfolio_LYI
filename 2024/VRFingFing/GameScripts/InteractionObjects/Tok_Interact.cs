using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


namespace VRTokTok.Interaction
{
    public enum InteractColor
    {
        WHITE = 0,
        BLUE = 1,
        GREEN = 2,
        YELLOW = 3,
        RED = 4,
        ORANGE = 5,
        PURPLE = 6,
        PINK = 7,
        BLACK = 8,
    }

    /// <summary>
    /// 7/6/2023-LYI
    /// 상호작용 가능한 상위 클래스
    /// 이 클래스를 상속받아 문 등 다양한 상호작용 제작
    /// </summary>
    public class Tok_Interact : MonoBehaviour
    {
        [Header("Tok Interact Parent")]
        public List<Tok_Interact> list_interactGizmo = new List<Tok_Interact>();
        public bool isInteractable = true;

        [Header("Interact Event")]
        public UnityEvent onActive;
        public UnityEvent onDisable;

        public Renderer m_renderer;
        public InteractColor interactColor;

        protected bool isInit = false;

        //private void Start()
        //{
        //    InteractChecker();
        //}


        /// <summary>
        /// 9/4/2023-LYI
        /// Interaction 연결된 오브젝트를 확인하기 위해 arr_onActiveInteractor 추가
        /// Editor 기즈모로 보인다
        /// 이거 사용할 때 이벤트 등록하도록 설정
        /// 이미 설정된 이벤트를 확인할 방법을 못찾아서 보류중
        /// </summary>
        //void InteractChecker()
        //{
        //    if (list_interactGizmo.Count > 0)
        //    {
        //        for (int i = 0; i < list_interactGizmo.Count; i++)
        //        {
        //            if (list_interactGizmo[i] != null)
        //            {
        //                onActive.AddListener(() => list_interactGizmo[i].ActiveInteraction());
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// 10/9/2023-LYI
        /// Call in StageManager
        /// 각 상호작용 초기화 시 호출
        /// </summary>
        public virtual void InteractInit()
        {
            Debug.Log(this.gameObject.name + ": Init");

        }


        /// <summary>
        /// 2/19/2024-LYI
        /// 색깔 자동 지정 함수
        /// 색깔 선택된 상태에서 시작하면 자동으로 색 할당
        /// </summary>
        /// <param name="color"></param>
        public virtual void SetColor(InteractColor color)
        {
            if (m_renderer == null)
            {
                return;
            }

            switch (color)
            {
                case InteractColor.WHITE:
                    m_renderer.material.color = Color.white;
                    break;
                case InteractColor.BLUE:
                    m_renderer.material.color = Color.blue;
                    break;
                case InteractColor.GREEN:
                    m_renderer.material.color = Color.green;
                    break;
                case InteractColor.YELLOW:
                    m_renderer.material.color = Color.yellow;
                    break;
                case InteractColor.RED:
                    m_renderer.material.color = Color.red;
                    break;
                case InteractColor.ORANGE:
                    m_renderer.material.color = new Color32(255, 168, 0, 255);
                    break;
                case InteractColor.PURPLE:

                    break;
                case InteractColor.PINK:
                    m_renderer.material.color = Color.magenta;
                    break;
                case InteractColor.BLACK:
                    m_renderer.material.color = Color.black;
                    break;
                default:
                    m_renderer.material.color = Color.white;
                    break;
            }
        }

        /// <summary>
        /// 7/6/2023-LYI
        /// 인터렉션 활성화
        /// </summary>
        public virtual void ActiveInteraction()
        {
            if (!isInteractable ||
                GameManager.Instance.playMgr.statPlay != Manager.PlayStatus.PLAY)
            {
                return;
            }
            Debug.Log(this.gameObject.name + ": Active");

            if (onActive != null)
            {
                onActive.Invoke();
            }
        }


        /// <summary>
        /// 인터렉션 비활성화
        /// </summary>
        public virtual void DisableInteraction()
        {
            Debug.Log(this.gameObject.name + ": Deactivate");
            if (onDisable != null)
            {
                onDisable.Invoke();
            }
        }

    }
}