using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public enum HeaderName
{
    KANTO = 0,
    ZINO,
    OODADA,
    COCO,
    DOINK,
    TENA,
}

public enum WordSubject
{
    FRUIT = 0,
    OBJECT,
    VERB,
}
/// <summary>
/// 타임라인 재생, 중지 관리
/// </summary>
public class WordCard : MonoBehaviour
{
    public PlayableDirector m_director { get; set; }

    public WordCard prev_word { get; set; }
    public WordCard next_word { get; set; }

    public Camera render_firstCam;
    public Camera render_lastCam;

    public TextMeshProUGUI[] wordText;
    public Image[] wordImage;

    public RawImage renderImage;

    //분류를 위한 카테고리 종류
    public WordSubject subject;
    public HeaderName header;
    public char firstChar { get; set; }
    
    public int num; //카드 번호
    public string word; //카드 글자
     
    private void Awake()
    {
        m_director = GetComponent<PlayableDirector>();
        firstChar = word[0];
    }

    void Start()
    {
        
    }

    public void PlayTimeline()
    {
        m_director.Play();
        renderImage.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
    }

    public void PauseTimeline()
    {
        m_director.Pause();
    }
    public void ResumeTimeline()
    {
        m_director.Resume();
    }

    public void StopTimeline()
    {
        m_director.Stop();
    }
}
