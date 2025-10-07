using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARPlaneTutorial : MonoBehaviour
{
    CanvasGroup m_group;
    Text tutorialText;

    int dir = 1;
    bool isChange = false;
    private void Awake()
    {
        m_group = GetComponent<CanvasGroup>();
        tutorialText = transform.GetChild(0).GetComponent<Text>();
    }

    //// Start is called before the first frame update
    //void OnEnable()
    //{
    //    ChangeTutorialPlaneText();
    //}

    private void OnDisable()
    {
        StopAllCoroutines();
        m_group.alpha = 0;
    }

    public void ChangeTutorialPlaneText()
    {
        isChange = true;
        StartCoroutine(AutoFade());
    }

    public IEnumerator AutoFade()
    {
        float _t = 1f;
        dir = -1;
        while (isChange)
        {
            if (_t > 1)
            {
                dir = -1;

                isChange = !isChange;

                yield return new WaitForSeconds(3f);
            }
            else if (_t < 0)
            {
                dir = 1;
                if (isChange)
                {
                    tutorialText.text = "When the terrain is placed in the desired location, press the start button";
                }
                else
                {
                    tutorialText.text = "Move your phone in a circle";
                }
                yield return new WaitForSeconds(0.5f);
            }

            _t += dir * 0.03f;
            m_group.alpha = _t;

            yield return new WaitForSeconds(0.01f);
        }
    }

}
