using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public Character header;
    public Door door;

    private void Start()
    {
        SetCageLock(door.isLock);
    }

    public void SetCageLock(bool _lock)
    {
        header.isCage = _lock;
    }


}
