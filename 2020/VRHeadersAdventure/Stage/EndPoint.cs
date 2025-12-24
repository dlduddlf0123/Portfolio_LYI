using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EndPoint : MonoBehaviour
{
    GameManager gameMgr;
    private void Awake()
    {
        gameMgr = GameManager.Instance;
        //we make sure the checkpoint is part of the Checkpoint layer, which is set to interact ONLY with the player layer.
        gameObject.layer = LayerMask.NameToLayer("Checkpoint");
    }
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Header"))
        {
            gameMgr.stageMgr.StartCoroutine(gameMgr.stageMgr.OnStageClear());
            gameObject.SetActive(false);
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red * 0.75f;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
