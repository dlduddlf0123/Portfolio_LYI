using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Character;
using VRTokTok.Manager;

namespace VRTokTok.Interaction.Crossing
{

    /// <summary>
    /// 8/7/2023-LYI
    /// 캐릭터가 올라설 이동하는 플랫폼
    /// </summary>
    public class Crossing_Platform : MonoBehaviour
    {
        GameManager gameMgr;
        public Crossing_Lane lane;
        public TokGround ground;
        public GameObject model;
        public Transform tr_parent;

        public float maxPos = 0f;
        public float laneSpeed = 1f; //떠다니는 오브젝트의 속도
        public int direction = 1; //떠다닐 방향

        [Header("Shake")]
        public bool isShake = false; //흔들림 효과
        public float shakeRange = 0.05f;
        public float shakeSpeed = 1f;

        Coroutine currentCoroutine = null;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            if (ground == null)
            {
                ground = transform.GetComponentInChildren<TokGround>();
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                coll.gameObject.transform.SetParent(tr_parent);
            }
        }
        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                coll.gameObject.GetComponent<Tok_Movement>().ResetParent();
            }
        }



        public void MovePlatform(float spd, int dir, float max)
        {
            laneSpeed = spd;
            direction = dir;
            maxPos = max * dir;

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            currentCoroutine = StartCoroutine(Move());
        }


        private IEnumerator Move()
        {
            bool isMove = true;

            //Shake
            int shakeDir = 1;
            float t = 0;
            Vector3 startShakePos = Vector3.zero;
            if (model != null)
            {
                startShakePos = model.transform.localPosition;
            }

            WaitForSeconds wait = new WaitForSeconds(0.01f);

            while (gameMgr.statGame == GameStatus.GAME &&
                isMove)
            {
                if (direction == -1)
                {
                    isMove = transform.localPosition.z > maxPos;
                }
                else
                {
                    isMove = transform.localPosition.z < maxPos;
                }

                float move = direction * laneSpeed * Time.deltaTime;
                transform.Translate(0, 0, move);

                if (isShake)
                {
                    if (t >= 1)
                    {
                        startShakePos = Vector3.forward * shakeDir * shakeRange;
                        shakeDir *= -1;
                        t = 0;
                    }


                    model.transform.localPosition = Vector3.Lerp(startShakePos, shakeDir * Vector3.forward * shakeRange, t);
                 

                    t += 0.01f * shakeSpeed;
                }
                yield return wait;
            }

            if (tr_parent.childCount > 0)
            {
                for (int i = 0; i < tr_parent.childCount; i++)
                {
                    tr_parent.GetChild(i).GetComponent<Tok_Movement>().ResetParent();
                }
            }

            model.transform.localPosition = Vector3.zero;
            lane.DisableObject(this.gameObject);
        }


    }
}