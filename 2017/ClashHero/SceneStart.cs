using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.anyKey)
        {
            if (CGame.Instance.bGameInit)
            {
                //CGameSnd.instance.PlaySound(eSound.ui_button);
                CGame.Instance.SceneChange(1);  //lobby 로 이동.
            }
        }
    }
}
