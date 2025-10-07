using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;

namespace VRTokTok.Interaction.Crossing
{

    /// <summary>
    /// 8/4/2023-LYI
    /// 길건너기에서 각 라인 관리 클래스 
    /// 각 라인에서 소환할 오브젝트
    /// 소환 타이밍, 오브젝트 속도 관리
    /// </summary>
    public class Crossing_Lane : MonoBehaviour
    {
        GameManager gameMgr;
        public Tok_Crossing crossing;

        [Header("Object Pooling")]
        public List<GameObject> list_active = new List<GameObject>();
        public List<GameObject> list_disable = new List<GameObject>();

        [SerializeField]
        Transform tr_disable;

        [Header("Crossing")]
        [SerializeField]
        Transform tr_spawn;


        public GameObject origin;

        public int laneNum = 1; //라인을 유지할 오브젝트 수
        public float laneSpace = 1f; //라인의 오브젝트 사이 거리
        public float laneSpeed = 1f; //떠다니는 오브젝트의 속도
        public float laneMaxPos = 0f;
        public bool isLeft = false; //떠다닐 방향

        bool isSpawning = false;
        Coroutine currentCoroutine;

        private void Awake()
        {
            gameMgr = GameManager.Instance;
        }


        public void LaneInit()
        {
            StopAllCoroutines();
            for (int i = 0; i < list_active.Count; i++)
            {
                gameMgr.objPoolingMgr.ObjectInit(list_disable, list_active[i], tr_disable);
            }
            for (int i = 0; i < list_active.Count; i++)
            {
                list_active.RemoveAt(0);
            }
            isSpawning = false;
        }


        /// <summary>
        /// 8/7/2023-LYI
        /// 각 레인 생성 시작
        /// </summary>
        public void StartSpawn()
        {
            if (!this.gameObject.activeSelf)
            {
                return;
            }

            if (isSpawning)
            {
                return;
            }

            isSpawning = true;

            //최대 이동 거리 할당
            laneMaxPos = Mathf.Abs(tr_spawn.localPosition.z);

            //먼저 생성되야 할 경우
            if (crossing.isPrewarn)
            {

            }


            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            currentCoroutine = StartCoroutine(SpawnLoop());
        }

        IEnumerator SpawnLoop()
        {
            WaitForSeconds wait = new WaitForSeconds(laneSpace);
            while (gameMgr.statGame == GameStatus.GAME)
            {
                SpawnObject();
                yield return wait;
            }
        }


        public void SpawnObject()
        {
            int dir = isLeft ? 1 : -1;
            Vector3 spawnPos = new Vector3(0, 0, dir * laneMaxPos);

            GameObject go = gameMgr.objPoolingMgr.CreateObject(list_disable, origin, spawnPos, this.transform);
            list_active.Add(go);


            go.transform.localPosition = spawnPos;
            go.GetComponent<Crossing_Platform>().lane = this;
            go.GetComponent<Crossing_Platform>().MovePlatform(laneSpeed, isLeft ? -1 : 1, laneMaxPos);
            GameManager.Instance.playMgr.tokMgr.GroundInit(go.GetComponent<Crossing_Platform>().ground);

            if (go.GetComponentInChildren<Tok_Interact>() != null)
            {
                Tok_Interact[] arr_interact = go.GetComponentsInChildren<Tok_Interact>();

                for (int i = 0; i < arr_interact.Length; i++)
                {
                    arr_interact[i].InteractInit();
                }

            }

        }

        public void DisableObject(GameObject go)
        {
            gameMgr.objPoolingMgr.ObjectInit(list_disable, go, tr_disable);
            list_active.Remove(go);


        }


        /// <summary>
        /// 8/4/2023-LYI
        /// 각 라인은 해당 라인의 오브젝트들을 일정 속도로 이동시킨다
        /// 라인마다 독립적인 속력을 가진다
        /// </summary>
        /// <returns></returns>

        //private void Update()
        //{
        //    //플레이중에 계속 움직인다
        //    if (gameMgr.currentStage.stageStat == GameStatus.PLAY)
        //    {
        //        int dir = isLeft ? -1 : 1;
        //        float move = dir * laneSpeed * Time.deltaTime;
        //        transform.Translate(0, 0, move);
        //    }
        //}


    }
}