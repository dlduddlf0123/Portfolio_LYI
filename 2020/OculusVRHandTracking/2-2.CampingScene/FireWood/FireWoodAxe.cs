using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWoodAxe : MonoBehaviour
{
    public Transform resetTransform;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            transform.position = resetTransform.position;
            transform.rotation = resetTransform.rotation;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

}
