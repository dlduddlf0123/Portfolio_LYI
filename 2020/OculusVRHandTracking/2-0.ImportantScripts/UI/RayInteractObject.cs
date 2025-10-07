using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RayInteractObject : MonoBehaviour
{
    public bool isActive = false;
    
    public UnityEvent m_RayEvent;

    private void OnEnable()
    {
        if (transform.parent.GetComponent<Button>())
        {
            m_RayEvent = transform.parent.GetComponent<Button>().onClick;
        }
        isActive = false;
    }
   
    private void OnDisable()
    {
        isActive = false;
        StopAllCoroutines();
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
