using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRunColl : MonoBehaviour
{
    public TireRunInteract runMgr;

    private void Update()
    {
        transform.localPosition += Vector3.forward * 5f * runMgr.moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("End"))
        {
            transform.localPosition -= Vector3.forward * 20f;

            runMgr.queue_tree.Enqueue(this.gameObject);
            gameObject.SetActive(false);
        }

        if (coll.gameObject.CompareTag("Player"))
        {
            if (coll.gameObject.GetComponent<RunningTire>())
            {
                coll.gameObject.GetComponent<RunningTire>().GetHit();
            }
        }
    }
}
