using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private LineRenderer lr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
                if (hit.collider.gameObject.CompareTag("Header"))
                {
                    Character header = hit.collider.gameObject.GetComponent<Character>();
                    if (!header.isDie)
                    {
                        header.StartCoroutine(header.Respawn());
                        
                    }
                }
                else { return; }
            }
            
        }
        else lr.SetPosition(1, transform.position + (transform.forward * 100));
    }
}