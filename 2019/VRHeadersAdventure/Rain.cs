using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    public ParticleSystem particle;

    float t = 0;

    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.CompareTag("Rain"))
        {
            Delete();
        }
        if (coll.CompareTag("Player"))
        {
            Debug.Log("Hand!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (t < 10f)
        {
            t += Time.deltaTime;
        }
        else
        {
            Delete();
        }
    }

    void Delete()
    {
        particle.Play();
        this.GetComponent<Rigidbody>().useGravity = false;
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, particle.main.duration);
    }
}
