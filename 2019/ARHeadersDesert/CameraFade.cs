using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour {

    GameManager gameMgr;
    GameObject fade;
	// Use this for initialization
	void Start () {
        gameMgr = GameManager.Instance;
        fade = gameMgr.uiMgr.transform.GetChild(0).GetChild(6).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Header")
            && !other.CompareTag("Watch")
            && !other.CompareTag("ball")
            && gameMgr.statGame == GameState.PLAYING)
        {
            fade.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        fade.SetActive(false);

    }
}
