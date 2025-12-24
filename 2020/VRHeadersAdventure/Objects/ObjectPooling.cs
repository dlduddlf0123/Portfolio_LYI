using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public GameObject respawnPoint;
    public Queue<GameObject> poolObject = new Queue<GameObject>();
    public GameObject Rock;
    public int number;
    public float timer;
    
    void Start()
    {
        
        for(int i = 0; i<number;i++)
        {
            poolObject.Enqueue(CreateNewObject());
            
        }
        Pullout();
    }

    

    public void Pullout()
    {
        var obj = poolObject.Dequeue();
        obj.transform.position = respawnPoint.transform.position;
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        obj.gameObject.SetActive(true);
        StartCoroutine("Timer",obj);           
    }

    public void Pullin(GameObject obj)
    {
        poolObject.Enqueue(obj);
        obj.SetActive(false);
    }

    private GameObject CreateNewObject()
    {
        var newobj = Instantiate(Rock);
        newobj.gameObject.SetActive(false);
        newobj.transform.SetParent(transform);
        return newobj;
    }

    IEnumerator Timer(GameObject obj)
    {
        yield return new WaitForSeconds(timer);
        Pullin(obj);
        Pullout();
    }

  
}
