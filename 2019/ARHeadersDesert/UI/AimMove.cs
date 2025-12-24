using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMove : MonoBehaviour {

    Transform mainCam;
    Transform aimPos;
    [SerializeField]
    float aimSpeed = 5f;

    RectTransform rtr;

    bool isMove;
    float t;

	// Use this for initialization
	void Start () {
        rtr = this.GetComponent<RectTransform>();
        mainCam = GameManager.Instance.missileMgr.mainCam.GetComponent<Transform>();
        aimPos = mainCam.GetChild(1);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (mainCam.hasChanged == true)
        {
            t = 0;
            isMove = true;
        }
        else
        {
            isMove = false;
        }

        if (isMove == true && t <1)
        {
            t += Time.deltaTime * aimSpeed;
        }

        rtr.position = Vector3.Lerp(this.transform.position, aimPos.position, t);
        rtr.LookAt(mainCam);
    }
}
