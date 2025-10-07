using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer : MonoBehaviour
{
    public bool isSlice = false;
    public float coolTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        isSlice = false;
    }

    public void StartCoolDown()
    {
        if (!isSlice)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(CoolTime());
    }

    public IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        isSlice = false;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Sliceable"))
    //    {

    //    }
    //}

}
