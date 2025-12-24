using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestTimer : MonoBehaviour {

    void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            GetComponent<Text>().text = "Timer: " + t;
            yield return new WaitForSeconds(0.1f);
        }

    }

}
