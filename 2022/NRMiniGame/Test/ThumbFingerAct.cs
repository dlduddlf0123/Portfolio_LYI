using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbFingerAct : MonoBehaviour
{
    [SerializeField]
    NRHandMove nrHand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Index"))
        {
            nrHand.isPinch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Index"))
        {
            nrHand.isPinch = false;
        }
    }
}
