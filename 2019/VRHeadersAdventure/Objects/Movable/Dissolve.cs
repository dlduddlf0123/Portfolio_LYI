using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : Movable
{
    public Material render { get; set; }
    private float a;

    public bool isActive = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        type = MoveType.DISSOLVE;
        render = gameObject.GetComponent<MeshRenderer>().material;
        a = 0;
       
    }
    private void Update()
    {
        if(a<0.98 && isActive)
        {
            a = a + 0.001f;
            render.SetFloat("_DissolveAmount", a);
        }
        else if(a >= 0.98)
        {
            gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    public override void Active(bool _active)
    {
        isActive = _active;
    }
}
 