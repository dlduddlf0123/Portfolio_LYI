using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace AroundEffect
{

    /// <summary>
    /// 8/30/2024-LYI
    /// Collider for check hand gesture
    /// just call back is colled with anything
    /// </summary>
    public class EventCollider : MonoBehaviour
    {
        public XRHandGestureInput handInput;

        public GameObject colledGameObject;
        public bool isColled = false;

        [Header("Event")]
        public UnityEvent OnEnterEvent;
        public UnityEvent OnExitEvent;

        public UnityAction<Collider> OnEnterCollider;
        public UnityAction<Collider> OnExitCollider;

        protected void OnTriggerEnter(Collider coll)
        {
            OnEnter(coll);
        }

        protected void OnTriggerStay(Collider coll)
        {
            OnStay(coll);
        }

        protected void OnTriggerExit(Collider coll)
        {
            OnExit(coll);
        }

        protected virtual void OnEnter(Collider coll)
        {
            if (coll != null)
            {
                isColled = true;
                colledGameObject = coll.gameObject;
                OnEnterEvent?.Invoke();
                OnEnterCollider?.Invoke(coll);
            }
        }
        protected virtual void OnStay(Collider coll)
        {
            if (coll != null)
            {
                isColled = true;
                colledGameObject = coll.gameObject;
            }
        }
        protected virtual void OnExit(Collider coll)
        {
            if (coll != null)
            {
                isColled = false;
                colledGameObject = coll.gameObject;
                OnExitEvent?.Invoke();
                OnExitCollider?.Invoke(coll);
            }
        }

    }
}