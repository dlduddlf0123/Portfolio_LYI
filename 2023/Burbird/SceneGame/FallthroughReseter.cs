using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallthroughReseter : MonoBehaviour
{
    PlatformEffector2D m_coll;
    float triggerTime = 0.5f;
    bool isEffect = false;
    private void Awake()
    {
        m_coll = GetComponent<PlatformEffector2D>();
    }
    public void StartFall()
    {
        triggerTime = 0.3f;
        if (isEffect)
        {
            return;
        }
        StartCoroutine(FallCoroutine());
    }

    IEnumerator FallCoroutine()
    {
        Debug.Log(gameObject.name + "FallCoroutine");
        isEffect = true;
        int playerLayerMask = 1 << LayerMask.NameToLayer("Player");

        m_coll.colliderMask &= ~playerLayerMask;

        while (triggerTime > 0)
        {
            triggerTime -= 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        m_coll.colliderMask |= playerLayerMask;
        isEffect = false;
    }
}