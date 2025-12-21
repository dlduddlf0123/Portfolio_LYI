using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitGuideCanvas : MonoBehaviour
{
    public Camera pivotCam;
    public GameObject Guideline;
    private void Awake()
    {
        transform.LookAt(transform.position - pivotCam.transform.position);
    }
    private void Update()
    {
        Vector3 screenPos = pivotCam.WorldToScreenPoint(Guideline.transform.position);
    }
}
