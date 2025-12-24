using UnityEngine;
using System.Collections;

public class ChangeObject : MonoBehaviour {
    public GameObject[] chairList;
    public GameObject chair;
    int currentIdx;

	void Update ()  {
	    if(Input.GetButtonDown("Fire2"))  {
            GameObject obj = Instantiate(chairList[currentIdx]);
            obj.transform.position = chair.transform.position;
            DestroyImmediate(chair, true);
            chair = obj;
            currentIdx++;

            if(currentIdx >= chairList.Length)  {
                currentIdx = 0;
            }
        }
	}
}
