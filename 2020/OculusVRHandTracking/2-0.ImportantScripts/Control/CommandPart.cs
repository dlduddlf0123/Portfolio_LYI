using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<MotionCommand>().motionStep++;
            gameObject.SetActive(false);
            transform.parent.GetComponent<MotionCommand>().CheckMotionRoll();
        }
    }
}
