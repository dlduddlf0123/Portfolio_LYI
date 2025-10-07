using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : Item
{
    public int brushCount = 0;
    public OVRGrabbable grabbable;

    private void Awake()
    {
        grabbable = GetComponent<OVRGrabbable>();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isThrowing)
            {
                stageMgr.interactHeader.MoveCharacter(transform.position, gameObject);
            }
            else
            {
                StartCoroutine(Reset(30));
            }
        }
    }

}
