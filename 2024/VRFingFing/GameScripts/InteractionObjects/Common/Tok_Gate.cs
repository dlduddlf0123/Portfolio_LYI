using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Character;

namespace VRTokTok.Interaction
{

    /// <summary>
    /// 11/25/2023-LYI
    /// 게이트 타입에 따라 등장, 나가기, 텔레포트 형태 구분
    /// </summary>
    public enum GateType
    {
        START = 0,
        EXIT,
        TELEPORT,
    }

    /// <summary>
    /// 11/25/2023-LYI
    /// 게이트 역할 오브젝트
    /// 텔레포트와 출구 역할
    /// 출구인 경우 모든 헤더스가 들어오면 클리어
    /// </summary>
    public class Tok_Gate : Tok_Interact
    {
        GameManager gameMgr;

        [Header("Tok Gate")]
        public GateType typeGate;

        
        public ParticleSystem efx_portal;

        [Header("Exit properties")]
        [SerializeField]
        Collider col_exit;
        public Transform tr_center;
        public Transform tr_jump;

        public int maxHeaderCount;
        public int currentHeaderCount;


        [Header("Teleport properties")]
        public Tok_Gate tel_destination;
        public Collider col_teleport;
        public bool isReady = true;


        [Header("Portal properties")]
        public bool isStartPortal = false;
        public bool isPortalActive = false;


        private void Awake()
        {
            //fade = GetComponent<OVRScreenFade>();
        }

        public override void InteractInit()
        {
            base.InteractInit();

            gameMgr = GameManager.Instance;

            col_teleport.gameObject.SetActive(false);
            col_exit.gameObject.SetActive(false);
            switch (typeGate)
            {
                case GateType.START:
                    break;
                case GateType.EXIT:
                    SetPortal(isStartPortal);

                    col_exit.gameObject.SetActive(true);

                    if (maxHeaderCount <= 1)
                    {
                        maxHeaderCount = gameMgr.playMgr.currentStage.characterNum;
                    }
                    currentHeaderCount = 0;
                    break;
                case GateType.TELEPORT:
                    SetTeleport(isStartPortal);
                    break;
                default:
                    break;
            }

        }


        public void SetPortal(bool isActive)
        {
            if (isActive)
            {
                efx_portal.Play();
                isPortalActive = true;
            }
            else
            {
                efx_portal.Stop();
                isPortalActive = false;
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (!isPortalActive)
            {
                return;
            }

            if (coll.gameObject.CompareTag("Header"))
            {
                switch (typeGate)
                {
                    case GateType.START:
                        break;
                    case GateType.EXIT:
                        currentHeaderCount++;
                        CheckClear();
                        break;
                    case GateType.TELEPORT:
                        Teleport(coll.gameObject);
                        break;
                    default:
                        break;
                }

            }
        }
        private void OnTriggerExit(Collider coll)
        {
            if (coll.gameObject.CompareTag("Header"))
            {
                switch (typeGate)
                {
                    case GateType.START:
                        break;
                    case GateType.EXIT:
                        currentHeaderCount--;
                        break;
                    case GateType.TELEPORT:
                        isReady = true;
                        SetPortal(true);
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// 11/26/2023-LYI
        /// 버튼을 통한 텔레포트 기능 설정
        /// </summary>
        /// <param name="isActive"></param>
        public void SetTeleport(bool isActive)
        {
            if (tel_destination == null)
            {
                Debug.Log(gameObject.name + ": Destination is empty!!");
                return;
            }

            SetPortal(isActive);
            col_teleport.gameObject.SetActive(isActive);

            tel_destination.SetPortal(isActive);
            tel_destination.col_teleport.gameObject.SetActive(isActive);

        }


        public void Teleport(GameObject coll)
        {
            if (!isReady || tel_destination == null)
            {
                return;
            }
            isReady = false;
            tel_destination.isReady = false;

            SetPortal(false);
            tel_destination.SetPortal(false);
            Debug.Log("Teleport!");

            //mmf_in.PlayFeedbacks();
            //tel_destination.mmf_in.PlayFeedbacks();

            Vector3 v1 = coll.transform.position;
            Vector3 v2 = tel_destination.transform.position;
            coll.transform.position = new Vector3(v2.x, v1.y, v2.z);
            coll.gameObject.GetComponent<Character.Tok_Movement>().Stop();
        }

        public void CheckClear()
        {
            if (currentHeaderCount >= maxHeaderCount)
            {
                GameManager.Instance.playMgr.currentStage.ClearStage();
            }
        }

        public override void ActiveInteraction()
        {
            base.ActiveInteraction();


            switch (typeGate)
            {
                case GateType.START:
                    break;
                case GateType.EXIT:
                    if (!isPortalActive)
                    {
                        SetPortal(true);
                    }
                    else
                    {
                        GameManager.Instance.playMgr.currentStage.ClearStage();
                    }
                    break;
                case GateType.TELEPORT:
                    //버튼으로 텔레포트 작동
                    SetTeleport(true);
                    break;
                default:
                    break;
            }
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}