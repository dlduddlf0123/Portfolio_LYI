using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserColl : MonoBehaviour
{
    public Character header;

    private void OnTriggerEnter(Collider other)
    {
        //위치 정보값 넘겨주기
        header.laserPos = other.transform;
    }
}
