using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodStick : MonoBehaviour
{
    Transform startTr;

    // Start is called before the first frame update
    void Start()
    {
        startTr = transform;  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Header"))
        {
            Character header = other.GetComponent<Character>();
            header.AI_Move(1);
            header.headerCanvas.ShowText(4, Random.Range(0, 2));
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Destroy());
        }
    }


    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);
        transform.position = startTr.position;
        transform.rotation = startTr.rotation;
    }
}
