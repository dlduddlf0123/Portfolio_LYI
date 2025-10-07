using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SizeChanger : MonoBehaviour
{
    Scrollbar scrollBar;
    public Transform changeTr;

    // Start is called before the first frame update
    void Start()
    {
        scrollBar = GetComponent<Scrollbar>();
        scrollBar.onValueChanged.AddListener((value) => changeTr.localScale = Vector3.one * (value + 0.5f));
    }

}
