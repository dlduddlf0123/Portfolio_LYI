using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    NONE,
    DOOR,
    BLOCK,
    PLATFORM,
    TRAP,
    DISSOLVE,
}

public class Movable : MonoBehaviour
{
    protected SoundManager soundMgr;
    public MoveType type;

    private void Awake()
    {
        soundMgr = GameManager.Instance.soundMgr;

        DoAwake();
    }

    protected virtual void DoAwake() { }

    /// <summary>
    /// 상속받은 이후 각 오브젝트들의 움직임
    /// </summary>
    public virtual void Active(bool _active)
    {

    }

    public virtual void Active(bool _active, int _num)
    {

    }
    public virtual void Plus()
    {

    }
    public virtual void Minus()
    {

    }
    public virtual void Left()
    {

    }
    public virtual void Right()
    {

    }
}
