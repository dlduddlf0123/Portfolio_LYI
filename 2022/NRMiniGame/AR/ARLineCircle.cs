using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARLineCircle : MonoBehaviour
{
    public float speed = 0.3f;

    private void Update()
    {
        transform.Rotate(Vector3.forward * speed);
    }

}
