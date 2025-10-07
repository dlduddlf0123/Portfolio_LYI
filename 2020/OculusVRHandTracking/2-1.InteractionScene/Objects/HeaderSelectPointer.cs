using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderSelectPointer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += Vector3.up * 3f;
    }
}
