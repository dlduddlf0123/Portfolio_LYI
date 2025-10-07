using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbFingerAct : MonoBehaviour
{
    [SerializeField]
    ManoHandMove manoHand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Index"))
        {
            manoHand.isPinch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Index"))
        {
            manoHand.isPinch = false;
        }
    }
}
