using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect
{

    /// <summary>
    /// 8/30/2024-LYI
    /// Collider for check hand gesture
    /// just call back is colled with anything
    /// </summary>
    public class HandCollider_Character : EventCollider
    {
        protected override void OnEnter(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                isColled = true;
                colledGameObject = coll.gameObject;
               
                //coll.GetComponentInParent<CharacterManager>().transform.SetParent(handInput.tr_characterAnchor);

            }
        }
        protected override void OnStay(Collider coll)
        {
            //if (coll.gameObject.CompareTag("isGround"))
            //{
            //    isColled = true;
            //    colledGameObject = coll.gameObject;
            //}
        }
        protected override void OnExit(Collider coll)
        {
            if (coll.gameObject.CompareTag(Constants.TAG.TAG_CHARACTER))
            {
                isColled = false;
                colledGameObject = coll.gameObject;
            }
        }

    }
}