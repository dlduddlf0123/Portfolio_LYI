using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchChecker : MonoBehaviour,IPointerClickHandler
{
    GameManager gameMgr;
    ARObjectSelect currentSelector;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click!");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (GameManager.Instance.statGame == GameStatus.SELECT)
            {
                if (hit.collider.GetComponent<ARSelectableObject>())
                {
                    currentSelector = hit.collider.transform.parent.GetComponent<ARObjectSelect>();
                    currentSelector.SelectObject(hit.collider.GetComponent<ARSelectableObject>());
                }
                else
                {
                    if (currentSelector != null)
                        currentSelector.DeSelectObject();
                }
            }
            if (hit.collider.GetComponent<RayInteractObject>())
            {
                RayInteractObject obj = hit.transform.GetComponent<RayInteractObject>();
                if (!obj.isActive)
                {
                    obj.Active();
                }
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 0.3f);
    }

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            if (GameManager.Instance.statGame == GameStatus.SELECT)
    //            {
    //                if (hit.collider.GetComponent<ARSelectableObject>())
    //                {
    //                    currentSelector = hit.collider.transform.parent.GetComponent<ARObjectSelect>();
    //                    currentSelector.SelectObject(hit.collider.GetComponent<ARSelectableObject>());
    //                }
    //                else
    //                {
    //                    if (currentSelector != null)
    //                        currentSelector.DeSelectObject();
    //                }
    //            }
    //            if (hit.collider.GetComponent<RayInteractObject>())
    //            {
    //                RayInteractObject obj = hit.transform.GetComponent<RayInteractObject>();
    //                if (!obj.isActive)
    //                {
    //                    obj.Active();
    //                }
    //                //currentSelector = hit.transform.parent.GetComponent<ARObjectSelect>();
    //                //currentSelector.SelectObject(() => { },
    //                //     hit.collider.GetComponent<ARSelectableObject>());
    //            }


    //        }
    //        Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 0.3f);
    //    }
    //}


}
