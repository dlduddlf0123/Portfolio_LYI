using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffColl : MonoBehaviour
{
    CliffInteraction cliff;

    private void Awake()
    {
        cliff = transform.parent.GetComponent<CliffInteraction>();
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Header")&&
            !cliff.isEnd)
        {
            if (!other.gameObject.GetComponent<Kanto>().isDrag)
            {
                cliff.EndInteraction();
            }
        }
    }
}
