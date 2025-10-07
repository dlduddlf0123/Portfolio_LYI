
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using MoreMountains.InventoryEngine;

namespace AroundEffect
{

    public class HeadersPicker : ItemPicker
    {


        protected override void PickSuccess()
        {
            base.PickSuccess();
            GameManager.Instance.invenMgr.RedrawInventory();
        }
    }
}