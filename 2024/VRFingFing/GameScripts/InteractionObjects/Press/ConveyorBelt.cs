using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction.Press
{

    /// <summary>
    /// 10/6/2023-LYI
    /// 이 오브젝트 위의 Rigidbody들을 움직인다
    /// </summary>
    public class ConveyorBelt : MonoBehaviour
    {
        public List<Transform> list_affectedObject = new List<Transform>(); // 영향을 받을 오브젝트 리스트

        public bool beltActive = true;
        public float beltSpeed = 1.0f; // 벨트의 속도

        private void Awake()
        {

        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                // 벨트와 충돌한 오브젝트를 리스트에 추가 (중복 체크)
                Transform tr = coll.gameObject.transform;
                if (tr != null && !list_affectedObject.Contains(tr))
                {
                    list_affectedObject.Add(tr);
                }
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                // 벨트에서 벗어난 오브젝트를 리스트에서 제거
                Transform tr = coll.gameObject.transform;
                if (tr != null)
                {
                    list_affectedObject.Remove(tr);
                }
            }
        }

        private void Update()
        {
            if (beltActive)
            {
                // 리스트에 있는 모든 오브젝트에 벨트의 속도를 적용
                Vector3 velocity = transform.forward * beltSpeed * Time.deltaTime;
                foreach (Transform tr in list_affectedObject)
                {
                    tr.Translate(velocity);
                }
            }
        }
    }
}