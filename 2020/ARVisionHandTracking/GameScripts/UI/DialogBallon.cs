using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBallon : MonoBehaviour
{
    Animator mAnimator;

    RectTransform canvas;
    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        canvas = transform.GetChild(0).GetComponent<RectTransform>();

        mAnimator.SetBool("isReady", false);
        gameObject.SetActive(false);
    }

    void Update()
    {
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
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
        transform.localPosition = Vector3.left * 0.6f;
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
    }
}
