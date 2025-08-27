using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchCamera : MonoBehaviour {
    Character chara;
	// Use this for initialization
	void OnEnable () {
        chara = this.transform.parent.GetComponent<Character>();
        GetHeader();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Point"
            && chara.characterState == CharacterState.FLEE)
        {
            chara.Stop();
            chara.AI_Move(3);
            Debug.Log("Point!!");
        }
    }

    public void GetHeader()
    {
        switch (chara.Status.header)
        {
            case Headers.NONE:
                break;
            case Headers.GIRRAFE:

                break;
            case Headers.ZEBRA:
                break;
            default:
                break;
        }
    }
}
