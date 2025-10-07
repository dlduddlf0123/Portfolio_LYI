using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) //Item Layer = 10
        {
            Destroy(other.gameObject);
            //쓰레기 버리는 소리
        }
    }
}
