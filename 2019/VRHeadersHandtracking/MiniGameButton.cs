using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiniGameButton : MonoBehaviour
{
    Animator mAnimator;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        mAnimator.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (mAnimator.enabled == false)
            {
                StartCoroutine(ButtonClick());
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }

    IEnumerator ButtonClick()
    {
        mAnimator.enabled = true;
        yield return new WaitForSeconds(0.5f);
        mAnimator.enabled = false;
    }
}