using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Shooting
{
    /// <summary>
    /// 10/10/2023-LYI
    /// 발사체에서 발사될 투사체
    /// 풍선을 터트리거나 물건을 부순다
    /// AddForce로 발사됨, 충돌체크와 초기화 기능
    /// </summary>
    public class Arrow : MonoBehaviour
    {
        public Shooter shooter;
        public Rigidbody m_rigidbody;
        public bool isShooting;

        public void ArrowInit()
        {
            isShooting = false;

            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.isKinematic = true;

            transform.SetParent(shooter.tr_arrowReady);
            transform.position = shooter.tr_arrowReady.position;
            transform.rotation = shooter.tr_arrowReady.rotation;
        }

        public void ShootArrow(Vector3 direction, float speed)
        {
            isShooting = true;
            transform.parent = null;
            m_rigidbody.isKinematic = false;
            m_rigidbody.AddForce(direction.normalized * speed);
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Balloon"))
            {
                ArrowInit();
            }
            if (coll.gameObject.CompareTag("Header"))
            {
                ArrowInit();
            }
            if (coll.gameObject.CompareTag("Wall") || 
                coll.gameObject.CompareTag("Ground") ||
                coll.gameObject.CompareTag("Item"))
            {
                ArrowInit();
            }

        }


        private void Update()
        {
            if (isShooting)
            {
                float angle = Mathf.Atan2(m_rigidbody.velocity.y, m_rigidbody.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

    }
}