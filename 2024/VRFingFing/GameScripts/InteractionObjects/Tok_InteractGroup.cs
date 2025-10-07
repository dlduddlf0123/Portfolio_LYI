using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTokTok.Interaction;

public class Tok_InteractGroup : Tok_Interact
{
    [Header("Tok Interact Group")]
    public Tok_Interact[] arr_interact;


    public override void ActiveInteraction()
    {
        base.ActiveInteraction();

        if (arr_interact.Length > 0)
        {
            for (int i = 0; i < arr_interact.Length; i++)
            {
                arr_interact[i].ActiveInteraction();
            }
        }
    }

    public override void DisableInteraction()
    {
        base.DisableInteraction();

        if (arr_interact.Length > 0)
        {
            for (int i = 0; i < arr_interact.Length; i++)
            {
                arr_interact[i].DisableInteraction();
            }
        }
    }

}
