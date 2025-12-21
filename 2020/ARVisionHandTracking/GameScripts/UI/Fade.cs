using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public CanvasGroup fadeTitleCanvasGroup;
    Image fadeImg;

    Coroutine currentCoroutine = null;

    private void Awake()
    {
        fadeImg = transform.GetChild(0).GetComponent<Image>();
        fadeCanvasGroup = this.GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        fadeCanvasGroup.alpha = 0;
        //SetImageColor();

    }


    void SetImageColor()
    {
        if (fadeImg.sprite != null)
        {
            fadeImg.color = new Color(1, 1, 1, 0);
        }
        else
        {
            fadeImg.color = new Color(0, 0, 0, 0);
        }
    }

    ///// <summary>
    ///// Change Fade Image Sprite
    ///// </summary>
    ///// <param name="_sprite"></param>
    //public void SetFadeImage(Sprite _sprite)
    //{
    //    fadeImg.sprite = _sprite;
    //}        

    public void SetFadeCanvas(CanvasGroup _canvas)
    {
        fadeTitleCanvasGroup = _canvas;
    }

    public void StopFade()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            fadeCanvasGroup.alpha = 0;
            //SetImageColor(); ;
        }
    }

    /// <summary>
    /// Start fading
    /// </summary>
    /// <param name="_action">function to play between fad in and fade out</param>
    /// <param name="_fadeSpeed">fade in & fade out speed</param>
    /// <param name="_blackTime">stay time after fade in</param>
    public void StartFade(UnityAction _action = null, float _fadeSpeed = 5f, float _blackTime = 0.5f)
    {
        StopFade();
        currentCoroutine = StartCoroutine(Fading(_fadeSpeed, _blackTime, _action));
    }
    //public void StartFade(UnityAction _endAction = null, UnityAction _middleAction = null, float _fadeSpeed = 3f, float _blackTime = 0.5f)
    //{
    //    StopFade();
    //    currentCoroutine = StartCoroutine(Fading(_endAction, _middleAction, _fadeSpeed, _blackTime));
    //}
    public void StartTitleFade(UnityAction _action = null, float _fadeSpeed = 5f, float _blackTime = 0.5f)
    {
        StartCoroutine(TitleFading(_action, _fadeSpeed, _blackTime));
    }

    //public void FadeStart(bool _fadeIn, float _fadingSpeed = 1f)
    //{
    //    StopFade();
    //    currentCoroutine = StartCoroutine(FadeInOut(_fadeIn, _fadingSpeed));
    //}

    /// <summary>
    /// FadeIn, Out
    /// </summary>
    /// <returns></returns>
    public IEnumerator Fading(float _fadingSpeed = 5f, float _blackTime = 0.5f, UnityAction _action = null)
    {
        yield return FadeInOut(true, _fadingSpeed);
        yield return new WaitForSeconds(_blackTime);
        if (_action != null)
        {
            _action.Invoke();
        }

        yield return FadeInOut(false, _fadingSpeed);
    }
    public IEnumerator Fading(UnityAction _endAction = null, UnityAction _middleAction = null, float _fadingSpeed = 5f, float _blackTime = 0.5f)
    {
        yield return FadeInOut(true, _fadingSpeed);
        yield return new WaitForSeconds(_blackTime);
        if (_middleAction != null)
        {
            _middleAction.Invoke();
        }

        yield return FadeInOut(false, _fadingSpeed);
        if (_endAction != null)
        {
            _endAction.Invoke();
        }
    }
    public IEnumerator TitleFading(UnityAction _endAction = null, float _fadingSpeed = 5f, float _blackTime = 0.5f)
    {
        yield return FadeInOut(fadeTitleCanvasGroup, true, _fadingSpeed);
        yield return new WaitForSeconds(_blackTime);

        yield return FadeInOut(fadeTitleCanvasGroup, false, _fadingSpeed);
        if (_endAction != null)
        {
            _endAction.Invoke();
        }
    }

    /// <summary>
    /// FadeIn, Out
    /// </summary>
    /// <param name="_fadeIn"> 어두워질건지? </param>
    /// <param name="_fadingSpeed"> 색 변화 속도 배속 </param>
    /// <returns></returns>
    public IEnumerator FadeInOut(bool _fadeIn, float _fadingSpeed = 5f)
    {
        float currentTime = 0f;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime * _fadingSpeed;

            fadeCanvasGroup.alpha = (_fadeIn) ?
                Mathf.Lerp(0, 1, currentTime) :
                Mathf.Lerp(1, 0, currentTime);

            //fadeImg.color = (_fadeIn) ?
            //Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), currentTime) :
            //Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), currentTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public IEnumerator FadeInOut(CanvasGroup _group, bool _fadeIn, float _fadingSpeed = 5f)
    {
        float currentTime = 0f;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime * _fadingSpeed;

            _group.alpha = (_fadeIn) ?
                Mathf.Lerp(0, 1, currentTime) :
                Mathf.Lerp(1, 0, currentTime);

            //fadeImg.color = (_fadeIn) ?
            //Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), currentTime) :
            //Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), currentTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
