using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public enum DoorState
{
    Normal = 0,
    Cage = 1,
    Trap = 2,
    Exit = 9,
}

public class Door : Movable
{
    public DoorState doorState;

    public Character header;

    public TeleportPoint teleportPoint;

    Rigidbody rb;
    public bool isLock;
    public int doornum; //번호가 맞는 열쇠여야 열린다

    private void Awake()
    {
        type = MoveType.DOOR;
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        SetLock(isLock);
    }

    /// <summary>
    /// 문의 잠금 상태를 변경한다
    /// </summary>
    /// <param name="_lock"></param>
    public void SetLock(bool _lock)
    {
        rb.isKinematic = _lock;
        switch (doorState)
        {
            case DoorState.Normal:
                break;
            case DoorState.Cage:
                if (header == null) { break; }
                header.isCage = _lock;
                header.transform.position = this.transform.parent.GetChild(0).position;
                header.mNavAgent.enabled = !_lock;
                this.gameObject.SetActive(_lock);
                header.AI_Move(3);
                break;
            case DoorState.Trap:
                break;
            case DoorState.Exit:
                if (teleportPoint == null) { break; }
                teleportPoint.locked = _lock;
                teleportPoint.UpdateVisuals();
                break;
            default:
                break;
        }
    }

    public override void Active(bool _active)
    {

    }
}
