using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour
{
    GameManager gameMgr;
    public AudioClip footSound = null;
    // Start is called before the first frame update
    void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        { 
            gameMgr.soundMgr.PlaySfx(transform, footSound,Random.Range(0.8f,1.2f));
        }
    }

}
