using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CharacterTransformRay : MonoBehaviour, IPointerDownHandler
{
    GameManager gameMgr;

    [SerializeField]
    [Tooltip("The prefab to be placed or moved.")]
    GameObject m_PrefabToPlace;

    public GameObject m_SpawnedObject;

    public delegate void onObjectSpawn();
    public onObjectSpawn onSpawn;

    /// <summary>
    /// The prefab to be placed or moved.
    /// </summary>
    public GameObject prefabToPlace
    {
        get => m_PrefabToPlace;
        set => m_PrefabToPlace = value;
    }

    /// <summary>
    /// The spawned prefab instance.
    /// </summary>
    public GameObject spawnedObject
    {
        get => m_SpawnedObject;
        set => m_SpawnedObject = value;
    }


    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ARRaycast(eventData.position);
    }


    public void ARRaycast(Vector2 touchPos)
    {
        Ray ray = gameMgr.mainCamera.ScreenPointToRay(touchPos);
        RaycastHit rayHit;

        int obstacleMask = (1 << LayerMask.NameToLayer("Default")) |
                (1 << LayerMask.NameToLayer("Ignore Raycast"));

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity,~obstacleMask))
        {
            //Parent 설정할 경우 캐릭터가 지면 인식 확장될 때 마다 움직임
            //Plane의 Y값만 받을것
            GameObject clickedObject = rayHit.collider.gameObject;

            Debug.Log("Clicked Object: " + clickedObject.name);

            // You can perform any desired action using the clickedObject
            // For example, you can change the color of the object, play a sound, etc.

            Debug.DrawRay(ray.origin, ray.direction * rayHit.distance, Color.blue, 2f);

            if (m_SpawnedObject == null)
            {
                //m_SpawnedObject = Instantiate(m_PrefabToPlace, rayHit.point, Quaternion.identity, clickedObject.transform);
                m_SpawnedObject = Instantiate(m_PrefabToPlace, rayHit.point, Quaternion.identity);
                onSpawn.Invoke();

                gameMgr.uiMgr.ui_characterTransform.SetSliderValuePosition(clickedObject.transform.position.y);
                m_SpawnedObject.transform.position = rayHit.point;
            }
            else
            {
                m_SpawnedObject.transform.position = rayHit.point;
                //m_SpawnedObject.transform.SetParent(clickedObject.transform);
                gameMgr.uiMgr.ui_characterTransform.SetSliderValuePosition(clickedObject.transform.position.y);
            }
            // gameMgr.playMgr.tokMgr.Tok(hitInfo.point, clickedObject);
        }

    }
}
