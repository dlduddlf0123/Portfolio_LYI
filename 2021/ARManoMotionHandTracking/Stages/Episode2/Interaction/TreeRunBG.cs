using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRunBG : MonoBehaviour
{
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("End"))
        {
            transform.localPosition -= Vector3.forward * 20f;
        }
    }
}
