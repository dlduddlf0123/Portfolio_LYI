using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : OVRGrabber
{
    public GameManager gameMgr;

    public bool isHit = false; //공격상태?
    public bool isDirty = false; //손 오염상태

    public Color[] handColor;

    public virtual void ChangeHandColor(Color _top, Color _bottom)
    {


    }

}
