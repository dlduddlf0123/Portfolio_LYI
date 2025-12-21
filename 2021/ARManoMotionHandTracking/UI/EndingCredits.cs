using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class EndingCredits : MonoBehaviour
{
    PlayableDirector m_Director;
    public AudioSource m_endingAudio;
    public GameObject txt_credit;
    Text[] arr_text;

    private void Awake()
    {
        m_Director = GetComponent<PlayableDirector>();
        m_endingAudio = GetComponent<AudioSource>();
        arr_text = txt_credit.GetComponentsInChildren<Text>();
    }

    private void OnEnable()
    {
        for (int index = 0; index < arr_text.Length; index++)
        {
            arr_text[index].color = new Color(arr_text[index].color.r, arr_text[index].color.g, arr_text[index].color.b, 1);
            arr_text[index].GetComponent<EndingCreditsText>().isHit = false;
        }

        m_Director.Play();
        GameManager.Instance.soundMgr.bgmSource.Stop();
        GameManager.Instance.uiMgr.ui_game.game_btn_skip.gameObject.SetActive(true);
    }

    public void EndingCreditText()
    {
        for (int index = 0; index < arr_text.Length; index++)
        {
            string[] temp = arr_text[index].text.Split(' ');

            temp[0] = "<size=50>"+temp[0]+"</size>";
            arr_text[index].text = null;
            for (int tempNum = 0; tempNum < temp.Length; tempNum++)
            {
                arr_text[index].text += temp[tempNum];
            }
        }
    }


    public float fadeSpeed = 1f;
    public IEnumerator TextFade(Text _text)
    {
        float t = 1;
        while (_text.color.a >= 0)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, t);
            t -= fadeSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.001f);
        }
    }

    public void SignalEndEpisode()
    {
        //GameManager.Instance.currentEpisode.currentStage.EndStage();
        //this.gameObject.SetActive(false);
    }
}
