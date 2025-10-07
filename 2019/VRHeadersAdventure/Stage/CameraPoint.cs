using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    public int camNum;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Header"))
        {
            StageManager stageMgr = GameManager.Instance.stageMgr;
            StartCoroutine(stageMgr.ChangeCameraPosition(camNum));
        }
    }
    
}
