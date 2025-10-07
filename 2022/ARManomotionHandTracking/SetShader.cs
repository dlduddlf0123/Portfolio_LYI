using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetShader : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("Standard"); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
