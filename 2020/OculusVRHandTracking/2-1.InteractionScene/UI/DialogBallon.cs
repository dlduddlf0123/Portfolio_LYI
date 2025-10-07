using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBallon : MonoBehaviour
{
    Animator mAnimator;
    public HeaderCanvas dialogCanvas;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        On();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MainCamera"))
        {
            transform.parent.Rotate(0, 180, 0);
        }
    }

    public void On()
    {
        mAnimator.SetBool("isReady", false);
        mAnimator.SetTrigger("isOn");
    }

    public void Off()
    {
        mAnimator.SetBool("isReady", false);
        mAnimator.SetTrigger("isOff");
        StartCoroutine(OffTimer());
    }

    public void Ready()
    {
        mAnimator = GetComponent<Animator>();
        mAnimator.SetBool("isReady", true);
    }

    IEnumerator OffTimer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        dialogCanvas.StopAllCoroutines();
        dialogCanvas.dialogText.gameObject.SetActive(false);
    }
}
