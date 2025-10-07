using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public bool isOpen = false;

    public Transform trOpen;
    public Transform trClose;

    float speed = 5f;

    public void DoorOpen()
    {
        StartCoroutine(DoorOpenMove());
    }
    public void DoorClose()
    {
        StartCoroutine(DoorCloseMove());
    }

    IEnumerator DoorOpenMove()
    {
        Vector3 startPos = transform.position;
        float t = 0;
        while(t <1)
        {
            t += 0.01f*speed;

            transform.position = Vector2.Lerp(startPos, trOpen.position, t);

            yield return new WaitForSeconds(0.01f);
        }

        isOpen = true;
    }

    IEnumerator DoorCloseMove()
    {
        Vector3 startPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            t += 0.01f * speed * 3;

            transform.position = Vector2.Lerp(startPos, trClose.position, t);

            yield return new WaitForSeconds(0.01f);
        }

        isOpen = false;
    }
}
