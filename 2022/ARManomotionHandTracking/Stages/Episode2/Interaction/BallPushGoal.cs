using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPushGoal : MonoBehaviour
{
    public BallPushInteract pushInteract;

    private void OnTriggerEnter(Collider other)
    {
        pushInteract.GoalSuccess();
    }
}
