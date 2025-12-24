using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDummyColl : MonoBehaviour
{
    ARPlaneChecker planeChecker;

    public bool isColled = false;

    private void Awake()
    {
        planeChecker = transform.parent.GetComponent<ARPlaneChecker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        isColled = true;
        planeChecker.CheckAllIn();
    }

    private void OnTriggerExit(Collider other)
    {
        isColled = false;
        planeChecker.CheckAllIn();
    }


}
