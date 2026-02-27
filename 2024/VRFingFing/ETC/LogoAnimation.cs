using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    public GameObject logoCanvas;
    
    
    void Start()
    {

    }

    public void OnAnimationEnd()
    {
        logoCanvas.gameObject.SetActive(false);
    }



}
