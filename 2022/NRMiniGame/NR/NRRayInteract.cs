using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Ray Interaction with NReal raycast
/// </summary>
public class NRRayInteract : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string rayOriginTag = null;
    public bool isActive = false;
    
    public UnityEvent m_RayEvent;

    public UnityEvent pointerClick;
    public UnityEvent pointerHover;
    public UnityEvent pointerExit;

    bool isEnter = false;

    private void OnEnable()
    {
        if (transform.parent.GetComponent<Button>())
        {
           pointerClick = transform.parent.GetComponent<Button>().onClick;
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


    /// <summary> when pointer click, set the cube color to random color. </summary>
    /// <param name="eventData"> Current event data.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("RayPointer Click!" + eventData.position);
        pointerClick.Invoke();

        GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_CLICK);
        GameManager.Instance.PlayTouchParticle(transform.position);
    }

    /// <summary> when pointer hover, set the cube color to green. </summary>
    /// <param name="eventData"> Current event data.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnter)
        {
            return;
        }

        Debug.Log("RayPointer Enter!" + eventData.position);
        isEnter = true;
        CancelInvoke("ResetEvent");
        Invoke("ResetEvent", 0.5f);
        pointerHover.Invoke();
    }

    /// <summary> when pointer exit hover, set the cube color to white. </summary>
    /// <param name="eventData"> Current event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerExit.Invoke();
    }

    void ResetEvent()
    {
        isEnter = false;
    }
}
