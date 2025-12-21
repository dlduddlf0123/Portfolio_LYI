using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCheckBlock : MonoBehaviour
{
    PlayManager inGameMgr;
    // Start is called before the first frame update
    void Awake()
    {
        inGameMgr = PlayManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pass"))
        {
            inGameMgr.uiMgr.ShowResult();
        }
    }
}
