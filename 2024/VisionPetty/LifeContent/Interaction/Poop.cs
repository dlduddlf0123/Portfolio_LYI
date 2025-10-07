using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

namespace AroundEffect
{
    /// <summary>
    /// 11/25/2024-LYI
    /// 똥. 배부르거나 밥먹을 때 확률적으로 싼다
    /// 주변 청결도 악화 ?초당?.
    /// 손가락으로 건들거나 핀치로 없앤다
    /// </summary>
    public class Poop : MonoBehaviour
    {
        GameManager gameMgr;
        public MMF_Player mmf_poof;
        public Transform tr_model;

        public ParticleSystem p_pooping; //쌀때 효과. 방구?
        public ParticleSystem p_poop_loop; //파리나 냄새
        public ParticleSystem p_poof; //펑 하고 사라짐

        private void Awake()
        {
            gameMgr = GameManager.Instance;
            PoopInit();
        }

        public void PoopInit()
        {
            tr_model.localScale = Vector3.one;

        }

        public void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_HAND))
            {
                PoopDisappear();
            }
        }



        public void OnPoop()
        {
            PoopInit();

            //p_pooping.Play();
            gameMgr.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_POOP);

        }

        public void PoopDisappear()
        {
            mmf_poof.PlayFeedbacks();
            //p_poof.Play();
        }



    }
}