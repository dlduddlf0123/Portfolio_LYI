using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour {

    GameManager gameMgr;
    public GameObject fade;
    public bool isWater;


	void Awake () {
        gameMgr = GameManager.Instance;
        isWater = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Header")
            && !other.CompareTag("Watch")
            && !other.CompareTag("ball")
            && !other.CompareTag("Water")
            && gameMgr.gameState == GameState.PLAYING)
        {
            fade.SetActive(true);
        }
        if (other.CompareTag("Water"))
        {
            isWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        fade.SetActive(false);

        if (other.CompareTag("Water"))
        {
            isWater = false;
        }
    }
}
