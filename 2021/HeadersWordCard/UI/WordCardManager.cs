using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WordCardManager : MonoBehaviour
{
    GameManager gameMgr;
    public RawImageManager rawImgMgr;

    /// <summary>
    /// 전체 WordCard들의 정보
    /// </summary>
    public List<WordCard> list_originWordCard = new List<WordCard>();

    public List<List<WordCard>> list__renderWordCard = new List<List<WordCard>>();

    public WordCard currentWord;

    private void Awake()
    {
        gameMgr = GameManager.Instance;

        //단어들 최초 배열 정리
        //List 형태로 모든 단어 저장
        WordCard[] arr_wordCard;
        arr_wordCard = transform.GetComponentsInChildren<WordCard>();
        for (int i = 0; i < arr_wordCard.Length; i++)
        {
            list_originWordCard.Add(arr_wordCard[i]);
        }
        currentWord = arr_wordCard[0];
    }

    //상하 활성화 상태 변경
    public void VerticalWordCardChange(WordCard _wordCard)
    {
        currentWord = _wordCard;

        //2칸 이상 떨어진 것 비활성화
        //1칸 전후 활성화
        if (rawImgMgr.currentSubjectNum - 2 >= 0)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum - 2][0].renderImage.gameObject.SetActive(false);
            list__renderWordCard[rawImgMgr.currentSubjectNum - 2][0].gameObject.SetActive(false);
        }
        if (rawImgMgr.currentSubjectNum + 2 < rawImgMgr.transform.GetChild(0).childCount)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum + 2][0].renderImage.gameObject.SetActive(false);
            list__renderWordCard[rawImgMgr.currentSubjectNum + 2][0].gameObject.SetActive(false);
        }
        if (rawImgMgr.currentSubjectNum - 1 >= 0)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum - 1][0].renderImage.gameObject.SetActive(true);
            list__renderWordCard[rawImgMgr.currentSubjectNum - 1][0].gameObject.SetActive(true);
        }
        if (rawImgMgr.currentSubjectNum + 1 < rawImgMgr.transform.GetChild(0).childCount)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum + 1][0].renderImage.gameObject.SetActive(true);
            list__renderWordCard[rawImgMgr.currentSubjectNum + 1][0].gameObject.SetActive(true);
        }

        currentWord.PlayTimeline();
    }

    //좌우 활성화 상태 변경
    public void HorizontalWordCardChange(WordCard _wordCard)
    {
        currentWord = _wordCard;
        //2칸 이상 떨어진 것 비활성화
        //1칸 전후 활성화
        if (rawImgMgr.currentImageNum - 2 >= 0)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum - 2].renderImage.gameObject.SetActive(false);
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum - 2].gameObject.SetActive(false);
        }
        if (rawImgMgr.currentImageNum + 2 < rawImgMgr.transform.GetChild(0).childCount)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum + 2].renderImage.gameObject.SetActive(false);
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum + 2].gameObject.SetActive(false);
        }
        if (rawImgMgr.currentImageNum - 1 >= 0)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum - 1].renderImage.gameObject.SetActive(true);
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum - 1].gameObject.SetActive(true);
        }
        if (rawImgMgr.currentImageNum + 1 < rawImgMgr.transform.GetChild(0).childCount)
        {
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum + 1].renderImage.gameObject.SetActive(true);
            list__renderWordCard[rawImgMgr.currentSubjectNum][rawImgMgr.currentImageNum + 1].gameObject.SetActive(true);
        }

        currentWord.PlayTimeline();
    }

    /// <summary>
    /// 카드 번호로 카드찾기
    /// </summary>
    /// <param name="_wordcord"></param>
    public WordCard FindWordCardWithNumber(int _cardNum)
    {
        WordCard card = new WordCard();

        for (int i = 0; i < list_originWordCard.Count; i++)
        {
            if(list_originWordCard[i].num == _cardNum)
            {
                card = list_originWordCard[i];
                break;
            }
        }

        return card;
    }

    /// <summary>
    /// 카드 이름로 카드찾기(영어 소문자만 사용)
    /// </summary>
    /// <param name="_wordcord"></param>
    public WordCard FindWordCardWithName(string _cardName)
    {
        WordCard card = new WordCard();

        foreach (WordCard item in list_originWordCard)
        {
            if (item.word == _cardName)
            {
                card = item;
                break;
            }
        }

        return card;
    }


}
