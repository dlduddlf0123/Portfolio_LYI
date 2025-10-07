using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherDispenser : MonoBehaviour
{
    public GameObject featherItem;

    float time = 0;
    // Update is called once per frame
    void Update()
    {
        if (time < 3)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
                GameObject item =  Instantiate(featherItem, transform);
                item.transform.position = transform.position;
        }
        
    }
}
