using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

namespace AroundEffect
{

    public enum FoodType
    {
        NONE = 0,
        MEAT,
        VEGETABLE,
    }

    /// <summary>
    /// 10/22/2024-LYI
    /// 캐릭터가 먹는 음식
    /// 먹히는 동작, 맛, 컬리더, 그랩 가능
    /// </summary>
    public class Food : MonoBehaviour
    {
        ItemPicker itemPicker;
        XRGrabInteractable grabInteractable;

        Rigidbody m_rigidbody;

        public FoodType foodType;



        private void Awake()
        {
            itemPicker = GetComponent<ItemPicker>();

            grabInteractable = GetComponent<XRGrabInteractable>();
            m_rigidbody = GetComponent<Rigidbody>();

            grabInteractable.selectEntered.AddListener(OnGrabStart);
            grabInteractable.selectExited.AddListener(OnGrabEnd);
        }



        /// <summary>
        /// 10/22/2024-LYI
        /// 음식 먹기
        /// 뭔가 효과?
        /// 음식 관리 매니저?
        /// 인벤토리의 아이템 관리?
        /// 아이템 생성, 소멸 관리 클래스?
        /// 땅으로 떨어지면 인벤토리로?
        /// 
        /// </summary>
        public void AteFood()
        {
            gameObject.SetActive(false);
        }

        public void OnGrabStart(SelectEnterEventArgs args)
        {


        }
        public void OnGrabEnd(SelectExitEventArgs args)
        {
            m_rigidbody.isKinematic = false;

        }


    }
}