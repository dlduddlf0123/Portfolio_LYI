using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRTokTok {
    public class TutorialLeftHandle : MonoBehaviour
    {
        public GameObject handleMark;
        public GameObject[] arr_mark;

        public ParticleSystem efx_end;

        public UnityAction onHandleEnd = null;

        public float waitTime = 1f;
        public bool isTutorial = false;


        public void StartLeftHandleTutorial()
        {
            GameManager.Instance.tableMgr.tableGrabHandle.onGrabStart += EndMark;
            GameManager.Instance.tableMgr.tableGrabHandle.onGrabEnd += EndTutorial;

            isTutorial = true;
            handleMark.gameObject.SetActive(true);
            StartCoroutine(MarkActive());
        }


        IEnumerator MarkActive()
        {
            for (int i = 0; i < arr_mark.Length; i++)
            {
                arr_mark[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < arr_mark.Length; i++)
            {
                arr_mark[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(waitTime);
            }
        }


        /// <summary>
        /// 7/22/2024-LYI
        /// 손잡이 잡을 때 호출
        /// </summary>
        public void EndMark()
        {
            if (!isTutorial)
            {
                return;
            }
            for (int i = 0; i < arr_mark.Length; i++)
            {
                arr_mark[i].gameObject.SetActive(false);
            }

            efx_end.Play();
            GameManager.Instance.soundMgr.PlaySfx(handleMark.transform.position,
                Constants.Sound.SFX_INTERACTION_HAND_MARKER);
        }



        /// <summary>
        /// 7/22/2024-LYI
        /// 손잡이 놓을 때 호출
        /// 파티클, 사운드 재생
        /// </summary>
        public void EndTutorial()
        {
            if (!isTutorial)
            {
                return;
            }
            isTutorial = false;

            GameManager.Instance.soundMgr.PlaySfx(handleMark.transform.position,
            Constants.Sound.SFX_STAGE_CLEAR);
            efx_end.Play();
            handleMark.gameObject.SetActive(false);

            if (onHandleEnd != null)
            {
                onHandleEnd.Invoke();
            }
            StartCoroutine(GameManager.Instance.LateFunc(()=>this.gameObject.SetActive(false),2f));
        }


    }
}