using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWoodColl : MonoBehaviour
{
    public FireWoodSpawner spawner;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            spawner.Spawn();
            spawner.list_fireWood.Add(this.gameObject);
        }
    }

}

