using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public bool isLeft;
    public bool isUp;
    public Transform startPosition;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.position.z > startPosition.position.z)
        {
            isLeft = true;            
        }
        else
        {
            isLeft = false;
        }
        if (other.gameObject.transform.position.y > 3)
        {
            isUp = true;
        }
        else
        {
            isUp = false;
        }
    }
}
