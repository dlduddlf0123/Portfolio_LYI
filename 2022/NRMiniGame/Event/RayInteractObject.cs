using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RayInteractObject : MonoBehaviour
{
    public string rayOriginTag = null;
    public bool isActive = false;
    
    public UnityEvent m_RayEvent;
    public UnityEvent collsionEnter;
    public UnityEvent collsionStay;
    public UnityEvent collsionExit;


    private void OnEnable()
    {
        isActive = false;
    }
   
    private void OnDisable()
    {
        isActive = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// 손과 충돌체크   
    /// </summary><param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collsionEnter != null)
            {
                collsionEnter.Invoke();
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collsionStay != null)
            {
                collsionStay.Invoke();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collsionExit != null)
            {
                collsionExit.Invoke();
            }
        }
    }

    public void Active()
    {
        if (isActive)
        {
            return;
        }
        isActive = true;

        if (gameObject.activeSelf)
        {
            StartCoroutine(Timer(0.5f));
        }

        m_RayEvent.Invoke();
    }


    IEnumerator Timer(float _time)
    {
        yield return new WaitForSeconds(_time);
        isActive = false;
    }
}
