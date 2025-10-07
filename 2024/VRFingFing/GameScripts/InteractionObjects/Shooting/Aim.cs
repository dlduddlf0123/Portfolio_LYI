using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction.Shooting
{
    public class Aim : MonoBehaviour
    {
        public bool isAim = false;

        public float aimSpeed = 80f;
        public float maxAngle = 90f;
        public float minAngle = 0f;
        public float currentAngle = 0;

        bool isUp = true;

        public void AimInit()
        {
            isAim = false;
            isUp = true;
            transform.localRotation = Quaternion.identity;
            currentAngle = 0;
        }



        // Update is called once per frame
        void Update()
        {
            if (isAim)
            {
                if (isUp)
                {
                    currentAngle += aimSpeed * Time.deltaTime;
                    if (currentAngle >= maxAngle)
                    {
                        currentAngle = maxAngle;
                        isUp = false;
                    }
                }
                else
                {
                    currentAngle -= aimSpeed * Time.deltaTime;
                    if (currentAngle <= minAngle)
                    {
                        currentAngle = minAngle;
                        isUp = true;
                    }
                }

                transform.localRotation = Quaternion.Euler(currentAngle,0,0);
            }

        }
    }
}
