using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyNum;  //번호가 맞는 문이면 연다

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Lock"))
        {
            Door door = collision.gameObject.GetComponent<Door>();
            if (door.doornum != keyNum)
            {
                Debug.Log("열쇠가 맞지 않는다");
                return;
            }
            if (door.isLock)
            {
                door.SetLock(false);
                if (door.doorState == DoorState.Cage)
                {
                    door.header.AI_Move(3);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
