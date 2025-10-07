using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Burbird
{
    public class RightMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public PlayerController2D player;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            player.MoveRight();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            player.MoveEnd();
        }
    }
}