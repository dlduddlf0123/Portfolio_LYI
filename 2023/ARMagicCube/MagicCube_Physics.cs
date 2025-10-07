using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCube_Physics : MagicCube
{
    public GameObject[] arr_physicsObj;

    Vector3[] arr_pos;
    Quaternion[] arr_rot;

    private void Start()
    {
        arr_pos = new Vector3[arr_physicsObj.Length];
        arr_rot = new Quaternion[arr_physicsObj.Length];

        for (int i = 0; i < arr_physicsObj.Length; i++)
        {
            arr_pos[i]= arr_physicsObj[i].transform.position;
            arr_rot[i] = arr_physicsObj[i].transform.rotation;
        }
    }

    public override void MagicCubeInit()
    {
        base.MagicCubeInit();

        if (arr_physicsObj.Length == 0 ||
            arr_physicsObj == null ||
            arr_pos == null)
        {
            return;
        }

        for (int i = 0; i < arr_physicsObj.Length; i++)
        {
            arr_physicsObj[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            arr_physicsObj[i].GetComponent<Rigidbody>().isKinematic = true;
            arr_physicsObj[i].transform.position = arr_pos[i];
            arr_physicsObj[i].transform.rotation = arr_rot[i];
        }
    }
}
