using MoreMountains.InventoryEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Burbird
{

    public class BurbirdItemCoin : BurbirdDropItem
    {

        protected override void Init()
        {
            StopAllCoroutines();
            m_rigidbody2D.velocity = Vector2.zero;
            m_coll.enabled = true;

            stageMgr.coinSpawner.Init(this);
            gameObject.SetActive(false);
        }

        protected override void GetItemToInven(ItemPicker picker)
        {
            stageMgr.GetCoin(picker.Quantity);
            base.GetItemToInven(picker);
        }
    }

}