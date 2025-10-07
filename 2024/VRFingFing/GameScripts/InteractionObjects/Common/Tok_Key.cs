using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using MoreMountains.Feedbacks;
using VRTokTok.Character;

namespace VRTokTok.Interaction
{

    /// <summary>
    /// 9/26/2023-LYI
    /// 열쇠
    /// 캐릭터로 획득 가능하며 획득 시 UI에 표시
    /// 문을 열 때 소모된다
    /// </summary>
    public class Tok_Key : Tok_Interact
    {
        StageManager stageMgr;
    
        [Header("Tok_Key")]
        Vector3 startPos;
        public Transform tr_follow;

        [Header("Materials")]
        public Material[] arr_matInteract;

        [Header("Particles")]
        public ParticleSystem efx_sparkle;
        public ParticleSystem efx_get;

        [Header("MMF Player")]
        public MMF_Player mmf_appear;
        public MMF_Player mmf_disappear;

        [Header("Properties")]
        public bool isHeader = false;
        public int index = 0;
        public float distanceThreshold = 0.1f;
        public float moveSpeed = 3f;

        private void Awake()
        {
            startPos = transform.localPosition;
        }

        public override void InteractInit()
        {
            base.InteractInit();
            
            stageMgr = GameManager.Instance.playMgr.currentStage;
            transform.localPosition = startPos;
            transform.localRotation = Quaternion.identity;
            this.gameObject.SetActive(true);

            isInteractable = true;
            isHeader = false;

            if (m_renderer != null)
            {
                SetColor(interactColor);
            }

            mmf_appear.PlayFeedbacks();
            efx_sparkle.Play();
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                if (isHeader)
                {
                    return;
                }
                isHeader = true;
                SetFollow(coll.gameObject.transform);

                GetKey();
            }
        }


        private void Update()
        {
            if (isHeader)
            {
                float dist = Vector3.Distance(transform.position, tr_follow.position);
                if (dist < 0.03f)
                {
                    return;
                }
                Vector3 targetPos = Vector3.Lerp(transform.position, tr_follow.position, 0.9f);// + Vector3.up * 0.035f;
                if (index == 0)
                {
                    //위치 캐릭터 머리 위로 변경
                    targetPos = tr_follow.position + Vector3.up * 0.08f;
                }
                this.transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
            }
        }

        public void SetFollow(Transform follow)
        {
            tr_follow = follow;
        }


        public void GetKey()
        {
            stageMgr.keyCount++;
            stageMgr.list_key.Add(this);

            if (stageMgr.list_key.Count > 1)
            {
                index = stageMgr.list_key.IndexOf(this);
                SetFollow(stageMgr.list_key[index - 1].transform);
            }
            else
            {
                index = 0;
            }

            isInteractable = false;

            efx_get.Play();
            GameManager.Instance.soundMgr.PlaySfx(transform.position, Constants.Sound.SFX_INTERACTION_KEY_GET);

            Debug.Log(gameObject.name + "GetKey()");
            Debug.Log("KeyCount: " + stageMgr.keyCount);
        }


        /// <summary>
        /// 7/3/2024-LYI
        /// 열쇠 사용될 때 처리 추가
        /// 열쇠 작아지는 효과, 파티클?
        /// 다음 열쇠 있으면 위치 변경
        /// </summary>
        public void UseKey()
        {
            mmf_disappear.PlayFeedbacks();
            efx_sparkle.Stop();
            efx_get.Play();

            //키 카운트 땡기기
            stageMgr.keyCount--;
            stageMgr.list_key.Remove(this);

            if (stageMgr.keyCount > 0)
            {
                for (int i = 0; i < stageMgr.list_key.Count; i++)
                {
                    stageMgr.list_key[i].index = i;
                }

                //다른 키 있으면 인덱스 변경
                //이 키가 0번이였을 경우
                if (index == 0)
                {
                    //현재 타겟(캐릭터) 넘겨주기
                    stageMgr.list_key[0].SetFollow(tr_follow);
                }
                else
                {
                    //0번이 아닌경우, 1번부터 재정렬
                    for (int i = 1; i < stageMgr.list_key.Count; i++)
                    {
                        stageMgr.list_key[i].SetFollow(stageMgr.list_key[index - 1].transform);
                    }
                }
            }
     

            Debug.Log(gameObject.name + "UseKey()");
            Debug.Log("KeyCount: " + stageMgr.keyCount);
        }


        /// <summary>
        /// 6/10/2024-LYI
        /// 범용 색 지정 추가
        /// </summary>
        /// <param name="color"></param>
        public override void SetColor(InteractColor color)
        {
            if (arr_matInteract == null)
            {
                base.SetColor(color);
                return;
            }

            if (m_renderer == null ||
                color == InteractColor.WHITE)
            {
                return;
            }

            Color newColor = arr_matInteract[0].color;

            switch (color)
            {
                case InteractColor.BLUE:
                    m_renderer.material = arr_matInteract[0]; 
                    newColor = arr_matInteract[0].color;
                    break;
                case InteractColor.GREEN:
                    m_renderer.material = arr_matInteract[1];
                    newColor = arr_matInteract[1].color;
                    break;
                case InteractColor.YELLOW:
                    m_renderer.material = arr_matInteract[2];
                    newColor = arr_matInteract[2].color;
                    break;
                case InteractColor.RED:
                    m_renderer.material = arr_matInteract[3];
                    newColor = arr_matInteract[3].color;
                    break;
                default:
                    newColor = arr_matInteract[0].color;
                    // m_renderer.material = arr_matInteract[0];
                    break;
            }

            ChangeParticlesColor(newColor);
        }


        /// <summary>
        /// 7/3/2024-LYI
        /// 파티클들의 색상 열쇠 색상에 따라 변경
        /// </summary>
        /// <param name="newColor"></param>
        public void ChangeParticlesColor(Color newColor)
        {
            //변경할 요소 긁어오기 set이 없어 간접 접근
            var sparkle1 = efx_sparkle.main;
            var sparkle2 = efx_sparkle.subEmitters.GetSubEmitterSystem(0).main;
            var get = efx_get.colorOverLifetime;
            var glow = efx_get.transform.GetChild(0).GetComponent<ParticleSystem>().main;

            // sparkle 효과 색상 변경
            sparkle1.startColor = new ParticleSystem.MinMaxGradient(newColor);
            sparkle2.startColor = new ParticleSystem.MinMaxGradient(newColor);

            //획득 효과 색상 변경
            Gradient gradient = get.color.gradient;
            //그라데이션 변경 시 이미 있는 키를 바꾸려니 바뀌지 않아 새로 생성해서 할당
            GradientColorKey whitekey = new GradientColorKey(Color.white, 0);
            GradientColorKey colorkey = new GradientColorKey(newColor, 1);
            gradient.colorKeys = new GradientColorKey[2] { whitekey, colorkey };

            get.color = new ParticleSystem.MinMaxGradient(gradient);
            glow.startColor = new ParticleSystem.MinMaxGradient(newColor);
        }

        public override void ActiveInteraction()
        {
            base.ActiveInteraction();
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }
    }
}