using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainMaker : MonoBehaviour
{
    public GameObject rainObject;
    public float range  = 1;
    public int many = 20;
    Transform Stage;
    // Start is called before the first frame update
    void Start()
    {
        Stage = GameManager.Instance.stage.transform;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < many; i++)
        {
            GameObject Rain = Instantiate(rainObject,this.transform);
            Rain.transform.position = new Vector3(Stage.position.x + Random.Range(-range, range), 5, Stage.position.z + Random.Range(-range, range));
        }
    }
}
