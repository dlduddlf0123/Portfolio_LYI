using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Movable
{
    Animator mAnim;
    public bool isMove = false;
    private void Awake()
    {
        type = MoveType.BLOCK;
        mAnim = GetComponent<Animator>();
    }

    public override void Active(bool _active)
    {
        isMove = _active;
        mAnim.SetBool("isMove", _active);
    }
}
