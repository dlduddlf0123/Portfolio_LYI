using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction
{
    public class Trap_Projectile : MonoBehaviour
    {
        Rigidbody m_rigidbody;

        public Trap_Shooting shooter;

        public Transform currentTarget;
        public float speed = 1f;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Wall") ||
                coll.gameObject.CompareTag("Ground"))
            {
                if (shooter != null)
                {
                    shooter.MissileHit(this);
                    shooter.MissileInit(this.gameObject);
                }
            }
            if (coll.gameObject.CompareTag("Header"))
            {
                if (shooter != null)
                {
                    shooter.MissileHit(this);
                    shooter.MissileInit(this.gameObject);
                }
            }
        }


        public void TargetShot(Transform target)
        {
            currentTarget = target;

            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.AddForce((currentTarget.transform.position - transform.position).normalized * speed, ForceMode.Impulse);
        }
        public void TargetShot(Vector3 target)
        {
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.AddForce(target.normalized * speed, ForceMode.Impulse);
        }
    }
}