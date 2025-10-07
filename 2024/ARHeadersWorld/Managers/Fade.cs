using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Fade 효과
/// </summary>
public class Fade : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    Image fadeImg;

    Coroutine currentCoroutine = null;

    public float defaultFadeTime = 0.5f;
    public float defualtFadeSpeed = 5f;

    private void Awake()
    {
        fadeImg = transform.GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        fadeCanvasGroup.alpha = 0;
    }

    public void SetImageColor(Color _color)
    {
        fadeImg.color = _color;
    }

    public void SetFadeCanvas(CanvasGroup _canvas)
    {
        fadeCanvasGroup = _canvas;
    }

    public void StopFade()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            fadeCanvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// Start fading
    /// </summary>
    /// <param name="func">function to play between fad in and fade out</param>
    /// <param name="fadeSpeed">fade in & fade out speed</param>
    /// <param name="blackTime">stay time after fade in</param>
    public void StartFadeMiddle(UnityAction func = null, float fadeSpeed = 5f, float blackTime = 0.5f)
    {
        StopFade();
        currentCoroutine = StartCoroutine(FadingMiddle(func, fadeSpeed, blackTime));
    }
    public void StartFadeMiddleEnd(UnityAction middlefunc = null, UnityAction endfunc = null, float fadeSpeed = 5f, float blackTime = 0.5f)
    {
        StopFade();
        currentCoroutine = StartCoroutine(FadingMiddleEnd(middlefunc, endfunc, fadeSpeed, blackTime));
    }
    public void StartFadeEnd(UnityAction endfunc = null, float fadeSpeed = 5f, float blackTime = 0.5f)
    {
        StopFade();
        currentCoroutine = StartCoroutine(FadingEnd(endfunc, fadeSpeed, blackTime));
    }


    /// <summary>
    /// FadeIn, Out
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadingMiddle(UnityAction func = null, float fadingSpeed = 5f, float blackTime = 0.5f)
    {
        yield return FadeInOut(true, fadingSpeed);

        yield return new WaitForSeconds(blackTime / 0.5f);
        if (func != null) {func.Invoke(); }
        yield return new WaitForSeconds(blackTime / 0.5f);

        yield return FadeInOut(false, fadingSpeed);
    }
    private IEnumerator FadingMiddleEnd(UnityAction middleFunc = null, UnityAction endFunc = null,  float fadingSpeed = 5f, float blackTime = 0.5f)
    {
        yield return FadeInOut(true, fadingSpeed);

        yield return new WaitForSeconds(blackTime / 0.5f);
        if (middleFunc != null) { middleFunc.Invoke(); }
        yield return new WaitForSeconds(blackTime / 0.5f);

        yield return FadeInOut(false, fadingSpeed);

        if (endFunc != null) {  endFunc.Invoke(); }
    }
    private IEnumerator FadingEnd(UnityAction func = null, float fadingSpeed = 5f, float blackTime = 0.5f)
    {
        yield return FadeInOut(true, fadingSpeed);

        yield return new WaitForSeconds(blackTime);

        yield return FadeInOut(false, fadingSpeed);
        if (func != null) { func.Invoke(); }
    }

    /// <summary>
    /// FadeIn, Out
    /// </summary>
    /// <param name="isFadeOut"> 어두워질건지? </param>
    /// <param name="fadingSpeed"> 색 변화 속도 배속 </param>
    /// <returns></returns>
    private IEnumerator FadeInOut(bool isFadeOut, float fadingSpeed = 5f)
    {
        float currentTime = 0f;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime * fadingSpeed;

            fadeCanvasGroup.alpha = (isFadeOut) ?
                Mathf.Lerp(0, 1, currentTime) :
                Mathf.Lerp(1, 0, currentTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    private IEnumerator FadeInOut(CanvasGroup group, bool isFadeOut, float fadingSpeed = 5f)
    {
        float currentTime = 0f;
        while (currentTime < 1)
        {
            currentTime += Time.deltaTime * fadingSpeed;

            group.alpha = (isFadeOut) ?
                Mathf.Lerp(0, 1, currentTime) :
                Mathf.Lerp(1, 0, currentTime);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
