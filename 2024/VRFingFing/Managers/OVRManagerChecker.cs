using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OVR;

public class OVRManagerChecker : MonoBehaviour
{
    public Camera mainCam;
    public Fade fade;

    private void Awake()
    {
        //if (FindObjectsOfType(typeof(OVRManager)).Length > 1)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}
        //DontDestroyOnLoad(this.gameObject);

        GameManager.Instance.ovrMgr = GetComponent<OVRManager>();
        GameManager.Instance.mainCam = mainCam;
       // GameManager.Instance.fade = fade;

    }

    private void Start()
    {
        //if (GameManager.Instance.statGame == GameStatus.LOADING)
        //{
        //    fade.fadeCanvasGroup.alpha = 1;
        //}
    }
}
