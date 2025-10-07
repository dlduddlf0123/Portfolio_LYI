using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoreMountains.InventoryEngine;

namespace AroundEffect
{
    public class ResetCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                coll.gameObject.GetComponentInParent<CharacterManager>().Movement.ResetPosition();
            }

            if (coll.gameObject.layer == LayerMask.NameToLayer(Constants.Layer.LAYERMASK_ITEM))
            {
                ItemPicker picker = coll.gameObject.GetComponentInParent<ItemPicker>();
                picker.Pick();
                picker.gameObject.SetActive(false);
            }
        }



    }
}