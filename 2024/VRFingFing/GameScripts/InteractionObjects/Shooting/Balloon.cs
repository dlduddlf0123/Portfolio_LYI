using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Shooting
{
    public class Balloon : Tok_Interact
    {
        Rigidbody m_rigidbody;

        private int hp = 0;
        public int maxHp = 1;

        public float floatingPower;
        public float movePower;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        public override void InteractInit()
        {
            base.InteractInit();

            BalloonInit();
        }

        public void BalloonInit()
        {
            hp = maxHp; //체력 최대로 설정, 그래픽 변경?
            m_rigidbody.velocity = Vector3.zero;
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Arrow"))
            {
                hp--;
                if (hp <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            m_rigidbody.velocity += Vector3.up * (floatingPower + movePower) * Time.deltaTime;
        }

    }
}