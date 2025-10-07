using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeImage : MonoBehaviour
{
    GameObject arPrefab;

    private void Awake()
    {
        arPrefab = transform.GetChild(0).gameObject;   
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.x < Quaternion.identity.x ||
            transform.rotation.y < Quaternion.identity.y)
        {
            if (arPrefab.activeSelf)
            {
                arPrefab.SetActive(false);
            }
        }
        else
        {
            if (!arPrefab.activeSelf)
            {
                arPrefab.SetActive(true);
            }
        }
    }
}
