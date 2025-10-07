using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.Feedbacks;

namespace VRTokTok.Interaction
{

    [System.Serializable]
    /// <summary>
    /// 10/10/2023-LYI
    /// 스위치 등에 대한 반응으로 이동하는 기둥
    /// 주로 지형에 박혀있다
    /// </summary>
    public class Tok_Pillar :Tok_Interact
    {
        [Header("Pillar Property")]
        public GameObject moveBody;
        [SerializeField]
        Collider bodyColl;

        public Transform tr_out; //활성화 시 위치
        public Transform tr_in; //비활성화 시 위치

        public MMF_Player mmf_out; //나오는 효과
        public MMF_Player mmf_in; //들어가는 효과

        public TokGround tokGround; //터치 활성화

        [Header("Direction")]
        private bool isOut = false;
        public bool isOutOnStart = false;

        private bool isFirst = true;

        [Header("Scale")]
        /// <summary>
        /// 11/1/2023-LYI
        /// Scale 기반 기둥 길이 조절 기능 추가
        /// 이동 방식의 아랫쪽이 보이는 문제로 제작
        /// </summary>
        public bool isScaleMode = false; //스케일을 이용하는 기둥 체크
        public GameObject scaleBody;
        public float scaleSpeed = 2f;

        [Header("Tile Options")]
        public float tileSize = 0.05f;

        public bool isFixedTile = false;
        public int moveTileCount = 0;



        public override void InteractInit()
        {
            base.InteractInit();
            if (isFirst)
            {
                GameManager.Instance.playMgr.tokMgr.GroundInit(tokGround);
                isFirst = false;
            }


            //고정 타일 방식인 경우 out 포지션 변경
            if (isFixedTile && moveTileCount > 0)
            {
                //방향에 값을? 값이 0이 아닌걸 1로 바꾼 뒤 변경?
                Vector3 tilePos = Vector3.zero;
                tilePos.x = Mathf.Abs(tr_out.localPosition.x) > float.Epsilon ? tileSize * moveTileCount  : 0;
                tilePos.y = Mathf.Abs(tr_out.localPosition.y) > float.Epsilon ? tileSize * moveTileCount : 0;
                tilePos.z = Mathf.Abs(tr_out.localPosition.z) > float.Epsilon ? tileSize * moveTileCount : 0;
                tr_out.localPosition = tilePos; 
            }


            if (isScaleMode)
            {
                if (isOutOnStart)
                {
                    // 스케일을 적용합니다.
                    scaleBody.transform.localScale = SetPillerScale(isOutOnStart);
                    isOut = true;
                    bodyColl.enabled = true;
                }
                else
                {
                    scaleBody.transform.localScale = SetPillerScale(isOutOnStart);
                    isOut = false;
                    bodyColl.enabled = false;
                }
            }
            else
            {
                if (isOutOnStart)
                {
                    moveBody.transform.position = tr_out.position;
                    isOut = true;
                    bodyColl.enabled = true;
                }
                else
                {
                    moveBody.transform.position = tr_in.position;
                    isOut = false;
                    bodyColl.enabled = false;
                }
            }
     

            tokGround.gameObject.SetActive(isOut);
        }


        /// <summary>
        /// 3/28/2024-LYI
        /// 스케일 계산 식
        /// </summary>
        /// <param name="isOut">true: Out, false: In</param>
        /// <returns></returns>
        Vector3 SetPillerScale(bool isOut)
        {
            Transform targetTransform = isOut ? tr_out.transform : tr_in.transform;
            Vector3 scale = Vector3.one;

            scale.x = Mathf.Abs(targetTransform.localPosition.x) > float.Epsilon ? targetTransform.localPosition.x / tileSize : 1f;
            scale.y = Mathf.Abs(targetTransform.localPosition.y) > float.Epsilon ? targetTransform.localPosition.y / tileSize : 1f;
            scale.z = Mathf.Abs(targetTransform.localPosition.z) > float.Epsilon ? targetTransform.localPosition.z / tileSize : 1f;

            return scale;
        }

        public void PillarOut()
        {
            isOut = true;
            bodyColl.enabled = true;

            tokGround.gameObject.SetActive(true);

            if (isScaleMode)
            {
                StartCoroutine(ScaleOutCoroutine());
            }
            else
            {
                mmf_out.PlayFeedbacks();
            }
        }
        public void PillarIn()
        {
            isOut = false;
            bodyColl.enabled = false;

            tokGround.gameObject.SetActive(false);

            if (isScaleMode)
            {
                StartCoroutine(ScaleInCoroutine());
            }
            else
            {
                mmf_in.PlayFeedbacks();
            }    
        }

        public void PillerActive()
        {
            if (isOut)
            {
                PillarIn();
            }
            else
            {
                PillarOut();
            }
        }

        /// <summary>
        /// 11/1/2023-LYI
        /// 튀어나오는 동작
        /// </summary>
        /// <returns></returns>
        IEnumerator ScaleOutCoroutine()
        {
            Vector3 startScale = scaleBody.transform.localScale;
            Vector3 targetScale = SetPillerScale(true);
            float t = 0;

            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (t < 1f)
            {
                t += 0.01f * scaleSpeed;
                scaleBody.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return wait;
            }

            scaleBody.transform.localScale = targetScale;
        }


        /// <summary>
        /// 11/1/2023-LYI
        /// 들어가는 동작
        /// </summary>
        /// <returns></returns>
        IEnumerator ScaleInCoroutine()
        {
            Vector3 startScale = scaleBody.transform.localScale;
            Vector3 targetScale = SetPillerScale(false);

            float t = 0;

            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (t < 1f)
            {
                t += 0.01f * scaleSpeed;
                scaleBody.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return wait;
            }

            scaleBody.transform.localScale = targetScale;
        }

        /// <summary>
        /// 10/10/2023-LYI
        /// 기둥 튀어나옴
        /// </summary>
        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            PillerActive();
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 기둥 들어감
        /// </summary>
        public override void DisableInteraction()
        {
            base.DisableInteraction();
            PillarIn();
        }

    }
}