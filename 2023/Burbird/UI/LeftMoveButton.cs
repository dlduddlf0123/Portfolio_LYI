using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Burbird
{
    public class LeftMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public PlayerController2D player;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            player.MoveLeft();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            player.MoveEnd();
        }
    }
}