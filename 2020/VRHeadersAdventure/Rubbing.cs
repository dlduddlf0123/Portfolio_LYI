using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubbing : MonoBehaviour
{
    public int rubbingCount;
    public GameObject dirtyThing;
    // Start is called before the first frame update
    void Start()
    {
        rubbingCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(rubbingCount == 10)
        {
            dirtyThing.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Dirty")
        {
            rubbingCount++;
        }
    }
}