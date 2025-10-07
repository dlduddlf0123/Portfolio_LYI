using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;



#if UNITY_EDITOR

/// <summary>
/// 8/22/2023-LYI
/// Add TokTok Input with mouse click in editor
/// </summary>
public class Tok_MouseDebug : MonoBehaviour
{
    GameManager gameMgr;


    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = gameMgr.pcCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            int obstacleMask = (1 << LayerMask.NameToLayer("Player")) |
                    (1 << LayerMask.NameToLayer("Character")) |
                    (1 << LayerMask.NameToLayer("Ignore Raycast")) |
                    (1 << LayerMask.NameToLayer("Default")) |
                     (1 << LayerMask.NameToLayer("TokMarker")) |
                     (1 << LayerMask.NameToLayer("Interact"));

            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~obstacleMask))
            {
                GameObject clickedObject = hitInfo.collider.gameObject;

                Debug.Log("Clicked Object: " + clickedObject.name);

                // You can perform any desired action using the clickedObject
                // For example, you can change the color of the object, play a sound, etc.

                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.blue, 2f);

                gameMgr.playMgr.tokMgr.Tok(hitInfo.point,clickedObject);
            }
        }
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    Vector2 mousePosition = eventData.position;

    //    gameMgr.currentStage.tokMgr.Tok(new Vector3(mousePosition.x, transform.position.y, mousePosition.y));


    //    //
    //    //Vector3 worldPosition = mainCam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCam.nearClipPlane));

    //    // Display the world position in the console
    //    //Debug.Log("Mouse Click World Position: " + worldPosition);
    //    //Vector3 rayOrigin = mainCam.transform.position;
    //    //Vector3 rayDirection = worldPosition - rayOrigin;

    //    //Debug.DrawRay(rayOrigin, rayDirection, Color.red, 2f);

    //}



}

#endif