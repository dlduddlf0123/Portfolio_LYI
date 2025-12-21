using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollStage : MonoBehaviour
{
    public Transform followTarget;

    public float scrollSpeed = 1f;
    bool isScroll = false;
    //followtarget.transform.forward Vector

    void Update()
    {
        if (followTarget == null)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position,
            transform.parent.position +  (followTarget.localPosition.x + followTarget.GetChild(0).localPosition.z)*GameManager.Instance.uiMgr.stageSize * transform.right,
            Time.deltaTime * 8);

        //if (Vector3.Distance(transform.position, followTarget.position) > 3
        //    && !isScroll)
        //{
        //    isScroll = true;
        //    StartCoroutine(UpdatePoisition());
        //}
       // transform.position = (-followTarget.localPosition.x + followTarget.GetChild(0).localPosition.z)* Vector3.right;
    }

    IEnumerator UpdatePoisition()
    {
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = (-followTarget.localPosition.x + followTarget.GetChild(0).localPosition.z) * Vector3.right;
        while (Vector3.Distance(transform.position, endPos) > 0.1f
            && t < 1)
        {
            Debug.Log(Vector3.Distance(transform.position, endPos - Vector3.left * 3f));

            t += Time.deltaTime * scrollSpeed;
            transform.position = Vector3.Lerp(startPos, endPos - Vector3.left * 3f, t) ;
            yield return new WaitForSeconds(0.01f);
        }

        isScroll = false;
    }

}

