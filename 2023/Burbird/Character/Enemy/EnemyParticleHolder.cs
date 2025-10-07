using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Burbird
{

    /// <summary>
    /// 적캐릭터 관련된 파티클 생성, 풀링 클래스
    /// </summary>
    public class EnemyParticleHolder : MonoBehaviour
    {
        StageManager stageMgr;

        Transform tr_active;
        Transform tr_disable;

        //폭파 이펙트
        [Header("Boom")]
        public GameObject[] arr_boom;
        List<GameObject>[] arr_list_boom = new List<GameObject>[4]; //4가지 폭파 이펙트 저장

        //소환 효과
        [Header("Spawn")]
        public GameObject particle_spawn;
        List<GameObject> list_spawn = new List<GameObject>();


        private void Awake()
        {
            stageMgr = StageManager.Instance;

            tr_active = transform.GetChild(0);
            tr_disable = transform.GetChild(1);


            //폭탄 배열 초기화
            for (int i = 0; i < arr_list_boom.Length; i++)
            {
                arr_list_boom[i] = new List<GameObject>();
            }
        }

        #region GameObject Management
        //오브젝트 생성
        GameObject CreateObject(List<GameObject> list, GameObject originGo, Vector3 pos)
        {
            GameObject go;

            if (list.Count == 0)
            {
                go = Instantiate(originGo);
            }
            else
            {
                go = list[0];
                list.RemoveAt(0);
            }

            go.transform.SetParent(tr_active);
            go.transform.position = pos;
            go.gameObject.SetActive(true);

            return go;
        }

        /// <summary>
        /// 해당 리스트에 그 오브젝트를 다시 배치, 비활성화
        /// </summary>
        /// <param name="list"></param>
        /// <param name="go"></param>
        public void ObjectInit(List<GameObject> list, GameObject go)
        {
            go.transform.localScale = Vector3.one;
            go.transform.SetParent(tr_disable);
            list.Add(go);
            go.SetActive(false);
        }

        /// <summary>
        /// 잠시 후에 오브젝트 초기화 진행
        /// </summary>
        /// <param name="list"></param>
        /// <param name="go"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public IEnumerator LateInit(List<GameObject> list, GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            ObjectInit(list, go);
        }
        #endregion

        #region General Type
        public void PlayParticle(List<GameObject> list, GameObject origin, Vector3 pos, UnityAction action = null)
        {
            GameObject go = CreateObject(list, origin, pos);

            ParticleSystem[] p = go.GetComponentsInChildren<ParticleSystem>();
            if (!p[0].main.playOnAwake)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].Play();
                }
            }

            if (action != null)
            {
                StartCoroutine(ParticleAction(go.GetComponent<ParticleSystem>(), action));
            }

            StartCoroutine(LateInit(list, go, 2f));
        }
        public void PlayParticle(List<GameObject> list, GameObject origin, Vector3 pos, float size, UnityAction action = null)
        {
            GameObject go = CreateObject(list, origin, pos);

            go.transform.localScale *= size;
            ParticleSystem[] p = go.GetComponentsInChildren<ParticleSystem>();
            if (!p[0].main.playOnAwake)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].Play();
                }
            }

            if (action != null)
            {
                StartCoroutine(ParticleAction(go.GetComponent<ParticleSystem>(), action));
            }

            StartCoroutine(LateInit(list, go, 2f));
        }
        public GameObject PlayParticle(List<GameObject> list, GameObject origin, Vector3 pos)
        {
            GameObject go = CreateObject(list, origin, pos);

            ParticleSystem[] p = go.GetComponentsInChildren<ParticleSystem>();
            if (!p[0].main.playOnAwake)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].Play();
                }
            }

            StartCoroutine(LateInit(list, go, 2f));

            return go;
        }

        IEnumerator ParticleAction(ParticleSystem particle, UnityAction action)
        {
            yield return new WaitForSeconds(particle.main.duration);

            action.Invoke();

        }

        #endregion

        #region Boom
        public void PlayParticle_BoomPlayer(BoomType type, int dmg, Vector3 pos)
        {
            int num = (int)type;

            GameObject go = PlayParticle(arr_list_boom[num], arr_boom[num], pos);

            //GameObject go = CreateObject(arr_list_boom[num], arr_boom[num], pos);
            go.GetComponent<Explosion>().damage = dmg;
            go.GetComponent<Explosion>().explosionType = type;
            go.GetComponent<Explosion>().isEnemy = false;
        }
        public GameObject PlayParticle_BoomEnemy(BoomType type, int dmg, Vector3 pos)
        {
            int num = (int)type;

            GameObject go = PlayParticle(arr_list_boom[num], arr_boom[num], pos);

            //GameObject go = CreateObject(arr_list_boom[num], arr_boom[num], pos);
            go.GetComponent<Explosion>().damage = dmg;
            go.GetComponent<Explosion>().explosionType = type;
            go.GetComponent<Explosion>().isEnemy = true;
            return go;
        }

        #endregion

        #region Spawn

        public void PlayParticle_Spawn(Vector3 pos, UnityAction action = null)
        {
            PlayParticle(list_spawn, particle_spawn, pos, action);
        }
        public void PlayParticle_Spawn(Vector3 pos, UnityAction action, EnemySize size)
        {
            switch (size)
            {
                case EnemySize.NORMAL:
                    PlayParticle(list_spawn, particle_spawn, pos, action);
                    break;
                case EnemySize.SMALL:
                    PlayParticle(list_spawn, particle_spawn, pos, 0.5f, action);
                    break;
                case EnemySize.BIG:
                    PlayParticle(list_spawn, particle_spawn, pos, 2f, action);
                    break;
            }
        }

        #endregion


    }
}