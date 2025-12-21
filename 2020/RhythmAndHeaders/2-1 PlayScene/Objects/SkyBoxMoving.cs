using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxMoving : MonoBehaviour
{
    public float skyboxSpeed;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxSpeed);
    }
}
