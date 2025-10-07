using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIMessage : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab_popupMessage;

    [SerializeField]
    private Transform tr_active;
    [SerializeField]
    private Transform tr_disable;

   private Queue<GameObject> queue_message = new ();


    void MessageInit(GameObject go)
    {
        go.transform.SetParent(tr_disable);
        go.SetActive(false);
        queue_message.Enqueue(go);
    }

    public GameObject CreateMessage()
    {
        GameObject go;

        if (queue_message.Count == 0)
        {
            go = Instantiate(prefab_popupMessage.gameObject, tr_active);
            go.transform.position = Vector3.zero;
        }
        else
        {
            go = queue_message.Dequeue().gameObject;
            go.transform.SetParent(tr_active);
            go.transform.position = Vector3.zero;

            go.SetActive(true);
        }

        return go;
    }


    public void PopupMessage(string msg)
    {
        PopupMessage popup = CreateMessage().GetComponent<PopupMessage>();

        popup.ShowMessage(msg, ()=>MessageInit(popup.gameObject));

    }



}
