using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlaneChecker : MonoBehaviour
{
    public ARDummyColl[] arr_coll;

    public Material transparentMat;
    public Material defaultMat;

    public void CheckAllIn()
    {
        int count = 0;
        for (int i = 0; i < arr_coll.Length; i++)
        {
            if (arr_coll[i].isColled )
            {
                count++;
            }
        }

        if (count == arr_coll.Length)
        {
            GetComponent<Renderer>().material = defaultMat;

            GameManager.Instance.uiMgr.ready_btn_start.gameObject.SetActive(true);
        }
        else
        {
            GetComponent<Renderer>().material = transparentMat;
            //GameManager.Instance.uiMgr.ready_btn_start.gameObject.SetActive(false);
        }
    }
}
