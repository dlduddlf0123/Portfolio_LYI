using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction.Shooting
{
    public class Fan : Tok_Interact
    {
        public MoreMountains.Tools.MMAutoRotate rotater;

        public List<Rigidbody> list_affectedObject = new List<Rigidbody>(); // 영향을 받을 오브젝트 리스트

        public bool isFanActive = true;

        public bool isAutoActiveChange = false;
        public float activeTime = 2f;
        float t = 0f;

        public float affectMoveSpeed = 1.0f; // 벨트의 속도
        public float speedAccelator = 1f;
        public float currentSpeed = 0;



        public override void InteractInit()
        {
            base.InteractInit();

            rotater.Rotating = isFanActive;
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Balloon"))
            {
                // 벨트와 충돌한 오브젝트를 리스트에 추가 (중복 체크)
                Rigidbody tr = coll.transform.parent.gameObject.GetComponent<Rigidbody>();
                if (tr != null && !list_affectedObject.Contains(tr))
                {
                    list_affectedObject.Add(tr);
                }
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Balloon"))
            {
                // 벨트에서 벗어난 오브젝트를 리스트에서 제거
                Rigidbody tr = coll.transform.parent.gameObject.GetComponent<Rigidbody>();
                if (tr != null)
                {
                    list_affectedObject.Remove(tr);
                }
            }
        }

        private void Update()
        {
            if (isAutoActiveChange)
            {
                t += Time.deltaTime;
                if (t > activeTime)
                {
                    isFanActive = !isFanActive;
                    rotater.Rotating = isFanActive;
                    t = 0;
                }
            }

            if (isFanActive)
            {
                // 리스트에 있는 모든 오브젝트에 속도를 적용
                currentSpeed = Mathf.Lerp(currentSpeed, affectMoveSpeed, speedAccelator * Time.deltaTime);
                Vector3 velocity = transform.forward * currentSpeed * Time.deltaTime;
                foreach (Rigidbody tr in list_affectedObject)
                {
                    tr.velocity += velocity;
                }
            }
            else
            {
                currentSpeed = 0;
            }
        }


        public override void ActiveInteraction()
        {
            base.ActiveInteraction();

            isFanActive = true;
            rotater.Rotating = isFanActive;
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();

            isFanActive = false;
            rotater.Rotating = isFanActive;
        }


    }
}