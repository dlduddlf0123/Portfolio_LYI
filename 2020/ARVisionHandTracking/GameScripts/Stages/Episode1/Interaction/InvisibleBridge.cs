using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBridge : MonoBehaviour
{
    public int bridgeNum = 0;
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(
        GameManager.Instance.isDebug);
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            transform.GetChild(0).gameObject.SetActive(true);
            if (bridgeNum == 0)
            {
                GameManager.Instance.currentEpisode.currentStage.list_interaction[3].GetComponent<CanyonInteraction>().StartMove();
            }
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            StartCoroutine(GameManager.Instance.LateFunc(() =>
            transform.GetChild(0).gameObject.SetActive(false), 1f));
        }
    }

}
