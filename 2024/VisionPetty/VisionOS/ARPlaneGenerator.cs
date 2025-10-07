using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;

namespace AroundEffect
{

    public enum ARPlaneMode
    {
        NONE = 0,
        MOVE,
        PLACED
    }

    public class ARPlaneGenerator : MonoBehaviour
    {
        GameManager gameMgr;


        //AR Manager Objects
        ARPlaneManager m_ARPlaneManager;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject dummyPrefab;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return dummyPrefab; }
            set { dummyPrefab = value; }
        }


        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        public Vector3 placedPos;
        public Quaternion placedRot { get; set; }

        Vector3 spawnedScale = Vector3.zero;

        public ARPlaneMode statPlane = ARPlaneMode.NONE;
        public bool isPlaced = false;


        private void Awake()
        {
            gameMgr = GameManager.Instance;

            m_ARPlaneManager = GetComponent<ARPlaneManager>();
            PlaceTransformInit();
        }
        

        public void PlaceTransformInit()
        {
            placedPos = Vector3.zero;
            placedRot = Quaternion.identity;

            statPlane = ARPlaneMode.NONE;
            SetARPlaneActive(false);
        }

        public void StartPlaceMode()
        {
            Debug.Log("ARPlaneGenerator.StartPlaceMode()");
            statPlane = ARPlaneMode.MOVE;
            SetARPlaneActive(true);
        }
        public void EndPlaceMode()
        {
            Debug.Log("ARPlaneGenerator.EndPlaceMode()");
            statPlane = ARPlaneMode.PLACED;
            SetARPlaneActive(false);

        }

        /// <summary>
        /// 9/12/2024-LYI
        /// 지형 배치 활성화 관리
        /// </summary>
        /// <param name="isActive"></param>
        public void SetARPlaneActive(bool isActive)
        {
            isPlaced = isActive;

            m_ARPlaneManager.enabled = isActive;

            SetAllPlanesActive(isActive);
        }

        /// <summary>
        /// Iterates over all the existing planes and activates
        /// or deactivates their <c>GameObject</c>s'.
        /// </summary>
        /// <param name="isActive">Each planes' GameObject is SetActive with this value.</param>
        public void SetAllPlanesActive(bool isActive)
        {

            if (spawnedObject != null)
            {
                spawnedObject.SetActive(isActive);
            }
            foreach (var plane in m_ARPlaneManager.trackables)
                plane.gameObject.SetActive(isActive);

        }



        public void CreateDummy()
        {
            if (spawnedObject != null)
            {
                spawnedObject.SetActive(true);
                return;
            }
            spawnedObject = Instantiate(dummyPrefab);
        }



        public void MoveDummy(Vector3 targetPos)
        {
            if (statPlane == ARPlaneMode.MOVE)
            {
                if (spawnedObject != null)
                {
                    spawnedObject.transform.position = targetPos;
                    placedPos = targetPos;

                }
            }
        }


    }
}