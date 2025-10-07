using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{


    /// <summary>
    /// 투사체 타입 설정
    /// </summary>
    public enum ProjectileType
    {
        NORMAL,       //일반
        PIERCE,         //관통
        BOMB,           //폭발
        SUMMON,      //소환
    }


    /// <summary>
    /// 4/13/2023-LYI
    /// 적 캐릭터들의 투사체를 관리하는 클래스
    /// EnemySpawner에서 참조, 일반 몬스터들의 투사체를 생성, 제거 한다
    /// </summary>
    public class EnemyProjectileManager : MonoBehaviour
    {
        StageManager stageMgr;

        [Header("Object Pooling")]
        public GameObject origin_missile;
        public Transform missilePool;
        public Transform tr_active;
        public Transform tr_disable;
        public List<GameObject> list_missile = new ();
        public List<EnemyProjectile> list_activeMissile = new ();


        void Awake()
        {
            stageMgr = StageManager.Instance;
        }


        public void MissileInit(EnemyProjectile missile)
        {
            Debug.Log("MissileInit" + missile);

            GameManager.Instance.objPoolingMgr.ObjectInit(list_missile, missile.gameObject, tr_disable);
            list_activeMissile.Remove(missile);
        }

        /// <summary>
        /// Create missile
        /// </summary>
        /// <returns></returns>
        public EnemyProjectile CreateMissile(GameObject originGo)
        {
            EnemyProjectile missile;
            if (originGo == null)
            {
                originGo = origin_missile;
            }

           // GameObject go = list_missile.Find(item => item.GetComponent<EnemyProjectile>().name == originGo.GetComponent<EnemyProjectile>().name);

            missile = GameManager.Instance.objPoolingMgr.CreateObject(
                list_missile, originGo, transform.position, tr_active).GetComponent<EnemyProjectile>();
            missile.shooter = this;
            list_activeMissile.Add(missile);

            return missile;
        }


        //public IEnumerator WaitForMissile()
        //{
        //    if (list_activeMissile.Count > 0)
        //    {
        //        yield return new WaitForSeconds(5f);
        //        ResetMissile();
        //    }
        //}

        /// <summary>
        /// 4/13/2023-LYI
        /// 모든 활성화 된 미사일 초기화, 방 지나갈 때 호출할 것
        /// </summary>
        public void ResetAllMissile()
        {
            for (int i = 0; i < list_activeMissile.Count; i++)
            {
                list_activeMissile[i].Init();
            }
        }

    }
}