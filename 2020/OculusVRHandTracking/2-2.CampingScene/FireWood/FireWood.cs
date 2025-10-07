using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWood : MonoBehaviour
{
    //public Material cutMat;
    public FireWoodSpawner spawner;
    FireWoodColl woodColl;

    public GameObject[] arr_wood;
    public GameObject particle;
    public GameObject dot;

    public AudioSource mAudio;

    bool isCut = false;

    private void Awake()
    {
        woodColl = transform.parent.GetComponent<FireWoodColl>();
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(Random.Range(-0.04f, 0.04f), 0.164f, Random.Range(-0.04f, 0.04f));
    }

    public void Init()
    {
        isCut = false;
        arr_wood[0].GetComponent<Renderer>().enabled = true;
        arr_wood[0].GetComponent<Rigidbody>().isKinematic = false;

        for (int i = 1; i < arr_wood.Length; i++)
        {
            arr_wood[i].transform.localPosition = Vector3.zero;
            arr_wood[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            arr_wood[i].SetActive(false);
        }

        transform.parent.GetComponent<Collider>().enabled = true;
        transform.parent.GetComponent<Rigidbody>().velocity = Vector3.zero;
        dot.SetActive(true);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Axe") && !isCut)
        {
            isCut = true;
            Debug.Log("Cut!!");

            arr_wood[0].GetComponent<Renderer>().enabled = false;
            arr_wood[0].GetComponent<Collider>().enabled = false;
            arr_wood[0].GetComponent<Rigidbody>().isKinematic = true;

            for (int i = 1; i < arr_wood.Length; i++)
            {
                arr_wood[i].SetActive(true);
                arr_wood[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 70f, 0));
                arr_wood[i].GetComponent<Rigidbody>().AddForce(-arr_wood[i].transform.right * 30f);
            }

            //mAudio.pitch = Random.Range(-0.7f, 1f);
            mAudio.PlayOneShot(mAudio.clip);

            spawner.GetScore();
            spawner.Spawn();

            dot.SetActive(false);

            ParticleSystem[] particles = particle.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Play();
            }

            spawner.list_fireWood.Add(transform.parent.gameObject);
        }
    }
}
