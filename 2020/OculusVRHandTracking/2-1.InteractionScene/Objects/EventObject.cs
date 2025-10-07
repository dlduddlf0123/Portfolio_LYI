using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        GameManager.Instance.StartCoroutine(GameManager.Instance.NextDay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.white);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

}
