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

    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player") &&
            GameManager.Instance.handCtrl.manoHandMove.handSide == HandSide.Palmside &&
            GameManager.Instance.handCtrl.manoHandMove.handGestureContinuous == ManoGestureContinuous.OPEN_HAND_GESTURE &&
            !transform.GetChild(0).gameObject.activeSelf)
        {
            StopAllCoroutines();
            transform.GetChild(0).gameObject.SetActive(true);
            if (bridgeNum == 0)
            {
                GameManager.Instance.currentEpisode.currentStage.list_interaction[3].GetComponent<CanyonInteraction>().StartMove();
            }
        }
        else
        {
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                StartCoroutine(GameManager.Instance.LateFunc(() =>
                transform.GetChild(0).gameObject.SetActive(false), 1f));
            }
        }
    }
    
}
