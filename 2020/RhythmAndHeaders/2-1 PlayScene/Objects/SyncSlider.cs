using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SyncSlider : MonoBehaviour, IDragHandler
{

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector3(Input.mousePosition.x - Screen.width / 2, -599, 0);
    }
}
