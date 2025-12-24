using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public enum SortType
{
    HEADER = 0,
    ALPHABET,
    SUBJECT,
}

/// <summary>
/// RenderTexture 정렬 관리
/// 앱 실행 시 동적으로 렌더 텍스쳐 생성 관리
/// 렌더텍스쳐 정렬 관리
/// 타임라인에서 끝날 때 페이드 기능 관리
/// Vertical Scroller와 Horizontal Scroller 관리
/// 각 오브젝트 하위에 어떤 것이 정렬 될 지 결정
/// HorizontalScroller 생성 관리
/// </summary>
public class RawImageManager : MonoBehaviour
{
    GameManager gameMgr;

    public List<HorizontalScroller> list_horizontalScroller = new List<HorizontalScroller>();


    public SortType e_sortType;

    //현재 화면에 표시되는 이미지 행,열 번호
    public int currentSubjectNum = 0;
    public int currentImageNum = 0;

    public bool isVerticalMove = false;
    public bool isHorizontalMove = false;


    private void Awake()
    {
        gameMgr = GameManager.Instance;

    }

    void Start()
    {
        CardSort(e_sortType);
        gameMgr.wordCardMgr.currentWord = gameMgr.wordCardMgr.list__renderWordCard[currentSubjectNum][currentImageNum];
    }

    public void SignalFade(float _speed)
    {

        StartCoroutine(FloatFadeIn(gameMgr.wordCardMgr.currentWord.renderImage.transform.GetChild(0).GetComponent<CanvasGroup>(),_speed));
    }
    public void SignalFadeOut(float _speed)
    {
        StartCoroutine(FloatFadeOut(list_horizontalScroller[0].list_rawImg[0].transform.GetChild(0).GetComponent<CanvasGroup>(), _speed));

    }

    public IEnumerator FloatFadeIn(CanvasGroup _target, float _speed = 1f)
    {
        if (_target.alpha == 0)
        {
            gameMgr.soundMgr.PlaySfx(Vector3.zero, "SFX 04 Success");

            while (_target.alpha < 1)
            {
                _target.alpha += 0.01f * _speed;
                yield return new WaitForSeconds(0.01f);
            }
            _target.alpha = 1;
        }
        gameMgr.soundMgr.PlaySfx(Vector3.zero, gameMgr.wordCardMgr.currentWord.word);
    }

    public IEnumerator FloatFadeOut(CanvasGroup _target, float _speed = 1f)
    {
        if (_target.alpha == 1)
        {
            _target.alpha = 1;
            while (_target.alpha > 0)
            {
                _target.alpha -= 0.01f * _speed;
                yield return new WaitForSeconds(0.01f);
            }
            _target.alpha = 0;
        }
    }

    /// <summary>
    /// 카드 이미지 삭제 후 재정렬
    /// </summary>
    /// <param name="_type"></param>
    public void CardSort(SortType _type)
    {
        e_sortType = _type;

        if (transform.GetChild(0).childCount != 0)
        {
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                Destroy(transform.GetChild(0).GetChild(i).gameObject);
            }
        }

        switch (_type)
        {
            case SortType.HEADER:
                //캐릭터 기준 카테고리 정렬 (6열)
                Sort_Header(); 
                break;
            case SortType.ALPHABET:
                //알파벳 기준 카테고리 정렬 (25열)
                Sort_AlphaBet();
                break;
            case SortType.SUBJECT:
                //정해진 주제 기준 카테고리 정렬 (?열)
                Sort_Subject();
                break;
            default:
                break;
        }

