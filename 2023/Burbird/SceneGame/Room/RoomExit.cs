using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{
    public class RoomExit : MonoBehaviour
    {
        bool isColl = false;

        private void Start()
        {
            isColl = false;
        }
        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (StageManager.Instance.currentRoom.isRoomClear)
                {
                    if (!isColl)
                    {
                        isColl = true;
                        StageManager.Instance.RoomNext();
                    }
                }
            }
        }

    }
}