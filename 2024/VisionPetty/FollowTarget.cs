using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    /// <summary>
    /// 9/4/2024-LYI
    /// Simple script for lerp move
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        public Transform tr_followTarget;

        public float f_lerpPercent = 0.8f;
        public Vector3 adjustPosition = new Vector3();

        [Header("Bool Follow")]
        public bool followPosition = true;
        public bool followRotation = false;

        [Header("FixedRotation")]
        public bool rotationFix_X = false;
        public bool rotationFix_Y = false;
        public bool rotationFix_Z = false;

        void FixedUpdate()
        {
            if (this.gameObject.activeInHierarchy)
            {
                if (tr_followTarget == null)
                {
                    return;
                }

                if (followPosition)
                {
                    transform.position = Vector3.Lerp(transform.position, tr_followTarget.position + adjustPosition, f_lerpPercent);
                }

                if (followRotation)
                {
                    if (rotationFix_X|| rotationFix_Y||rotationFix_Z)
                    {
                        Quaternion q = Quaternion.Lerp(transform.rotation, tr_followTarget.rotation, f_lerpPercent);
                        float x = rotationFix_X ? 0 : q.x;
                        float y = rotationFix_Y ? 0 : q.y;
                        float z = rotationFix_Z ? 0 : q.z;
                        transform.rotation = new Quaternion(x, y, z, q.w);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, tr_followTarget.rotation, f_lerpPercent);
                    }
                }

            }
        }
    }
}