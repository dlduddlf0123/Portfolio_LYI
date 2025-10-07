using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameChanger : MonoBehaviour
{
    public int nameLength = 6;
    // Start is called before the first frame update
    void Start()
    {
        Transform[] arr_go = GetComponentsInChildren<Transform>();

        for (int i = 0; i < arr_go.Length; i++)
        {
            arr_go[i].name = arr_go[i].name.Substring(nameLength);
        }
       //gameObject.name = gameObject.name.Substring(nameLength);
    }
}
