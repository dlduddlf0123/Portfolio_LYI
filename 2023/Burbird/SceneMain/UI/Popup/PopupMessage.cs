using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopupMessage : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup fade;

    [SerializeField]
    private TextMeshProUGUI txt_message;

    public void MessageInit()
    {
        fade.alpha = 1;
    }

    public void ShowMessage(string messageText, UnityAction action)
    {
        txt_message.text = messageText;
        StartCoroutine(MessageMove(action));
    }

    IEnumerator MessageMove(UnityAction action)
    {
        float t = 0;
        float time = 0.01f;

        WaitForSeconds wait = new WaitForSeconds(time);

        transform.localPosition = Vector3.zero - Vector3.up * 5f;
        while (t < 0.2f)
        {
            t += time;
            transform.localPosition += Vector3.up * 2.5f;
           // fade.alpha -= time * 5f;

            yield return wait;
        }

        yield return new WaitForSeconds(0.2f);
        t = 0;
        while (t < 0.2f)
        {
            t += time;
            transform.localPosition += Vector3.up * 2.5f;
            fade.alpha -= time * 5f;

            yield return wait;
        }

        MessageInit();
        action.Invoke();

    }
}
