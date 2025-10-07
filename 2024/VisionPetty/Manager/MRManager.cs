using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.XR.CoreUtils;
using Unity.PolySpatial;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;


namespace AroundEffect
{

    /// <summary>
    /// 9/13/2024-LYI
    /// Management MR scripts
    /// MR 관련 기능 관리. GameManger에서 기능 이관
    /// 
    /// 지형 관련 정보 관리?
    /// </summary>
    public class MRManager : MonoBehaviour
    {
        [Header("MR")]
        /// <summary>
        /// XR origin contains mainCamera
        /// Players head position
        /// </summary>
        public XROrigin XR_Origin;
        /// <summary>
        /// AR Session for ar plane stuff
        /// </summary>
        public ARSession AR_Session;
        /// <summary>
        /// Include AR Plane manager and generated planes
        /// </summary>
        public ARPlaneGenerator AR_PlaneGenerator;


        [Header("Vision Pro")]
        /// <summary>
        /// VolumeCamara affects all stuff position in MR
        /// </summary>
        public VolumeCamera VolumeCamera;

        /// <summary>
        /// PolySpatial input manage input from vision pro
        /// </summary>
        public VisionPolySpatialInput polySpatialInput;

        public Button btn_place;
        public Button btn_confirm;
        public Button btn_minigame;


        public Transform tr_MRAnchor;

        private void Awake()
        {
            MRInit();
        }

        public void MRInit()
        {
            btn_place.onClick.AddListener(OnPlaneLocateModeStart);
            btn_confirm.onClick.AddListener(OnPlaneLocateModeEnd);
            btn_minigame.onClick.AddListener(MiniGameButton);

            SetMRButtonActive(false);
        }


        public void OnPlaneLocateModeStart()
        {
            Debug.Log("btn_place.Active()");
            SetMRButtonActive(true);
            AR_PlaneGenerator.StartPlaceMode();

        }
        public void OnPlaneLocateModeEnd()
        {
            Debug.Log("btn_confirm.Active()");
            SetMRButtonActive(false);

            AR_PlaneGenerator.EndPlaceMode();

        }

        public void MiniGameButton()
        {
            GameManager.Instance.lifeMgr.StartMiniGame();
        }

        void SetMRButtonActive(bool isMoving)
        {
            btn_place.gameObject.SetActive(!isMoving);
            btn_confirm.gameObject.SetActive(isMoving);
        }



    }
}