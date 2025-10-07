using MoreMountains.InventoryEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    public class BurbirdItemHeart : MonoBehaviour
    {
        protected StageManager stageMgr;
        protected Rigidbody2D m_rigidbody2D;
        protected Collider2D m_coll;
        protected SpriteRenderer m_sprite;


        public int itemQuantity = 1; //획득할 수량

        private void Awake()
        {
            stageMgr = StageManager.Instance;
            m_rigidbody2D = GetComponent<Rigidbody2D>();

            m_coll = transform.GetChild(0).GetComponent<Collider2D>();
            m_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        }


        protected void Init()
        {
            StopAllCoroutines();
            m_rigidbody2D.velocity = Vector2.zero;
            m_coll.enabled = true;

            stageMgr.heartSpawner.Init(this);
            gameObject.SetActive(false);
        }

        public void StartAbsorbMove()
        {
            StartCoroutine(AbsorbMove());
        }

        /// <summary>
        /// 플레이어에게로 이동
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected IEnumerator AbsorbMove()
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            float distance = Vector2.Distance(transform.position, stageMgr.playerControll.centerTr.position);
            Vector3 moveVec = stageMgr.playerControll.centerTr.position - transform.position;
            float moveSpeed = 10f;

            m_coll.enabled = false;
            while (distance > 0.3f)
            {
                distance = Vector2.Distance(transform.position, stageMgr.playerControll.centerTr.position);
                moveVec = stageMgr.playerControll.centerTr.position - transform.position;
                // transform.Translate(moveVec.normalized * moveSpeed);
                m_rigidbody2D.velocity = moveVec.normalized * moveSpeed;
                yield return new WaitForSeconds(0.01f);
            }

            stageMgr.playerControll.player.GetHeal(stageMgr.playerControll.player.playerStatus.maxHp * 0.1f);
           
            Init();
        }
    }

}