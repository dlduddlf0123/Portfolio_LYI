using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipInteract : InteractionManager
{

    public override void StartInteraction()
    {
        EndInteraction();
    }
}