        //빈 열 삭제하기
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).childCount == 0)
            {
                Destroy(transform.GetChild(0).GetChild(i).gameObject);
            }
        }
    }


    /// <summary>
    /// 대가리들 기준 리스트 정렬
    /// </summary>
    void Sort_Header()
    {
        for (int i = 0; i < 6; i++)
        {
            HorizontalScroller _scroller = new GameObject().AddComponent<HorizontalScroller>();
            _scroller.gameObject.AddComponent<VerticalScroller>();
            RectTransform _rect = _scroller.gameObject.AddComponent<RectTransform>();
            List<WordCard> list_wordCard = new List<WordCard>();
            gameMgr.wordCardMgr.list__renderWordCard.Add(list_wordCard);

            _scroller.transform.SetParent(transform.GetChild(0));
            _scroller.list_rawImg = new List<RawImage>();

            HeaderName a = (HeaderName)i;
            _scroller.name = a.ToString();

            _rect.anchorMin = new Vector2(0, 0.5f);
            _rect.anchorMax = new Vector2(0, 0.5f);
            _rect.pivot = new Vector2(0, 0.5f);

            _rect.anchoredPosition3D = new Vector3(0, i * -gameMgr.screenHeight, 0);
            _rect.sizeDelta = new Vector2(0, gameMgr.screenHeight);
            _rect.rotation = Quaternion.identity;
            _rect.localScale = Vector3.one;

            list_horizontalScroller.Add(_scroller);
        }
        for (int i = 0; i < gameMgr.wordCardMgr.list_originWordCard.Count; i++)
        {
            //Parent를 어디로 할당할 지 기준
            CreateRawImage((int)gameMgr.wordCardMgr.list_originWordCard[i].header, i);
        }
    }

    /// <summary>
    /// 주제 기준 리스트 정렬
    /// </summary>
    void Sort_Subject()
    {
        //i의 최대값은주제의 개수에 따라 변경할 것
        for (int i = 0; i < 3; i++)
        {
            HorizontalScroller _scroller = new GameObject().AddComponent<HorizontalScroller>();
            RectTransform _rect = _scroller.gameObject.AddComponent<RectTransform>();
            List<WordCard> list_wordCard = new List<WordCard>();
            gameMgr.wordCardMgr.list__renderWordCard.Add(list_wordCard);

            _scroller.transform.SetParent(transform.GetChild(0));
            _scroller.list_rawImg = new List<RawImage>();

            WordSubject a = (WordSubject)i;
            _scroller.name = a.ToString();

            _rect.anchorMin = new Vector2(0, 0.5f);
            _rect.anchorMax = new Vector2(0, 0.5f);
            _rect.pivot = new Vector2(0, 0.5f);

            _rect.anchoredPosition3D = new Vector3(0, i * -gameMgr.screenHeight, 0);
            _rect.sizeDelta = new Vector2(0, gameMgr.screenHeight);
            _rect.rotation = Quaternion.identity;
            _rect.localScale = Vector3.one;

            list_horizontalScroller.Add(_scroller);
        }
        for (int i = 0; i < gameMgr.wordCardMgr.list_originWordCard.Count; i++)
        {
            CreateRawImage((int)gameMgr.wordCardMgr.list_originWordCard[i].subject, i);
        }
    }


    /// <summary>
    /// 알파벳 기준 리스트 정렬
    /// </summary>
    void Sort_AlphaBet()
    {
        for (int i = 0; i < 25; i++)
        {
            HorizontalScroller _scroller = new GameObject().AddComponent<HorizontalScroller>();
            RectTransform _rect = _scroller.gameObject.AddComponent<RectTransform>();
            List<WordCard> list_wordCard = new List<WordCard>();
            gameMgr.wordCardMgr.list__renderWordCard.Add(list_wordCard);

            _scroller.transform.SetParent(transform.GetChild(0));
            _scroller.list_rawImg = new List<RawImage>();
            char c = (char)(i + 97);
            _scroller.name = c.ToString();

            _rect.anchorMin = new Vector2(0, 0.5f);
            _rect.anchorMax = new Vector2(0, 0.5f);
            _rect.pivot = new Vector2(0, 0.5f);

            _rect.anchoredPosition3D = new Vector3(0, i * -gameMgr.screenHeight, 0);
            _rect.sizeDelta = new Vector2(0, gameMgr.screenHeight);
            _rect.rotation = Quaternion.identity;
            _rect.localScale = Vector3.one;

            list_horizontalScroller.Add(_scroller);
        }

        for (int i = 0; i < gameMgr.wordCardMgr.list_originWordCard.Count; i++)
        {
            CreateRawImage((int)gameMgr.wordCardMgr.list_originWordCard[i].word[0] - 97, i);
        }
    }

    /// <summary>
    /// 카메라에 보여질 UI
    /// RenderTexture를 보여줄 오브젝트 생성
    /// WordCardManager에서 정렬한 순서에 따라 생성
    /// </summary>
    /// <param name="_imgNum">WordCardManager에서 생성된 순서</param>
    public void CreateRawImage(int _scrollNum, int _imgNum)
    {
        WordCard WordCard = gameMgr.wordCardMgr.list_originWordCard[_imgNum];

        //Parent를 어디로 할당할 지 기준
        HorizontalScroller Scroller = list_horizontalScroller[_scrollNum];

        //Raw Image 오브젝트 생성, 관계 설정
        RawImage firstImage = new GameObject().AddComponent<RawImage>();
        RawImage lastImage = new GameObject().AddComponent<RawImage>();
        lastImage.gameObject.AddComponent<CanvasGroup>().alpha = 0; //처음엔 보이지 않는다

        firstImage.name = WordCard.num + WordCard.name ;
        lastImage.name = " lastImage";
        firstImage.transform.SetParent(Scroller.transform);
        lastImage.transform.SetParent(firstImage.transform);

        //현재 인덱스에 따른 위치 설정
        TransformSetter(firstImage, Scroller.list_rawImg.Count);
        TransformSetter(lastImage, Scroller.list_rawImg.Count);
        lastImage.rectTransform.anchoredPosition3D = Vector3.zero;

        //RenderTexture 생성
        RenderTexture firstRender = new RenderTexture(gameMgr.screenWidth, gameMgr.screenHeight, 24);
        RenderTexture lastRender = new RenderTexture(gameMgr.screenWidth, gameMgr.screenHeight, 24);
        firstRender.Create();
        lastRender.Create();

        //RenderTexture 할당
        firstImage.texture = firstRender;
        lastImage.texture = lastRender;

        WordCard.render_firstCam.targetTexture = firstRender;
        WordCard.render_lastCam.targetTexture = lastRender;

        Scroller.list_rawImg.Add(firstImage);
        Scroller.GetComponent<RectTransform>().sizeDelta = new Vector2(Scroller.list_rawImg.Count * gameMgr.screenWidth, gameMgr.screenHeight);

        WordCard.renderImage = firstImage; //단어카드에 이미지 추가
        gameMgr.wordCardMgr.list__renderWordCard[_scrollNum].Add(WordCard); //배열에 단어카드 정렬
    }

    /// <summary>
    /// 기기 해상도에 맞춘 이미지 크기 세팅
    /// </summary>
    /// <param name="_img"></param>
    /// <param name="_imgNum"></param>
    void TransformSetter(RawImage _img, int _imgNum)
    {
        _img.rectTransform.pivot = new Vector2(0, 0.5f);
        _img.rectTransform.anchorMin = new Vector2(0, 0.5f);
        _img.rectTransform.anchorMax = new Vector2(0, 0.5f);

        _img.rectTransform.anchoredPosition3D = new Vector3(_imgNum * gameMgr.screenWidth, 0,0);
        _img.rectTransform.sizeDelta = new Vector2(gameMgr.screenWidth, gameMgr.screenHeight);
        _img.rectTransform.rotation = Quaternion.identity;
        _img.rectTransform.localScale = Vector3.one;
    }

}
