using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Burbird
{
    public class AimFire : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public PlayerController2D player;
        public AimSprites aimSprites;
        Vector2 startVec;
        Vector2 aimVec;
        Vector2 fireVec;

        float firePower = 0;
        bool isAim = false;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (player.currentFeatherCount <= 0)
            {
                return;
            }
            startVec = eventData.position;
            //Debug.Log("AimStartVector: " + startVec);

            isAim = true;

            //각도기 생성
            //캐릭터 머리 돌리기
            aimSprites.ActiveAimSprite(true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            aimVec = eventData.position;
            Debug.Log("AimingVector: " + aimVec);

            //각도기, 캐릭터 머리 돌리기
            fireVec = (startVec - aimVec).normalized;
            float angle = Mathf.Atan2(fireVec.y, fireVec.x) * Mathf.Rad2Deg;
            if (player.isLeft)
            {
                angle += 180f;
            }
            aimSprites.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            aimVec = eventData.position;
            fireVec = (startVec - aimVec).normalized;
            firePower = Vector2.Distance(startVec, aimVec);
            // Debug.Log("FirePower: " + firePower);

            //각도기 제거

            //player.Fire(fireVec, firePower);
            isAim = false;
            aimSprites.ActiveAimSprite(false);
        }

    }
}