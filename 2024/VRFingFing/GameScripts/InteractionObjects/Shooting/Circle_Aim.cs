using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Shooting
{
    public class Circle_Aim : MonoBehaviour
    {
        public Material[] arr_matAim;
        public Renderer[] arr_aimPoint;

        public float aimTime = 0.5f;
        public bool isAim = false;

        public int currentAim = 0;
        float t = 0f;
        bool isUp = true;

        // Start is called before the first frame update
        void Start()
        {
            AimInit();
        }


        public void AimInit()
        {
            currentAim = 0;
            isUp = true;
            t = 0;

            for (int i = 0; i < arr_aimPoint.Length; i++)
            {
                arr_aimPoint[i].material = arr_matAim[0]; //비활성 상태
            }
            arr_aimPoint[currentAim].material = arr_matAim[1];//활성 상태
        }

        public Vector3 GetAimPoint()
        {
            return arr_aimPoint[currentAim].transform.position - transform.position;
        }


        private void Update()
        {
            if (isAim)
            {
                t += Time.deltaTime;

                if (t>= aimTime)
                {
                    if (isUp)
                    {
                        currentAim++;
                        if (currentAim >= arr_aimPoint.Length - 1)
                        {
                            isUp = false;
                        }
                    }
                    else
                    {
                        currentAim--;
                        if (currentAim <= 0)
                        {
                            isUp = true;
                        }
                    }

                    for (int i = 0; i < arr_aimPoint.Length; i++)
                    {
                        arr_aimPoint[i].material = arr_matAim[0]; //비활성 상태
                    }
                    arr_aimPoint[currentAim].material = arr_matAim[1];//활성 상태

                    t = 0;
                }


            }
        }

    }
}