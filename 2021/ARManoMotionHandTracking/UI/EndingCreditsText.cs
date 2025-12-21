using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCreditsText : MonoBehaviour
{
    public bool isHit = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hit" && isHit == false)
        {
            isHit = true;

            StartCoroutine(GetComponentInParent<EndingCredits>().TextFade(GetComponent<Text>()));
        }
    }
}
