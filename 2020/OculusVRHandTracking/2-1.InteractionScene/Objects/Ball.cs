using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Item
{
    public bool isTalk = false;
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isThrowing)
            {
                if (!isTalk)
                {
                    isTalk = true;
                    stageMgr.interactHeader.headerCanvas.ShowText(12, Random.Range(0, 2));
                }
                stageMgr.interactHeader.MoveCharacter(transform.position, gameObject);
            }
        }
    }


    protected override IEnumerator Reset(float _time)
    {
        isTalk = false;
        return base.Reset(_time);
    }
}
