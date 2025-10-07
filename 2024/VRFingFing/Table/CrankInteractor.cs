using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok
{

    /// <summary>
    /// 10/27/2023-LYI
    /// 테이블 옆의 크랭크
    /// 손잡이를 잡고 돌려 테이블의 사이즈를 변경한다
    /// </summary>다
    public class CrankInteractor : MonoBehaviour
    {
        private Quaternion initialRotation;
        private bool isInteracting = false;

        public Quaternion rotationChange;

        void Start()
        {
            initialRotation = transform.rotation;
        }

        private void Update()
        {
            CalculateRotationChange();
        }


        // Method called when the VR controller grabs the valve handle
        public void StartInteraction()
        {
            isInteracting = true;
        }

        // Method called when the VR controller releases the valve handle
        public void EndInteraction()
        {
            isInteracting = false;
        }

        // Calculate the rotation change based on the initial and current rotations
        public float CalculateRotationChange()
        {
            if (isInteracting)
            {
                Quaternion currentRotation = transform.rotation;
                rotationChange = Quaternion.Inverse(initialRotation) * currentRotation;
                return rotationChange.eulerAngles.z;
            }
            else
            {
                return 0f;
            }
        }
    }
}