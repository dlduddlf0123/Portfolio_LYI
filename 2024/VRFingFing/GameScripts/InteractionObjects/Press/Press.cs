using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRTokTok.Interaction.Press
{
    /// <summary>
    /// 9/26/2023-LYI
    /// 일정 시간마다 바닥을 쿵쿵 찍는다
    /// </summary>
    public class Press : Tok_Interact
    {
        Rigidbody m_rigidbody;

        [SerializeField]
        Collider m_bodyColl;

        [SerializeField]
        GameObject deathZone;

        [SerializeField]
        ParticleSystem efx_press;

        [Header("Press")]
        public float startDelay = 0f;
        public float pressBeforeTime = 2f;
        public float pressAfterTime = 1f;

        public float pressSpeed = 1f;

        public bool isGround = false;
        bool isPress = false;

        [Header("Move")]
        public bool isMove = false;
        int moveDirection = 1;
        [SerializeField]
        Transform moveL, moveR; //이동시 범위

        Vector3 startPos = Vector3.zero;

        bool isOnce = false;

        public bool isDebug = false;

        public override void InteractInit()
        {
            base.InteractInit();

            if (!isInit)
            {
                m_rigidbody = GetComponent<Rigidbody>();
                startPos = transform.localPosition;

                isInit = true;
            }


            StopAllCoroutines();
            isOnce = false;
            isPress = false;
            transform.localPosition = startPos;

            StartPress();
        }


        //void Update()
        //{
        //    RayCheckGround();
        //}

        void StartPress()
        {
            if (isPress)
            {
                return;
            }
            StartCoroutine(PressCoroutine());
        }


        /// <summary>
        /// 10/6/2023-LYI
        /// isMove가 켜졌을 때 좌우 움직임 진행
        /// </summary>
        /// <returns></returns>
        IEnumerator MoveHorizontalCoroutine()
        {

            WaitForSeconds wait = new WaitForSeconds(0.01f);
            float t = 0;
            while (t < pressBeforeTime)
            {
                t += 0.01f;
                //올라오는 속도보다 조금 빠르게 좌우 이동
               transform.Translate(Vector3.right * moveDirection * pressSpeed * 0.2f * 0.01f);


                if (transform.position.x < moveL.position.x)
                {
                    moveDirection = 1;
                }

                if (transform.position.x > moveR.position.x)
                {
                    moveDirection = -1;
                }
                

                yield return wait;
            }

            //떨어지기 전에 잠시 멈춤, 흔들림?
            yield return new WaitForSeconds(0.2f);
        }


        /// <summary>
        /// 5/9/2024-LYI
        /// 누르는 동작
        /// 동작 시작 시 레이캐스트로 지면까지 거리 측정
        /// 레이가 닿는곳 까지 0.05f 크기 타일로 몇 타일인지 계산
        /// 도착 타일까지 이동, 거리측정
        /// </summary>
        /// <returns></returns>
        IEnumerator PressCoroutine()
        {
            if (isDebug)
            {
                Debug.Log(gameObject.name + ": Press");
            }

            isPress = true;

            if (isMove)
            {
                yield return StartCoroutine(MoveHorizontalCoroutine());
            }
            else
            {
                yield return new WaitForSeconds(pressBeforeTime);
                if (!isOnce)
                {
                    yield return new WaitForSeconds(startDelay);
                    isOnce = true;
                }

            }

            deathZone.SetActive(true);


            Vector3 desirePoint = RayCheckGround();

            float dist = Vector3.Distance(transform.position, desirePoint);
            //  m_rigidbody.isKinematic = false;
            while (transform.position != desirePoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, desirePoint, pressSpeed * Time.deltaTime);
                //m_rigidbody.velocity = Physics.gravity;
                //dist = Vector3.Distance(transform.position, desirePoint);
                // transform.Translate(Vector3.down* pressSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = desirePoint;

            //이펙트, 효과음
            efx_press.transform.position = desirePoint - Vector3.up * 0.025f;
            efx_press.Play();

            GameManager.Instance.soundMgr.PlaySfxRandomPitch(desirePoint, Constants.Sound.SFX_TRAP_PRESS, 0.3f);

            StartCoroutine(MoveUpCoroutine());
        }

        IEnumerator MoveUpCoroutine()
        {
            if (isDebug)
            {
                Debug.Log(gameObject.name + ": Up");
            }
            deathZone.SetActive(false);
            yield return new WaitForSeconds(pressAfterTime);

            m_rigidbody.velocity = Vector3.zero;
        //    m_rigidbody.isKinematic = true;

            while (transform.localPosition.y < startPos.y)
            {
                transform.Translate(Vector3.up * pressSpeed * 0.1f * Time.deltaTime);
                yield return null;
            }
            transform.localPosition = startPos;

            isPress = false;
            isGround = false;
            StartCoroutine(PressCoroutine());
        }


        float groundRayLength = Mathf.Infinity;
        private Vector3 RayCheckGround()
        {
            Vector3 start = transform.position - Vector3.up * m_bodyColl.bounds.size.y * 0.25f;

            int frontRayMask = (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_HAND)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_CHARACTER)) |
               (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_IGNORE)) |
                   (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TRAP)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_TOKMARKER)) |
                    (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE_ONLY)) |
                     (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE)) |
                       (1 << LayerMask.NameToLayer(Constants.Layer.LAYERMASK_INTERACT));

            //플랫폼 레이 체크
            RaycastHit hit;

            Color rayColor = Color.blue;

            if (Physics.Raycast(start, Vector3.down, out hit, groundRayLength, ~frontRayMask))
            {
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    //isGround = true;
                    Debug.DrawRay(start, Vector3.down * groundRayLength, rayColor, 0.1f);

                    rayColor = Color.green;

                    // y축만 타일 크기에 맞게 조정
                    Vector3 snappedPoint = new Vector3(hit.point.x,hit.point.y +0.025f * transform.lossyScale.x, hit.point.z);


                    return snappedPoint;
                }
            }
            else
            {
                Debug.DrawRay(start, Vector3.down * groundRayLength, rayColor, 0.1f);
               // isGround = false;
                return Vector3.zero;
            }

            return Vector3.zero;

        }


        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
            StartPress();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}