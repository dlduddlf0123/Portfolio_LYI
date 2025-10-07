using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimSprites : MonoBehaviour
{
    public SpriteRenderer[] arr_animSprite;

    Animator m_animator;

    //temp
    float blinkTime = 0f;
    float randTime = 0f;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        ActiveAimSprite(false);
        randTime = Random.Range(1, 3);
    }

    private void Update()
    {
        AnimEyeBlink();
    }
    public void ActiveAimSprite(bool _isActive)
    {
        for (int i = 0; i < arr_animSprite.Length; i++)
        {
            arr_animSprite[i].enabled = !_isActive;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = _isActive;
        }
    }


    /// <summary>
    /// 일정 시간마다 눈 깜빡이기
    /// </summary>
    void AnimEyeBlink()
    {
        blinkTime += Time.deltaTime;
        if (blinkTime > randTime)
        {
            randTime = Random.Range(1, 3);
            m_animator.SetTrigger("isBlink");
            blinkTime = 0;
        }
    }
}
