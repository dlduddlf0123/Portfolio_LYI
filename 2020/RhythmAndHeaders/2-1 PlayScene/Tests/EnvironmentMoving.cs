using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMoving : MonoBehaviour
{
    public float value = 0.2f;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(-value * Time.deltaTime, 0, 0);
    }
}
