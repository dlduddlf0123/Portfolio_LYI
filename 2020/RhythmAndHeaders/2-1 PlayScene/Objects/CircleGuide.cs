using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGuide : MonoBehaviour
{
    PlayManager ingameMgr;
    public Vector3 scale;
    public float interval;
    public int lineNum;
    private float bpm;
    // Update is called once per frame
    private void Awake()
    {
        ingameMgr = PlayManager.Instance;
        bpm = ingameMgr.bpm;
        interval = 0.008f * bpm;
    }
    void Update()
    {
        if (transform.localScale.x > scale.x)
        {
            transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * interval;
            transform.position = ingameMgr.currentCam.WorldToScreenPoint(ingameMgr.uiMgr.HitGuide.transform.GetChild(lineNum).transform.position);
        }
        else
        {
            ingameMgr.uiMgr.circleGuide.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
    }

}
