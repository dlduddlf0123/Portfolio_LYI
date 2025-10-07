using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using MoreMountains.InventoryEngine;


namespace Burbird
{
    public class BurbirdDropItem : MonoBehaviour
    {
        protected StageManager stageMgr;
        protected Rigidbody2D m_rigidbody2D;
        protected Collider2D m_coll;
        protected SpriteRenderer m_sprite;

        public ItemPicker itemPicker;

        public int itemQuantity = 1; //획득할 수량


        private void Awake()
        {
            stageMgr = StageManager.Instance;
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_coll = transform.GetChild(0).GetComponent<Collider2D>();
            m_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

            itemPicker = GetComponent<ItemPicker>();
        }

        protected virtual void Init()
        {
            StopAllCoroutines();
            m_rigidbody2D.velocity = Vector2.zero;
            m_coll.enabled = true;

            stageMgr.itemSpawner.Init(this);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 아이템 코드에 따른 아이템으로 변경
        /// 데이터매니저에서 불러오기?
        /// </summary>
        /// <param name="itemName"></param>
        public void SetItem(string itemName)
        {
            InventoryItem item = GameManager.Instance.invenChecker.LoadItem(itemName);

            m_sprite.sprite = item.Icon;
            itemPicker.Item = item;
            itemPicker.Quantity = itemQuantity;
        }
        public void SetItem(InventoryItem item)
        {
            m_sprite.sprite = item.Icon;
            itemPicker.Item = item;
            itemPicker.Quantity = itemQuantity;
        }

        /// <summary>
        /// 아이템 인벤토리로 등록
        /// </summary>
        /// <param name="picker"></param>
        protected virtual void GetItemToInven(ItemPicker picker)
        {
            picker.Pick(picker.Item.TargetInventoryName, "Stage");
        }


        public void StartAbsorbMove(UnityAction action = null, bool isLate = false)
        {
            StartCoroutine(AbsorbMove(action, isLate));
        }

        /// <summary>
        /// 플레이어에게로 이동
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual IEnumerator AbsorbMove(UnityAction action = null, bool isLate = false)
        {
            if (isLate)
            {
                yield return new WaitForSeconds(2f);
            }

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

            if (action != null)
            {
                action.Invoke();
            }

            GetItemToInven(itemPicker);

            Init();
        }

    }
}