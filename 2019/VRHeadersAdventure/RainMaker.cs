using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainMaker : MonoBehaviour
{
    public GameObject rainObject;
    float range  = 3;
    int many = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < many; i++)
        {
            GameObject Rain = Instantiate(rainObject,this.transform);
            Rain.transform.position = new Vector3(Random.Range(-range, range), 10, Random.Range(-range, range));
        }
    }
}
