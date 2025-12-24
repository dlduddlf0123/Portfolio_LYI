using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackRain : MonoBehaviour
{
    public int heal = 1;
    float t = 0.0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Header"))
        {
            t += Time.deltaTime;
            if (t >= 1f)
            {
                other.GetComponent<Character>().TakeHeal(heal);
                t = 0;
            }
        }

    }
}
