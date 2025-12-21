using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeCanvas : MonoBehaviour
{
    public Camera mainCam;

    public Text judgeText;

    private void Update()
    {
      //  transform.LookAt(transform.position - mainCam.transform.position);
    }


    public void ChangeJudgeText(int _judge)
    {
        switch (_judge)
        {
            case 0:
                judgeText.text = "Bad";
                judgeText.color = Color.red;
                break;
            case 1:
                judgeText.text = "Good!";
                judgeText.color = Color.green;
                break;
            case 2:
                judgeText.text = "Great!";
                judgeText.color = Color.green;
                break;
            case 3:
                judgeText.text = "Perfect!";
                judgeText.color = Color.yellow;
                break;
            case 4:
                judgeText.text = "Fantastic!";
                judgeText.color = Color.cyan;
                break;
            case 5:
                judgeText.text = "Missing!";
                judgeText.color = Color.red;
                break;
            default:
                judgeText.text = "Bad";
                break;
        }

        judgeText.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(DelayToggle());
    }

    IEnumerator DelayToggle()
    {
        yield return new WaitForSeconds(1);
        judgeText.gameObject.SetActive(false);
    }
}
