using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollRectAutoSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.childCount * 800f + 2200,1100f);
    }
}
