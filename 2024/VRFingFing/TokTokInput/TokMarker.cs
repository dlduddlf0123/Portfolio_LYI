using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Interaction;


namespace VRTokTok
{
    /// <summary>
    /// 바닥 찍었을 때 등장하는 마커
    /// 같은 곳에 찍히면 이동
    /// 클릭 시 이펙트 들고있음
    /// 클릭한 곳의 Interactor 가져오기?
    /// </summary>
    public class TokMarker : MonoBehaviour
    {
        GameManager gameMgr;
        [SerializeField]
        ParticleSystem fx_marker;
        [SerializeField]
        ParticleSystem fx_touch;

        public bool isMarkerActive = false;
        public float selectDistance = 0.03f; //최소 클릭 거리

        public Tok_Interact clickedInteraction;
        public bool isColled = false; 

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }

        void Start()
        {
            this.gameObject.SetActive(false);
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                OffTok();
            }
        }

        private void OnTriggerStay(Collider coll)
        {
            if (coll.gameObject.GetComponentInParent<Tok_Interact>())
            {
                isColled = true;
                if (clickedInteraction == null)
                {
                    clickedInteraction = coll.gameObject.GetComponentInParent<Tok_Interact>();
                }
            }
            else
            {
                isColled = false;
            }

        }


        //private void OnTriggerStay(Collider coll)
        //{
        //    if (coll.gameObject.CompareTag("Button"))
        //    {
        //        if (clickedInteraction == null)
        //        {
        //            if (coll.gameObject.GetComponentInParent<Tok_Button>())
        //            {
        //                clickedInteraction = coll.gameObject.GetComponentInParent<Tok_Button>();
        //            }
        //        }

        //    }
        //    else
        //    {
        //        clickedInteraction = null;
        //    }
        //}

        //private void OnTriggerExit(Collider coll)
        //{
        //    if (coll.gameObject.CompareTag("Button"))
        //    {
        //        if (coll.gameObject.GetComponentInParent<Tok_Button>())
        //        {
        //            clickedButton = null;
        //        }
        //    }
        //}


        public void OnTok()
        {
            //if (!isMarkerActive)
            //{
            //    PlayParticle(fx_marker);
            //}
            //PlayParticle(fx_touch);

            gameMgr.playMgr.tokMgr.PlayTokParticle(fx_touch.transform.position);

            isMarkerActive = true;
        }

        public void OffTok()
        {
            //StopParticle(fx_marker);
            //StopParticle(fx_touch);
            transform.SetParent(null);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            isMarkerActive = false;
        }



        public void PlayParticle(ParticleSystem p)
        {
            ParticleSystem[] arr_p = p.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < arr_p.Length; i++)
            {
                arr_p[i].Play();
            }
        }
        public void StopParticle(ParticleSystem p)
        {
            ParticleSystem[] arr_p = p.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < arr_p.Length; i++)
            {
                arr_p[i].Stop();
            }
        }

        /// <summary>
        /// 3/6/2024-LYI
        /// 클릭 시 호출
        /// 클릭 지점의 인터렉션 체크
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Tok_Interact GetClosestInteract()
        {

            Tok_Interact interact = null;
            float distance = selectDistance;
            for (int i = 0; i < gameMgr.playMgr.currentStage.list_interact.Count; i++)
            {
                float d2 = Vector3.Distance(transform.position, gameMgr.playMgr.currentStage.list_interact[i].transform.position);
                if (d2 < distance)
                {
                    interact = gameMgr.playMgr.currentStage.list_interact[i];
                    distance = d2;
                }
            }

            if (interact != null)
            {
                return interact;
            }
            else
            {
                return null;
            }
        }

    }
}