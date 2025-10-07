using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Character
{
    public class Tok_CrashCheck : MonoBehaviour
    {
        Tok_Movement tok_move;

        private void Awake()
        {
            tok_move = transform.GetComponentInParent<Tok_Movement>();
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (tok_move != null)
            {
                if (coll.gameObject.layer == LayerMask.NameToLayer(Constants.Layer.LAYERMASK_OBSTACLE))
                {
                    tok_move.OnCrash();
                }
            }
        }


    }
}