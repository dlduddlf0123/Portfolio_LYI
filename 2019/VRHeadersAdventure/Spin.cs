using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    float t = 0;
    public float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        t += speed;
        transform.localRotation = Quaternion.Euler(0, t, 45);
    }
}
